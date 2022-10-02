using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistentObject
{
    public GameStorage storage;
    private List<Shape> shapes;
    private const int Version = 1;

    public float SpawnSpeed { get; set; }

    public float DespawnSpeed { get; set; }

    float creationProgress;
    float destroyProgress;

    [SerializeField]
    private ShapeFactory shapeFactory;
    #region Keybinds
    [SerializeField]
    KeyCode saveKey = KeyCode.S;

    [SerializeField]
    KeyCode loadKey = KeyCode.L;

    [SerializeField]
    KeyCode createObject = KeyCode.C;

    [SerializeField]
    KeyCode newGame = KeyCode.N;

    [SerializeField]
    KeyCode destroyShape = KeyCode.X;
    #endregion

    private void Awake()
    {
        LoadLevel();
        shapes = new List<Shape>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            storage.Store(this, Version);
        }

        if(Input.GetKeyDown(loadKey))
        {
            storage.Restore(this);
        }

        if (Input.GetKeyDown(createObject))
        {
            CreateInstance();
        }

        if (Input.GetKeyDown(newGame))
        {
            StartNewGame();
        }

        if (Input.GetKeyDown(destroyShape))
        {
            DestroyShape();
        }
      
        creationProgress += SpawnSpeed * Time.deltaTime;
        while (creationProgress >= 1)
        {
            creationProgress -= 1;
            CreateInstance();
        }

        destroyProgress += DespawnSpeed * Time.deltaTime;
        while (destroyProgress >= 1)
        {
            destroyProgress -= 1;
            DestroyShape();
        }


    }

    void LoadLevel()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level 1"));
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        int count = version > 0 ? reader.ReadInt() : -version;

        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape o = shapeFactory.GetShape(shapeId, materialId);
            o.Load(reader);
            shapes.Add(o);
        }
        
    }

    public override void Save(GameDataWriter writer)
    {
        
        writer.Write(shapes.Count);

        for (int i = 0; i< shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
        
    }

    void CreateInstance()
    {
        Shape o = shapeFactory.GetShapeRandom();
        Transform t = o.transform;
        t.transform.localPosition = Random.insideUnitSphere * 5f;
        t.transform.localRotation = Random.rotation;
        t.transform.localScale = Vector3.one * Random.Range(0.1f, 2f);
        o.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(o);
    }

    void StartNewGame()
    {
        ClearList();
    }

    void ClearList()
    {
        if (shapes.Count > 0)
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                shapeFactory.Reclaim(shapes[i]);
            }
        }

        shapes.Clear();
    }

    void DestroyShape()
    {
        int index = Random.Range(0, shapes.Count);
        shapeFactory.Reclaim(shapes[index]);
        int lastIndex = shapes.Count - 1;
        shapes[index] = shapes[lastIndex];
        shapes.RemoveAt(lastIndex);
    }
}
