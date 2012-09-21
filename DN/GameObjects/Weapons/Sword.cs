using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures;
using OpenTK;

namespace DN.GameObjects.Weapons
{
    public class Sword:Weapon
    {
        private float _offset = 0;

        public Sword(GameWorld gameWorld, Creature creature)
            :base(gameWorld, creature)
        {
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            Console.Clear();
            Console.WriteLine(new Vector2(X + _offset * (sbyte)Creature.Direction, Y));            
        }

        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex("sword_sprite"),
                                             new Vector2(X + _offset * (sbyte)Creature.Direction, Y) ,
                                             Color.White);
        }

        protected override void PerformAttack(float dt)
        {
            base.PerformAttack(dt);
            _offset += 200 * dt;
        }

        protected override void FinishAttack()
        {
            _offset = 0;
        }
    }
}
