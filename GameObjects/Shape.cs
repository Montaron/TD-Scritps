using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistentObject
{
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock propertyBlock;
    private int shapeId = int.MinValue;
    private int materialId;
    private Color color;
    public int MaterialId
    {
        get { return materialId; }
    }
    private MeshRenderer meshRenderer;

    public int ShapeId
    {
        get { return shapeId; }
        set { if (shapeId == int.MinValue && value != int.MinValue) { shapeId = value; } }

    }

    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        this.materialId = materialId;

    }

    public void SetColor(Color color)
    {
        this.color = color;
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
        propertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
