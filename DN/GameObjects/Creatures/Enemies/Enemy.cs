using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures.Enemies.Behaviours;
using Blueberry;
using OpenTK.Graphics;

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
