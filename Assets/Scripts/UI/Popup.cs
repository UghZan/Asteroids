using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(TimedDisable());
    }

    IEnumerator TimedDisable()
    {
        yield return new WaitForSeconds(GameSettings.instance.ScorePopupLingerTime);
        gameObject.SetActive(false);
    }
}
