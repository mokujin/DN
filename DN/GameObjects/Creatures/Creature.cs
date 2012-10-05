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
using DN.Effects;

namespace DN.GameObjects.Creatures
{
    
    public delegate void CollisionEventHandler(GameObject sender, GameObject gameObject);

    public delegate void DeathEventHandler();


    public abstract class Creature:CollidableGameObject
    {

        private bool _jump = false;
        protected BloodEmitter BloodEmitter;

        public event DeathEventHandler Death;

        public float InvulnerabilityDuration
        {
            get
            {
                return _invulnerabilityDuration;
            }
            set
            {
                _invulnerabilityDuration = value;
                _invulnerabilityDuration_dt = value;
            }
        }

        public bool Invulnerable
        {
            get { return InvulnerabilityDuration > _invulnerabilityDuration_dt; }
        }

        private float _invulnerabilityDuration;
        private float _invulnerabilityDuration_dt;

        public float Health { get; protected set; }
        public float JumpAcceleration = 90f;
        public float JumpMaxVelocity = 5f;

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
                if (Death != null)
                {
                    Death();
                    Death = null;
                }
                World.RemoveObject(this);
                return;
            }

            if (BloodEmitter != null && Invulnerable)
                BloodEmitter.Position = Position;

            if (_jump)
            {
                if (Math.Abs(Velocity.Y) > JumpMaxVelocity || ClimbLadder)
                {
                    _jump = false;
                }
                else
                    Move(new Vector2(0, -1), JumpAcceleration * dt, false);
            }

            if (Invulnerable)
            {
                _invulnerabilityDuration_dt += dt;
            }
        }

        public virtual void AddHealth(float amount)
        {
            Health += amount;
        }

        public virtual bool TakeDamage(float amount, Direction direction, float push = 0.0f, bool createBlood = false, float bloodSpeed = 0.0f, int bloodCount = 0)
        {
            if (InvulnerabilityDuration <= _invulnerabilityDuration_dt)
            {
                Health -= amount;
                if(push > 0)
                    Move(direction == Direction.Right ? new Vector2(1, 0) : new Vector2(-1, 0), push, true);
                _invulnerabilityDuration_dt = 0;

                Vector2 vel = Velocity;
                vel.Y = 0;
                if(createBlood)
                    BloodEmitter = World.BloodSystem.InitEmitter(Position, vel * bloodSpeed,
                                                                  bloodCount, 0f, 1);
                return true;
            }
            return false;
        }

        public void Jump()
        {
            if (OnGround || (OnLadder && ClimbLadder))
            {
                _jump = true;
                ClimbLadder = false;
            }
        }
        public void StopJump()
        {
            _jump = false;
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
