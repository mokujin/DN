using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Items
{
    public class Potion:Item
    {
        public Potion(GameWorld gameWorld) : base(gameWorld)
        {
        }

        public override void Draw(float dt)
        {
            throw new NotImplementedException();
        }
    }
}
