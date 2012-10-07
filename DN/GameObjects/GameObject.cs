using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
namespace DN.GameObjects
{

    public enum Direction : sbyte
    {
        Left = -1,
        Right = 1
    }

    public delegate void CollisionEventHandler(GameObject sender, GameObject gameObject);

    public abstract class GameObject
    {
        public event CollisionEventHandler CollisionWithObjects;

        public bool IgnoreCollisions = false;

        protected GameWorld World;


        public Direction Direction { get; set; }

        private float _x, _y;
        
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Vector2 Position
        {
            get { return new Vector2(_x, _y); }
            set
            {
                _x = value.X;
                _y = value.Y;
            }
        }

        public Point Cell
        {
            get 
            {
                return new Point((int)(X / 64),(int)( Y / 64)); 
            }
            set 
            {
                _x = value.X * 64 + Size.Width/2; _y = value.Y * 64 + Size.Width/2; 
            }
        }

        private Size _size;
        public Size Size
        {
            get { return new Size(_size.Width, _size.Height); }
            set { _size.Width = value.Width; _size.Height = value.Height; }
        }

        public virtual RectangleF Bounds
        {
            get
            {
                return new RectangleF((X - Size.Width / 2), (Y - Size.Height / 2), Size.Width, Size.Height);
            }
        }
        public float Left { get { return Bounds.Left; } set { X = value + Size.Width / 2; } }
        public float Right { get { return Bounds.Right; } set { X = value - Size.Width / 2; } }
        public float Top { get { return Bounds.Top; } set { Y = value + Size.Height / 2; } }
        public float Bottom { get { return Bounds.Bottom; } set { Y = value - Size.Height / 2; } }

        public GameObject(GameWorld gameWorld)
        {
            this.World = gameWorld;
            this.World.AddObject(this);
        }
        public virtual void Update(float dt)
        {
            Vector2 pos = Position;
            Vector2 offset = Vector2.Zero;

            CheckCollisions(ref offset,ref pos);

            Position = pos;
        }
        public abstract void Draw(float dt);

        public void CollisionWithObject(GameObject sender, GameObject gameObject)
        {
            if(CollisionWithObjects != null)
            CollisionWithObjects(sender, gameObject);
        }

        protected virtual void CheckCollisions(ref Vector2 offset, ref Vector2 position)
        {
            if (IgnoreCollisions) return;
         //   CheckCollisionsWithObjects(ref offset, ref position);
        }

    }
}
