using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public TextAsset textAsset;
    public string path;
    public Sprite image;

    public Level(string path)
    {
        this.path = path;
        textAsset = Resources.Load<TextAsset>(path + "/level");
        image = Resources.Load<Sprite>(path + "/ss");
    }
}
