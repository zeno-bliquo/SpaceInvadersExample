using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Sprite explosionSprite;
    public static Sprite projectileSprite;

    private const float speed = 1;
    void Start()
    {
        transform.localScale = new Vector3(0.3f, .3f, .3f);
    }

    private void Update()
    {
        if (PlayerController.HorizontalCameraExtent < transform.position.y)
            Destroy(gameObject);
        //transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
    }
    void FixedUpdate()
    {
        gameObject.transform.Translate(0, speed, 0.0f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        var alienPos = col.gameObject.transform.position;
        Destroy(col.otherCollider.gameObject);
        Destroy(col.gameObject);
        Destroy(CreateExplosion(alienPos),0.5f);
    }
 
    public static GameObject Create(Vector3 shipPosition)
    {
        GameObject projectile = new GameObject("projectile");
        SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
        renderer.sprite = projectileSprite;
        projectile.AddComponent<Projectile>();
        projectile.transform.position = shipPosition;
        var collider =  projectile.AddComponent<BoxCollider2D>();

        Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        return projectile;
    }

    public static GameObject CreateExplosion(Vector3 position)
    {
        GameObject explosion = new GameObject("explosion");
        SpriteRenderer renderer = explosion.AddComponent<SpriteRenderer>();
        renderer.sprite = explosionSprite;
        explosion.transform.position = position;
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, .5f);
        return explosion;
    }
}
