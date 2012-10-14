using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures;
using DN.Helpers;
using OpenTK;
using OpenTK.Graphics;

namespace DN.GameObjects.Items.Weapons.Projectives
{
    public abstract class  Projective:Weapon
    {
        public CollidableGameObject Parent
        {
            get;
            set;
        }

        public String Sprite;

        public Projective(GameWorld gameWorld)
            : base(gameWorld)
        {
            GravityAffected = true;
            CollisionWithTiles += OnCollisionWithTiles;
            CollisionWithObjects += OnCollision;
            Size = new Size(64,16);
            //MaxVelocity = new Vector2(0,15);
        }

        public void Init(Vector2 direction, float acceleration)
        {
            Move(direction, acceleration, false);
        }

        private void OnCollisionWithTiles(CollidableGameObject sender, CollidedCell collidedCell)
        {
            if(collidedCell.CellType == CellType.Wall)
                Destroy();
        }

        private void OnCollision(GameObject sender, GameObject gameObject)
        {
            if (gameObject is Creature)
            {
                var creature = gameObject as Creature;
                if (creature != Creature)
                    {
                        if (creature.TakeDamage(Damage, HDirection, Damage * 20, true, 1.0f, 6))
                        {
                            if (!HitRegistered) CM.I.Sound("swordA").PlayDynamic();
                            HitRegistered = true;
                        }
                    }
            }
        }

        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex(Sprite), Bounds, RectangleF.Empty, Color4.White,
               FunctionHelper.Vector2ToRadians(Velocity), new Vector2(0.5f, 0.5f), VDir == 1, HDir == -1);
        }

    }
}
