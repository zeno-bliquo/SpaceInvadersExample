using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien_MotherShip : MonoBehaviour
{
    public static Sprite mothershipSprite;

    private const float speed = 0.03f;

    private int mothershipLife = 5;

    private const int pointsMothership = 5;

    public static string mothership_GO_name = "alienMothership";

    void Start()
    {
        mothershipLife += (int)System.MathF.Floor(AlienPool.getLevel()/2);
        Debug.Log($"Started a new mothership with life: {mothershipLife} \n position: {gameObject.transform.position.x}");
    }

    /*
        if mothership passes the ship is gameover
    */
    private void Update()
    {
        if (-PlayerController.VerticalCameraExtent > transform.position.y){
            Destroy(gameObject);
            ScoreManager.instance.GameOver();
        }
    }
    void FixedUpdate()
    {
        gameObject.transform.Translate(0, -speed, 0.0f);
    }

    /*
        touch mothership is gameover, defeat is gamewin
    */
    void OnCollisionEnter2D(Collision2D col)
    {
        mothershipLife -= 1;
                
        if( mothershipLife < 1 ){
            Debug.Log("Mothership killed");
            Destroy(gameObject);
            Destroy(Projectile.CreateExplosion(gameObject.transform.position), 0.5f);
            User.instance.AddAlienKilled(pointsMothership);
            ScoreManager.instance.GameWin();
            return;
        }
        Debug.Log($"OnCollisionEnter2D() mothership remaining life {mothershipLife}");

        if( col.gameObject.name == "ship" ){
            ScoreManager.instance.GameOver();
        }
    }

    /*
        increase horizontal box-collider for mothership to fit the sprite size (and game difficulty)
    */
    public static GameObject Create(Vector3 position)
    {
        GameObject alien = new GameObject(mothership_GO_name);
        SpriteRenderer renderer = alien.AddComponent<SpriteRenderer>();
        renderer.sprite = mothershipSprite;
        alien.transform.position = position;
        var collider = alien.AddComponent<BoxCollider2D>();

        collider.size = new Vector3(collider.size.x * 2, collider.size.y);



        float scalingMotherShip = .3f;
        if( User.instance.isLastLevel() ){
            Debug.Log("User.instance.isLastLevel() bigger mothership");
            scalingMotherShip = .6f;
        }

        alien.AddComponent<Alien_MotherShip>();
        alien.transform.localScale = new Vector3(scalingMotherShip, scalingMotherShip, scalingMotherShip);

        return alien;
    }
}
