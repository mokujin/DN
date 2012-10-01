using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DN.GameObjects.Creatures.Enemies.Behaviours;
using OpenTK;

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
                    enemy.MaxVelocity = new Vector2(3, 3);
                    enemy.MaxLadderVelocity = new Vector2(3,3);
                    enemy.AddHealth(5);
                    enemy.Size = new Size(16, 16);
                    enemy.Sprite = "bat_sprite";
                    enemy.InvulnerabilityDuration = 1;
                    break;
            }
            return enemy;
        }
    }
}
