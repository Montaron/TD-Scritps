using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    public virtual void Save(GameDataWriter writer)
    {
        writer.Write(transform.localPosition);
        writer.Write(transform.localRotation);
        writer.Write(transform.localScale);
    }

    public virtual void Load(GameDataReader reader)
    {
        transform.localPosition = reader.ReadPosition();
        transform.localRotation = reader.ReadQuaternion();
        transform.localScale = reader.ReadPosition();
    }
}