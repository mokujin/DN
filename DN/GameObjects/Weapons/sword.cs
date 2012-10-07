﻿using System;
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
            CollisionWithObjects += OnCollision;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (NoCreature)
                _rotation += 10 * dt;
        }

        public override void Draw(float dt)
        {
            if(Creature != null)
                dir = (sbyte)Creature.Direction;
            else
                dir = 1;

            SpriteBatch.Instance.DrawTexture(CM.I.tex("sword_sprite"),
                                             new RectangleF(
                                                 X + _offset * dir
                                                 ,Y, Size.Width, Size.Height),
                                             RectangleF.Empty,
                                             Color.White,
                                             dir * _rotation,
                                             dir == -1 ? new Vector2(1f, 0.5f): new Vector2(0,0.5f),
                                             false, false);
          //  SpriteBatch.Instance.OutlineRectangle(Bounds, Color4.White);
        }


        private void OnCollision(GameObject sender, GameObject gameObject)
        {
            if (gameObject is Creature)
            {
                var creature = gameObject as Creature;
                if(creature != this.Creature)
                if (Attacking)
                {
                    if(creature.TakeDamage(Damage, Direction, Damage*20, true, 1.0f, 6))
                    {
                        //play sound
                    }
                }
            }
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
            _offset += 150 * dt;
            _rotation += 6 * dt;
        }

        protected override void FinishAttack()
        {
            _offset = 0;
            _rotation = 0;
        }
    }
}
