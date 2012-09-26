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
            Creature.CollisionWtihObjects += BatCollisionWithHero;
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
        static private void BatCollisionWithHero(GameObject sender, GameObject gameObject)
        {
            Creature bat = (Creature)sender;
            bat.MaxVelocity = new Vector2(0, 0);
        }
    }
}
