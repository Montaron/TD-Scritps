using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private Transform ground = default;
    private Vector2Int size;
    [SerializeField] GameTile gameTile;
    private GameTile[] gameTiles;
    private Queue<GameTile> searchFrontier = new Queue<GameTile>();
    private GameTileContentFactory factory;
    [SerializeField]
    Texture2D gridTexture = default;
    private bool showPath,showGrid;
    List<GameTile> spawnPoints = new List<GameTile>();
    List<GameTileContent> updatingContent = new List<GameTileContent>();
    public bool ShowPath
    {
        get => showPath;
        set
        {
            showPath = value;
            if (showPath)
            {
                foreach (GameTile t in gameTiles)
                {
                    t.ShowPath();

                }
            }
            else
            {
                foreach (GameTile t in gameTiles)
                {
                    t.HidePath();
                }
            }
        }
    }
    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material m = ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_MainTex", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }

    public GameTile GetSpawnPoint(int id)
    {
        return spawnPoints[id];
    }

    public int GetSpawnPointCount()
    {
        return spawnPoints.Count;
    }

    public void Initialize(Vector2Int size, GameTileContentFactory factory)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);
        ground.localPosition = new Vector3(0f,-0.6f, 0f);
        this.factory = factory;
        gameTiles = new GameTile[size.x * size.y];

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);

        for (int i = 0, x = 0; x < size.x; x++)
        {

            for (int y = 0; y < size.y; y++, i++)
            {
                GameTile instance = gameTiles[i] = Instantiate(gameTile);
                Vector3 position = new Vector3(x - offset.x, 0f, y - offset.y);
                instance.transform.SetParent(transform, false);
                instance.transform.localPosition = position;
                instance.IsAlternative = (x & 1) == 0;
                instance.TileContent = factory.Get(GameTileContentType.Empty);
                if ((y & 1) == 0)
                {
                    instance.IsAlternative = !instance.IsAlternative;
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthRelationship(gameTiles[i], gameTiles[i - 1]);
                }
                if (x > 0)
                {
                    GameTile.MakeEastWestRelationship(gameTiles[i], gameTiles[i - size.x]);
                }

            }
        }
        ToggleSpawnPoint(gameTiles[0]);
        ToggleDestination(gameTiles[gameTiles.Length / 2]);
    }

    private bool FindPath()
    {
        foreach (GameTile t in gameTiles)
        {
            if (t.TileContent.Type == GameTileContentType.Destination)
            {
                t.BecomeDestination();
                searchFrontier.Enqueue(t);
            }
            else
            {
                t.ClearPath();
            }

        }

        if (searchFrontier.Count == 0) { return false; }
        while (searchFrontier.Count > 0)
        {
            GameTile i = searchFrontier.Dequeue();
            if (i != null)
            {
                if (i.IsAlternative)
                {
                    searchFrontier.Enqueue(i.GrowPathNorth());
                    searchFrontier.Enqueue(i.GrowPathEast());
                    searchFrontier.Enqueue(i.GrowPathSouth());
                    searchFrontier.Enqueue(i.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(i.GrowPathWest());
                    searchFrontier.Enqueue(i.GrowPathSouth());
                    searchFrontier.Enqueue(i.GrowPathEast());
                    searchFrontier.Enqueue(i.GrowPathNorth());
                }
            }
        }

        foreach (GameTile t in gameTiles)
        {
            if (t.HasPath)
            {
                if (showPath)
                {
                    t.ShowPath();
                }
                else
                {
                    t.HidePath();
                }
                
            }
            else
            {
                return false;
            }
        }
       
        return true;
    }
    
    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return gameTiles[y + x * size.y];
            }     
        }
        return null;
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.TileContent.Type == GameTileContentType.Destination)
        {
            tile.TileContent = factory.Get(GameTileContentType.Empty);
            if (!FindPath())
            {
                tile.TileContent = factory.Get(GameTileContentType.Destination);
                FindPath();
            }
        }
        else if (tile.TileContent.Type == GameTileContentType.Empty)
        {
            tile.TileContent = factory.Get(GameTileContentType.Destination);
            FindPath();
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.TileContent.Type == GameTileContentType.Empty)
        {
            tile.TileContent = factory.Get(GameTileContentType.Wall);
            if (!FindPath())
            {
                tile.TileContent = factory.Get(GameTileContentType.Empty);
                FindPath();
            }
        }
        else if (tile.TileContent.Type == GameTileContentType.Wall)
        {
            tile.TileContent = factory.Get(GameTileContentType.Empty);
            FindPath();
        }
    }

    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.TileContent.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                spawnPoints.Remove(tile);
                tile.TileContent = factory.Get(GameTileContentType.Empty);
            }
        }
        else if (tile.TileContent.Type == GameTileContentType.Empty)
        {
            tile.TileContent = factory.Get(GameTileContentType.SpawnPoint);
            spawnPoints.Add(tile);
        }
    }

    public void ToggleTower(GameTile tile, TowerType towerType)
    {
        if (tile.TileContent.Type == GameTileContentType.Tower)
        {
            updatingContent.Remove(tile.TileContent);
            if (((Tower)tile.TileContent).TowerType == towerType)
            {
                tile.TileContent = factory.Get(GameTileContentType.Empty);
                FindPath();
            }   
            else
            {
                tile.TileContent = factory.Get(towerType);
                updatingContent.Add(tile.TileContent);
            }

        }
        else if (tile.TileContent.Type == GameTileContentType.Empty)
        {
            tile.TileContent = factory.Get(towerType);
            if (!FindPath())
            {
                tile.TileContent = factory.Get(GameTileContentType.Empty);
                FindPath();
            }
            else
            {
                updatingContent.Add(tile.TileContent);
            }
        }
        else if (tile.TileContent.Type == GameTileContentType.Wall)
        {
            tile.TileContent = factory.Get(towerType);
            updatingContent.Add(tile.TileContent);
        }
    }
    public void GameUpdate()
    {
        for (int i = 0; i < updatingContent.Count; i++)
        {
            updatingContent[i].GameUpdate();
        }
    }
}
