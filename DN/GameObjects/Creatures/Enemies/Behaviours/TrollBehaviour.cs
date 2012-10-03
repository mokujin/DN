using Blueberry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.Helpers;

namespace DN.GameObjects.Creatures.Enemies.Behaviours
{
    public class TrollBehaviour:IBehaviour
    {

        public GameWorld GameWorld
        {
            get;
            set;
        }

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

        private Timer _changeDirectionTimer;
        private Timer _waitTimer;

        void IBehaviour.Initialize()
        {
            _changeDirectionTimer = new Timer();

            Creature.Direction = RandomTool.RandBool() ? Direction.Right: Direction.Left;
            Creature.CollisionWithTiles += Creature_CollisionWithTiles;

        }

        private void Creature_CollisionWithTiles(OpenTK.Vector2 velocity, CollidedCell collidedCell)
        {

        }

        void IBehaviour.Update(float dt)
        {

        }

    }
}
