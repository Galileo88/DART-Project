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
    /// Event data generated when one object collided with another.
    /// </summary>
    public class DARTCollisionReport
    {
        /// <summary>
        /// The Part that generated the event.
        /// </summary>
        public Part originator;

        /// <summary>
        /// The Part that the originator collided with.
        /// </summary>
        public Part collidedWithPart;

        /// <summary>
        /// The GameObject that the originator collided with.
        /// Set if we couldn't find a Part object that we collided with. 
        /// </summary>
        public GameObject collidedWithGameObject;

        /// <summary>
        /// The velocity at which the originator collided with the collidedWithPart.
        /// </summary>
        public float collisionVelocity;

        /// <summary>
        /// Mass of the vessel that collided with the originator.
        /// </summary>
        public float collidedWithVesselMass;

        /// <summary>
        /// The momentum of the vessel that the originator collided with.
        /// Calculated as collidedWithVesselMass * collisionVelocity.
        /// </summary>
        public float collidedWithMomentum;

        /// <summary>
        /// The momentum of the originator's vessel.
        /// Calculated as originator vessel's mass * collisionVelocity.
        /// </summary>
        public float originatorMomentum;
    }
}
