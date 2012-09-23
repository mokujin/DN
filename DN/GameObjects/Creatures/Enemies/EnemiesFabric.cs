using System;
using System.Collections.Generic;
using System.Drawing;
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
            var enemy = new Enemy(gameWorld);

            switch (type)
            {
                case EnemyType.Bat:
                    enemy.SetBehaviour(new BatBehaviour
                    {
                        Creature = enemy, Hero = gameWorld.Hero, GameWorld = gameWorld
                    });
                    enemy.GravityAffected = false;
                    enemy.Size = new Size(20, 16);
                    enemy.Sprite = "bat_sprite";
                    break;
            }
            return enemy;
        }
    }
}
