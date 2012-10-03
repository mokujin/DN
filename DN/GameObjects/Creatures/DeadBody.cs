using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry;
using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;

namespace DN.GameObjects.Creatures
{
    class DeadBody:CollidableGameObject
    {
        public string Sprite;

        private float _alpha = 0.5f;
        private float _rotation = 0;


        public DeadBody(GameWorld gameWorld) : base(gameWorld)
        {
            IgnoreCollisions = true;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            _alpha -= dt / 10;
            if(_alpha <= 0)
                World.RemoveObject(this);

         //   if (!OnGround)
                _rotation += dt * RandomTool.RandFloat() * 10;
        }

        public override void Draw(float dt)
        {
            Texture tex = CM.I.tex(Sprite);
            SpriteBatch.Instance.DrawTexture(tex,
                                             new RectangleF(Position.X, Position.Y, tex.Size.Width, tex.Size.Height),
                                             RectangleF.Empty,
                                             new Color4(1, 1, 1, _alpha), _rotation, new Vector2(0.5f, 0.5f), false,
                                             false);
        }
    }
}
