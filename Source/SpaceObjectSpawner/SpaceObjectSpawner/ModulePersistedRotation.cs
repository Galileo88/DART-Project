using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Created by Mike Billard (Angel-125) on 4/9/2021
namespace DART.SpaceObjects
{
    /// <summary>
    /// Persists the vessel's rotation through timewarp, going on/off rails, and scene changes.
    /// Future enhancement: Turn this into a VesselModule so that all vessels can have persisted rotation, and make persisted rotation optional through a game setting.
    /// At that point this module would just be used to setup the initial rotation.
    /// </summary>
    public class ModulePersistedRotation: PartModule
    {
        #region Constants
        const int kUpdateFrameCount = 2;
        #endregion

        #region Fields
        /// <summary>
        /// Handy flag to enable debug mode.
        /// </summary>
        [KSPField()]
        public bool debugMode = false;

        /// <summary>
        /// Imparts an initial rotation in degrees per second to the vessel along the pitch (x), roll (y), and yaw (z) axis.
        /// This value will become the first persisted rotation. External forces can alter the persisted rotation.
        /// The default is 0, which means the vessel won't initially be rotating but its rotation will still be persisted.
        /// </summary>
        [KSPField]
        public Vector3 initialRotationDegPerSec = Vector3.zero;

        /// <summary>
        /// Debug info showing current rotation in degrees per second.
        /// </summary>
        [KSPField(guiName = "Rotation", guiUnits = "deg/s", guiFormat = "n4")]
        public Vector3 debugRotationDisplay = Vector3.zero;

        /// <summary>
        /// Current angular velocity in radians per second.
        /// </summary>
        [KSPField(isPersistant = true, guiUnits = "rad/s", guiFormat = "n4")]
        public Vector3 angularVelocity;
        #endregion

        #region Housekeeping
        Vector3 initialRotationRadiansPerSec = Vector3.zero;
        int waitUpdateFrames = -1;
        Vector3 angularMomentum = Vector3.zero;
        #endregion

        #region Overrides
        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight ||
                vessel == null ||
                vessel.rootPart.Rigidbody ==  null ||
                vessel.situation == Vessel.Situations.LANDED ||
                vessel.situation == Vessel.Situations.FLYING ||
                vessel.situation == Vessel.Situations.PRELAUNCH ||
                vessel.situation == Vessel.Situations.SPLASHED
                )
                return;

            if (TimeWarp.CurrentRateIndex == 0 && !vessel.packed)
            {
                // If we're waiting to update then count down the wait frames.
                if (waitUpdateFrames > 0)
                {
                    waitUpdateFrames -= 1;
                }

                // Set initial rotation if needed.
                else if (angularVelocity.magnitude <= 0 && initialRotationRadiansPerSec.magnitude > 0)
                {
                    waitUpdateFrames = kUpdateFrameCount;
                    initialRotationRadiansPerSec = calculateInitialVelocityRadians();
                    angularVelocity = initialRotationRadiansPerSec;
                }

                // If we have angular velocity to apply and we've waited long enough then apply the torque corresponding to the angular velocity.
                else if (waitUpdateFrames == 0 && angularVelocity.magnitude > 0)
                {
                    addVesselTorque(angularVelocity);
                    waitUpdateFrames = -1;
                }

                // Record the current angular velocity.
                else if (waitUpdateFrames == -1)
                {
                    angularVelocity = vessel.angularVelocity;
                    debugRotationDisplay = angularVelocity * (float)UtilMath.Rad2Deg;
                }
            }

            // Handle manual spin
            else if (angularVelocity.magnitude > 0)
            {
                handleManualSpin();
                waitUpdateFrames = kUpdateFrameCount;
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            // Debug fields
            Fields["angularVelocity"].guiActive = debugMode;
            Fields["debugRotationDisplay"].guiActive = debugMode;
            Events["AddInitialSpin"].guiActive = debugMode;

            // Calculate the rotational period in radians per second.
            calculateInitialVelocityRadians();
        }
        #endregion

        #region Events
        [KSPEvent(guiActive = true)]
        public void PointToGround()
        {
            Vector3 planetUp = (vessel.rootPart.transform.position - vessel.mainBody.position).normalized;
            vessel.SetRotation(Quaternion.FromToRotation(vessel.GetTransform().up, planetUp) * vessel.transform.rotation, true);
        }

        [KSPEvent()]
        public void AddInitialSpin()
        {
            addVesselTorque(initialRotationRadiansPerSec);
        }
        #endregion

        #region Helpers
        private void addVesselTorque(Vector3 angularVelocity)
        {
            Vector3 COM = vessel.CoM;
            Quaternion rotation = vessel.ReferenceTransform.rotation;
            Rigidbody rigidbody;

            // Applying force on every part
            foreach (Part vesslePart in vessel.parts)
            {
                rigidbody = vesslePart.GetComponent<Rigidbody>();
                if (vesslePart.mass == 0 || rigidbody == null)
                    continue;

                rigidbody.AddTorque(rotation * angularVelocity, ForceMode.VelocityChange);
                rigidbody.AddForce(Vector3.Cross(rotation * angularVelocity, (vesslePart.transform.position - COM)), ForceMode.VelocityChange);
            }
        }

        private void handleManualSpin()
        {
            vessel.SetRotation(Quaternion.AngleAxis(angularVelocity.magnitude * TimeWarp.CurrentRate, vessel.ReferenceTransform.rotation * angularVelocity) * vessel.transform.rotation, false);
        }

        private Vector3 calculateInitialVelocityRadians()
        {
            initialRotationRadiansPerSec = initialRotationDegPerSec * (float)UtilMath.Deg2Rad;
            return initialRotationRadiansPerSec;
        }
        #endregion
    }
}
