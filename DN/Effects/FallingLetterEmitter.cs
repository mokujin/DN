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
    public class FallingLetterEmitter : Emitter
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        private static BitmapFont font;
        private float triggerInterval;
        private float temp = 0;

        public FallingLetterEmitter(Vector2 position, Vector2 direction, float triggerInterval)
            : base()
        {
            Position = position;
            Direction = direction;
            this.triggerInterval = triggerInterval;
            if(font == null)
                font = new BitmapFont(Path.Combine("Content", "Fonts", "monofur.ttf"), 16);
            
            ReleaseQuantity = 1;
            ReleaseSpeed = 0;
            ReleaseScale = 1;
            ReleaseColour = Color4.CornflowerBlue;
            ReleaseOpacity = 0.5f;
        }
        public override void Initialise()
        {
            base.Initialise();
            for (int i = 0; i < Budget; i++)
            {
                _particles[i] = new LetterParticle();
            }
        }
        int tc = 0;
        public override void Update(float dt)
        {
            if (temp <= 0)
            {
                Trigger(dt);
                tc++;
                temp = triggerInterval;
            }
            else
                temp -= dt;
            Position += Direction * 50 * dt;
            base.Update(dt);
        }
        protected override void GeneratePositionOffsetAndForce(out Vector2 position, out Vector2 offset, out Vector2 force)
        {
            offset = Vector2.Zero;
            position = Position;
            force = Vector2.Zero;
        }

        string current = "0";
        float time_to_change_current = 0.01f;
        public override void Draw(float dt)
        {
            var currentIndex = this.ActiveIndex;
            var currentParticleCount = this.ActiveParticlesCount;

            for (var i = 0; i < currentParticleCount; i++)
            {
                // Extract the particle from the buffer...
                LetterParticle particle = this._particles[currentIndex] as LetterParticle;

                SpriteBatch.Instance.PrintText(font, particle.letter.ToString(), particle.Position, particle.Colour, particle.Rotation, particle.Scale);
                currentIndex = (currentIndex + 1) % this.Budget;
            }
            SpriteBatch.Instance.PrintText(font, current, Position, new Color4(ReleaseColour.Red.Minimum, ReleaseColour.Green.Maximum, ReleaseColour.Blue.Minimum, ReleaseOpacity.Maximum), RandomTool.RandFloat(ReleaseRotation), RandomTool.RandFloat(ReleaseScale));
            if (time_to_change_current <= 0)
            {
                current = LetterParticle.aviableLetters[RandomTool.RandInt(0, LetterParticle.aviableLetters.Length)].ToString();
                time_to_change_current = 0.01f;
            }
            else
                time_to_change_current -= dt;
            base.Draw(dt);
        }
    }
}
