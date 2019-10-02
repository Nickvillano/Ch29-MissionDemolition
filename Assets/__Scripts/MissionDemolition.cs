using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// TODO: You must set the values for the enum
public enum GameMode
{
    idle,
    playing,
    levelEnd

}


// TODO: implement the MissionDemolition script
public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S; // a private Singleton

    [Header("Set in Inspector")]
    public Text uitLevel; //The UIText_Level Text
    public Text uitShots; // The UIText_Shots Text
    public Text uitButton; // The Text on UIButton_View
    public Vector3 castlePos; // The place to put castles
    public GameObject[] castles; // An array of the castles

    [Header("Set Dynamically")]
    public int level; //current level
    public int levelMax; // number of levels
    public int shotsTaken;
    public GameObject castle; // current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";//FollowCam mode

	// Use this for initialization
	void Start ()
    {
        S = this;

        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }
	
	void StartLevel()
    {
        // Get rid of the old castle if one exists
        if(castle != null)
        {
            Destroy(castle);
        }

        //Destroy old projectiles if they exist
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        //Instantiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        //Reset the camera
        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        //Reset the goal
        Goal.goalMet = false;

        UpdateGUI();
        mode = GameMode.playing;
    }

    void UpdateGUI()
    {
        //show the data in GUITexts
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }

    void Update()
    {
        UpdateGUI();

        //check for level end
        if ( (mode == GameMode.playing) && Goal.goalMet)
        {
            //Change the mode to stop checking for level end
            mode = GameMode.levelEnd;
            //Zoom Out
            SwitchView("Show Both");
            //Start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            level = 0;
        }
        StartLevel();
    }

    public void SwitchView(string eView = "")
    {
        if (eView == "")
        {
            eView = uitButton.text;
        }
        showing = eView;
        switch(showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;

            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    // Static method that allows code anywhere to increment shotsTaken
    public static void ShotFired()
    {
        S.shotsTaken++;
    }
}
