using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] float Acceleration;
    [SerializeField] float MaxSpeed;
    [SerializeField] float RotationSpeed;
    [SerializeField] InputController InputControl;

    [Header("Attack Settings")]
    [SerializeField] ObjectPool ShotPool;
    [SerializeField] float ShotSpeed;
    [SerializeField] float ShotsPerSecond;

    [Header("Visual Settings")]
    [SerializeField] ParticleSystem AccelerationEffect;
    [SerializeField] ParticleSystem ShootEffect;

    [Header("Audio Settings")]
    [SerializeField] AudioSource ShipAudioSource;

    public bool LockedControls;
    Quaternion _rotation;
    Vector2 _currentVelocity;
    float _shotDelay, _shotDistance;
    Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _shotDistance = _camera.orthographicSize * 2 * _camera.aspect;
    }

    public void ResetToStart()
    {
        if(AccelerationEffect.isPlaying) AccelerationEffect.Stop();
        transform.position = Camera.main.ViewportToWorldPoint(Vector3.one / 2); //get a position in the center of the screen
        transform.rotation = Quaternion.identity;
        _currentVelocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSettings.GamePaused || LockedControls) return;

        UpdateShipRotation();
        RotateShip();

        Accelerate();
        ApplyAcceleration();

        Shoot();

        _shotDelay += Time.deltaTime;
    }

    void Shoot()
    {
        //if shoot button is pressed and shot cooldown is over
        if(InputControl.IsShooting() && _shotDelay > 1/ShotsPerSecond)
        {
            SpawnManager.instance.CreatePlayerShot(transform.position + transform.up * 2, transform.up, ShotSpeed, _shotDistance);
            ShootEffect.Play();
            _shotDelay = 0;

        }
    }

    void ApplyAcceleration()
    {
        transform.Translate(_currentVelocity * Time.deltaTime,Space.World); 
    }

    void RotateShip()
    {
        transform.rotation = _rotation;
    }

    void Accelerate()
    {
        if (InputControl.IsAccelerating())
        {
            if (!AccelerationEffect.isPlaying) AccelerationEffect.Play();
            if (!ShipAudioSource.isPlaying) ShipAudioSource.Play();
            //smoothly accelerate towards current direction of ship
            _currentVelocity = Vector2.Lerp(_currentVelocity, transform.up * MaxSpeed, Time.deltaTime * Acceleration);
        }
        else
        {
            if (AccelerationEffect.isPlaying) AccelerationEffect.Stop();
            ShipAudioSource.Stop();
        }
    }

    void UpdateShipRotation()
    {
        if (GameSettings.ControlScheme) //mouse+keyboard control scheme
        {
            Vector2 shipScreenPosition = _camera.WorldToScreenPoint(transform.position);
            //get angle towards mouse with a 90 degree correction
            float requiredAngle = Mathf.Atan2(Input.mousePosition.y - shipScreenPosition.y, Input.mousePosition.x - shipScreenPosition.x) * Mathf.Rad2Deg - 90f;
            _rotation = Quaternion.RotateTowards(_rotation, Quaternion.AngleAxis(requiredAngle, Vector3.forward), RotationSpeed);

        }
        else
        {
            _rotation = transform.rotation * Quaternion.AngleAxis(-InputControl.GetRotation() * RotationSpeed, Vector3.forward);
        }
    }
}
