using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DART.SpaceObjects
{
    /// <summary>
    /// This is a simple helper class to keep objects tidally locked around a celestial body. It's designed for single-part vessels; vessels with more than one part won't be tidally locked.
    /// </summary>
    public class ModuleTidalLock: PartModule
    {
        /// <summary>
        /// Handy flag to enable debug mode.
        /// </summary>
        [KSPField()]
        public bool debugMode = false;

        /// <summary>
        /// Flag to determine whether or not to enable tidal lock.
        /// </summary>
        [KSPField(guiName = "Tidal Lock (Experimental)", isPersistant = true)]
        [UI_Toggle(enabledText = "Enabled", disabledText = "Disabled")]
        bool enableTidalLock = true;

        /// <summary>
        /// Post-collision angular velocity
        /// </summary>
        [KSPField(guiName = "Post-collision ang. vel.", guiUnits = "rad/sec", guiFormat = "n7")]
        Vector3 collisionAngularVelocity;

        /// <summary>
        /// Post-collision angular velocity magnitude
        /// </summary>
        [KSPField(guiName = "Post-coll. ang. vel. mag.", guiUnits = "rad/sec", guiFormat = "n7")]
        double collisionAngularVelocityMagnitude;

        /// <summary>
        /// Post-collision orbital period
        /// </summary>
        [KSPField(guiName = "Post-coll. obt. period", guiUnits = "sec", guiFormat = "n7")]
        double orbitPeriod;

        /// <summary>
        /// Collision force
        /// </summary>
        [KSPField(guiName = "Collision force", guiFormat = "n4")]
        double collisionForce;

        /// <summary>
        /// Amount of force that it takes to break tidal lock.
        /// </summary>
        [KSPField]
        public double tidalLockBreakRatio = 0.025;

        public override void OnAwake()
        {
            base.OnAwake();

            if (!HighLogic.LoadedSceneIsFlight)
                return;

            DARTSpaceObjectScenario.onCollidedWithObject.Add(onCollidedWithObject);
        }

        public void OnDestroy()
        {
            DARTSpaceObjectScenario.onCollidedWithObject.Remove(onCollidedWithObject);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            // Debug fields
            Fields["enableTidalLock"].guiActive = debugMode;
            Fields["collisionAngularVelocity"].guiActive = debugMode;
            Fields["collisionAngularVelocityMagnitude"].guiActive = debugMode;
            Fields["orbitPeriod"].guiActive = debugMode;
            Fields["collisionForce"].guiActive = debugMode;

            // For some reason KSP isn't reading tidalLockBreakRatio. We need to load it manually
            ConfigNode node = getPartConfigNode();
            if (!node.HasValue("tidalLockBreakRatio"))
                return;
            double.TryParse(node.GetValue("tidalLockBreakRatio"), out tidalLockBreakRatio);
        }


        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight ||
                vessel == null ||
                vessel.rootPart.Rigidbody == null ||
                vessel.situation == Vessel.Situations.LANDED ||
                vessel.situation == Vessel.Situations.FLYING ||
                vessel.situation == Vessel.Situations.PRELAUNCH ||
                vessel.situation == Vessel.Situations.SPLASHED
                )
                return;

            if (enableTidalLock && vessel.Parts.Count == 1)
            {
                Vector3 planetUp = (vessel.rootPart.transform.position - vessel.mainBody.position).normalized;
                vessel.SetRotation(Quaternion.FromToRotation(vessel.GetTransform().up, planetUp) * vessel.transform.rotation, true);
            }
        }

        void onCollidedWithObject(DARTCollisionReport collisionReport)
        {
            // Abs(angularVelocity.magnitude / (2 * pi / orbitalPeriod) - 1)
            collisionAngularVelocity = vessel.angularVelocity;
            collisionAngularVelocityMagnitude = vessel.angularVelocity.magnitude;

            orbitPeriod = vessel.orbit.period;

            collisionForce = Math.Abs(collisionAngularVelocityMagnitude / ( 2 * Math.PI / vessel.orbit.period) - 1 );
            if (collisionForce > tidalLockBreakRatio)
                enableTidalLock = false;
        }

        /// <summary>
        /// Retrieves the module's config node from the part config.
        /// </summary>
        /// <returns>A ConfigNode for the part module.</returns>
        public ConfigNode getPartConfigNode()
        {
            if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight)
                return null;
            if (this.part.partInfo.partConfig == null)
                return null;
            ConfigNode[] nodes = this.part.partInfo.partConfig.GetNodes("MODULE");
            ConfigNode partConfigNode = null;
            ConfigNode node = null;
            string moduleName;

            //Get the switcher config node.
            for (int index = 0; index < nodes.Length; index++)
            {
                node = nodes[index];
                if (node.HasValue("name"))
                {
                    moduleName = node.GetValue("name");
                    if (moduleName == this.ClassName)
                    {
                        partConfigNode = node;
                        break;
                    }
                }
            }

            return partConfigNode;
        }
    }
}
