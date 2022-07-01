using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class with common Asteroid behaviour
public class Asteroid : MonoBehaviour
{
    [SerializeField] Vector2Int ChildrenAsteroidSpeedRange;

    SphereCollider _collider;
    float _speed = 5;
    int _size = 2;
    GameObject _visual;
    Vector3 _randomVisualRotationVector; //doesn't influence collisions, just visual rotation
    ObjectPool _asteroidPool;

    //all asteroid sizes are the same prefab, just with visual model and collider scaled accordingly
    //init, as always, is called upon getting a pooled object to give it needed stats
    public void Init(ObjectPool parentPool, int size, float speed)
    {
        //init sets up collider and visual size of asteroid
        switch(size)
        {
            case 0:
                _visual.transform.localScale = Vector3.one * 2;
                _collider.radius = 1.5f;
                break;
            case 1:
                _visual.transform.localScale = Vector3.one * 4;
                _collider.radius = 2.5f;
                break;
            case 2:
                _visual.transform.localScale = Vector3.one * 6;
                _collider.radius = 3.5f;
                break;
        }
        _size = size;
        _speed = speed;
        _randomVisualRotationVector = Random.insideUnitSphere;
        _asteroidPool = parentPool;
    }

    void OnEnable()
    {
        _collider = GetComponent<SphereCollider>();
        _visual = transform.GetChild(0).gameObject;
    }


    void Update()
    {
        if (GameSettings.instance.GamePaused) return;

        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        _visual.transform.Rotate(_randomVisualRotationVector * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("UFO"))
        {
            DestroyAsteroid(false);
        }
        else if (other.CompareTag("Shot"))
        {
            PlayerScore.ScoreGainEvent.Invoke(GetScoreForAsteroid());//so that score is gained only by destroying asteroid with player shots
            PopupManager.ShowScorePopupEvent.Invoke(GetScoreForAsteroid(), transform.position);//so that score is gained only by destroying asteroid with player shots
            DestroyAsteroid(true);
        }
        else if (other.CompareTag("EnemyShot"))
        {
            DestroyAsteroid(true);
        }
    }

    //Behaviour for destroying asteroid
    //createChildren decides if asteroid will spawn lesser asteroids
    void DestroyAsteroid(bool createChildren)
    {
        //if it's a smallest asteroid or we collided with UFO/player, just destroy the asteroid, otherwise create children
        if (_size > 0 && createChildren)
        {
            SpawnManager.instance.CreateChildAsteroids(_size, transform.position, transform.up);
        }
        gameObject.SetActive(false);
        SpawnManager.instance.CreateAsteroidDebris(transform.position);

        _asteroidPool.OnObjectDisabled.Invoke();
    }

    int GetScoreForAsteroid()
    {
        switch(_size)
        {
            case 0:
                return GameSettings.instance.ScorePerSmallAsteroid;
            case 1:
                return GameSettings.instance.ScorePerMediumAsteroid;
            case 2:
                return GameSettings.instance.ScorePerLargeAsteroid;
            default:
                return 0;
        }
    }
}
