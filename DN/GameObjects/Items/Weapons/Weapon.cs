using DN.GameObjects.Creatures;
using DN.Helpers;

namespace DN.GameObjects.Items.Weapons
{
    public abstract class Weapon:Item
    {
        public float Damage
        {
            get;
            set;
        }


        public Weapon(GameWorld gameWorld)
            :base(gameWorld)
        {
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }


        public override void DoAction()
        {
            base.DoAction();
        }
    }
}
