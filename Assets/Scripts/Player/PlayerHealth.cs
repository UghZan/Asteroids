using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnDeath = new UnityEvent();

    [Header("Visual")]
    [SerializeField] GameObject PlayerVisual;
    [SerializeField] Animator PlayerVisualAnimator;

    [Header("Gameplay Settings")]
    [SerializeField] float ImmunityOnStart; //how much seconds of immunity player has on start
    [SerializeField] float ImmunityOnDeath; //ditto, but for after dying

    [Header("Misc References")]
    [SerializeField] PlayerController Controller;

    float _immunityTimer;
    public int Lives { get; private set; }

    public void ResetToStart()
    {
        Lives = GameSettings.PlayerLivesOnStart;
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
        if (GameSettings.GamePaused) return;

        //if we are immune, blinking animation plays
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
        Controller.ResetToStart(); //reset to center of the screen, no rotation, no velocity

        yield return new WaitForSeconds(1.0f);

        PlayerVisual.SetActive(true);
        _immunityTimer = ImmunityOnDeath; //give immunity
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
