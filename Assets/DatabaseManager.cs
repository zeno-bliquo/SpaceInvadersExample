using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    public InputField Name;

    public Text MainLogText;

    private static string userId;

    private static DatabaseReference dbReference;

    public static bool isUserLogged = false;

    public GameObject playerController;

    /*
        stringsvars > db cohesion
    */
    private static string USER_COL = "users";
    private static string USER_COL_NAME = "userName";
    public static string USER_COL_PRECISION = "precisionGlobal";
    public static string USER_COL_HIGHSCORE = "highestScore";

    /* 
        enable instance and insert a new session
    */
    private void Awake(){
        instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        userId = SystemInfo.deviceUniqueIdentifier;
    } 

    void Start()
    {
        createUserAttached();
        StartCoroutine(GetUser( res_user => User.instance = res_user ));

        // if user created has less than 10min of total game experience, show 'quest' reminder 10sec after login procedure
        this.Invoke( ()=>{ 
            User.instance.addSingleSession();
            if(User.instance.timePlayedTotal < 60 * 10) 
                notify("Q U E S T \n defeat aliens and level up \n until reaching the farthest galaxy!", 10);
        },10);
    }

    // triggered by onClick event
    public void CreteUser()
    {
        if(Name.text == ""){
            PauseMenu.instance.setText_log("Error add a valid name");
            return;
        }
        CreateUserFirebase(Name.text);
    }
    
    /*
        - triggered by keyboard press: 
        - create a new firebase user with name as input field and set new score 0
    */
    public static void CreateUserFirebase(string _name){
        if( _name == "" ) 
            return;

        User.instance.setUserName(_name);
        isUserLogged = true;
        UpdateUser();

        PauseMenu.instance.setText_log($"Hello {_name} \nNow you signed!");
    }
    
    /*
        if user exist on db, update stats and commit to firebase
    */
    public static void UpdateUser()
    {
        if( isUserLogged ){
            User.instance.alienKilledTotal += User.instance.getAlienKilled();
            User.instance.bulletsFiredTotal += User.instance.getBulletFired();
            User.instance.precisionGlobal = (int) Math.Floor( (double) User.instance.alienKilledTotal / User.instance.bulletsFiredTotal * 100 );
            User.instance.timePlayedTotal += User.instance.getTimeDelta();
            
            dbReference.Child(USER_COL).Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(User.instance));
            Debug.Log($"Firebase update user() \n {JsonUtility.ToJson(User.instance)}");
        }
        else Debug.Log("Cant update user, please log in");
    }

    /*
        check if userId exist on db,
        load or create a local player,
        notify if logged
        preload pause menu
    */
    public IEnumerator GetUser(Action<User> onCallback)
    {
        var userRef = dbReference.Child(USER_COL).Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: ()=> userRef.IsCompleted);

        Func<string, string> trimmaNome = n => Regex.Replace(n, @"[^a-zA-Z]+", String.Empty);

        if(userRef!=null){
            DataSnapshot snapshot = userRef.Result;

            if(snapshot!=null && snapshot.Value!=null){                
                User.instance
                        .setUserName(trimmaNome(snapshot.Child(USER_COL_NAME).GetRawJsonValue()))
                        .setLevelUnlocked(int.Parse(snapshot.Child("difficultyUnlocked").GetRawJsonValue()))
                        .SetHighestScore(int.Parse(snapshot.Child(USER_COL_HIGHSCORE).GetRawJsonValue()));

                User.instance.alienKilledTotal = int.Parse(snapshot.Child("alienKilledTotal").GetRawJsonValue());
                User.instance.bulletsFiredTotal = int.Parse(snapshot.Child("bulletsFiredTotal").GetRawJsonValue());
                User.instance.deadsPlayer = int.Parse(snapshot.Child("deadsPlayer").GetRawJsonValue());
                User.instance.precisionGlobal = int.Parse(snapshot.Child("precisionGlobal").GetRawJsonValue());
                User.instance.timePlayedTotal = int.Parse(snapshot.Child("timePlayedTotal").GetRawJsonValue());
                User.instance.totalNumberSessions = int.Parse(snapshot.Child("totalNumberSessions").GetRawJsonValue());

                this.Invoke( ()=>notify($"Hi {User.instance.getUserName()}", 2.1f), 1);
                DatabaseManager.isUserLogged = true;
                
                onCallback.Invoke( User.instance );
            }
            else{
                notify("Press ESC to create your user", 2.1f);
                DatabaseManager.isUserLogged = false;
                onCallback.Invoke( User.instance.setUserName("Player") );
            }
            PauseMenu.instance.toggleInputNewUser( ! isUserLogged );
            PauseMenu.instance.setText_log($"Hi {User.instance.getUserName()}");
        }
        else Debug.LogWarning("Firebase: not found user reference!");
    }

    /*
        initialize user instance and attach to component PlayerController
    */
    private void createUserAttached()
    {
        if((playerController.GetComponent("User") as User) == null)
            playerController.AddComponent<User>().initializeUser("Player",0);
        else
           Debug.Log("User component class already exist!");
    }

    // utility: show on top of the screen a message for tot. seconds
    public void notify(string message, float seconds){
        MainLogText.text = message;
        this.Invoke( ()=> MainLogText.text = "" , seconds );
    }

    /*
        - create 2 lambda utils
        - get db reference for query, get datas
        - check consistency
        - create string with ranked list of players + queryValue retrived from data's reference
        - callback return the string created
    */
    public static IEnumerator getQueryTops(string query, Action<string> onCallback){
        Func<string, string> trimmaName = n => Regex.Replace(n, @"[^a-zA-Z]+", String.Empty);
        Func<DataSnapshot, string, bool> existChild = (snapToCheck, field) => { return snapToCheck.Child(field)!=null && snapToCheck.Child(field).Value!=null; };

        var queryRefs = FirebaseDatabase.DefaultInstance.GetReference(USER_COL).OrderByChild(query).LimitToLast(5).GetValueAsync();
        yield return new WaitUntil(predicate: ()=> queryRefs.IsCompleted);

        if(queryRefs!=null){
            DataSnapshot snapshot = queryRefs.Result;
            if(snapshot!=null && snapshot.Value!=null)
            {
                string ranking = "";
                int rankingIdx = 1;
                foreach (Firebase.Database.DataSnapshot childSnapshot in Enumerable.Reverse( snapshot.Children ) )
                {
                    if( existChild(childSnapshot,USER_COL_NAME) && existChild(childSnapshot, query) )
                        ranking += $"\n {rankingIdx++} {trimmaName(childSnapshot.Child(USER_COL_NAME).Value.ToString())} " 
                                    + $"{childSnapshot.Child(query).Value}{( query==USER_COL_PRECISION ? '%' : "" )}";
                    
                }
                onCallback.Invoke( ranking );
            }
            else{ Debug.Log($"getQueryTops() exist but is empty!"); }
        }
        else Debug.Log($"getQueryTops() return null from firebase searching: {query}");
    }
}