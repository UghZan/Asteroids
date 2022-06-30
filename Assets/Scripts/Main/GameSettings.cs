using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static bool GameStarted = false;
    public static bool GamePaused = true;
    public static bool ControlScheme = false; //if false, the keyboard scheme is used, otherwise mouse+keyboard

    public static int PlayerLivesOnStart = 3;

    public static float MaxScreenWrapBorder = 1; //rightmost/upmost viewport coordinate
    public static float MinScreenWrapBorder = 0; //leftmost/lowest viewport coordinate

    public static bool SmallerAsteroidsAreFaster = true; //if true, smaller asteroids may be a bit faster
    public static int ChildrenAsteroidsPerAsteroid = 2; //decides how much smaller asteroids spawn after a bigger one is destroyed

    public static int ScorePerLargeAsteroid = 20;
    public static int ScorePerMediumAsteroid = 50;
    public static int ScorePerSmallAsteroid = 100;
    public static int ScorePerUFO = 200;
}
