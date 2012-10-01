using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DN.GameObjects
{
    public enum Direction : sbyte
    {
        Left = -1,
        Right = 1
    }


    
    //dunno how to name it
    public abstract class CollidableGameObject:GameObject
    {
        

        public bool GravityAffected = true;
        

        protected List<CollidedCell> Collisions;


        public Vector2 MaxLadderVelocity;
        public Vector2 MaxVelocity;

        protected Vector2 Velocity;

        public float Friction;
        public float LadderFriction;

        public bool OnGround
        {
            get
            {
                return Collisions.Any(p => p.CellType == CellType.Wall
                                          && p.Direction.Y == 1);
            }
        }

        public bool OnStairs
        {
            get { return Collisions.Any(p => p.CellType == CellType.Ladder || p.CellType == CellType.VRope); }
        }

        //private Direction _lastDirection;
        public Direction Direction
        {
            //get
            //{
            //    Vector2 direction = Velocity;
            //    direction.Normalize();
            //    if (!float.IsNaN(direction.X))
            //    {
            //        _lastDirection = direction.X > 0 ? Direction.Right : Direction.Left;
            //    }
            //    return _lastDirection;
            //}
            get; set;
        }

        public CollidableGameObject(GameWorld gameWorld)
            :base(gameWorld)
        {
            Collisions = new List<CollidedCell>();
        }

        public void Move(Vector2 direction, float speed, bool checkOverspeed = true)
        {
            Velocity += direction*speed;

            if(checkOverspeed)
            {
                Vector2 vel = Velocity;
                if (!OnStairs)
                {
                    CheckOverSpeed(ref vel.X, MaxVelocity.X);
                    CheckOverSpeed(ref vel.Y, MaxVelocity.Y);
                }
                else
                {
                    CheckOverSpeed(ref vel.X, MaxLadderVelocity.X);
                    CheckOverSpeed(ref vel.Y, MaxLadderVelocity.Y);
                }
                Velocity = vel;
            }
        }

        public override void Update(float dt)
        {
            if (dt >= 0.1f) return; // duct tape xD sometimes dt is extremly big, so that cause movement lag
            UpdateParametrs(dt);
        }

        private void UpdateParametrs(float dt)
        {

            if (!OnStairs)
            {
                if (GravityAffected)
                {
                    Velocity += World.GravityDirection*World.G*dt;

                    Vector2 velocity = Velocity;

                    CheckOverSpeed(ref velocity.X, MaxVelocity.X);
                    CheckOverSpeed(ref velocity.Y, MaxVelocity.Y);
                    Velocity = velocity;
                }
                if (OnGround)
                    UpdateFriction(ref Velocity.X, Friction, dt);
            }
            else
            {
                UpdateFriction(ref Velocity.Y, LadderFriction, dt);
                UpdateFriction(ref Velocity.X, LadderFriction, dt);
            }

            Vector2 pos = Position;
            Vector2 vel = Velocity;
            Vector2 oldVel = vel;
            //if (!OnStairs)
            //{
            //    CheckOverSpeed(ref vel.X, MaxVelocity.X);
            //    CheckOverSpeed(ref vel.Y, MaxVelocity.Y);
            //}
            //else
            //{
            //    CheckOverSpeed(ref vel.X, MaxLadderVelocity.X);
            //    CheckOverSpeed(ref vel.Y, MaxLadderVelocity.Y);
            //}

            CheckCollisions(ref vel, ref pos);
            Position = pos;
            if(oldVel != vel)
                Velocity = vel;

            Position += Velocity;
        }

        private void UpdateFriction(ref float vel, float friction, float dt)
        {
            if (vel > 0)
            {
                vel -= friction * dt;
                if (vel < 0)
                    vel = 0;
            }
            else if (vel < 0)
            {
                vel += friction * dt;
                if (vel > 0)
                    vel = 0;
            }
        }

        protected override void CheckCollisions(ref Vector2 offset, ref Vector2 position)
        {
            if (IgnoreCollisions) return;
            base.CheckCollisions(ref offset, ref position);

            CheckCollisionsWithTiles(ref offset, ref position);
        }
        

        private void CheckCollisionsWithTiles(ref Vector2 offset, ref Vector2 position)
        {
            Collisions.Clear();
            List<CollidedCell> tilesX = null;
            List<CollidedCell> tilesY = null;
            List<CollidedCell> tiles = null;
            //  if (offset.X != 0)
            {
                float oldOffset = offset.X;
                if (offset.X < 1 && offset.X > 0)
                    offset.X = 1;
                if (offset.X > -1 && offset.X < 0)
                    offset.X = -1;

                tilesX = World.GetCollisionsWithTiles(new RectangleF((Left + offset.X),
                                                                     Top,
                                                                     Size.Width,
                                                                     Size.Height));

                foreach (var cell in tilesX)
                {
                    if (cell.CellType == CellType.Wall)
                    {
                        if (offset.X > 0)
                        {
                            position.X = cell.Rectangle.X - Size.Width / 2;
                            cell.Direction = new Point(1, 0);
                        }
                        else if (offset.X < 0)
                        {
                            position.X = cell.Rectangle.X + cell.Rectangle.Width + Size.Width / 2;
                            cell.Direction = new Point(-1, 0);
                        }
                        offset.X = 0;
                        break;
                    }
                }
                if (offset.X != 0)
                    offset.X = oldOffset;
            }

            //  if (offset.Y != 0)
            {
                float oldOffset = offset.Y;
                if (offset.Y < 1 && offset.Y > 0)
                    offset.Y = 1;
                if (offset.Y > -1 && offset.Y < 0)
                    offset.Y = -1;

                tilesY = World.GetCollisionsWithTiles(new RectangleF((Left),
                                                                     Top + offset.Y,
                                                                     Size.Width,
                                                                     Size.Height));
                foreach (var cell in tilesY)
                {
                    if (cell.CellType == CellType.Wall)
                    {
                        if (offset.Y > 0)
                        {
                            position.Y = cell.Rectangle.Y - Size.Height / 2;
                            cell.Direction = new Point(0, 1);
                        }
                        else if (offset.Y < 0)
                        {
                            position.Y = cell.Rectangle.Y + cell.Rectangle.Height + Size.Height / 2;
                            cell.Direction = new Point(0, -1);
                        }
                        offset.Y = 0;
                        break;
                    }
                }
                if (offset.Y != 0)
                    offset.Y = oldOffset;
            }


            if (offset.X != 0 && offset.Y != 0)
            {
                tiles = World.GetCollisionsWithTiles(new RectangleF((Left + offset.X),
                                                                    Top + offset.Y,
                                                                    Size.Width,
                                                                    Size.Height));
                foreach (var cell in tiles)
                {
                    if (cell.CellType == CellType.Wall)
                    {
                        offset.X = 0;
                        offset.Y = 0;
                    }
                }
            }
            if (tilesX != null)
                Collisions.AddRange(tilesX);
            if (tilesY != null)
                Collisions.AddRange(tilesY);
            if (tiles != null)
                Collisions.AddRange(tiles);
        }

        protected void CheckOverSpeed(ref float velocity, float maxVelocity)
        {
            if (velocity > 0)
            {
                if (velocity > maxVelocity)
                {
                    velocity = maxVelocity;
                }
            }
            else if (velocity < 0)
            {
                if (velocity < -maxVelocity)
                {
                    velocity = -maxVelocity;
                }
            }
        }
    }
}
