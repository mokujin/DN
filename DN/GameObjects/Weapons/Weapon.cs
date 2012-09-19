using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures;

namespace DN.GameObjects.Weapons
{
    public abstract class Weapon:GameObject
    {
        public bool Attacking
        {
            get { return AttackStarted; }
        }

        private float _elapsed;
        
        protected Creature Creature; // creature which using this weapon

        protected bool AttackStarted;

        protected bool CanAttack
        {
            get{return _elapsed >= AttackSpeed && !AttackStarted;}
        }

        public float AttackSpeed;
        public float TimeToFinishAttack;

        public Weapon(GameWorld gameWorld, Creature creature)
            :base(gameWorld)
        {
            Creature = creature;

        }

        public override void Update(float dt)
        {
            if (AttackStarted)
                PerformAttack(dt);
            else
            if(_elapsed < AttackSpeed)
                _elapsed += dt;
            Position = Creature.Position;
        }
        public virtual void StartAttack()
        {
            if (CanAttack)
            {
                _elapsed = 0;
                AttackStarted = true;
            }
        }
        protected virtual void PerformAttack(float dt)
        {
            _elapsed += dt;
            if (_elapsed >= TimeToFinishAttack)
            {
                _elapsed = 0;
                FinishAttack();
            }
        }
        protected abstract void FinishAttack();
    }
}
