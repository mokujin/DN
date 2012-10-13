using Blueberry;
using Blueberry.Graphics;
using Blueberry.Graphics.Fonts;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public class DustPointEmitter : Emitter
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float Dispersion { get; set; }

        public DustPointEmitter(Vector2 position, Vector2 direction, float dispersion):base()
        {
            Position = position;
            Direction = direction;
            Dispersion = dispersion;
            
            ReleaseQuantity = 1;
            ReleaseSpeed = new Range(5, 10);
            ReleaseScale = 0.8f;
            ReleaseColour = Color4.Gray;
            ReleaseOpacity = 0.8f;
        }
        public override void Initialise()
        {
            base.Initialise();
            for (int i = 0; i < Budget; i++)
            {
                _particles[i] = new DustParticle();
            }
        }
        public override void Update(float dt)
        {
            base.Update(dt);
        }
        protected override void GeneratePositionOffsetAndForce(out Vector2 position, out Vector2 offset, out Vector2 force)
        {
            offset = Vector2.Zero;
            position = Position;
            Vector2 f = -Vector2.UnitY;
            MathUtils.RotateVector2(ref f, (Direction.X > 0 ? 0.5f : -0.5f) + RandomTool.RandFloat(-(Dispersion / 2f), Dispersion / 2f));
            f *= 10;
            force = f;
        }
        public float TriggerInterval { get; set; }
        private float temp = 0;
        public override void Trigger(float dt)
        {
            temp += dt;
            if (temp >= TriggerInterval)
            {
                temp = 0;
                base.Trigger(dt);
            }
        }
        public override void Draw(float dt)
        {
            var currentIndex = this.ActiveIndex;
            var currentParticleCount = this.ActiveParticlesCount;
            BitmapFont f = CM.I.Font("Small");
            for (var i = 0; i < currentParticleCount; i++)
            {
                // Extract the particle from the buffer...
                DustParticle particle = this._particles[currentIndex] as DustParticle;

                SpriteBatch.Instance.PrintText(f, particle.letter, particle.Position, particle.Colour, particle.Rotation, particle.Scale);
                currentIndex = (currentIndex + 1) % this.Budget;
            }
            base.Draw(dt);
        }
    }
}
