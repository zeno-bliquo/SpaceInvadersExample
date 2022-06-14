using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public static Sprite alienSprite;

    private const float speed = 0.02f;
    void Start()
    {
    
    }

    private void Update()
    {
        if (-PlayerController.VerticalCameraExtent > transform.position.y)
            Destroy(gameObject);
    }
    void FixedUpdate()
    {
        gameObject.transform.Translate(0, -speed, 0.0f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        var pos = gameObject.transform.position;
        Destroy(gameObject);
        Destroy(Projectile.CreateExplosion(pos), 0.5f);
    }

    public static GameObject Create(Vector3 position)
    {
        GameObject alien = new GameObject("alien");
        SpriteRenderer renderer = alien.AddComponent<SpriteRenderer>();
        renderer.sprite = alienSprite;
        alien.transform.position = position;
        var collider = alien.AddComponent<BoxCollider2D>();
        alien.AddComponent<Alien>();
        //Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
        //rb.bodyType = RigidbodyType2D.Dynamic;
        alien.transform.localScale = new Vector3(0.2f, .2f, .2f);
        return alien;
    }
}
