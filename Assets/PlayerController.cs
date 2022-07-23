using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject ship;
    public Camera mycamera;
    public Sprite projectileSprite;
    public Sprite explosionSprite;

    private static float _horizontalCameraExtent;
    private static float _verticalCameraExtent;
    public static float HorizontalCameraExtent { get { return _horizontalCameraExtent; } }
    public static float VerticalCameraExtent { get { return _verticalCameraExtent; } }

    public static float playerSpeed = 0.3f;

    // speedup the ship of 5% for each level
    void Start()
    {
        _verticalCameraExtent = mycamera.orthographicSize;
        _horizontalCameraExtent = _verticalCameraExtent * Screen.width / Screen.height;

        Projectile.explosionSprite = explosionSprite;
        Projectile.projectileSprite = projectileSprite;

        this.Invoke( ()=>{ 
            float speedUp = 0.05f * (AlienPool.getLevel()-1);
            playerSpeed += speedUp; Debug.Log($"Speed-up ship by: {speedUp.ToString()} \t level: {AlienPool.getLevel()}");
        }, 1.5f);
    }
    
    // Update is called once per frame
    void Update()
    {
        if( GameEndController.isGameEndControllerActive || PauseMenu.isGamePaused ) return;

        var newposx = ship.transform.position.x + Input.GetAxis("Horizontal") * playerSpeed;
        int reducedXrange = 2;
        if(Math.Abs(newposx) < _horizontalCameraExtent - reducedXrange )
            ship.transform.position = new Vector3(newposx, ship.transform.position.y, ship.transform.position.z);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Projectile.Create(ship.transform.position);
            User.instance.addProjectileFired(1);
        }
    }   
}
