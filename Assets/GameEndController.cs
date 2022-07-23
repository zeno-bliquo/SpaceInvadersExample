using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndController : MonoBehaviour
{
    public Text gameEndHeader;
    public Text finalscoreText;
    public Text topPlayersText;
    public Text topSnipersText;
    public Button continue_restart_btn;

    public static bool isGameEndControllerActive = false;

    /*
        Stop time and show menu
        Set headline text
        Set subtitle text
        Update user on db
        Load stats from db
        if gameover, restart from level 1
    */
    public void Setup(bool hasWin)
    {
        gameObject.SetActive(true);
        isGameEndControllerActive = true;
        Time.timeScale = 0;

        gameEndHeader.text = hasWin ? "W I N" : "G  A  M  E       O  V  E  R";
        gameEndHeader.color = hasWin ? Color.blue : Color.red;
        finalscoreText.text = getSubtitleText();

        continue_restart_btn.GetComponentInChildren<Text>().text = hasWin ? $"Go to level { AlienPool.getLevel() }":"Restart";
        if( User.instance.isLastLevel() && hasWin ){
            continue_restart_btn.GetComponentInChildren<Text>().text = "";
        }
        
        DatabaseManager.UpdateUser(); 

        StartCoroutine( DatabaseManager.getQueryTops(DatabaseManager.USER_COL_HIGHSCORE, res => topPlayersText.text = $"Top ranked players:\n{res}" ));
        StartCoroutine( DatabaseManager.getQueryTops(DatabaseManager.USER_COL_PRECISION, res => topSnipersText.text = $"Top snipers players:\n{res}" ) );

        Debug.Log($"Game end setup : actual level is {AlienPool.getLevel()} \t now continue or reset");
       
        if( ! hasWin ) AlienPool.setLevel(1);
    }

    public void Continue_or_Restart()
    {
        Debug.Log("GameEndController.Continue_or_Restart()");
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
        isGameEndControllerActive = false;
    }

    public void Exit()
    {
        Debug.Log("Exit!");
        Application.Quit();
    }

    private string getSubtitleText()
    {
        Func<int, string> precisionText = i => i==-1 ? "-" : $"{i}%";
        return $"Final score: {User.instance.getCurrentScore()}\n" + 
                $"Peak score: {User.instance.highestScore}\n" +
                $"Precision: {precisionText(User.instance.getCurrentPrecision())}";
    }
}
