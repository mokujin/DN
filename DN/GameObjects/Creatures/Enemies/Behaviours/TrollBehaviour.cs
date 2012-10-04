using Blueberry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.Helpers;
using OpenTK;

namespace DN.GameObjects.Creatures.Enemies.Behaviours
{
    public class TrollBehaviour:IBehaviour
    {
        private bool _sawPlayer;


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

        private Timer _moveTimer;
        private Timer _waitTimer;

        void IBehaviour.Initialize()
        {
            _moveTimer = new Timer();
            _moveTimer.Duration = 5;
            _moveTimer.TickEvent += OnMoveTimerTickEvent;
            _moveTimer.UpdateEvent += OnMoveTimerUpdateEvent;
            _moveTimer.Run();
            _waitTimer = new Timer();
            _waitTimer.Duration = 7;
            _waitTimer.TickEvent += OnWaitTimerTickEvent;

            Creature.Direction = RandomTool.RandBool() ? Direction.Right: Direction.Left;
            Creature.CollisionWithTiles += Creature_CollisionWithTiles;

        }

        private void OnMoveTimerUpdateEvent(float dt)
        {
            Creature.Move(new Vector2((sbyte)Creature.Direction, 0), Creature.Acceleration * dt);
        }

        private void OnWaitTimerTickEvent()
        {
            _moveTimer.Run();
        }

        private void OnMoveTimerTickEvent()
        {
            if (RandomTool.RandBool())
            {
                _moveTimer.Run();
                _moveTimer.Duration = RandomTool.RandInt(1, 4);
                Creature.Direction = RandomTool.RandBool() ? Direction.Right : Direction.Left;
            }
            else
            {
                _moveTimer.Duration = RandomTool.RandInt(1, 4);
                _waitTimer.Run();
            }
        }

        private void Creature_CollisionWithTiles(OpenTK.Vector2 velocity, CollidedCell collidedCell)
        {
            if (collidedCell.Direction.X != 0)
            {
                if (!_sawPlayer)
                    Creature.Direction = (Direction)(sbyte)((sbyte)(Creature.Direction) * -1);
                else
                {
                    Creature.Jump();
                    //todo: jump
                }

            }
        }

        void IBehaviour.Update(float dt)
        {

            if (!_sawPlayer)
            {
                if (LineOfSight.Get(GameWorld.TileMap, Creature.Cell, Hero.Cell))
                {
                    _sawPlayer = true;
                }
                else
                {
                    _waitTimer.Update(dt);
                    _moveTimer.Update(dt);
                }
            }
            else
            {
                Vector2 dir;
                if (Hero.X > Creature.X)
                {
                    dir = new Vector2(1, 0);
                }
                else
                {
                    dir = new Vector2(-1, 0);
                }
                Creature.Move(dir, Creature.Acceleration * dt);
            }
        }
    }
}
