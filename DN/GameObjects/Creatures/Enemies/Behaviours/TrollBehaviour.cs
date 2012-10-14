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

            Creature.HDirection = RandomTool.RandBool() ? HDirection.Right: HDirection.Left;
            Creature.CollisionWithTiles += Creature_CollisionWithTiles;

        }

        private void OnMoveTimerUpdateEvent(float dt)
        {
            Creature.Move(new Vector2((sbyte)Creature.HDirection, 0), Creature.Acceleration * dt);
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
                Creature.HDirection = RandomTool.RandBool() ? HDirection.Right : HDirection.Left;
            }
            else
            {
                _moveTimer.Duration = RandomTool.RandInt(1, 4);
                _waitTimer.Run();
            }
        }

        private void Creature_CollisionWithTiles(CollidableGameObject velocity, CollidedCell collidedCell)
        {
            if (collidedCell.Direction.X != 0)
            {
                if (!_sawPlayer)
                    Creature.HDirection = (HDirection)((sbyte)(Creature.HDirection) * -1);
                else
                {
                    Creature.Jump();
                }
            }
        }

        void IBehaviour.Update(float dt)
        {

            if (!_sawPlayer)
            {
                if (FunctionHelper.GetLineOfSight(GameWorld.TileMap, Creature.Cell, Hero.Cell))
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
                Vector2 dir = Hero.Position.X > Creature.Position.X ? new Vector2(1, 0) : new Vector2(-1, 0);
                Creature.Move(dir, Creature.Acceleration * dt);
            }
        }
    }
}
