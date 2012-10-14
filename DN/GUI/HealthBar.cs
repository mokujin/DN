using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures;
using OpenTK.Graphics;

namespace DN.GUI
{
    public class HealthBar : GUIObject
    {
        private Creature _creature;
        Texture heart = CM.I.tex("heart");

        public HealthBar(Creature creature)
        {
            _creature = creature;
        }

        public override void Update(float dt)
        {
        }

        public override void Draw(float dt)
        {
            float c;
            int i;

            for (i = 3; i < _creature.Health; i += 3)
            {
                SpriteBatch.Instance.DrawTexture(heart, X + 36*i/3, Y, 32, 32, Rectangle.Empty, Color.White);
            }
            c = _creature.Health - i;
            if (c <= 0)
            {
                SpriteBatch.Instance.DrawTexture(heart,
                                                 X + 36*i/3, Y, 32, 32,
                                                 Rectangle.Empty, new Color4(1, 1, 1, c == 3
                                                                                          ? 0
                                                                                          : c == 0
                                                                                                ? 1
                                                                                                : 1f/(-(c) + 1)));


            }
        }
    }
}
