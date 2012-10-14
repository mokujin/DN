using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures;
using DN.Helpers;
using OpenTK;

namespace DN.GameObjects.Items
{
    public abstract class Item:CollidableGameObject
    {
        public bool DoingAction
        {
            get { return PerfermingTimer.Running; }
        }
        public bool CanDoAction
        {
            get { return !IntervalTimer.Running && !PerfermingTimer.Running; }
        }

        private Creature _creature;

        public Creature Creature
        {
            get { return _creature; }
            protected set
            {
                    _creature = value;
            }
        }

        public float IntervalDuration {set { IntervalTimer.Duration = value; }}
        protected float PerformingDuration { set { PerfermingTimer.Duration = value; } }

        protected readonly Timer IntervalTimer;
        protected readonly Timer PerfermingTimer;


        public Item(GameWorld gameWorld)
            :base(gameWorld)
        {
            IntervalTimer = new Timer();
            PerfermingTimer = new Timer();
            GravityAffected = true;
            MaxVelocity = new Vector2(10, 10);
            Friction = 1.0f;
        }
        public virtual void SetOwner(Creature creature, bool changeParameters = true)
        {
            _creature = creature;

            if(!changeParameters) return;

            if (_creature == null)
            {
                GravityAffected = true;
                IgnoreWalls = false;
            }
            else
            {
                Velocity = new Vector2(0, 0);
                Position = Creature.Position;
                GravityAffected = false;
                IgnoreWalls = true;
            }
        }

        public override void Destroy()
        {
            base.Destroy();
           // if(Creature != null)
           //     Creature.DropItem();
        }

        public virtual void DoAction()
        {
            if(!CanDoAction) return;

            IntervalTimer.Run();
            PerfermingTimer.Run();
        }
        public virtual void FinishAction()
        {

        }


        public override void Update(float dt)
        {
            if(Destroyed)
                throw new Exception("Destroyed item cannot be used");



            if (!DoingAction)
                IntervalTimer.Update(dt);
            else
                PerfermingTimer.Update(dt);

            base.Update(dt);
        }
    }
}
