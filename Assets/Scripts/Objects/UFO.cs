using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    //x is min, y is max
    //Vector2Int is used simply for convenience
    [SerializeField] Vector2Int UFOShootFrequency;
    [SerializeField] int UFOShotSpeed;
    float _UFOSpeed, _shotDistance;
    Transform _player;

    ObjectPool _UFOPool;

    bool _direction = true;

    public void Init(ObjectPool parentPool, bool direction)
    {
        //direction decides whether UFO flies right or left
        _direction = direction;
        //to correctly count disabled/enabled pool objects
        _UFOPool = parentPool;
    }

    private void OnEnable()
    {
        if (_player == null) _player = GameObject.Find("Player")?.transform;

        _UFOSpeed = (Camera.main.orthographicSize * 2 * Camera.main.aspect) / 10; //it will pass the screen in ~10 seconds, no matter the screen size
        _shotDistance = Camera.main.orthographicSize * 2; //screen vertical size
        StartCoroutine(ShootPlayer());
    }

    private void Update()
    {
        if (GameSettings.GamePaused) return;

        transform.Translate((_direction ? Vector2.right : Vector2.left) * _UFOSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
            DisableUFO();
        else if (other.CompareTag("Shot"))
        {
            DisableUFO();
        }
    }

    void DisableUFO()
    {
        PlayerScore.ScoreGainEvent.Invoke(GameSettings.ScorePerUFO);

        SpawnManager.instance.CreateExplosion(transform.position);
        _UFOPool.OnObjectDisabled.Invoke();
        gameObject.SetActive(false);
    }

    IEnumerator ShootPlayer()
    {
        while (true)
        {
            float nextShotIn = Random.Range(UFOShootFrequency.x, UFOShootFrequency.y + 1);
            float timer = 0;
            while(timer < nextShotIn)
            {
                if (!GameSettings.GamePaused) //if the game is paused, UFO cooldown doesn't update
                    timer += Time.deltaTime;
                yield return null;
            }

            SpawnManager.instance.CreateHostileShot(transform.position - transform.up * 2, (_player.position - transform.position).normalized, UFOShotSpeed, _shotDistance);
            yield return null;
        }
    }
}
