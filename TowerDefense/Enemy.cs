using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Enemy : MonoBehaviour
{
    private EnemyFactory originFactory;
    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined Factory");
            originFactory = value;
        }
    }
    float Health { get; set; }
    public int trackNumber;
    public float showHealth;

    public float Scale { get; private set; }

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress, progressFactor;
    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;
    [SerializeField]
    Transform model = default;

   
    public void Recycle()
    {
        originFactory.Reclaim(this);
    }

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "No Next Tile");
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        progress = 0f;
        PrepareIntro();
    }

    private void PrepareIntro()
    {
        positionFrom = tileFrom.transform.position;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.directionPath;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;

    }

    public bool GameUpdate()
    {
        showHealth = Health;
        if (Health <= 0)
        {
            originFactory.Reclaim(this);
            return false;
        }
        progress += Time.deltaTime * progressFactor;
        while (progress >= 1f)
        {

            if (tileTo == null)
            {
                originFactory.Reclaim(this);
                return false;
            }
            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }

        if (directionChange == DirectionChange.None)
        {
            transform.localPosition =
            Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }

        if (directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(
            directionAngleFrom, directionAngleTo, progress
            );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }

        return true;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }

        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.directionPath);
        direction = tileFrom.directionPath;
        directionAngleFrom = directionAngleTo;
        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }
    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = Vector3.zero;
        progressFactor = 1f;

    }
    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        model.localPosition = new Vector3(-1f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);
    }
    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3(1f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);
    }
    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180f;
        model.localPosition = Vector3.zero;
        transform.localPosition = positionFrom;
        progressFactor = 2f;
    }
    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = Vector3.zero;
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }
    public void Initialize(float scale)
    {
        Scale = scale;
        model.localScale = new Vector3(scale, scale, scale);
        Health = 100f * scale;
    }
    public void isTracked()
    {
        TurnRed();
    }
    void TurnRed()
    {
        if (trackNumber > 0)
        {
            MeshRenderer targetMat = transform.GetComponentInChildren<MeshRenderer>();
            targetMat.material.color = Color.red;
        }
        else
        {
            MeshRenderer targetMat = transform.GetComponentInChildren<MeshRenderer>();
            targetMat.material.color = Color.blue;
        }

    }
    public void ApplyDamage(float amount)
    {
        Debug.Assert(amount >= 0f, "Damage applied is negative");
        Health -= amount;
    }
}
