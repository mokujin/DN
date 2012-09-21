using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures.Enemies.Behaviours;
namespace DN.GameObjects.Creatures.Enemies
{
    public enum EnemyType
    {
        Bat
    }
    static public class EnemiesFabric
    {    
        static public Enemy CreateEnemy(GameWorld gameWorld,EnemyType type)
        {
            Enemy enemy = new Enemy(gameWorld);

            switch (type)
            {
                case EnemyType.Bat:
                    enemy.SetBehaviour(new BatBehaviour() 
                    {
                        Creature = enemy, Hero = gameWorld.Hero, GameWorld = gameWorld 
                    });
                    break;
            }
            return enemy;
        }
    }
}
