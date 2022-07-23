using UnityEngine;
using System;

public class User : MonoBehaviour
{
    public static User instance;

    public string userName;

    int currentScore;   // temp datas are not synced with firestore

    public int highestScore;

    int bulletsFired;   // temp datas are not synced with db

    public int bulletsFiredTotal;

    int alienKilled;    // temp datas are not synced with db

    public int alienKilledTotal;

    public int deadsPlayer;

    public int precisionGlobal;

    public int totalNumberSessions;

    float timePlayedCurrent;

    public float timePlayedTotal;

    public int difficultyUnlocked;

    public static bool isGodMode = false;    // Only for testing env

    /*
    *    internal score update
    */
    private int counterKillsWithOutDeath = 0;
    private int tresholdScoreMultiply = 10;
    private int scoreBonus = 5;
    private const int lastLevel = 5;        // max difficulty avaiable

    /*
    *    set local vars (unsynced with db)
    */
    public User()
    {
        Debug.Log($"User() class created");
        currentScore = 0;
        alienKilled = 0;
        instance = this;
    }


    public void initializeUser(string _name, int _highestScore)
    {
        Debug.Log($"User() initializeUser {_name} ");
        userName = _name;
        highestScore = _highestScore;
    }

    public User setUserName(string _name){
        this.userName = _name;
        return this;
    }

    public string getUserName(){
        return userName;
    }

    public User addProjectileFired(int num){
        bulletsFired += num;
        return this;
    }

    // con 10 kill senza morte c'è un bonus di 5 punti score
    public User AddAlienKilled(int k)
    {
        alienKilled += k;
        if( counterKillsWithOutDeath++ > tresholdScoreMultiply){
            DatabaseManager.instance.notify($"N E W S \n 10 straight kills \n +{scoreBonus} score",3.0f);
            addCurrentScore(scoreBonus);
            counterKillsWithOutDeath = 0;
        }
        return this;
    }

    public int addCurrentScore(int p){
        currentScore += isGodMode ? p * 4 : p;
        return currentScore;
    }

    public void addSession(int s){
        totalNumberSessions += s;
        Debug.Log($"Session added {s} \t total: {totalNumberSessions}");
    }

    public void addSingleSession(){
        addSession(1);
    }

    public void addDeath(int d){
        deadsPlayer += d;
        counterKillsWithOutDeath = 0;
    }

    public User setCurrentSessionTime(float _sessionTime){
        timePlayedCurrent = _sessionTime;
        return this;
    }

    public int getCurrentScore(){
        return currentScore;
    }

    public int getAlienKilled(){ 
        return this.alienKilled; 
    }

    public int getBulletFired(){
        return this.bulletsFired;
    }

    public int getTimeDelta(){
        return (int) MathF.Floor(this.timePlayedCurrent);
    }

    public int getDifficultyUnlocked(){
        return difficultyUnlocked;
    }

    public User setLevelUnlocked(int lvl){
        if(lvl > lastLevel || lvl < 1 ) Debug.Log($"User {userName} has reached max level {lastLevel}, cant go to {lvl}");
        difficultyUnlocked = lvl;
        return this;
    }

    public bool isLastLevel(){
        int difficultyCurrent = AlienPool.getLevel();
        return difficultyCurrent == lastLevel;
    }

    public void SetHighestScore(int hsp){
        highestScore = hsp;
    }

    public bool isHighestScore(){
        return currentScore >= highestScore;
    }

    /*
        use temporary 'updated' stats, saved at game win/over
    */
    public string getStatsText(){
        float updatedTotalTime = timePlayedCurrent + timePlayedTotal;
        setCurrentSessionTime( Time.realtimeSinceStartup ); // TODO UPDATE DEL TEMPO NON DEVE STARE QUA

        Func<int, string> precisionText = i => i==-1 ? "-" : $"{i}%";

        return "C U R R E N T\n" +
                $"score {currentScore}\n" +
                $"# projectiles {bulletsFired}\n" +
                $"# aliens defeated {alienKilled}\n" +
                $"precision {precisionText(getCurrentPrecision())}\n" +
                "\n\n" +
                $"time {Math.Floor(timePlayedCurrent).ToString()}s\n" +
                $"avg. time/session {Math.Floor(Math.Floor(updatedTotalTime) / totalNumberSessions )}s\n" +    // TODO
                "\n" +
                $"level { AlienPool.getLevel() }";
    }

    /*
        use temporary 'updated' stats, saved at game win/over
    */
    public string getStatsGlobalText(){
        int updateTotalAlienKilled = alienKilledTotal + alienKilled;
        int updatedTotalBulletsFired = bulletsFired + bulletsFiredTotal;
        int globalPrecision = updatedTotalBulletsFired==0 ? 0 : (int) Math.Floor( (double) updateTotalAlienKilled / updatedTotalBulletsFired * 100 );

        return "G L O B A L S\n" +
                $"{highestScore}\n" +
                $"{updatedTotalBulletsFired}\n" +
                $"{updateTotalAlienKilled}\n" +
                $"{globalPrecision}%\n" +
                $"{Math.Round((double)deadsPlayer/updateTotalAlienKilled,2)} d/k\n" +
                "\n" +
                $"{getDurationFromTime( timePlayedCurrent + timePlayedTotal)}\n" +
                $"{totalNumberSessions} sessions\n" +
                "\n" +
                $"{difficultyUnlocked} unlocked";
    }

    /*
        data time helper
    */
    public string getDurationFromTime(float seconds){
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"hh\:mm\:ss");
    }

    /*
        computed current precision helper
    */
    public int getCurrentPrecision(){
        return bulletsFired==0 ? -1 : (int) Math.Floor((double) alienKilled / bulletsFired * 100 );
    }
}
