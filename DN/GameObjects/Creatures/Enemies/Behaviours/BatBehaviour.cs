using DN.GameObjects.Weapons;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Creatures.Enemies.Behaviours
{
    class BatBehaviour:IBehaviour
    {
        private bool SawPlayer = false;

        public Creature Creature
        {
            get;
            set;
        }
        public Hero Hero
        {
            get;
            set;
        }
        public GameWorld GameWorld
        {
            get;
            set;
        }

        public void Initialize()
        {
            Creature.CollisionWithObjects += BatCollisionWithHero;
        }

        public void Update(float dt)
        {
            if (!SawPlayer)
                if (GameWorld.DistanceToObject(Creature, Hero) < 500)
                    if (LineOfSight.Get(GameWorld.TileMap, Creature.Cell, Hero.Cell))
                        SawPlayer = true;

            if (SawPlayer)
            {
                Vector2 dir = GameWorld.DirectionToObject(Creature, Hero);
                Creature.Move(dir, 1*dt); //TODO: Remove constant!
            }
        }

        //just for test
        private void BatCollisionWithHero(GameObject sender, GameObject gameObject)
        {
            if (gameObject is Weapons.Weapon)
            {
                Weapon weapon = gameObject as Weapon;
                if(weapon.Attacking)
                    GameWorld.RemoveObject(sender);
            }
        }
    }
}
