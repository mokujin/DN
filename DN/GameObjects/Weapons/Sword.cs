using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures;
namespace DN.GameObjects.Weapons
{
    public class Sword:Weapon
    {        
        public Sword(GameWorld gameWorld, Creature creature)
            :base(gameWorld, creature)
        {
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Draw(float dt)
        {
            throw new NotImplementedException();
        }

        protected override void FinishAttack()
        {
            
        }
    }
}
