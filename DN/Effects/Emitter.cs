using Blueberry;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public delegate void ParticleDeathHandler(Emitter sender, Particle particle); 
    public abstract class Emitter
    {
        private int _releaseQuantity;
        private int _budget;

        public int Budget
        {
            get { return this._budget; }
            set
            {
                if (!Initialised)
                    this._budget = Math.Abs(value);
                else
                    throw new Exception("Cannot change budget after emitter has been initialised!");
            }
        }
        public bool Initialised { get; private set; }
        private float _term;

        public float Term
        {
            get { return this._term; }
            set
            {
                if (!Initialised)
                    this._term = value;
                else
                    throw new Exception("Cannot change term after emitter has been initialised!");
            }
        }
        public bool Enabled { get; set; }

        private float TotalSeconds;

        public Int32 ReleaseQuantity
        {
            get { return this._releaseQuantity; }
            set
            {
                this._releaseQuantity = Math.Abs(value);
            }
        }

        public Range ReleaseSpeed { get; set; }

        public Range ReleaseOpacity { get; set; }

        public Range ReleaseScale { get; set; }

        public ColourRange ReleaseColour { get; set; }

        public Range ReleaseRotation { get; set; }

        protected Particle[] _particles;

        private int IdleIndex;

        internal int ActiveIndex;

        public int ActiveParticlesCount { get; private set; }

        public event ParticleDeathHandler OnParticleDeath;

        public Emitter()
        {
            Enabled = true;
            
        }
        public virtual void Initialise()
        {
            this._particles = new Particle[this.Budget];
            this.IdleIndex = 0;
            this.ActiveIndex = 0;
            this.ActiveParticlesCount = 0;

            this.TotalSeconds = 0f;

            Initialised = true;
        }
        public void Initialise(int budget, float term)
        {
            this.Initialised = false;

            this.Budget = budget;
            this.Term = term;
            Initialise();
        }
        public virtual void Update(float dt)
        {
            this.TotalSeconds += dt;
            if (this.ActiveParticlesCount < 1)
                return;
            var currentIndex = this.ActiveIndex;
            var currentParticleCount = this.ActiveParticlesCount;

            for (var i = 0; i < currentParticleCount; i++)
            {
                // Extract the particle from the buffer...
                Particle particle = this._particles[currentIndex];

                // Calculate the age of the particle in seconds...
                var actualAge = this.TotalSeconds - particle.Inception;

                // Check to see if the particle has expired...
                if (actualAge > this.Term)
                {
                    // Increment the index of the first active particle...
                    this.ActiveIndex = (this.ActiveIndex + 1) % this.Budget;
                    
                    // Decrement the active particles count...
                    this.ActiveParticlesCount = (this.ActiveParticlesCount - 1);

                    if (OnParticleDeath != null) OnParticleDeath(this, particle);
                }
                else
                {
                    // Calculate the normalized age of the particle...
                    particle.Age = actualAge / this.Term;

                    // Apply particle movement...
                    particle.Position.X += (particle.Velocity.X * dt);
                    particle.Position.Y += (particle.Velocity.Y * dt);

                    // Put the mutated particle back in the buffer...
                    this._particles[currentIndex] = particle;
                }
                currentIndex = (currentIndex + 1) % this.Budget;
            }
            if (this.ActiveParticlesCount > 0)
            {
                currentIndex = ActiveIndex;
                for (var i = 0; i < ActiveParticlesCount; i++)
                {
                    this._particles[currentIndex].Update(dt);
                    currentIndex = (currentIndex + 1) % this.Budget;
                }
            }
        }
        public virtual void Trigger(float dt)
        {
            if (!this.Initialised) throw new Exception("Emitter has not yet been initialized!");

            if (!this.Enabled)
                return;

            {
                var currentIndex = this.IdleIndex;
                var startIndex = currentIndex;
                var particlesAdded = 0;

                // Calculate the number of particles to release - the lesser of [ReleaseQuantity] and
                // [remaining idle particles]...
                var releaseCount = Math.Min(ReleaseQuantity, this.Budget - this.ActiveParticlesCount);

                for (var i = 0; i < releaseCount; i++)
                {
                    Particle particle = this._particles[currentIndex];
                    particle.Prepare(dt);

                    Vector2 offset, force, position;

                    this.GeneratePositionOffsetAndForce(out position, out offset, out force);

                    var releaseSpeed = RandomTool.RandFloat(this.ReleaseSpeed);

                    particle.Age = 0f;
                    particle.Inception = this.TotalSeconds;

                    particle.Position.X = position.X + offset.X;
                    particle.Position.Y = position.Y + offset.Y;

                    particle.Velocity.X = force.X * releaseSpeed;
                    particle.Velocity.Y = force.Y * releaseSpeed;

                    particle.Scale = RandomTool.RandFloat(this.ReleaseScale);

                    particle.Colour = RandomTool.RandColor(this.ReleaseColour);
                    particle.Colour.A = RandomTool.RandFloat(this.ReleaseOpacity);

                    particle.Rotation = RandomTool.RandFloat(this.ReleaseRotation);

                    this._particles[currentIndex] = particle;

                    this.IdleIndex = (this.IdleIndex + 1) % this.Budget;

                    this.ActiveParticlesCount++;

                    currentIndex = (currentIndex + 1) % this.Budget;

                    particlesAdded++;
                }
            }
        }
            /// <summary>
        /// Generates offset and force vectors for a newly released particle.
        /// </summary>
        /// <param name="offset">Defines an offset vector from the trigger position.</param>
        /// <param name="force">A unit vector defining the inital force applied to the particle.</param>
        protected virtual void GeneratePositionOffsetAndForce(out Vector2 position, out Vector2 offset, out Vector2 force)
        {
            position = Vector2.Zero;

            offset = Vector2.Zero;

            force = RandomTool.NextUnitVector2();
        }

        public virtual void Draw(float dt)
        {

        }

    
    }
}
