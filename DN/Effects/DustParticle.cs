using Blueberry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    class DustParticle : Particle
    {
        public string letter;
        public int cw;
        public DustParticle()
        {
            letter = RandomTool.ChooseRandom("d", "u", "s", "t");
            cw = RandomTool.RandBool(0.5) ? -1 : 1;

        }
        public override void Update(float dt)
        {
            Colour.A = 0.7f - this.Age * 0.7f;

            Rotation += cw * this.Age * dt * 6;
                
            Velocity.X = MathUtils.Lerp(Velocity.X, 0, dt*2);
            Velocity.Y -= dt*3;
            Scale += dt/1.5f;
            base.Update(dt);
        }
    }
}
