using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used to store settings or variables, that are needed in many places at once, are gamewide or don't make sense to leave in certain classes
//Most one place settings are set in their respective classes
//Settings for objects like UFOs or asteroids are set on their prefabs to make it possible to create variants
public class GameSettings : MonoBehaviour
{
    public static GameSettings instance; //would like not to use Singleton, but I want to change settings through inspector, so...

    [Header("Common Variables")]
    public bool GameStarted = false;
    public bool GamePaused = true;
    public bool ControlScheme = false; //if false, the keyboard scheme is used, otherwise mouse+keyboard

    [Header("Screen Wrap Settings")]
    public float MaxScreenWrapBorder = 1; //rightmost/upmost viewport coordinate
    public float MinScreenWrapBorder = 0; //leftmost/lowest viewport coordinate

    [Header("Gameplay Settings")]
    public bool SmallerAsteroidsAreFaster = true; //if true, smaller asteroids may be a bit faster
    public int ChildrenAsteroidsPerAsteroid = 2; //decides how much smaller asteroids spawn after a bigger one is destroyed

    [Header("Score Settings")]
    public int ScorePerLargeAsteroid = 20;
    public int ScorePerMediumAsteroid = 50;
    public int ScorePerSmallAsteroid = 100;
    public int ScorePerUFO = 200;

    [Header("UI Settings")]
    public float ScorePopupLingerTime = 3f; //time in seconds for which score popup will remain visible

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }
}
