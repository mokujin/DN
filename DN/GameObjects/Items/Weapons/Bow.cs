using DN.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Items.Weapons
{
    public class Bow:RangeWeapon
    {
        private float tensionGrowingSpeed = 5;
        float _maxTension = 7;
        float _minTension = 2;
        float tension = 0;

        public override float ProjectiveSpeed
        {
            get
            {
                return base.ProjectiveSpeed * tension;
            }
            set
            {
                base.ProjectiveSpeed = value;
            }
        }

        public Bow(GameWorld gameWorld)
            :base(gameWorld)
        {
            PerfermingTimer.Duration = _maxTension;
            PerfermingTimer.UpdateEvent += OnPerfemingTimerUpdate;
        }


        public override void DoAction()
        {
            if(DoingAction)
                return;
            base.DoAction();
            tension = _minTension;
        }
        public override void FinishAction()
        {
            if (!DoingAction)
                return;
            base.FinishAction();
            PerfermingTimer.Stop();
        }
        private void OnPerfemingTimerUpdate(float dt)
        {
            if (_maxTension > tension)
                tension += dt * tensionGrowingSpeed;
            if(_maxTension < tension)
                tension = _maxTension;
        }


        public override void Draw(float dt)
        {
            //SpriteBatch.Instance.DrawTexture("", Position, Color4.White);
        }
    }
}
