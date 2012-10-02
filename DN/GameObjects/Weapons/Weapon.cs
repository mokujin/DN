﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures;

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
            get { return AttackStarted; }
        }

        public Direction Direction
        {
            get { return Creature.Direction; }
        }

        protected float _elapsed;
        
        protected Creature Creature; // creature which using this weapon

        protected bool AttackStarted;

        protected bool CanAttack
        {
            get{return _elapsed >= AttackSpeed && !AttackStarted;}
        }


        private float _attackSpeed;
        public float AttackSpeed
        {
            get 
            {
                return _attackSpeed; 
            }
            set
            {
                _attackSpeed = value;
                _elapsed = value;
            }
        }
        public float TimeToFinishAttack;
        public float Damage
        {
            get;
            set;
        }

        public Weapon(GameWorld gameWorld, Creature creature)
            :base(gameWorld)
        {
            Creature = creature;
            _elapsed = AttackSpeed;

        }

        public override void Update(float dt)
        {
            if (AttackStarted)
                PerformAttack(dt);
            else
            if(_elapsed < AttackSpeed)
                _elapsed += dt;
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

            _elapsed = 0;
            AttackStarted = true;
        }
        protected virtual void PerformAttack(float dt)
        {
            _elapsed += dt;
            if (_elapsed >= TimeToFinishAttack)
            {
                _elapsed = 0;
                AttackStarted = false;
                FinishAttack();
            }
        }
        protected abstract void FinishAttack();
    }
}
