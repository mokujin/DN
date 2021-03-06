﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
using Blueberry;
namespace DN.GameObjects
{

    public enum HDirection : sbyte
    {
        Left = -1,
        Right = 1,
        No = 0
    }

    public enum VDirection : sbyte
    {
        Up = -1,
        Down = 1,
        No = 0,
    }

    public delegate void CollisionEventHandler(GameObject sender, GameObject gameObject);
    public delegate void DestroyEventHandler();

    public abstract class GameObject:IQuadTreeItem
    {
        public event CollisionEventHandler CollisionWithObjects;

        public bool IgnoreCollisions = false;
        public bool Destroyed = false;

        protected GameWorld World;
        public event DestroyEventHandler DestroyEvent;

        public HDirection HDirection { get; set; }
        public VDirection VDirection { get; set; }

        private float _x, _y;
        public Vector2 Position
        {
            get { return new Vector2(_x, _y); }
            set
            {
                _x = value.X;
                _y = value.Y;
                if (OnPositionChange != null)
                    OnPositionChange(this);
            }
        }

        public Point Cell
        {
            get 
            {
                return new Point((int)(Position.X / 64),(int)( Position.Y / 64)); 
            }
            set 
            {

                _x = value.X * 64 + Size.Width/2; _y = value.Y * 64 + Size.Height/2;
            }
        }

        private Size _size;

        public Size Size
        {
            get { return new Size(_size.Width, _size.Height); }
            set { _size.Width = value.Width; _size.Height = value.Height; }
        }

        Rectangle IQuadTreeItem.Bounds
        {
            get 
            {
                return new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height);
            }
        }

        public virtual RectangleF DrawingBounds
        {
            get { return Bounds;}
        }

        public virtual RectangleF Bounds
        {
            get
            {
                return new RectangleF((Position.X - Size.Width / 2), (Position.Y - Size.Height / 2), Size.Width, Size.Height);
            }
        }

        public Vector2 Center
        {
            get {return new Vector2(Position.X + Size.Width, Position.Y + Size.Height);}
        }

        public float Left { get { return Bounds.Left; }}
        public float Right { get { return Bounds.Right; }}
        public float Top { get { return Bounds.Top; } }
        public float Bottom { get { return Bounds.Bottom; }}

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
            
        }
        public abstract void Draw(float dt);

        public void CollisionWithObject(GameObject sender, GameObject gameObject)
        {
            if(CollisionWithObjects != null)
            CollisionWithObjects(sender, gameObject);
        }

        public virtual void Destroy()
        {
            if(DestroyEvent != null)
                DestroyEvent();
            CollisionsOff();
            World.RemoveObject(this);
        }
        public void CollisionsOff()
        {
            if (OnRemoveFromScene != null)
            OnRemoveFromScene(this);
        }

        public Vector2 GetVectorDirectionFromDirection()
        {
            Vector2 dir = new Vector2((sbyte)HDirection, (sbyte)VDirection);
            dir.Normalize();
            return dir;
        }

        protected abstract void CheckCollisions(ref Vector2 offset, ref Vector2 position);

        public event PositionChangeHandler OnPositionChange;
        public event RemoveFromSceneHandler OnRemoveFromScene;
    }
}
