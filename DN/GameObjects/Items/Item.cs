using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures;
using DN.Helpers;

namespace DN.GameObjects.Items
{
    public abstract class Item:GameObject
    {
        public bool DoingAction
        {
            get { return PerfermingTimer.Running; }
        }
        public bool CanDoAction
        {
            get { return !IntervalTimer.Running && !PerfermingTimer.Running; }
        }
        public Creature Creature { get; set; }



        public float IntervalDuration {set { IntervalTimer.Duration = value; }}
        protected float PerformingDuration { set { PerfermingTimer.Duration = value; } }

        protected readonly Timer IntervalTimer;
        protected readonly Timer PerfermingTimer;


        public Item(GameWorld gameWorld)
            :base(gameWorld)
        {
            IntervalTimer = new Timer();
            PerfermingTimer = new Timer();
        }

        public virtual void DoAction()
        {
            if(!CanDoAction) return;

            IntervalTimer.Run();
            PerfermingTimer.Run();
        }

        public override void Update(float dt)
        {
            if (!DoingAction)
                IntervalTimer.Update(dt);
            else
                PerfermingTimer.Update(dt);


        }
    }
}
