using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField]
    Enemy enemyPrefab = default;

    [SerializeField, FloatRangeSlider(0.5f, 2f)]
    FloatRange scale = new FloatRange(1f);
    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(enemyPrefab);
        instance.OriginFactory = this;
        instance.Initialize(scale.RandomValueInRange);
        
        return instance;
    }
    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "Wrong factory attempting to reclaim");
        Destroy(enemy.gameObject);
    }

}
