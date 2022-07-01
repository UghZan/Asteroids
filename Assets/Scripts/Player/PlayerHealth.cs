using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Class responsible for lives/damage system for player
public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnDeath = new UnityEvent();

    [Header("Visual")]
    [SerializeField] GameObject PlayerVisual;
    [SerializeField] Animator PlayerVisualAnimator;

    [Header("Gameplay Settings")]
    [SerializeField] int LivesOnStart;
    [SerializeField] float ImmunityOnStart; //how much seconds of immunity player has on start
    [SerializeField] float ImmunityOnDeath; //ditto, but for after dying

    [Header("Misc References")]
    [SerializeField] PlayerController Controller;

    float _immunityTimer; //while this is not 0, player is immune and plays blinking animation
    public int Lives { get; private set; }

    //Called on new game to reset lives and give starting immunity
    public void ResetToStart()
    {
        Lives = LivesOnStart;
        _immunityTimer = ImmunityOnStart;
    }

    private void Start()
    {
        ResetToStart();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_immunityTimer > 0) return;

        if (other.CompareTag("EnemyShot") || other.CompareTag("Asteroid") || other.CompareTag("UFO"))
        {
            if (Lives > 1)
                StartCoroutine(DeathSequence());
            else
                GameOverSequence();
        }
    }

    private void Update()
    {
        if (GameSettings.instance.GamePaused) return;

        //If we are immune, blinking animation plays
        if(_immunityTimer > 0)
        {
            _immunityTimer -= Time.deltaTime;
            if(!PlayerVisualAnimator.GetBool("immune")) PlayerVisualAnimator.SetBool("immune", true);
        }
        else
            if (PlayerVisualAnimator.GetBool("immune")) PlayerVisualAnimator.SetBool("immune", false);
    }

    IEnumerator DeathSequence()
    {
        Lives--;
        Controller.LockedControls = true;
        SpawnManager.instance.CreateExplosion(transform.position);
        PlayerVisual.SetActive(false);
        Controller.ResetToStart(); //Reset to center of the screen, no rotation, no velocity

        yield return new WaitForSeconds(1.0f);

        PlayerVisual.SetActive(true);
        _immunityTimer = ImmunityOnDeath; //Give immunity to player
        Controller.LockedControls = false;

        yield return null;
    }

    void GameOverSequence()
    {
        OnDeath.Invoke();
        gameObject.SetActive(false);
        SpawnManager.instance.CreateExplosion(transform.position);
    }
}
