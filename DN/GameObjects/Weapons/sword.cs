using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures;
using OpenTK;
using OpenTK.Graphics;

namespace DN.GameObjects.Weapons
{
    public class Sword:Weapon
    {
        private float _offset = -2;
        private float _rotation = 0;
        private sbyte dir;

        public override RectangleF Bounds
        {
            get
            {
                return new RectangleF((X - Size.Width / 2 + _offset * dir * 1.5f), (Y - Size.Height / 2), Size.Width, Size.Height);
            }
        }

        public Sword(GameWorld gameWorld, Creature creature)
            :base(gameWorld, creature)
        {
            Size = new Size(40, 20);
        }

        public override void Update(float dt)
        {
            base.Update(dt);       
        }

        public override void Draw(float dt)
        {
            dir = (sbyte)Creature.MovementDirection;

            SpriteBatch.Instance.DrawTexture(CM.I.tex("sword_sprite"),
                                             new RectangleF(
                                                 X + _offset * dir
                                                 ,Y, Size.Width, Size.Height),
                                             RectangleF.Empty,
                                             Color.White,
                                             dir * _rotation,
                                             dir == -1 ? new Vector2(1f, 0.5f): new Vector2(0,0.5f),
                                             false, false);
            //SpriteBatch.Instance.OutlineRectangle(Bounds, Color4.White);
        }

        public override void StartAttack()
        {

            if (!CanAttack) return;
            _offset = 0;
            _rotation = 4.71f;
            base.StartAttack();
        }

        protected override void PerformAttack(float dt)
        {
            _offset += 250 * dt;
            _rotation += 10 * dt;
            base.PerformAttack(dt);
        }

        protected override void FinishAttack()
        {
            _offset = 0;
            _rotation = 0;
        }
    }
}
