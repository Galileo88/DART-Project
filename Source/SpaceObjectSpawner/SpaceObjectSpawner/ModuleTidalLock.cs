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
        [KSPField(guiName = "Tidal Lock (Experimental)")]
        [UI_Toggle(enabledText = "Enabled", disabledText = "Disabled")]
        bool enableTidalLock = true;

        /// <summary>
        /// Amount of force that it takes to break tidal lock.
        /// </summary>
        [KSPField]
        public double tidalLockBreakForce = 1;

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
            // Abs(angularVelocity.magnitude / orbitalVelocity.magnitude - 1) > tidalLockBreakForce ?
            double currentForce = Math.Abs(vessel.angularVelocity.magnitude / vessel.orbit.vel.magnitude - 1 );
            if (currentForce > tidalLockBreakForce)
                enableTidalLock = false;
        }
    }
}
