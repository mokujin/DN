using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;

namespace DN.GameObjects.Items
{
    public enum PotionType
    {
        Healing
    }

    public class Potion:Item
    {
        public PotionType PotionType { get;private set; }
        public float Power { get; private set; }

        public Potion(GameWorld gameWorld, PotionType potionType, float power) : base(gameWorld)
        {
            PotionType = potionType;
            Power = power;
            Size = new Size(16,20);
        }

        public override void DoAction()
        {
            switch (PotionType)
            {
                case PotionType.Healing:
                    {
                        Creature.AddHealth(Power);
                        Destroy();
                    }
                    break;
            }
        }


        public override void Update(float dt)
        {
            base.Update(dt);
        }

        //todo: generate awesome texture
        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex("potion"), Bounds,Rectangle.Empty,
                                             Color4.White, 0f, new Vector2(0,0));
        }
    }
}
