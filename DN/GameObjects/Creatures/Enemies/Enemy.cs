using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures.Enemies.Behaviours;
using Blueberry;

namespace DN.GameObjects.Creatures.Enemies
{
    public class Enemy:Creature
    {
        public string Sprite;
        IBehaviour _behaviour;

        public Enemy(GameWorld gameWorld, IBehaviour behaviour = null)
            :base(gameWorld)
        {
            _behaviour = behaviour;
        }

        public void SetBehaviour(IBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            _behaviour.Update(dt);
        }

        public override void Draw(float dt)
        {
            if(Sprite != null)
                SpriteBatch.Instance.DrawTexture(CM.I.tex(Sprite), Position, Color.White);
        }
    }
}
