using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures.Enemies.Behaviours;


namespace DN.GameObjects.Creatures.Enemies
{
    public class Enemy:Creature
    {
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
        }
    }
}
