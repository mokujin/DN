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

        public override RectangleF Bounds
        {
            get
            {
                return new RectangleF((Position.X - Size.Width / 2 + _offset * HDir * 1.5f), (Position.Y - Size.Height / 2), Size.Width, Size.Height);
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
                                                 Position.X + _offset * HDir
                                                 , Position.Y, Size.Width, Size.Height),
                                             RectangleF.Empty,
                                             Color.White,
                                             HDir * _rotation,
                                             HDir == -1 ? new Vector2(1f, 0.5f): new Vector2(0,0.5f),
                                             false, false);
          //  SpriteBatch.Instance.OutlineRectangle(Bounds, Color4.White);
        }

        private void OnCollision(GameObject sender, GameObject gameObject)
        {
            if (gameObject is Creature)
            {
                var creature = gameObject as Creature;
                if(creature != Creature)
                if (DoingAction)
                {
                    if(creature.TakeDamage(Damage, HDirection, Damage*20, true, 1.0f, 6))
                    {
                        if (!HitRegistered) CM.I.Sound("swordA").PlayDynamic();
                        HitRegistered = true;
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
            HitRegistered = false;
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
