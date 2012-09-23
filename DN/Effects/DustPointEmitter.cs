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
    class DustPointEmitter : Emitter
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float Dispersion { get; set; }
        private BitmapFont font;

        public DustPointEmitter(Vector2 position, Vector2 direction, float dispersion):base()
        {
            Position = Position;
            Direction = direction;
            Dispersion = dispersion;
            font = new BitmapFont(Path.Combine("Content", "Fonts", "monofur.ttf"), 14);
            
            ReleaseQuantity = 1;
            ReleaseSpeed = new Range(5, 10);
            ReleaseScale = 1;
            ReleaseColour = Color4.White;
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
                DustParticle particle = this._particles[currentIndex] as DustParticle;

                SpriteBatch.Instance.PrintText(font, particle.letter.ToString(), particle.Position, particle.Colour, particle.Rotation, particle.Scale);
                //SpriteBatch.Instance.FillCircle(particle.Position, 10, Color4.White, 10);
                currentIndex = (currentIndex + 1) % this.Budget;
            }
            base.Draw(dt);
        }
    }
}
