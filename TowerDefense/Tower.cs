using UnityEngine;

public enum TowerType { Laser, Mortar }
public abstract class Tower : GameTileContent
{
    static Collider[] targetsBuffer = new Collider[100];
    const int enemyLayerMask = 1 << 9;

    [SerializeField, Range(1.5f, 10.5f)]
    protected float targetingRange = 1.5f;
    public abstract TowerType TowerType { get; }
    protected bool AcquireTarget(out TargetPoint target)
    {
        Vector3 a = transform.localPosition;
        Vector3 b = a;
        b.y += 3f;
        int hits = Physics.OverlapCapsuleNonAlloc(a, b, targetingRange, targetsBuffer, enemyLayerMask);
        if (hits > 0)
        {
            target = targetsBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
            if (target != null)
            {
                target.Enemy.trackNumber++;
                target.Enemy.isTracked();
                return true;
            }


        }
        target = null;
        return false;
    }
    protected bool trackTarget(ref TargetPoint target)
    {
        if (target == null)
        {
            return false;
        }
        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        float x = a.x - b.x;
        float z = a.z - b.z;
        float r = targetingRange + 0.125f * target.Enemy.Scale;
        if (x * x + z * z > r * r)
        {
            target.Enemy.trackNumber--;
            target.Enemy.isTracked();
            target = null;
            return false;
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }
}