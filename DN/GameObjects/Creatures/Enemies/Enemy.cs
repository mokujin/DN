﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures.Enemies.Behaviours;
using Blueberry;
using OpenTK.Graphics;
using OpenTK;

namespace DN.GameObjects.Creatures.Enemies
{
    public class Enemy:Creature
    {
        public string Sprite;
        private float _dt;
        IBehaviour _behaviour;

        public Enemy(GameWorld gameWorld, IBehaviour behaviour = null)
            :base(gameWorld)
        {
            DestroyEvent += CreateDeadBodyOnDestroyEvent;
            DestroyEvent += CreateLettersOnDestroyEvent;
            CollisionWithObjects += OnCollision;
            

            _behaviour = behaviour;
        }

        public void SetBehaviour(IBehaviour behaviour)
        {
            _behaviour = behaviour;
            _behaviour.Initialize();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if(!IsDead)
                _behaviour.Update(dt);
            _dt = dt;
        }

        public override void Draw(float dt)
        {
            if(Sprite != null)
                if(!IsDead)
                    SpriteBatch.Instance.DrawTexture(CM.I.tex(Sprite),
                                            Position,
                                            Invulnerable?new Color4(1,1,1,RandomTool.RandFloat()): Color.White);
        }

        private void OnCollision(GameObject sender, GameObject gameObject)
        {
            if (gameObject is Hero)
            {
                var hero = (Hero)gameObject;
                var t = hero.TakeDamage(1, Direction, 5);
                if (t)
                    MoveInOppositeDirection();
            }
            else if(gameObject is Enemy)
            {
                var enemy = (Enemy) gameObject;
                Vector2 moveDir;
                if (enemy.X > X)
                {
                    moveDir = new Vector2(-1, 0);
                }
                else if (enemy.X < X)
                {
                    moveDir = new Vector2(1, 0);
                }
                else
                {
                    moveDir = new Vector2(RandomTool.RandInt(1) * RandomTool.RandSign(), 0);
                }
                Move(moveDir, Acceleration*_dt);
            }
        }

        private void CreateLettersOnDestroyEvent()
        {
            new Letter(World, (char)RandomTool.RandByte(97, 122))
                             {
                                 Cell = Cell
                             };
        }

        private void CreateDeadBodyOnDestroyEvent()
        {
            var deadBody = new DeadBody(World)
                                    {
                                        Cell = Cell,
                                        Sprite = Sprite,
                                        MaxVelocity = MaxVelocity,
                                        MaxLadderVelocity = MaxVelocity,
                                        Friction = 3,
                                        Size = Size
                                    };
            deadBody.SetMove(Velocity, false);
        }
    }
}
