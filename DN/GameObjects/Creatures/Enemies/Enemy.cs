using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures.Enemies.Behaviours;
using Blueberry;
using OpenTK.Graphics;
using OpenTK;
using DN.GameObjects.Weapons;

namespace DN.GameObjects.Creatures.Enemies
{
    public class Enemy:Creature
    {
        public string Sprite;
        IBehaviour _behaviour;

        public Enemy(GameWorld gameWorld, IBehaviour behaviour = null)
            :base(gameWorld)
        {
            Death += CreateDeadBodyOnDeath;
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
            if (gameObject is Weapon)
            {
                var weapon = gameObject as Weapon;
                if (weapon.Attacking)
                {
                    if (TakeDamage(weapon.Damage, weapon.Direction, weapon.Damage * 20, true, 1.0f, 6))
                    {
                       // Vector2 vel = Velocity;
                        //vel.Y = 0;

                        //BloodEmitter = World.BloodSystem.InitEmitter(Position, vel * 0.2f,
                                             //                             7, 0f, 1);
                    }

                }
            }
            else if (gameObject is Hero)
            {
                Hero hero = (Hero)gameObject;
                bool t = hero.TakeDamage(1, Direction, 5);
                if (t)
                    MoveInOppositeDirection();
            }
        }



        private void CreateDeadBodyOnDeath()
        {
            DeadBody deadBody = new DeadBody(World)
                                    {
                                        Position = Position,
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
