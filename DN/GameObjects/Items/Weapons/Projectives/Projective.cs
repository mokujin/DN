using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

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

        internal Projective(GameWorld gameWorld) : base(gameWorld)
        {
            GravityAffected = true;
            CollisionWithTiles += OnCollisionWithTiles;
            //MaxVelocity = new Vector2(0,15);
        }

        internal void Init(Vector2 direction, float acceleration)
        {
            Move(direction, acceleration, false);
        }

        private void OnCollisionWithTiles(CollidableGameObject sender, CollidedCell collidedCell)
        {
            if(collidedCell.CellType == CellType.Wall)
                Destroy();
        }

    }
}
