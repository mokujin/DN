using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Creatures.Enemies.Behaviours
{
    class BatBehaviour:IBehaviour
    {
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

        public void Update(float dt)
        {
            if (GameWorld.DistanceToObject(Creature, Hero) < 500)
                if (LineOfSight.Get(GameWorld.TileMap, Creature.Cell, Hero.Cell))
                {
                   Vector2 dir = GameWorld.DirectionToObject(Creature, Hero);
                   Creature.Move(dir, 1 * dt);//TODO: Remove constant!
                }
        }
    }
}
