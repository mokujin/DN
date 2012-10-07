using Blueberry;
using Blueberry.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Hero Hero
        {
            get;
            private set;
        }

        public const float G = 15f; // gravity acceleration;
        public static readonly Vector2 GravityDirection = new Vector2(0, 1);
        private List<GameObject> _gameObjects;
        private Queue<GameObject> _addNewObjectsQueue;
        private Queue<GameObject> _deleteObjectsQueue;

        public Camera Camera { get; private set; }

        private float _alphaEffect = 0;

        ParallaxBackground background;
       public BloodSystem BloodSystem;

        public GameWorld(int width, int height)
        {
            Game.g_Keyboard.KeyDown += g_Keyboard_KeyDown;
            Width = width;
            Height = height;
            TileMap = new TileMap(Width, Height);

            _gameObjects = new List<GameObject>();
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
         //   TileMap.PrintDebug();
           // Console.ReadKey();
            InsertHero();

            background = new ParallaxBackground(this);
            BloodSystem = new BloodSystem(this);
            BloodSystem.Init();
            BloodSystem.BlendWith(back);

            for (int i = 0; i < 30; i++)
            {
                Creature bat = EnemiesFabric.CreateEnemy(this, EnemyType.Troll);
                bat.Cell = GetRandomPoint();   
            }
            
        }

        void g_Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
           // BloodSystem.InitEmitter(Hero.Position, Vector2.UnitX, 3, 0.4f, 2);
        }

        public void InsertHero()
        {
            Hero = new Hero(this);
            Point p = GetRandomPoint();
            Hero.Position = new Vector2((p.X * 64)+32, (p.Y * 64)+32);
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
            return (float)Math.Sqrt(Math.Pow(g1.X - g2.X,2) + Math.Pow(g1.Y - g2.Y,2));;
        }
        public Vector2 DirectionToObject(GameObject g1, GameObject g2)
        {
            float angle = (float)Math.Atan2(g1.Y - g2.Y, g1.X - g2.X);
            return new Vector2(-(float)Math.Cos(angle), -(float)Math.Sin(angle));  
        }

        public void Update(float dt)
        {
            BloodSystem.Update(dt);

            Camera.MoveTo(Hero.Position);

           // foreach (var gameObject in _gameObjects)
            //    gameObject.Update(dt);

            Parallel.ForEach(_gameObjects, gameObject => gameObject.Update(dt));
            CheckCollisionsWithObjects();

            if (Game.g_Keyboard[Key.Plus])
                Camera.ScaleOn(0.01f);
            if (Game.g_Keyboard[Key.Minus])
                Camera.ScaleOn(-0.01f);

            

            Camera.Update(dt);
            UpdateObjectsEnqueues();

            background.Update(dt);

            if(Hero.IsDead)
            {
                _alphaEffect += dt;
            }
        }

        private void UpdateObjectsEnqueues()
        {
            while (_addNewObjectsQueue.Count > 0)
                _gameObjects.Add(_addNewObjectsQueue.Dequeue());

            while (_deleteObjectsQueue.Count > 0)
                _gameObjects.Remove(_deleteObjectsQueue.Dequeue());
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


        public bool InRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool InRange(Point cell)
        {
            return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
        }

        public void CheckCollisionsWithObjects()
        {
            Parallel.ForEach(_gameObjects, gO1 => Parallel.ForEach(_gameObjects, gO2 =>
                                                                                     {
                                                                                         if (gO1 != gO2)
                                                                                             if (gO1.Bounds.IntersectsWith(gO2.Bounds))
                                                                                             {
                                                                                                 gO1.CollisionWithObject(gO1, gO2);
                                                                                             }
                                                                                     }));
        }
        //internal List<GameObject> GetCollisionsWithObjects(GameObject gameObject)
        //{
        //    List<GameObject> list = new List<GameObject>();
        //    foreach (GameObject gO in _gameObjects)
        //    {
        //        if (gO != gameObject)
        //        {
        //            if (gO.Bounds.IntersectsWith(gameObject.Bounds))
        //            {
        //                list.Add(gO);
        //            }
                    
        //        }
        //    }
        //    return list;
        //}

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
                                                                     rectangle.Y / 64));

            var list = new List<CollidedCell>();
            foreach (Rectangle rect in tilesToCheck)
            {
                if (rect.IntersectsWith(rectangle)) 
                    list.Add(new CollidedCell(rect, TileMap[rect.X / 64, rect.Y / 64]));
            }
            return list;
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
