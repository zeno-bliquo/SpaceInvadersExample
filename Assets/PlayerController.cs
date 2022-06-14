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
    void Start()
    {
        _verticalCameraExtent = mycamera.orthographicSize;
        _horizontalCameraExtent = _verticalCameraExtent * Screen.width / Screen.height;

        Projectile.explosionSprite = explosionSprite;
        Projectile.projectileSprite = projectileSprite;
    }
    // Update is called once per frame
    void Update()
    {
        var newposx = ship.transform.position.x + Input.GetAxis("Horizontal");
        if(Math.Abs(newposx) < _horizontalCameraExtent)
            ship.transform.position = new Vector3(newposx, ship.transform.position.y, ship.transform.position.z);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Projectile.Create(ship.transform.position);
        }
    }
}
