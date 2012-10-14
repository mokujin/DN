
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
        
        public Arrow(GameWorld gameWorld) 
            : base(gameWorld)
        {
            Sprite = "arrow_sprite";
        }
    }
}
