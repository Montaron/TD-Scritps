using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameTileContentType
{
    Destination, Empty, Wall, SpawnPoint, Tower
}
[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    [SerializeField]
    GameTileContentType contentType;
    private GameTileContentFactory originFactory;
    public GameTileContentType Type => contentType;
    public bool BlocksPath =>
 Type == GameTileContentType.Wall || Type == GameTileContentType.Tower;

    public GameTileContentFactory OriginFactory { 
        get => originFactory; 
        set {
            Debug.Assert(originFactory == null, "Defined Factory");
            originFactory = value;
        } 
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }

    public virtual void GameUpdate() { }
}
