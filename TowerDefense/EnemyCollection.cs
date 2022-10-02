using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollection : MonoBehaviour
{
    List<Enemy> enemies = new List<Enemy>();
    
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public void GameUpdate()
    {
     for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].GameUpdate())
            {
                int lastIndex = enemies.Count - 1;
                enemies[i] = enemies[lastIndex];
                enemies.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }
}
