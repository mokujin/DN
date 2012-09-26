using Blueberry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public class LetterParticle:Particle
    {
        public char letter;
        public LetterParticle()
        {
            letter = aviableLetters[RandomTool.RandInt(0, aviableLetters.Length)];
        }
        public static string aviableLetters = "0123456789abcdefghigklmnopqrstuvwxyz?!@#$%&^*+=~";
        public override void Update(float dt)
        {
            Colour.A = 1 - this.Age;
            //Rotation += this.Age * dt;
            //Velocity.X -= dt*3;
            //Velocity.Y -= dt*3;
            Scale += dt/7;
            base.Update(dt);
        }
        public override void Prepare(float dt)
        {
            letter = aviableLetters[RandomTool.RandInt(0, aviableLetters.Length)];
            base.Prepare(dt);
        }
    }
}
