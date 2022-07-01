using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class responsible for creating game objects
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    [Header("Pools")]
    [SerializeField] ObjectPool AsteroidPool;
    [SerializeField] ObjectPool UFOPool;
    [SerializeField] ObjectPool ShotPool;

    [Header("Effect Pools")]
    [SerializeField] ObjectPool ExplosionPool;
    [SerializeField] ObjectPool DebrisPool;

    [Header("Asteroid Spawn Settings")]
    //x is min, y is max
    //Vector2Int is used simply for convenience
    public Vector2Int AsteroidSpeedRange;
    public int StartingAsteroids;
    public float AsteroidSpawnDelay;

    [Header("UFO Spawn Settings")]
    public Vector2Int UFOSpawnTimeRange;

    int _asteroidsToSpawn;
    Camera _mainCamera;
    bool _allAsteroidsDown, _UFODown;
    float _nextAsteroidSpawnCountdown, _nextUFOSpawnCountdown;

    void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
    }

    public void ResetToStart()
    {
        AsteroidPool.ResetPool();
        UFOPool.ResetPool();
        ShotPool.ResetPool();
        ExplosionPool.ResetPool();
        DebrisPool.ResetPool();

        _asteroidsToSpawn = StartingAsteroids;
        _UFODown = true;
        _nextUFOSpawnCountdown = GetUFOSpawnDelay();
        
        FirstWave();
    }

    void FirstWave()
    {
        //first wave of asteroids
        for (int i = 0; i < _asteroidsToSpawn; i++)
        {
            CreateAsteroidOnField();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSettings.instance.GamePaused) return;

        //if no UFOs on field, count down til next spawn
        if (UFOPool.ObjectsActive == 0 && !_UFODown)
        {
            _UFODown = true;
            _nextUFOSpawnCountdown = GetUFOSpawnDelay();
        }

        if (_UFODown)
        {
            _nextUFOSpawnCountdown -= Time.deltaTime;
            if(_nextUFOSpawnCountdown <= 0)
            {
                SpawnUFO(GetRandomUFOPosition());
                _UFODown = false;
            }
        }

        //if no asteroids on field, count down till the next wave
        if (AsteroidPool.ObjectsActive == 0 && !_allAsteroidsDown)
        {
            _allAsteroidsDown = true;
            _nextAsteroidSpawnCountdown = AsteroidSpawnDelay;
        }

        if (_allAsteroidsDown)
        {
            _nextAsteroidSpawnCountdown -= Time.deltaTime;
            if (_nextAsteroidSpawnCountdown < 0)
            {
                _asteroidsToSpawn++;
                _allAsteroidsDown = false;
                for (int i = 0; i < _asteroidsToSpawn; i++)
                {
                    CreateAsteroidOnField();
                }
            }
        }
    }
    
    //gets a random position from outside of viewport
    //used for asteroids
    Vector2 GetRandomPositionOutsideViewport()
    {
        if (Random.value < 0.5f) //position outside of upper/lower bound
        {
            if (Random.value < 0.5f) //above
            {
                return new Vector2(Random.Range(0.0f, 1.0f), 1f);
            }
            else
            {
                return new Vector2(Random.Range(0.0f, 1.0f), 0f);
            }
        }
        else //position outside of left/right bound
        {
            if (Random.value < 0.5f) //right
            {
                return new Vector2(1f, Random.Range(0.0f, 1.0f));
            }
            else
            {
                return new Vector2(0f, Random.Range(0.0f, 1.0f));
            }
        }
    }
    
    //transforms viewport position into world position for main camera
    //sets Z to 0
    Vector3 GetCorrectWorldPositionFromViewport(Vector2 position)
    {
        Vector2 viewPort = GetRandomPositionOutsideViewport();
        Vector3 worldSpawnPos = _mainCamera.ViewportToWorldPoint(position);
        worldSpawnPos.z = 0;
        return worldSpawnPos;
    }
    
    //gets a position outside of either right or left side of screen
    //used for UFOs
    Vector2 GetRandomUFOPosition()
    {
        Vector2 position;
        if (Random.value < 0.5f) //spawning on left
        {
            position = new Vector2(0f, Random.Range(0.2f, 0.8f));
        }
        else
        {
            position = new Vector2(1f, Random.Range(0.2f, 0.8f));
        }
        return position;
    }
    
    //creates UFO in certain point
    //flight direction is calculated automatically
    void SpawnUFO(Vector2 position)
    {
        Vector3 worldSpawnPos = _mainCamera.ViewportToWorldPoint(position);
        worldSpawnPos.z = 0;

        GameObject UFO = UFOPool.GetPooledObject();
        UFO.transform.position = worldSpawnPos;
        UFO.GetComponent<UFO>().Init(UFOPool, position.x < 0.5f);
        UFO.SetActive(true);
    }
    
    //gets next UFO delay
    int GetUFOSpawnDelay()
    {
        return Random.Range(UFOSpawnTimeRange.x, UFOSpawnTimeRange.y + 1);
    }
    
    //wrapper for SpawnRandomAsteroid method
    //sets up random position, direction and speed
    void CreateAsteroidOnField()
    {
        Vector2 position = GetRandomPositionOutsideViewport();
        Vector3 worldPosition = GetCorrectWorldPositionFromViewport(position);
        Quaternion randomRotation = Quaternion.LookRotation(Vector3.forward, Random.insideUnitCircle); //gives asteroid random rotation so that they fly towards it
        float speed = Random.Range(AsteroidSpeedRange.x, AsteroidSpeedRange.y + 1);
        SpawnRandomAsteroid(worldPosition, randomRotation, speed, 2);
    }

    //creates an asteroid
    void SpawnRandomAsteroid(Vector3 position, Quaternion rotation, float speed, int size)
    {
        GameObject asteroid = AsteroidPool.GetPooledObject();
        asteroid.transform.position = position;
        asteroid.transform.rotation = rotation;
        asteroid.GetComponent<Asteroid>().Init(AsteroidPool, size, speed);
        asteroid.SetActive(true);
    }
   
    //creates an explosion for UFOs and player deaths
    public void CreateExplosion(Vector3 position)
    {
        GameObject effect = ExplosionPool.GetPooledObject();
        effect.transform.position = position;
        effect.SetActive(true);
    }

    //creates a debris effect for destroyed asteroids
    public void CreateAsteroidDebris(Vector3 position)
    {
        GameObject effect = DebrisPool.GetPooledObject();
        effect.transform.position = position;
        effect.SetActive(true);
    }

    //create a set amount of children asteroids for a certain position that continue to fly on a certain direction with a bit of deviation
    //correctly counts angles for different amount of children
    public void CreateChildAsteroids(int oldSize, Vector3 position, Vector3 originalDirection, int count)
    {
        float newSpeed;

        if (GameSettings.instance.SmallerAsteroidsAreFaster)
            newSpeed = Random.Range(AsteroidSpeedRange.x + (2 - oldSize), AsteroidSpeedRange.y + 1 + (2 - oldSize));
        else
            newSpeed = Random.Range(AsteroidSpeedRange.x, AsteroidSpeedRange.y + 1);

        for (int i = 0; i < count; i++)
        {
            Quaternion nextDirectionRotation = Quaternion.AngleAxis(Mathf.Rad2Deg * (-Mathf.PI/2 + i * Mathf.PI/count), Vector3.forward);
            SpawnRandomAsteroid(position, Quaternion.LookRotation(Vector3.forward, originalDirection) * nextDirectionRotation, newSpeed, oldSize - 1);
        }
    }

    //create a shot that destroys player and asteroids, doesn't collide with UFOs
    public void CreateHostileShot(Vector3 position, Vector2 direction, float speed, float distance)
    {
        Debug.Log("Hostile shot created");
        CreateShot(position, Quaternion.LookRotation(Vector3.forward, direction), speed, distance, true);
    }

    //create a shot that destroys UFOs and asteroids, doesn't collide with player
    public void CreatePlayerShot(Vector3 position, Vector2 direction, float speed, float distance)
    {
        CreateShot(position, Quaternion.LookRotation(Vector3.forward, direction), speed, distance, false);
    }

    //creates a specific shot
    void CreateShot(Vector3 position, Quaternion rotation, float speed, float distance, bool hostile)
    {
        GameObject shot = ShotPool.GetPooledObject();
        shot.transform.position = position;
        shot.transform.rotation = rotation;
        shot.GetComponent<Shot>().Init(speed, distance, hostile);
        shot.SetActive(true);
    }
}
