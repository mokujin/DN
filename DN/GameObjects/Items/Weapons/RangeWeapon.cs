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

        protected virtual Vector2 ProjectivePosition
        {
            get { return new Vector2(Position.X + 32, Position.Y); }
        }

        protected Projective CurrentProjective;

        protected ProjectiveType projectiveType;

        public RangeWeapon(GameWorld gameWorld) : base(gameWorld)
        {
            CreateNewProjective();
            IntervalTimer.TickEvent += IntervalTimerTickEvent;
        }

        private void IntervalTimerTickEvent()
        {
            CreateNewProjective();
        }

        protected void CreateNewProjective()
        {
            CurrentProjective = ProjectiveFactory.Create(World, projectiveType);
            CurrentProjective.IgnoreWalls = true;
            CurrentProjective.IgnoreCollisions = true;
            CurrentProjective.GravityAffected = false;
            CurrentProjective.Freeze = true;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (CurrentProjective != null)
            {
                CurrentProjective.Position = ProjectivePosition;
            }
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
            if (Creature == null)
                return;
            base.FinishAction();

            Vector2 dir = Creature.GetVectorDirectionFromDirection();

            CurrentProjective.Move(dir, ProjectiveSpeed, false);
            CurrentProjective.Move(new Vector2(0, -1), 2, false);// small hack
            CurrentProjective.SetOwner(Creature, false);
            CurrentProjective.Damage = Damage;
            CurrentProjective.IgnoreWalls = false;
            CurrentProjective.IgnoreCollisions = false;
            CurrentProjective.GravityAffected = true;
            CurrentProjective.Freeze = false;

            CurrentProjective = null;
        }
    }
}
