using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



[CreateAssetMenu(fileName = "ShapeFactory")]
public class ShapeFactory : ScriptableObject
{
    Scene poolScene;

    [SerializeField]
    bool recycle;

    List<Shape>[] pools;

    [SerializeField]
    Shape[] prefabs;

    [SerializeField]
    Material[] materials;

    void CreatePools()
    {

        pools = new List<Shape>[prefabs.Length];
        
        for (int i = 0; i < prefabs.Length; i++)
        {
            pools[i] = new List<Shape>();
        }

        if (Application.isEditor)
        {
            poolScene = SceneManager.GetSceneByName(name);
            if (poolScene.isLoaded)
            {
                GameObject[] rootObjects = poolScene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf)
                    {
                        pools[pooledShape.ShapeId].Add(pooledShape); 
                    }
                }
                return;
            }
        }
        poolScene = SceneManager.CreateScene(name);
    }

    public Shape GetShape(int shapeId, int materialId)
    {
        Shape instance;
        if (recycle)
        {
            if (pools == null)
            {
                CreatePools();
            }

            int lastindex = pools[shapeId].Count - 1;
            if (lastindex >= 0)
            {
                instance = pools[shapeId][lastindex];
                instance.gameObject.SetActive(true);
                pools[shapeId].RemoveAt(lastindex);
            }
            else
            {
                instance = Instantiate(prefabs[shapeId]);
                instance.SetMaterial(materials[materialId], materialId);
                instance.ShapeId = shapeId;
                SceneManager.MoveGameObjectToScene(instance.gameObject, poolScene);
            }

        }
        else
        {
            instance = Instantiate(prefabs[shapeId]);
            instance.SetMaterial(materials[materialId], materialId);
            instance.ShapeId = shapeId;
           
        }

        return instance;


    }


    public Shape GetShapeRandom()
    {
        return GetShape(Random.Range(0, prefabs.Length), 
            Random.Range(0, materials.Length));
    }

    public void Reclaim(Shape shape)
    {
       if (recycle)
        {
            if (pools == null)
            {
                CreatePools();
            }
            pools[shape.ShapeId].Add(shape);
            shape.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shape.gameObject);
        }
    }

}
