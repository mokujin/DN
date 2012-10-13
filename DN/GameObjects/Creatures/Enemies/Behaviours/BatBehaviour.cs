using Blueberry;
using DN.Effects;
using DN.Helpers;
using OpenTK;

namespace DN.GameObjects.Creatures.Enemies.Behaviours
{
    class BatBehaviour:IBehaviour
    {

        private Timer ChangeDirectionTimer;
        private bool SawPlayer = false;
        private Vector2 _direction;
        

        
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
                       Creature.CollisionWithTiles += Creature_CollisionWithTiles;
            ChangeDirectionTimer = new Timer
                                       {
                                           Repeat = true,
                                           Duration = RandomTool.RandInt(5) + 5
                                       };

            ChangeDirectionTimer.TickEvent += OnTimerTick;
            ChangeDirectionTimer.UpdateEvent += OnTimerUpdate;

            ChangeDirectionTimer.Run(true);

        }

        private void Creature_CollisionWithTiles(CollidableGameObject sender, CollidedCell collidedCell)
        {
            if (collidedCell.CellType == CellType.Wall)
                ChangeDirectionTimer.Finish();
        }

        public void Update(float dt)
        {


            if (!SawPlayer)
                if (GameWorld.DistanceToObject(Creature, Hero) < 500)
                    if (FunctionHelper.GetLineOfSight(GameWorld.TileMap, Creature.Cell, Hero.Cell))
                        SawPlayer = true;

            if (SawPlayer)
            {
                _direction = GameWorld.DirectionToObject(Creature, Hero);
                Creature.Move(_direction, Creature.Acceleration * dt);
                Creature.HDirection = _direction.X > 0 ? HDirection.Right : HDirection.Left;
            }
            else
            {
                ChangeDirectionTimer.Update(dt);
            }
        }

        private void OnTimerTick()
        {
            _direction = RandomTool.NextUnitVector2();
            Creature.HDirection = _direction.X > 0 ? HDirection.Right : HDirection.Left;
        }
        private void OnTimerUpdate(float dt)
        {
            Creature.Move(_direction, Creature.Acceleration * dt);
        }


        
        //just for test
        
    }
}
