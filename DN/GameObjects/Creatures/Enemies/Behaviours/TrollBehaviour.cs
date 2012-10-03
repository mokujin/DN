using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Creatures.Enemies.Behaviours
{
    public class TrollBehaviour:IBehaviour
    {

        GameWorld IBehaviour.GameWorld
        {
            get;
            set;
        }

        Creature IBehaviour.Creature
        {
            get;
            set;
        }

        Hero IBehaviour.Hero
        {
            get;
            set;
        }

        void IBehaviour.Initialize()
        {
        }

        void IBehaviour.Update(float dt)
        {

        }
    }
}
