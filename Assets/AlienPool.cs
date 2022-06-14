using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienPool : MonoBehaviour
{
    public Sprite alienSprite;
   
    private float time = 0.0f;
    public float waveWaitTimeSecs = 3f;

    // Start is called before the first frame update
    void Start()
    {
        Alien.alienSprite = alienSprite; 
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= waveWaitTimeSecs)
        {
            time = 0.0f;
            SpawnWave(8);
        }
    }

    private void SpawnWave(int numAliens)
    {
        var ypos = PlayerController.VerticalCameraExtent * (3 / 4.0f);
        var horizontalExtent = PlayerController.HorizontalCameraExtent;
        var space = horizontalExtent / numAliens;
        for(int i =0; i < numAliens; i++)
        {
            Alien.Create(new Vector3(-horizontalExtent/2 + space * i, ypos, 0.5f));
        }
    }
}
