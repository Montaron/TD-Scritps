using UnityEngine;

public class TowerLaser : Tower
{
    TargetPoint target;
    Vector3 laserScaleIni;
    Vector3 laserPosIni;
    Vector3 laserGlobPosIni;
    float scaleFactor;
    float damagePerSecond = 20f;
    public override TowerType TowerType {  get { return TowerType.Laser; } }

    private void Awake()
    {
        laserScaleIni = laser.localScale;
        laserPosIni = laser.localPosition;

    }
    private void Start()
    {
        laserGlobPosIni = laserIniPos.position;
    }

    
    [SerializeField]
    Transform turret, laser, laserIniPos = default;
    public override void GameUpdate()
    {
        if (trackTarget(ref target) || AcquireTarget(out target))
        {
            Shoot();

        }
        else
        {
            turret.localRotation = Quaternion.identity;
            laser.localScale = laserScaleIni;
            laser.localPosition = laserPosIni;
        }
    }

    void Shoot()
    {
        turret.LookAt(target.Position);
        float d = Vector3.Distance(laserGlobPosIni, target.Position);
        Vector3 laserScale = laserScaleIni;
        scaleFactor = d / turret.localScale.z;
        laserScale.z = scaleFactor;
        laser.localScale = laserScale;
        laser.localPosition = laserPosIni + new Vector3(0, 0, scaleFactor * 0.5f);
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}
