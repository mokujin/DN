using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Items.Weapons.Projectives
{
    public static class ProjectiveFactory
    {
        static public Projective Create(GameWorld gameWolrd, ProjectiveType projectiveType, Vector2 position, Vector2 direction, float acceleration)
        {
            Projective projective = null;
            switch (projectiveType)
            {
                case ProjectiveType.Arrow:
                    projective = new Arrow(gameWolrd);
                    break;
                default:
                    break;
            }

            projective.Position = position;
            projective.Init(direction, acceleration);

            return projective;
        }
    }
}
