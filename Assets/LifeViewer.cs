using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Linq; 

public class LifeViewer : MonoBehaviour
{
    public static Sprite lifeSprite;

    public static GameObject Create(Vector3 position)
    {
        GameObject life = new GameObject("life");

        SpriteRenderer renderer = life.AddComponent<SpriteRenderer>();
        renderer.sprite = lifeSprite;
        
        life.transform.position = position;
        life.AddComponent<LifeViewer>();
        float scale = 0.05f;
        life.transform.localScale = new Vector3(scale, scale, scale);

        return life;
    }
    /* 
    void OnDestroy() {
        Debug.Log("Life was destroyed");
    }
    */
}
