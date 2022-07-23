using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;

    public static PauseMenu instance;

    public GameObject userMenu;

    public InputField inputFieldUsername;

    public Button saveInputFieldUsername; 

    public Text textLogPause;

    public Text textStats;

    public Text TextStatsGlobal;

    private void Awake()
    {
        instance = this;
    } 

    void Start(){
        this.Invoke( ()=> textLogPause.text = $"Hi {User.instance.getUserName()}",5);
    }

    /*
        show/hide pause menu elements
        - if un-registred, show a form to signin
        - trigger on keypress enter for creating a new user
    */
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            userMenu.SetActive( ! isGamePaused );
            Time.timeScale = isGamePaused ? 1f : 0;
            isGamePaused = ! isGamePaused;

            if( ! isGamePaused ) showInfosMenuPause();
        }   

        if( (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return)) && isGamePaused)
            DatabaseManager.CreateUserFirebase(inputFieldUsername.text);
    }

    private void showInfosMenuPause()
    {
        textStats.text = User.instance.getStatsText();
        TextStatsGlobal.text = User.instance.getStatsGlobalText();
    }

    public void setText_log(string msg)
    {
        textLogPause.text = msg;
    }

    public void toggleInputNewUser(bool showed)
    {
        inputFieldUsername.gameObject.SetActive( showed );
        saveInputFieldUsername.gameObject.SetActive( showed );
    }
}
