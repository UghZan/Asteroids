using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for common Shot capability
public class Shot : MonoBehaviour
{
    float _speed, _distance;
    [SerializeField] Material[] shotMaterials;

    //Called upon getting a shot object from pool
    //Same objects are used for both player and UFO shots
    public void Init(float speed, float distance, bool hostile)
    {
        _speed = speed;
        _distance = distance;
        tag = hostile ? "EnemyShot" : "Shot";
        GetComponent<Renderer>().material = hostile ? shotMaterials[1] : shotMaterials[0];
    }

    void Update()
    {
        //Shot will not fly when game is paused, obviously
        if (GameSettings.instance.GamePaused) return;

        transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);

        _distance -= _speed * Time.deltaTime;
        if (_distance <= 0)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Shot"))
        {
            if (other.CompareTag("Asteroid") || other.CompareTag("UFO"))
                gameObject.SetActive(false);
        }
        else if (CompareTag("EnemyShot"))
        {
            if (other.CompareTag("Asteroid") || other.CompareTag("Player"))
                gameObject.SetActive(false);
        }
    }
}
