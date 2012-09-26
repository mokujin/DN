using Blueberry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    class DustParticle : Particle
    {
        public char letter;
        public DustParticle()
        {
            letter = LetterParticle.aviableLetters[RandomTool.RandInt(0, LetterParticle.aviableLetters.Length)];// char.ConvertFromUtf32(RandomTool.RandByte())[0];
        }
        public override void Update(float dt)
        {
            Colour.A = 1 - this.Age;
            Rotation += this.Age * dt;
            Velocity.X -= dt*3;
            Velocity.Y -= dt*3;
            Scale += dt/2;
            base.Update(dt);
        }
    }
}
