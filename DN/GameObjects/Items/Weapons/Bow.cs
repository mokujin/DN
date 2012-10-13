﻿using System.Drawing;
using Blueberry.Graphics;
using DN.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace DN.GameObjects.Items.Weapons
{
    public class Bow : RangeWeapon
    {
        public float tensionGrowingSpeed = 10;

        public float MaxTension = 18;
        public float MinTension = 2;

        private float _tension = 0;

        public override float ProjectiveSpeed
        {
            get { return base.ProjectiveSpeed*_tension; }
            set { base.ProjectiveSpeed = value; }
        }

        protected override Vector2 ProjectivePosition
        {
            get
            {
                return new Vector2(base.ProjectivePosition.X + (16 - _tension + Size.Width*2f)*HDir,
                                   base.ProjectivePosition.Y);
            }
        }

        private float StringXposition
        {
            get
            {
                if (CurrentProjective == null)
                    return Position.X;

                return CurrentProjective.Position.X - HDir*Size.Width - CurrentProjective.Size.Width;
            }
        }


        private Vector2 NextBowPosition
        {
            get
            {
                return Creature != null
                           ? new Vector2((sbyte) Creature.HDirection*(Size.Width + 32),

                                            Creature.VDirection != 0?
                                         (sbyte) Creature.VDirection*(Size.Height/2): 1)
                           : new Vector2(0, 0);
            }
        }

        private float NextBowDirection
        {
            get { return FunctionHelper.GetDirectionFromVelocity(new Vector2(HDir, VDir)); }
        }


        private float _bowDirection;
        private Vector2 _bowPosition;




        public Bow(GameWorld gameWorld)
            :base(gameWorld)
        {
            PerfermingTimer.Duration = MaxTension;
            PerfermingTimer.UpdateEvent += OnPerfemingTimerUpdate;
            Size = new Size(16, 64);
            _tension = MinTension;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            Vector2 nextBowPosition = NextBowPosition;
            if (_bowPosition.X != nextBowPosition.X)
            {
                _bowPosition.X += nextBowPosition.X / Math.Abs(nextBowPosition.X) * dt * 400;
                if (Math.Abs(_bowPosition.X) > Math.Abs(nextBowPosition.X))
                {
                    _bowPosition.X = nextBowPosition.X;
                }
            }
            if (_bowPosition.Y != nextBowPosition.Y)
            {


                _bowPosition.Y += nextBowPosition.Y / Math.Abs(nextBowPosition.Y) * dt * 400;
                if (Math.Abs(_bowPosition.Y) > Math.Abs(nextBowPosition.Y))
                {
                    _bowPosition.Y = nextBowPosition.Y;
                }
            }

            float nextBowDirection = NextBowDirection;

            if(nextBowDirection != _bowDirection)
            {
                float dir = FunctionHelper.GetSign(nextBowDirection - _bowDirection);
                if (dir == 0)
                    dir = 1;
                _bowDirection += dir*dt * 10;
                if(FunctionHelper.GetSign(nextBowDirection - _bowDirection) != dir)
                {
                    _bowDirection = nextBowDirection;
                }
            }

        }


        public override void DoAction()
        {
            if(DoingAction)
                return;
            base.DoAction();
            _tension = MinTension;
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
            if (MaxTension > _tension)
                _tension += dt * tensionGrowingSpeed;
            if(MaxTension < _tension)  
                _tension = MaxTension;
        }


        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex("bow_sprite"),
                                 new RectangleF(Position.X + _bowPosition.X,
                                 Position.Y + _bowPosition.Y,
                                 Size.Width,
                                 Size.Height), 
                                 RectangleF.Empty,
                                 Color.White,
                                 _bowDirection , new Vector2(0.5f, 0.5f),
                                 false, false);


      //      SpriteBatch.Instance.DrawLine(StringXposition, Position.Y,
          //                                Position.X + Size.Width * HDir, Position.Y + Size.Height / 2, Color4.White, 1.0f);


        }
    }
}
