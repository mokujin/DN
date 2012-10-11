using DN.GameObjects.Items.Weapons.Projectives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace DN.GameObjects.Items.Weapons
{
    public enum ProjectiveType
    {
        Arrow
    }

    public abstract class RangeWeapon:Weapon
    {
        private float _projectiveSpeed;
        public virtual float ProjectiveSpeed 
        {
            get { return _projectiveSpeed; }
            set { _projectiveSpeed = value; } 
        }
        private ProjectiveType projectiveType;

        public RangeWeapon(GameWorld gameWorld) : base(gameWorld)
        {
        }

        public override void DoAction()
        {
            if (DoingAction)
                return;
            base.DoAction();


        }

        public override void FinishAction()
        {
            if (!DoingAction)
                return;
            base.FinishAction();
            Vector2 dir = Creature.GetVectorDirectionFromDirection();

            ProjectiveFactory.Create(World, projectiveType,Creature.Position,
               dir, ProjectiveSpeed);
        }
    }
}
