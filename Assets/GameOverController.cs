using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public Text finalscoreText;
    public Text topPlayersText;
    public Text topSnipersText;

    /*
        stop time, block keybinding, 
        show gameover screen, 
        update user data and 
        lastly async load stats of top players and snipers (most precise)
    */
    public void Setup(){
        finalscoreText.text = $"Final score: {User.instance.getCurrentScore()} \n peak score: {User.instance.highestScore}";
        
        gameObject.SetActive(true);

        Time.timeScale = 0;
        
        DatabaseManager.UpdateUser(); 

        StartCoroutine( DatabaseManager.getQueryTops(DatabaseManager.USER_COL_HIGHSCORE, res => topPlayersText.text = $"Top ranked players:\n{res}" ));
        StartCoroutine( DatabaseManager.getQueryTops(DatabaseManager.USER_COL_PRECISION, res => topSnipersText.text = $"Top snipers players:\n{res}" ) );

    }

    // TODO : reload user with updated statistics!
    // Gameover restart with 0 difficulty!
    public void Restart(){
        Debug.Log("GameOverController.Restart()");
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }

    public void Exit(){
        Debug.Log("Exit!");
        Application.Quit();
    }
}
