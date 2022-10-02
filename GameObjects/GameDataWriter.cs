using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataWriter
{
    BinaryWriter writer;

    public GameDataWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }

    public void Write(Vector3 pos)
    {
        writer.Write(pos.x);
        writer.Write(pos.y);
        writer.Write(pos.z);
    }

    public void Write(float value)
    {
        writer.Write(value);
    }

    public void Write(int value)
    {
        writer.Write(value);
    }


    public void Write(Quaternion value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
        writer.Write(value.w);
    }

    public void Write(Color value)
    {
        writer.Write(value.r);
        writer.Write(value.b);
        writer.Write(value.g);
        writer.Write(value.a);
    }

}
