using System.Drawing;
using Blueberry.Graphics;
using DN.GameObjects.Creatures;
using OpenTK;

namespace DN.GameObjects.Items.Weapons
{
    public class MeleeWeapon:Weapon// todo: it must be abstract class
    {
        private float _offset = -2;
        private float _rotation = 0;
        private float Dir
        {
            get { return Direction == Direction.Right ? 1 : -1; }
        }


        public override RectangleF Bounds
        {
            get
            {
                return new RectangleF((X - Size.Width / 2 + _offset * Dir * 1.5f), (Y - Size.Height / 2), Size.Width, Size.Height);
            }
        }

        public MeleeWeapon(GameWorld gameWorld)
            :base(gameWorld)
        {
            Size = new Size(40, 20);
            PerformingDuration = 0.3f;
            PerfermingTimer.TickEvent += FinishAttack;
            PerfermingTimer.UpdateEvent += PerformAttack;

            CollisionWithObjects += OnCollision;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex("sword_sprite"),
                                             new RectangleF(
                                                 X + _offset * Dir
                                                 ,Y, Size.Width, Size.Height),
                                             RectangleF.Empty,
                                             Color.White,
                                             Dir * _rotation,
                                             Dir == -1 ? new Vector2(1f, 0.5f): new Vector2(0,0.5f),
                                             false, false);
          //  SpriteBatch.Instance.OutlineRectangle(Bounds, Color4.White);
        }

        private bool _hitRegistered = false;
        private void OnCollision(GameObject sender, GameObject gameObject)
        {
            if (gameObject is Creature)
            {
                var creature = gameObject as Creature;
                if(creature != Creature)
                if (DoingAction)
                {
                    if(creature.TakeDamage(Damage, Direction, Damage*20, true, 1.0f, 6))
                    {
                        if (!_hitRegistered) CM.I.Sound("swordA").PlayDynamic();
                        _hitRegistered = true;
                    }
                }
            }
        }

        public override void DoAction()
        {

            if (!CanDoAction) return;
            _offset = 0;
            _rotation = 4.71f;
            CM.I.Sound("swordB").PlayDynamic();
            _hitRegistered = false;
            base.DoAction();
        }

        protected void PerformAttack(float dt)
        {
            _offset += 150 * dt;
            _rotation += 6 * dt;
        }

        protected void FinishAttack()
        {
            _offset = 0;
            _rotation = 0;
        }
    }
}
