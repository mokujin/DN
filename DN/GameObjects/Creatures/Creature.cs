using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blueberry;

namespace DN.GameObjects.Creatures
{
    
    public delegate void CollisionEventHandler(GameObject sender, GameObject gameObject);

    public abstract class Creature:CollidableGameObject
    {
        public float InvulnerabilityDuration;
        private float _invulnerabilityDuration_dt;
        public float Health { get; protected set; }
        public bool IsDead
        {
            get { return Health <= 0; }
        }


        public Creature(GameWorld gameWorld)
            :base(gameWorld)
        {
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if(IsDead)
            {
                //todo: add some event on death

                World.RemoveObject(this);
                return;
            }
            if (InvulnerabilityDuration > _invulnerabilityDuration_dt)
            {
                _invulnerabilityDuration_dt += dt;
            }
        }

        public virtual void AddHelath(float amount)
        {
            Health += amount;
        }

        public virtual void TakeDamage(float amount)
        {
            if (InvulnerabilityDuration <= _invulnerabilityDuration_dt)
            {
               Health -= amount;
                _invulnerabilityDuration_dt = 0;
            }
        }


        // get bounding rectangle with some shift
        public RectangleF GetBoundsWithShift(float sx, float sy)
        {
            RectangleF r = Bounds;
            r.X = (Left + sx);
            r.Y = (Top + sy);
            return r;
        }
        public RectangleF GetBoundsWithShift(Vector2 shift)
        {
            return GetBoundsWithShift(shift.X, shift.Y);
        }
    }
}
