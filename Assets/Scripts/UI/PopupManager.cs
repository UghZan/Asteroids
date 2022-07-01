using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

//Class responsible for showing little score popups on object destruction
public class PopupManager : MonoBehaviour
{
    public static UnityEvent<int, Vector3> ShowScorePopupEvent = new UnityEvent<int, Vector3>();

    [Header("References")]
    [SerializeField] ObjectPool PopupPrefabPool;
    [SerializeField] Canvas UICanvas;
    [SerializeField] Camera MainCamera;

    void Start()
    {
        ShowScorePopupEvent.AddListener(CreatePopupForScore);
        MainCamera = Camera.main;
    }

    private void CreatePopupForScore(int score, Vector3 worldPosition)
    {
        //I could call the score increment event here to decrease amount of events, but it will be confusing when reading the codebase
        //I also could create Popups in PlayerScore, but that would be kinda strange, as it's a "logic" class, not an "UI" one, so let's leave two events apart from each other
        GameObject _popup = PopupPrefabPool.GetPooledObject();
        _popup.transform.SetParent(UICanvas.transform);
        _popup.transform.position = MainCamera.WorldToScreenPoint(worldPosition);
        _popup.GetComponent<TextMeshProUGUI>().text = "+" + score.ToString();
        _popup.SetActive(true);
    }
}
