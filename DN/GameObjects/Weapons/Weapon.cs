using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures;
using DN.Helpers;

namespace DN.GameObjects.Weapons
{
    public abstract class Weapon:GameObject
    {
        protected bool NoCreature
        {
            get { return Creature == null || Creature.IsDead; }
        }

        public bool Attacking
        {
            get { return _perfermingAttackTimer.Running; }
        }

        public Direction Direction
        {
            get { return Creature.Direction; }
        }

        protected Creature Creature; // creature which using this weapon


        protected bool CanAttack
        {
            get{return !_attackTimer.Running;}
        }

        private readonly Timer _attackTimer;
        private readonly Timer _perfermingAttackTimer;

        public float AttackSpeed
        {
            get
            {
                return _attackTimer.Duration;
            }
            set
            {
                _attackTimer.Duration = value;
            }
        }

        public float TimeToFinishAttack
        {
            get
            {
                return _perfermingAttackTimer.Duration;
            }
            set
            {
                _perfermingAttackTimer.Duration = value;
            }
        }

        public float Damage
        {
            get;
            set;
        }

        public Weapon(GameWorld gameWorld, Creature creature)
            :base(gameWorld)
        {
            Creature = creature;

            _attackTimer = new Timer();
            _perfermingAttackTimer = new Timer();

            _perfermingAttackTimer.TickEvent += FinishAttack;
            _perfermingAttackTimer.UpdateEvent += PerformAttack;
        }

        public override void Update(float dt)
        {
            _attackTimer.Update(dt);
            _perfermingAttackTimer.Update(dt);

            if (NoCreature)
            {
                Y += 1000*dt;
                Creature = null;
            }
            else
                Position = Creature.Position;
        }


        public virtual void StartAttack()
        {
            if (!CanAttack) return;

            _attackTimer.Run();
            _perfermingAttackTimer.Run();
        }

        protected abstract void PerformAttack(float dt);
        protected abstract void FinishAttack();
    }
}
