using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DART.SpaceObjects
{
    public class ModuleTidalLock: PartModule
    {
        /// <summary>
        /// Handy flag to enable debug mode.
        /// </summary>
        [KSPField()]
        public bool debugMode = false;

        [KSPField(guiName = "Tidal Lock (Experimental)")]
        [UI_Toggle(enabledText = "Enabled", disabledText = "Disabled")]
        bool enableTidalLock = true;

        /// <summary>
        /// Current angular momentum.
        /// </summary>
        [KSPField(guiName = "Current Ang. Mom.", guiUnits = "Nm/s", guiFormat = "n4", guiActive = true)]
        public Vector3 currentAngularMomentum;

        [KSPField(guiName = "Current Mom. Mag.", guiUnits = "Nm/s", guiFormat = "n4", guiActive = true)]
        public float momentumMagnitude;

        /// <summary>
        /// Post-collision angular momentum.
        /// </summary>
        [KSPField(guiName = "Collision Ang. Mom.", guiUnits = "Nm/s", guiFormat = "n4", guiActive = true)]
        public Vector3 collideAngularMomentum;

        [KSPField(guiName = "Collision Mom. Mag.", guiUnits = "Nm/s", guiFormat = "n4", guiActive = true)]
        public float collisionMomentumMagnitude;

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

            currentAngularMomentum = vessel.angularMomentum;
            momentumMagnitude = currentAngularMomentum.magnitude;
        }

        void onCollidedWithObject(DARTCollisionReport collisionReport)
        {
            collideAngularMomentum = vessel.angularMomentum;
            collisionMomentumMagnitude = collideAngularMomentum.magnitude;
        }
    }
}
