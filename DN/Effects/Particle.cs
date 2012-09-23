using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public  class Particle
    {
        /// <summary>
        /// Represents the position of the particle in 3D space.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Represents the current velocity of the particle in 3D space.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// Represents the rotation of the particle around all three axes.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// Represents the colour of the particle.
        /// </summary>
        public Color4 Colour;

        /// <summary>
        /// Represents the scale of the particle.
        /// </summary>
        public float Scale;

        /// <summary>
        /// Contains the time at which the particle was released.
        /// </summary>
        public float Inception;

        /// <summary>
        /// Contains the current age of the particle in the range [0,1].
        /// </summary>
        public float Age;

        public virtual void Update(float dt)
        {

        }
    }
}
