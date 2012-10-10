using Blueberry;
using Blueberry.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DN.GUI;
using DN.GameObjects.Items;
using OpenTK.Graphics;
using OpenTK.Input;
using DN.GameObjects.Creatures;
using DN.LevelGeneration;
using DN.GameObjects;
using OpenTK;
using DN.GameObjects.Creatures.Enemies;
using System.IO;
using DN.Effects;
using OpenTK.Graphics.OpenGL;

namespace DN
{
    public class GameWorld
    {


        public int Width{get; private set;}
        public int Height{get; private set;}
        public TileMap TileMap { get; private set; }
        private GUIManager _guiManager;

        public Hero Hero
        {
            get;
            private set;
        }

        public const float G = 15f; // gravity acceleration;
        public static readonly Vector2 GravityDirection = new Vector2(0, 1);
        
        private List<GameObject> _gameObjects;
        private QuadTree<GameObject> _quadTree;

        private Queue<GameObject> _addNewObjectsQueue;
        private Queue<GameObject> _deleteObjectsQueue;

        public Camera Camera { get; private set; }
        private float _scale = 1.0f;
        private float _alphaEffect = 0;

        ParallaxBackground background;
        public BloodSystem BloodSystem;
        MagicBackground mback;

        public GameWorld(int width, int height)
        {
            Width = width;
            Height = height;
            TileMap = new TileMap(Width, Height);


            _gameObjects = new List<GameObject>();
            _quadTree = new QuadTree<GameObject>(new Rectangle(0,0, Width * 64, Height * 64));
            _quadTree.MaxGeneration = 4;
            _addNewObjectsQueue = new Queue<GameObject>();
            _deleteObjectsQueue = new Queue<GameObject>();

            Camera = new Camera(Game.g_screenSize, new Point(Game.g_screenSize.Width / 2, Game.g_screenSize.Height / 2), true);
            Camera.ScaleTo(1f);
            Camera.MoveSpeed = 7;
            
            var lg = new LevelGenerator
                         {
                             RoomsMaxWidth = 10,
                             RoomsMaxHeight = 15,
                             RoomCount = 0,
                             Scale = 0.5f,
                             WallSmoothing = 100f
                         };
            lg.Generate(this);
            /*
            for (int i = 0; i < width; i++)
            {
                TileMap[0, i] = TileMap[width - 1, i] = CellType.Wall;
            }
            for (int j = 0; j < height; j++)
            {
                TileMap[j, 0] = TileMap[j, height - 1] = CellType.Wall;
            }
            */
            //TileMap.PrintDebug();
            // Console.ReadKey();
            InsertHero();

            background = new ParallaxBackground(this);
            BloodSystem = new BloodSystem(this);
            BloodSystem.Init();
            BloodSystem.BlendWith(back);

            mback = new MagicBackground();

            for (int i = 0; i < 100; i++)
            {
                Creature bat = EnemiesFabric.CreateEnemy(this,RandomTool.RandBool()? EnemyType.Bat : EnemyType.Troll);
                bat.Cell = GetRandomPoint();   
            }

            for (int i = 0; i < 1; i++)
            {
                Potion potion = new Potion(this, PotionType.Healing, 3);
                potion.Cell =GetRandomPoint();
            }


            _guiManager = new GUIManager();
            HealthBar healthBar = new HealthBar(Hero)
                                      {
                                          Y = Game.g_screenRect.Bottom - 48,
                                          X = Game.g_screenRect.Left + 48
                                      };
            _guiManager.Add(healthBar);

            UpdateObjectsEnqueues();
        }

        public void Update(float dt)
        {
            BloodSystem.Update(dt);

            Camera.MoveTo(Hero.Position);

            foreach (var gameObject in _gameObjects)
            {
               gameObject.Update(dt);
            }
          //  Parallel.ForEach(_gameObjects, gameObject => gameObject.Update(dt));
            CheckCollisionsWithObjects();

            Vector2 vel = Hero.GetVelocity();
            if(vel.Y > 10)
            {
                if (_scale > 0.5f)
                    _scale -= dt;
                Game.g_Gamepad.Vibrate(0.0f, 0.1f, 0.2f);
            }
            else
            {
                _scale = 1f;
            }
            Camera.ScaleTo(_scale);

            Camera.Update(dt);

            UpdateObjectsEnqueues();

            background.Update(dt);
            _guiManager.Update(dt);

            if(Hero.IsDead)
            {
                _alphaEffect += dt;
            }
        }
        Color[] colors = { Color.Red, Color.Green, Color.Yellow, Color.Violet, Color.Pink, Color.White, Color.Black, Color.Blue, Color.Brown };
        private void VisualizeQuadTree()
        {
            List<QuadTreeNode<GameObject>> nodes = _quadTree.Nodes;
            foreach (var n in nodes)
            {
                SpriteBatch.Instance.OutlineRectangle(n.Area, colors[n.Generation]);
            }
        }
        private void UpdateObjectsEnqueues()
        {
            while (_addNewObjectsQueue.Count > 0)
            {
                GameObject gameObject = _addNewObjectsQueue.Dequeue();
                _gameObjects.Add(gameObject);
                if(!gameObject.IgnoreCollisions)
                    _quadTree.Insert(gameObject);
            }

            while (_deleteObjectsQueue.Count > 0)
            {
                GameObject obj = _deleteObjectsQueue.Dequeue();
                obj.Destroyed = true;
                _gameObjects.Remove(obj);
             //   obj.CollisionsOff();//.RemoveItem(obj);
            }
        }

        Texture back = new Texture(Game.g_screenSize);
        public void Draw(float dt)
        {
            
            GL.ClearColor(0, 0, 0, 0);
            SpriteBatch.Instance.Begin(Camera.GetViewMatrix());
            RenderTiles(dt);
            SpriteBatch.Instance.End(back, true, true);

            BloodSystem.PredrawBloodTexture(dt);

            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            mback.Draw(dt);
            background.Draw(dt);

            BloodSystem.DrawBackground(dt);
            if (!Hero.IsDead)
            {
                SpriteBatch.Instance.Begin(Camera.GetViewMatrix());
                var rect = Camera.BoundingRectangle;
                foreach (var gameObject in _gameObjects)
                    if (gameObject.Bounds.IntersectsWith(rect))
                        gameObject.Draw(dt);
                BloodSystem.DrawParticles(dt);
                SpriteBatch.Instance.End();
            }

            if (Hero.IsDead)
            {
                SpriteBatch.Instance.Begin();

                    SpriteBatch.Instance.FillRectangle(Game.g_screenRect, new Color4(0, 0, 0, _alphaEffect));

                    SpriteBatch.Instance.PrintText(CM.I.Font("Big"), "You are dead!", Game.g_screenSize.Width / 4,
                                                   Game.g_screenSize.Height / 4, new Color4(255, 255, 255, _alphaEffect));

                SpriteBatch.Instance.End();
            }
            SpriteBatch.Instance.Begin();
            _guiManager.Draw(dt);
            SpriteBatch.Instance.End();
            SpriteBatch.Instance.Begin(Camera.GetViewMatrix());
            //VisualizeQuadTree();
            SpriteBatch.Instance.End();

        }
        private void RenderTiles(float dt)
        {
            Rectangle rect = Camera.BoundingRectangle;
            rect.X /= 64;
            rect.Y /= 64;
            rect.Width /= 64;
            rect.Height /= 64;

            rect.Width += 2;
            rect.Height += 2;

            TileMap.Draw(rect);
        }




        public void CheckCollisionsWithObjects()
        {
            Parallel.ForEach(_gameObjects, gO1 =>
         //   foreach (var gO1 in _gameObjects)
            {
                {
                    List<GameObject> list = _quadTree.Query((gO1 as IQuadTreeItem).Bounds);

                    foreach (var gameObject in list)
                    {
                        if(gameObject != gO1)
                            gameObject.CollisionWithObject(gameObject, gO1);
                    }
                }
            }
               );
        }

        internal List<CollidedCell> GetCollisionsWithTiles(RectangleF rectangle)
        {
            return GetCollisionsWithTiles(new Rectangle((int)Math.Round(rectangle.X),
                                               (int)Math.Round(rectangle.Y),
                                               (int)Math.Round(rectangle.Width),
                                               (int)Math.Round(rectangle.Height)));
        }

        internal List<CollidedCell> GetCollisionsWithTiles(Rectangle rectangle)
        {

            var tilesToCheck = TileMap.GetRectanglesAround(new Point(rectangle.X / 64,
                                                                     rectangle.Y / 64), CellType.Free);

            var list = new List<CollidedCell>();
            foreach (Rectangle rect in tilesToCheck)
            {
                if (rect.IntersectsWith(rectangle)) 
                    list.Add(new CollidedCell(rect, TileMap[rect.X / 64, rect.Y / 64]));
            }
            return list;
        }


        public void InsertHero()
        {
            Hero = new Hero(this);
            Point p = GetRandomPoint();
            Hero.Position = new Vector2((p.X * 64) + 32, (p.Y * 64) + 32);
            Hero.CollisionWithTiles += HeroOnCollisionWithTiles;
            Hero.TakeDamageEvent += HeroOnTakeDamageEvent;
            Camera.MoveTo(Hero.Position);
        }

        public Point GetRandomPoint()
        {
            Point p;
            do
            {
                p = new Point(RandomTool.RandInt(0, Width), RandomTool.RandInt(0, Height));
            } while (!TileMap.IsFree(p));

            return p;
        }

        public void AddObject(GameObject gameObject)
        {
            _addNewObjectsQueue.Enqueue(gameObject);
        }
        public void RemoveObject(GameObject gameObject)
        {
            _deleteObjectsQueue.Enqueue(gameObject);
        }

        public float DistanceToObject(GameObject g1, GameObject g2)
        {
            return (float)Math.Sqrt(Math.Pow(g1.X - g2.X, 2) + Math.Pow(g1.Y - g2.Y, 2)); ;
        }
        public Vector2 DirectionToObject(GameObject g1, GameObject g2)
        {
            float angle = (float)Math.Atan2(g1.Y - g2.Y, g1.X - g2.X);
            return new Vector2(-(float)Math.Cos(angle), -(float)Math.Sin(angle));
        }

        public bool InRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool InRange(Point cell)
        {
            return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
        }


        private void HeroOnTakeDamageEvent(GameObject sender, float amount)
        {
            Camera.Rumble(0.2f, 4, 4);
            Game.g_Gamepad.Vibrate(0.6f, 0.6f, 0.2f);
        }
        private void HeroOnCollisionWithTiles(CollidableGameObject sender, CollidedCell collidedCell)
        {

            if (collidedCell.CellType == CellType.Wall)
            {
                if (collidedCell.Direction.Y == 1)
                    if (sender.GetVelocity().Y >= 10)
                    {
                        Camera.Rumble(0.2f, 8, 4);
                        Game.g_Gamepad.Vibrate(0.8f, 0.8f, 0.2f);
                    }
            }
        }

    }
    

    public class CollidedCell
    {
        public Rectangle Rectangle;
        public CellType CellType;

        /// <summary>
        /// Determines in which directection object collided this tile
        /// </summary>
        public Point Direction;

        public CollidedCell(Rectangle rect, CellType cellType)
        {
            CellType = cellType;
            Rectangle = rect;
        }
    }
}
