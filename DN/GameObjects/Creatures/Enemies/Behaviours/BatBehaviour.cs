using Blueberry;
using DN.GameObjects.Weapons;
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
            Creature.CollisionWithObjects += BatOnCollision;
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

        private void Creature_CollisionWithTiles(Vector2 velocity, CollidedCell collidedCell)
        {
            if (collidedCell.CellType == CellType.Wall)
                ChangeDirectionTimer.Finish();
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
                Creature.Move(dir, 4*dt); //TODO: Remove constant
                Creature.Direction = dir.X > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                ChangeDirectionTimer.Update(dt);
            }
        }

        private void OnTimerTick()
        {
            _direction = RandomTool.NextUnitVector2();
            Creature.Direction = _direction.X > 0 ? Direction.Right : Direction.Left;
        }
        private void OnTimerUpdate(float dt)
        {
            Creature.Move(_direction, 4 * dt);
        }


        
        //just for test
        private void BatOnCollision(GameObject sender, GameObject gameObject)
        {
            if (gameObject is Weapon)
            {
                var weapon = gameObject as Weapon;
                if (weapon.Attacking)
                {
                   Creature.TakeDamage(weapon.Damage, weapon.Direction, 20);
                }
            }
            else if (gameObject is Hero)
            {
                Hero hero = (Hero)gameObject;
                bool t = hero.TakeDamage(1, Creature.Direction, 5);
                if(t)
                    Creature.MoveInOppositeDirection();
            }
        }
    }
}
