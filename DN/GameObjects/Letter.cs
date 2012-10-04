using Blueberry.Graphics;
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
        public readonly char _char;
        static public readonly Size CharSize = CM.I.Font("Middle").Measure("#").ToSize();


        public Letter(GameWorld gameWorld, char ch)
            :base(gameWorld)
        {
            _char = ch;
            Size = CharSize;
        }




        public override void Draw(float dt)
        {
            SpriteBatch.Instance.PrintText(CM.I.Font("Middle"), _char.ToString(), Position, Color4.White);
        }
    }
}
