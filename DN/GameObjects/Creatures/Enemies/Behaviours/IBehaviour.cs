using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Creatures.Enemies.Behaviours
{
    public interface IBehaviour
    {
        GameWorld GameWorld
        {
            get;
            set;
        }
        Creature Creature
        {
            get;
            set;
        }
        Hero Hero
        {
            get;
            set;
        }

        void Initialize();
        void Update(float dt);
    }
}
