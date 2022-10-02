using System.IO;
using UnityEngine;

public class GameDataReader
{
    BinaryReader reader;
    public int Version { get;  }

    public GameDataReader(BinaryReader reader, int version)
    {
        this.reader = reader;
        Version = version;
    }
    
    public Vector3 ReadPosition()
    {
        Vector3 pos;
        pos.x = reader.ReadSingle();
        pos.y = reader.ReadSingle();
        pos.z = reader.ReadSingle();
        return pos;
    }
    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }
    public Quaternion ReadQuaternion()
    {
        Quaternion value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        value.w = reader.ReadSingle();
        return value;
    }
    public Color ReadColor()
    {
        Color value;
        value.r = reader.ReadSingle();
        value.b = reader.ReadSingle();
        value.g = reader.ReadSingle();
        value.a = reader.ReadSingle();
        return value;
    }
}
