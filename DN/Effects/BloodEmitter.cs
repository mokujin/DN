using Blueberry;
using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public class BloodEmitter:Emitter
    {
        
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float Dispersion { get; set; }
        public float triggerInterval = 0.4f;
        private float temp = 0;
        public int countOfSpreads = 100;

        public BloodEmitter(Vector2 position, Vector2 direction, float dispersion)
            : base()
        {
            Position = position;
            Direction = direction;
            Dispersion = dispersion;
            
            ReleaseQuantity = 3;
            ReleaseSpeed = new Range(5, 10);
            ReleaseScale = 1;
            ReleaseColour = Color4.Red;
            ReleaseOpacity = 1;
        }
        public override void Initialise()
        {
            base.Initialise();
            for (int i = 0; i < Budget; i++)
            {
                _particles[i] = new BloodParticle();
            }
        }
        public override void Update(float dt)
        {
            if (Enabled)
            {
                if (temp <= 0)
                {
                    Trigger(dt);
                    temp = triggerInterval;
                    countOfSpreads -= 1;
                    if (countOfSpreads <= 0)
                        this.Enabled = false;
                }
                else
                    temp -= dt;
            }
            base.Update(dt);
        }
        protected override void GeneratePositionOffsetAndForce(out Vector2 position, out Vector2 offset, out Vector2 force)
        {
            offset = Vector2.Zero;
            position = Position;
            Vector2 f = Direction;
            MathUtils.RotateVector2(ref f, RandomTool.RandFloat(-(Dispersion / 2f), Dispersion / 2f));
            f *= 10;
            force = f;
        }
        public override void Draw(float dt)
        {
            var currentIndex = this.ActiveIndex;
            var currentParticleCount = this.ActiveParticlesCount;

            for (var i = 0; i < currentParticleCount; i++)
            {
                // Extract the particle from the buffer...
                BloodParticle particle = this._particles[currentIndex] as BloodParticle;

                SpriteBatch.Instance.FillRegularPolygon(3, 5 * particle.Scale, particle.Position, particle.Rotation, particle.Colour);
                //SpriteBatch.Instance.FillCircle(particle.Position, 10, Color4.White, 10);
                currentIndex = (currentIndex + 1) % this.Budget;
            }
            base.Draw(dt);
        }
    
    }
}
