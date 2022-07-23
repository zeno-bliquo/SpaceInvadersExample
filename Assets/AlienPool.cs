using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Alien Pool controls the level ( difficulty )  1 to 5
    > choose the background sprite
*/
public class AlienPool : MonoBehaviour
{
    public static AlienPool instance;
    
    static int level = 1;

    public Sprite alienSprite;

    public Sprite mothershipSprite;
   
    private static float time = 0.0f;

    public float waveWaitTimeSecs = 3f;

    public GameObject background;

    public Sprite[] nebulaSprites;

    private const int thresholdScoreSpawMothership = 50;

    private void Awake()
    {
        instance = this;
    } 

    /*
        speed up spawns after 3' level
    */
    void Start()
    {
        Alien.alienSprite = alienSprite;
        Alien_MotherShip.mothershipSprite = mothershipSprite;
        refreshBackgroundLevel();

        if(level > 3){
            waveWaitTimeSecs -= (float) 0.2 * level;
            Debug.Log($"Now aliens spawns quicker, every {waveWaitTimeSecs}s!");
        }
    }

    /*
        spawn aliens with random quantity range of ± 4, increasing on levelup
        or spawn a single instance of mothership when reach xzy score, and after end the game
    */
    void Update()
    {
        time += Time.deltaTime;

        if (time >= waveWaitTimeSecs)
        {
            time = 0.0f;
            int rInt = Random.Range(0 + level, 3 + level );
            
            bool isMothershiopSpawned = GameObject.Find(Alien_MotherShip.mothership_GO_name) != null;
            if( User.instance.getCurrentScore() > thresholdScoreSpawMothership && ! isMothershiopSpawned){
                SpawnSingleMothership();
                return;
            }
            
            SpawnWave(rInt);
        }
    }

    private void SpawnWave(int numAliens)
    {
        var ypos = PlayerController.VerticalCameraExtent * (3 / 4.0f);
        var horizontalExtent = PlayerController.HorizontalCameraExtent;
        for(int i =0; i < numAliens; i++)
        {
            var space = horizontalExtent / numAliens * i;
            float randomPosX = new System.Random().Next(-1,1);
            Alien.Create(new Vector3(-horizontalExtent/2 + space + randomPosX, ypos, 0.5f));
        }
    }

    private void SpawnSingleMothership()
    {
        var ypos = PlayerController.VerticalCameraExtent * (3 / 4.0f);
        var horizontalExtent = PlayerController.HorizontalCameraExtent;
        var xpos = - horizontalExtent / 2 + new System.Random().Next(0,10);
        Alien_MotherShip.Create(new Vector3( xpos, ypos, 0.5f));
    }

    public static int getLevel()
    {
        return level;
    }

    public static int setLevel(int lvl)
    {
        bool isValid = (lvl > 0) && (lvl < 6);
        if( ! isValid ) 
            Debug.Log($"setLevel() invalid value: {lvl}");
        level = isValid ? lvl : 1;
        return level;
    }

    public static int goNextLevel()
    {
        return setLevel(++level);
    }

    private void refreshBackgroundLevel()
    {
        int levelIndex = level -1;
        if( levelIndex < nebulaSprites.Length )
            background.GetComponent<SpriteRenderer>().sprite = nebulaSprites[levelIndex];
    }
}
