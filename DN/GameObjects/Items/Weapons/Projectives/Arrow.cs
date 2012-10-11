
using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Items.Weapons.Projectives
{
    public class Arrow: Projective
    {
        
        internal Arrow(GameWorld gameWorld) 
            : base(gameWorld)
        {
            Sprite = "arrow_sprite";
        }

        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex(Sprite), Position, Color4.White);
        }
    }
}
