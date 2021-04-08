using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Created by Mike Billard (Angel-125) on 4/7/2021
namespace DART.SpaceObjects
{
    /// <summary>
    /// In order to smash probes into asteroids at high speeds, we need to set the asteroid part's crashTolerance to a high level or else it will explode on impact.
    /// But the problem is that when the asteroid smashes into terrain, its high crashTolerance will leave it intact. This part module gets around the problem by
    /// watching for collisions. If the part hits terrain, then it checks the impact velocity against the module's crashTolerance. If velocity exceeds the module's
    /// crashTolerance then it will cause the part to explode.
    /// </summary>
    public class ModuleImpactMonitor: PartModule
    {
        /// <summary>
        /// The crash tolerance that we check when the part crashes into the terrain.
        /// Default is 160
        /// </summary>
        [KSPField()]
        public double crashTolerance = 160f;

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        public virtual void OnCollisionEnter(Collision collision)
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            if (collision.collider == null || collision.collider.isTrigger || !collision.collider.enabled)
                return;

            // Get the part (if any) that we collided with.
            Part collidedPart = null;
            if (collision.collider.attachedRigidbody != null)
                collidedPart = collision.collider.attachedRigidbody.GetComponent<Part>();

            // Did we hit terrain?
            if (collision.collider.gameObject.GetComponent<PQ>())
            {
                if (CheatOptions.NoCrashDamage)
                    return;

                // If we exceeded our crash tolerance then it's time to die.
                if (collision.relativeVelocity.magnitude > crashTolerance)
                {
                        GameEvents.onCrash.Fire(new EventReport(FlightEvents.CRASH, part, part.partInfo.title, "terrain", 0, "", collision.relativeVelocity.magnitude));
                        part.explode();
                }
            }

            // Did we collide with a part?
            else if (collidedPart != null)
            {
                DARTCollisionReport collisionReport = new DARTCollisionReport();
                collisionReport.originator = part;
                collisionReport.collidedWithPart = collidedPart;
                collisionReport.collisionVelocity = collision.relativeVelocity.magnitude;
                collisionReport.collidedWithVesselMass = collidedPart.vessel.GetTotalMass();
                collisionReport.collidedWithMomentum = collisionReport.collidedWithVesselMass * collisionReport.collisionVelocity;
                collisionReport.originatorMomentum = part.vessel.GetTotalMass() * collisionReport.collisionVelocity;

                DARTSpaceObjectScenario.onCollidedWithObject.Fire(collisionReport);
            }

            // We collided with something but we don't know what.
            else
            {
                DARTCollisionReport collisionReport = new DARTCollisionReport();
                collisionReport.originator = part;
                collisionReport.collidedWithGameObject = collision.collider.gameObject;
                collisionReport.collisionVelocity = collision.relativeVelocity.magnitude;
                collisionReport.originatorMomentum = part.vessel.GetTotalMass() * collisionReport.collisionVelocity;

                DARTSpaceObjectScenario.onCollidedWithObject.Fire(collisionReport);
            }
        }
        #endregion
    }
}
