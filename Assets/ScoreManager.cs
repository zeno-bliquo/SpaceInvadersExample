using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    // public GameOverController gameOverController;

    public GameEndController gameEndController;

    public Sprite lifeSprite;

    public Text scoreText;

    List<GameObject> lifes_GameObj;

    int lifes = 5;

    private bool notifyHighestScore = true;     // if false, dont notify when user makes a new record 

    private void Awake()
    {
        instance = this;
        lifes_GameObj = new List<GameObject>();
        LifeViewer.lifeSprite = lifeSprite;
    }

    void Start()
    {
        if( AlienPool.getLevel() > 3 ) lifes = 3;
        drawLifes(lifes);
    }

    public void AddSinglePoint()
    {
        int updatedScore = User.instance.addCurrentScore(1);
        scoreText.text = $"Score: {updatedScore}";

        if( User.instance.isHighestScore() ){
            if( notifyHighestScore ){
                DatabaseManager.instance.notify("N E W S \n ! score record !", 3.0f);
                notifyHighestScore = false;
            }
            User.instance.SetHighestScore(updatedScore);
        }
    }

    public void RemoveLife()
    {
        if( User.isGodMode ) return;

        if( lifes < 1 ){ GameOver(); return; }
        
        lifes--;
        int lastIndex = lifes_GameObj.Count-1;
        Destroy( lifes_GameObj[lastIndex] );
        lifes_GameObj.RemoveAt(lastIndex);
    }

    /*
        invoked by 0 lifes remaining or mothership passes ship horizon
    */
    public void GameOver()
    {
        User.instance.addDeath(1);
        gameEndController.Setup(false);
    }

    /*
        invoked by defeating mothership
    */
    public void GameWin()
    {
        if( ! User.instance.isLastLevel() )
            User.instance.setLevelUnlocked( AlienPool.goNextLevel() );
        gameEndController.Setup(true);
    }

    private void drawLifes(int numLifes)
    {
        var ypos = PlayerController.VerticalCameraExtent * (3 / 4.0f) + 0.5f;
        var horizontalExtent = PlayerController.HorizontalCameraExtent;
        var space = horizontalExtent / numLifes / 2;
        float xPosFirstLife = -horizontalExtent/4 + 7;
        for(int i = 0; i < numLifes; i++){
            lifes_GameObj.Add( LifeViewer.Create(new Vector3( xPosFirstLife + space * i, ypos, 0.5f)) );
        }
    }
}
