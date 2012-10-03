using Blueberry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    class BloodParticle : Particle
    {
        public BloodParticle()
        {
            this.Scale = RandomTool.RandFloat(0.5f, 2f);
        }
        public override void Update(float dt)
        {
            
            Colour.A = 1 - this.Age * 0.75f;
            Rotation += this.Age * dt;
            Velocity.X -= dt * 5;
            //Velocity.Y -= dt * 5;
            Velocity += 10*dt*GameWorld.G * GameWorld.GravityDirection;
            base.Update(dt);
        }
    }
}
