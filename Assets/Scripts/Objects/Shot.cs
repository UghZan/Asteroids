using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    float _speed, _distance;
    [SerializeField] Material[] shotMaterials;

    public void Init(float speed, float distance, bool hostile)
    {
        _speed = speed;
        _distance = distance;
        tag = hostile ? "EnemyShot" : "Shot";
        GetComponent<Renderer>().material = hostile ? shotMaterials[1] : shotMaterials[0];
    }
    // Update is called once per frame
    void Update()
    {
        if (GameSettings.GamePaused) return;

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
