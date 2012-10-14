using Blueberry;
using Blueberry.Graphics;
using DN.GameObjects.Creatures;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DN.GameObjects
{
    public class Letter:CollidableGameObject
    {
        private readonly char _char;
        private bool _heroWasCollided;


        static public readonly Size CharSize = CM.I.Font("Middle").Measure("#").ToSize();
        private float _alphaEffect = 1.0f;


        public Letter(GameWorld gameWorld, char ch)
            :base(gameWorld)
        {
            CollisionWithObjects += HeroOnCollisionWithObjects;
            _char = ch;
            Size = CharSize;
            Velocity = RandomTool.NextUnitVector2()*40;
            MaxVelocity = new Vector2(3,3);
            Friction = 0.5f;
        }

        private void HeroOnCollisionWithObjects(GameObject sender, GameObject gameObject)
        {
            if(gameObject is Hero)
            {
                _heroWasCollided = true;
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (IgnoreWalls)
            {
                Move(World.DirectionToObject(this, World.Hero), 20*dt);
            }
            else
            {
                if (World.DistanceToObject(World.Hero, this) < 100)
                {
                   // IgnoreCollisions = false;
                    IgnoreWalls = true;
                    GravityAffected = false;
                }
            }

            if(_heroWasCollided)
            {
                _alphaEffect -= dt;
            }
            if(_alphaEffect <= 0)
            {
                World.Hero.Inventory.Add(_char);
                World.RemoveObject(this);
            }
        }

        public override void Draw(float dt)
        {
            SpriteBatch.Instance.PrintText(CM.I.Font("Middle"), _char.ToString(), Position, new Color4(1,1,1,_alphaEffect));
        }
    }
}
