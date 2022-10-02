using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameM : MonoBehaviour
{
    [SerializeField]
    private Vector2Int boardSize = new Vector2Int(3, 3);
    [SerializeField]
    private GameBoard gameBoard;
    [SerializeField]
    private GameTileContentFactory tileFactory;
    [SerializeField]
    private EnemyFactory enemyFactory;
    [SerializeField, Range(0.1f, 10f)]
    float spawnSpeed = 0.1f;
    private float time;
    private Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    EnemyCollection enemies = new EnemyCollection();
    TowerType selectTowerType;
    void Awake()
    {
        gameBoard.Initialize(boardSize, tileFactory);
        gameBoard.ShowGrid = true;
       
    }

    public void OnValidate()
    {
        if (boardSize.x < 2) { boardSize.x = 2; }
        if (boardSize.y < 2) { boardSize.y = 2; }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleAlternativeTouch();
        }
        if (Input.GetMouseButtonDown(1))
        {
            HandleTouch();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            gameBoard.ShowPath = !gameBoard.ShowPath;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameBoard.ShowGrid = !gameBoard.ShowGrid;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectTowerType = TowerType.Mortar;
        }
        SpawnEnemy();
        enemies.GameUpdate();
        Physics.SyncTransforms();
        gameBoard.GameUpdate();
    }

    private void HandleTouch()
    {
        GameTile t = gameBoard.GetTile(TouchRay);
        if (t != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                gameBoard.ToggleTower(t, selectTowerType);
            }
            else
            {
                gameBoard.ToggleDestination(t);
            }
            
        }
        
    }
    private void HandleAlternativeTouch()
    {
        GameTile t = gameBoard.GetTile(TouchRay);
        if (t != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                gameBoard.ToggleSpawnPoint(t);
            }
            gameBoard.ToggleWall(t);
        }

    }

    public void SpawnEnemy()
    {
        time += Time.deltaTime * spawnSpeed;
        while (time >= 1)
        {
            Enemy e = enemyFactory.Get();
            GameTile t = gameBoard.GetSpawnPoint(Random.Range(0, gameBoard.GetSpawnPointCount()));
            e.SpawnOn(t);
            enemies.AddEnemy(e);
            time -= 1;
        }
    }
}
