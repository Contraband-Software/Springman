using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int highscore;
    public bool musicOn = true;
    public bool soundsOn = true;
    public string language = "english";
    public int langIndex = 0;
    public int gold;
    public int silver;
    public bool tutorialComplete = false;
    public int ads;

    public SaveData(int allTimeHighscore, 
        bool musicOn, 
        bool soundsOn, 
        string language, 
        int langIndex, 
        int gold, 
        int silver, 
        bool tutorialComplete, 
        int ads)
    {
        highscore = allTimeHighscore;
        this.musicOn = musicOn;
        this.soundsOn = soundsOn;
        this.language = language;
        this.langIndex = langIndex;
        this.gold = gold;
        this.silver = silver;
        this.tutorialComplete = tutorialComplete;
        this.ads = ads;
    }
}
