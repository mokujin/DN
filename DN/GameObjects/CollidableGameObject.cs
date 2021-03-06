﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DN.GameObjects
{


    public delegate void TileCollisionEventHandler(CollidableGameObject sender, CollidedCell collidedCell);
    
    //dunno how to name it
    public abstract class CollidableGameObject:GameObject
    {
        public event TileCollisionEventHandler CollisionWithTiles;

        public bool GravityAffected = true;
        public bool IgnoreWalls = false;
        public bool Freeze = false;
        protected bool ClimbLadder;

        protected List<CollidedCell> Collisions;


        public Vector2 MaxLadderVelocity;
        public Vector2 MaxVelocity;


        protected Vector2 Velocity;

        public float Friction;
        public float LadderFriction;

        public float Acceleration;
        

        public bool OnGround
        {
            get
            {
                return Collisions.Any(p => p.CellType == CellType.Wall
                                          && p.Direction.Y == 1);
            }
        }

        public bool OnLadder
        {
            get { return Collisions.Any(p => p.CellType == CellType.Ladder || p.CellType == CellType.VRope); }
        }


        public CollidableGameObject(GameWorld gameWorld)
            :base(gameWorld)
        {
            Collisions = new List<CollidedCell>();
        }

        public Vector2 GetVelocity()
        {
            return Velocity;
        }


        public void SetMove(Vector2 velocity, bool checkOverspeed = true)
        {
            Velocity = velocity;
            if (checkOverspeed)
                CheckOverSpeed();
        }

        public void SetMove(Vector2 direction, float speed, bool checkOverspeed = true)
        {
            Velocity = direction*speed;
            if(checkOverspeed)
                CheckOverSpeed();
        }

        public void SetMoveY(float speed, bool checkOverspeed = true)
        {
            Velocity.Y = speed;
            if (checkOverspeed)
                CheckOverSpeed();
        }
        public void SetMoveX(float speed, bool checkOverspeed = true)
        {
            Velocity.X = speed;
            if (checkOverspeed)
                CheckOverSpeed();
        }

        public void Move(Vector2 direction, float speed, bool checkOverspeed = true)
        {
            Velocity += direction*speed;

            if(checkOverspeed)
            {
                CheckOverSpeed();
            }
        }
        public void Move(Vector2 direction, bool checkOverspeed = true)
        {
            throw  new NotImplementedException();
        }
        private void CheckOverSpeed()
        {
            Vector2 vel = Velocity;
            if (!OnLadder || !ClimbLadder)
            {
                if(MaxVelocity.X != 0)
                    CheckOverSpeed(ref vel.X, MaxVelocity.X);
                if(MaxVelocity.Y != 0)
                    CheckOverSpeed(ref vel.Y, MaxVelocity.Y);
            }
            else
            {
                if (MaxLadderVelocity.X != 0)
                    CheckOverSpeed(ref vel.X, MaxLadderVelocity.X);
                if (MaxLadderVelocity.Y != 0)
                    CheckOverSpeed(ref vel.Y, MaxLadderVelocity.Y);
            }
            Velocity = vel;
        }

        public void MoveInOppositeDirection(bool checkOverSpeed = true)
        {
            Velocity *= -1;
            if (checkOverSpeed)
                CheckOverSpeed();
        }

        public override void Update(float dt)
        {
            if (dt >= 0.1f) return; // duct tape xD sometimes dt is extremly big, so that cause movement lag
            UpdateParametrs(dt);
        }

        private void UpdateParametrs(float dt)
        {
            if (!Freeze)
            {
                if (!OnLadder || !ClimbLadder)
                {
                    if (GravityAffected)
                    {
                        if (Velocity.Y < MaxVelocity.Y)
                            Move(GameWorld.GravityDirection, GameWorld.G*dt, false);
                    }
                    if (OnGround)
                        UpdateFriction(ref Velocity.X, Friction, dt);
                    ClimbLadder = false;
                }
                else
                {
                    UpdateFriction(ref Velocity.Y, LadderFriction, dt);
                    UpdateFriction(ref Velocity.X, LadderFriction, dt);
                }

                Vector2 pos = Position;

                Vector2 vel = Velocity*dt*50; // i guess it is dirty hack

                CheckCollisions(ref vel, ref pos);

                Position = pos;

                if (vel.X == 0)
                    Velocity.X = 0;
                if (vel.Y == 0)
                    Velocity.Y = 0;

                Position += vel;
            }
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

            CheckCollisionsWithTiles(ref offset, ref position);
        }


        private void CheckCollisionsWithTiles(ref Vector2 offset, ref Vector2 position)
        {
            if (IgnoreWalls)
                return;

            Collisions.Clear();
            List<CollidedCell> tilesX = null;
            List<CollidedCell> tilesY = null;
            List<CollidedCell> tiles = null;

           // if (offset.X != 0)
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
                    if (cell.CellType == CellType.Wall)
                    {
                        if (offset.X > 0)
                        {
                            position.X = cell.Rectangle.X - Size.Width/2;
                            cell.Direction = new Point(1, 0);
                        }
                        else if (offset.X < 0)
                        {
                            position.X = cell.Rectangle.X + cell.Rectangle.Width + Size.Width/2;
                            cell.Direction = new Point(-1, 0);
                        }
                        offset.X = 0;
                        break;
                    }
                if (offset.X != 0)
                    offset.X = oldOffset;
            }

            //if (offset.Y != 0)
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
                    if (cell.CellType == CellType.Wall)
                    {
                        if (offset.Y > 0)
                        {
                            position.Y = cell.Rectangle.Y - Size.Height/2;
                            cell.Direction = new Point(0, 1);
                        }
                        else if (offset.Y < 0)
                        {
                            position.Y = cell.Rectangle.Y + cell.Rectangle.Height + Size.Height/2;
                            cell.Direction = new Point(0, -1);
                        }
                        offset.Y = 0;
                        break;
                    }
                    else if(ClimbLadder 
                        && cell.CellType == CellType.Free
                        && World.TileMap[cell.Rectangle.X / 64, (cell.Rectangle.Y / 64) + 1] == CellType.Ladder)
                    {
                        if (offset.Y < 0)
                        {
                        //    if(Position.Y < cell.Rectangle.Y)
                         //       position.Y = cell.Rectangle.Y + cell.Rectangle.Height + Size.Height/2;
                        //    cell.Direction = new Point(0, -1);
                        //    offset.Y = 0;
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
                    if (cell.CellType == CellType.Wall)
                    {
                        offset.X = 0;
                        offset.Y = 0;
                    }
            }

            if (tilesX != null)
                Collisions.AddRange(tilesX);
            if (tilesY != null)
                Collisions.AddRange(tilesY);
            if (tiles != null)
                Collisions.AddRange(tiles);

            if (CollisionWithTiles != null)
                foreach (var collidedCell in Collisions)
                    CollisionWithTiles(this, collidedCell);
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
