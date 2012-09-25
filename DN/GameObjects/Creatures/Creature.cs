using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blueberry;

namespace DN.GameObjects.Creatures
{
    public enum MovementDirection:sbyte
    {
        Left = -1,
        Right = 1
    }

    public abstract class Creature:GameObject
    {
        public bool GravityAffected = true;
        public bool IgnoreCollisions = false;

        protected bool Landed = false;
        protected List<Vector2> Collisions; 

        protected Vector2 Direction;

        protected Vector2 MaxVelocity;
        protected Vector2 Velocity;

        protected float Friction;

        //protected bool OnStairs;
        //protected bool OnGround;

        public bool OnGround
        {
            get { return Collisions.Any(p => p.Y == 1); }
        }

        public bool OnStairs
        {
            get { return false; }
        }

        public Rectangle DrawingBounds
        {
            get {return new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height);}
        }


        public MovementDirection MovementDirection
        {
            get { return Direction.X > 0 ? MovementDirection.Right : MovementDirection.Left; }
        }


        public Creature(GameWorld gameWorld)
            :base(gameWorld)
        {
            Collisions = new List<Vector2>();
        }

    


        public event Action StandOnStairs;
        public event Action StandOutStairs;
        public event Action StandOnGround;
        public event Action StandOutGround;

        public void Move(Vector2 direction, float speed)
        {
            Direction += direction;
            Direction.Normalize();

            Velocity += direction*speed;
        }

        public override void Update(float dt)
        {
            if (dt >= 0.1f) return; // duct tape xD sometimes dt is extremly big, so that cause movement lag
            UpdateParametrs(dt);


        }

        private void UpdateParametrs(float dt)
        {
            if (Collisions.Count > 0)
            {
                if (Velocity.X > 0)
                {
                    Velocity.X -= Friction;
                    if (Velocity.X < 0)
                        Velocity.X = 0;
                }
            }
            Velocity += World.GravityDirection * World.G * dt;

            Vector2 pos = Position;
            CheckCollisions(ref Velocity, ref pos);
            Position = pos;
            Vector2 vel = Velocity;
            
            CheckOverSpeed(ref vel.X, MaxVelocity.X);
            CheckOverSpeed(ref vel.Y, MaxVelocity.Y);
            Velocity = vel;
            Position += Velocity;
        }

        protected void CheckCollisions(ref Vector2 offset, ref Vector2 position)
        {
            if (IgnoreCollisions) return;

            //List<GameObject> collisionsWithObjectsX = World.GetCollisionsWithObjects(new RectangleF((X + offset.X),
            //                                                                      Y,
            //                                                                      Size.Width,
            //                                                                      Size.Height));

            //List<GameObject> collisionsWithObjectsY = World.GetCollisionsWithObjects(new RectangleF(X,
            //                                                                     Y + offset.Y,
            //                                                                     Size.Width,
            //                                                                     Size.Height));



            //List<GameObject> commonCollision = new List<GameObject>();

        //    Console.WriteLine(offset);

            Collisions.Clear();

            if (offset.X != 0)
            {
                float oldOffset = offset.X;
                if (offset.X < 1 && offset.X > 0)
                    offset.X = 1;
                if (offset.X > -1 && offset.X < 0)
                    offset.X = -1;

                var tilesX = World.GetCollisionsWithTiles(new RectangleF((Left + offset.X),
                                                                         Top,
                                                                         Size.Width,
                                                                         Size.Height));

                foreach (var cell in tilesX)
                {
                    if (cell.CellType == CellType.Wall)
                    {
                            if (offset.X > 0)
                                position.X = cell.Rectangle.X - Size.Width/2;
                            else if (offset.X < 0)
                            {
                                position.X = cell.Rectangle.X + cell.Rectangle.Width + Size.Width/2;
                            }
                            offset.X = 0;
                    }
                }
                if (offset.X != 0)
                    offset.X = oldOffset;
            }

            if (offset.Y != 0)
            {
                float oldOffset = offset.Y;
                if (offset.Y < 1 && offset.Y > 0)
                    offset.Y = 1;
                if (offset.Y > -1 && offset.Y < 0)
                    offset.Y = -1;

                var tilesY = World.GetCollisionsWithTiles(new RectangleF((Left),
                                                                         Top + offset.Y,
                                                                         Size.Width,
                                                                         Size.Height));
                foreach (var cell in tilesY)
                {
                    if (cell.CellType == CellType.Wall)
                    {
                        if (offset.Y > 0)
                        {
                            position.Y = cell.Rectangle.Y - Size.Height/2;
                            Landed = true;
                        }
                        else if (offset.Y < 0)
                        {
                            position.Y = cell.Rectangle.Y + cell.Rectangle.Height + Size.Height / 2;
                        }
                        offset.Y = 0;
                        break;

                    }
                }
                if (offset.Y != 0)
                    offset.Y = oldOffset;
            }



            if(offset.X != 0 && offset.Y !=0)
            {
                var tiles = World.GetCollisionsWithTiles(new RectangleF((Left + offset.X),
                                                         Top + offset.Y,
                                                         Size.Width,
                                                         Size.Height));
                foreach (var cell in tiles)
                {
                    if (cell.CellType == CellType.Wall)
                    {
                        offset.X = 0;
                        offset.Y = 0;
                        Console.WriteLine("Weird");
                    }
                }
            }

            


            //if (offset.X != 0 && offset.Y != 0)
            //{
            //    //if (!this.Bullet)
            //    {
            //        foreach (var item in collisionsWithObjectsX)
            //        {
            //            if (item != this)
            //            {
            //                //if (item.Solid)
            //                {
            //                    offset.X = 0;
            //                    if (Position.X <= item.X)
            //                    {
            //                        Collisions.Add(new Vector2(-1, 0));
            //                    }
            //                    else
            //                    {
            //                        Collisions.Add(new Vector2(1, 0));
            //                    }
            //                }
            //                commonCollision.Add(item);
            //            }
            //        }

            //        foreach (var item in collisionsWithObjectsY)
            //        {
            //            if (item != this)
            //            {
            //                //if (item.Solid)
            //                {
            //                    offset.Y = 0;
            //                    if (Position.Y <= item.Y)
            //                    {
            //                        Collisions.Add(new Vector2(0, -1));
            //                    }
            //                    else
            //                    {
            //                        Collisions.Add(new Vector2(0, 1));
            //                    }
            //                }
            //                if (!commonCollision.Contains(item))
            //                    commonCollision.Add(item);
            //            }
            //        }
            //    }

            //    List<GameObject> collisionsAll = World.GetCollisionsWithObjects(new RectangleF((X + offset.X),
            //                                                                                   Y + offset.Y,
            //                                                                                   Size.Width,
            //                                                                                   Size.Height));

            //    foreach (var item in collisionsAll)
            //    {
            //        if (item != this)
            //        {
            //            //if (item.Solid && !this.Bullet)
            //            {
            //                offset.X = 0;
            //                offset.Y = 0;
            //                break;
            //            }
            //            if (!commonCollision.Contains(item))
            //                commonCollision.Add(item);
            //        }
            //    }

                //if (OnCollision != null)
                //    for (int index = 0; index < commonCollision.Count; index++)
                //    {
                //        var item = commonCollision[index];

                //        OnCollision(item);
                //    }
           // }
        }



        protected void CheckOverSpeed(ref float velocity, float maxVelocity)
        {
            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
        }
      
        // get bounding rectangle with some shift
        public RectangleF GetBoundsWithShift(float sx, float sy)
        {
            RectangleF r = Bounds;
            r.X = (Left + sx);
            r.Y = (Top + sy);
            return r;
        }
        public RectangleF GetBoundsWithShift(Vector2 shift)
        {
            return GetBoundsWithShift(shift.X, shift.Y);
        }
    }
}
