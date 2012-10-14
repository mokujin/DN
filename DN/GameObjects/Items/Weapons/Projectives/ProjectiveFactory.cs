using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GameObjects.Items.Weapons.Projectives
{
    public static class ProjectiveFactory
    {
        static public Projective Create(GameWorld gameWorld, ProjectiveType projectiveType)
        {
            return Create(gameWorld, projectiveType, Vector2.Zero, Vector2.Zero, 0.0f);
        }

        static public Projective Create(GameWorld gameWorld, ProjectiveType projectiveType, 
                                        Vector2 position, Vector2 direction, float acceleration)
        {
            Projective projective = null;
            switch (projectiveType)
            {
                case ProjectiveType.Arrow:
                    projective = new Arrow(gameWorld);
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
