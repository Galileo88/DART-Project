using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DART.SpaceObjects
{
    /// <summary>
    /// Keeps the part tidally locked with the celestial body that it orbits.
    /// </summary>
    public class ModuleTidalLock: PartModule
    {
        #region Fields
        /// <summary>
        /// Handy flag to enable debug mode.
        /// </summary>
        [KSPField()]
        public bool debugMode = false;

        /// <summary>
        /// Flag to determin if the part is tidally locked.
        /// </summary>
        [KSPField(isPersistant = true, guiName = "Is tidally locked")]
        [UI_Toggle(enabledText = "Enabled", disabledText = "Disabled")]
        public bool isTitallyLocked = true;

        /// <summary>
        /// In seconds, the amount of time it takes to make a full revolution along the roll (x), pitch (y) and yaw (z) axis.
        /// </summary>
        [KSPField]
        public Vector3 initialRotationPeriod = Vector3.zero;

        /// <summary>
        /// Change in angular velocity needed to break tidal lock.
        /// </summary>
        [KSPField]
        public float tidalLockBreakThreshold = 0.001f;

        /// <summary>
        /// Debug info showing rotation in degrees per second.
        /// </summary>
        [KSPField(guiName = "Rotation Period", guiUnits = "deg/s", guiFormat = "n4")]
        public Vector3 debugRotationDisplay = Vector3.zero;
        #endregion

        #region Housekeeping
        Vector3 initialAngularVelocityRadians = Vector3.zero;
        #endregion

        #region Overrides
        public void FixedUpdate()
        {
            if (!isTitallyLocked || !HighLogic.LoadedSceneIsFlight)
                return;
            if (part.vessel.situation == Vessel.Situations.PRELAUNCH || 
                part.vessel.situation == Vessel.Situations.LANDED || 
                part.vessel.situation == Vessel.Situations.SPLASHED || 
                part.vessel.situation == Vessel.Situations.FLYING
                )
                return;

            // If our angular velocity no longer matches the initial angular velocity then break tidal lock.
            float angularVelocityDelta = Math.Abs(initialAngularVelocityRadians.magnitude - part.vessel.angularVelocity.magnitude);
            if (angularVelocityDelta > tidalLockBreakThreshold)
            {
                isTitallyLocked = false;
                return;
            }

            // points prograde
            part.vessel.transform.LookAt(part.vessel.mainBody.transform, Vector3.left);

            //Vector3d accelerationVector = (this.part.vessel.CoM - this.vessel.mainBody.position).normalized;

            // points retrograde
            //part.vessel.transform.LookAt(part.vessel.mainBody.transform, Vector3.forward);

            // points radially
            //            part.vessel.transform.LookAt(part.vessel.mainBody.transform, part.vessel.upAxis);

            // Radial in
            //            Vector3 target = -Vector3.Cross(part.vessel.obt_velocity, part.vessel.orbit.h.xzy);
            //            part.vessel.transform.LookAt(target);

            // points radial
            //            part.vessel.transform.LookAt(this.vessel.orbit.h.xzy);

            // points radial
            //            Vector3 target = (part.vessel.mainBody.transform.position - part.vessel.transform.position).normalized;
            //            part.vessel.transform.LookAt(target);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!debugMode && !HighLogic.LoadedSceneIsFlight)
                return;
            debugRotationDisplay = part.vessel.angularVelocity * (float)UtilMath.Rad2Deg;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            // Debug fields
            Fields["debugRotationDisplay"].guiActive = debugMode;
            Fields["isTitallyLocked"].guiActive = debugMode;
            Events["SpinAsteroid"].guiActive = debugMode;

            // Calculate the rotational period in radians per second.
            Vector3 initialAngularVelocityRadians = initialRotationPeriod;
            if (initialAngularVelocityRadians.x > 0)
                initialAngularVelocityRadians.x = 360 / initialAngularVelocityRadians.x;
            if (initialAngularVelocityRadians.y > 0)
                initialAngularVelocityRadians.y = 360 / initialAngularVelocityRadians.y;
            if (initialAngularVelocityRadians.z > 0)
                initialAngularVelocityRadians.z = 360 / initialAngularVelocityRadians.z;
            initialAngularVelocityRadians = initialAngularVelocityRadians * (float)UtilMath.Deg2Rad;

        }
        #endregion

        #region Events
        [KSPEvent()]
        public void SpinAsteroid()
        {
            part.Rigidbody.angularVelocity = new Vector3(0, 0.1f, 0);
        }
        #endregion
    }
}
