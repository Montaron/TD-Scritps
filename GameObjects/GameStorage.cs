using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameStorage : MonoBehaviour
{
    string filepath;

    private void Awake()
    {
        filepath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Store(PersistentObject o, int version)
    {
        using (
            var bwriter = new BinaryWriter(File.Open(filepath, FileMode.Create))
        ) {
            bwriter.Write(-version);
            o.Save(new GameDataWriter(bwriter));
        }
    }

    public void Restore(PersistentObject o)
    {
        using (
            var breader = new BinaryReader(File.Open(filepath, FileMode.Open))
         ) {
            o.Load(new GameDataReader(breader, -breader.ReadInt32()));
        }
    }


}
