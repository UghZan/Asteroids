using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    //set this up in Asteroid prefab
    //x is min, y is max
    //Vector2Int is used simply for convenience
    [SerializeField] Vector2Int ChildrenAsteroidSpeedRange;

    SphereCollider _collider;
    float _speed = 5;
    int _size = 2;
    GameObject _visual;
    Vector3 _randomVisualRotationVector; //doesn't influence collider, just visual rotation
    ObjectPool _asteroidPool;

    public void Init(ObjectPool parentPool, int size, float speed) //size is 0 for small, 1 for medium, 2 for big
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
        if (GameSettings.GamePaused) return;

        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        _visual.transform.Rotate(_randomVisualRotationVector * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("UFO"))
        {
            DestroyAsteroid(false);
        }
        else if (other.CompareTag("Shot") || other.CompareTag("EnemyShot"))
        {
            DestroyAsteroid(true);
        }
    }

    void DestroyAsteroid(bool createChildren)
    {
        //if it's a smallest asteroid or we collided with UFO/player, just destroy the asteroid, otherwise create children
        if (_size > 0 && createChildren)
        {
            SpawnManager.instance.CreateChildAsteroids(_size, transform.position, transform.up, GameSettings.ChildrenAsteroidsPerAsteroid);
        }
        gameObject.SetActive(false);
        SpawnManager.instance.CreateAsteroidDebris(transform.position);

        _asteroidPool.OnObjectDisabled.Invoke();
        PlayerScore.ScoreGainEvent.Invoke(GetScoreForAsteroid());
    }

    int GetScoreForAsteroid()
    {
        switch(_size)
        {
            case 0:
                return GameSettings.ScorePerSmallAsteroid;
            case 1:
                return GameSettings.ScorePerMediumAsteroid;
            case 2:
                return GameSettings.ScorePerLargeAsteroid;
            default:
                return 0;
        }
    }
}