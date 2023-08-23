using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameManager.OnGameStateChangedEventArgs GameManager_OnGameStateChangedEventArgs { get; private set; }

    [SerializeField] Transform multicardContainer;
    [SerializeField] Transform singleContainer;

    [SerializeField] GameState state;
   
    private void Awake() {
        if (Instance == null){
            Instance = this;
        }
        else{
            Debug.LogWarning("Duplicated Instances");
        }
        HideUICanvas(singleContainer);
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }



    private void GameManager_OnGameStateChanged(object sender, GameManager.OnGameStateChangedEventArgs e)
    {
        switch (e.newState) {
            case GameState.MultiCard: 
                ShowMulti();
                break;
            case GameState.SingleCard:
                CardSelect();
                break;
            default:
                break;
        }
    }

    private void ShowMulti()
    {
        ShowUICanvas(multicardContainer);
        HideUICanvas(singleContainer);
    }


    public void CardSelect(){
        ShowUICanvas(singleContainer);
        HideUICanvas(multicardContainer);
    }

   


    void ShowUICanvas(Transform uiCanvas){
        uiCanvas.gameObject.SetActive(true);
    }

    void HideUICanvas(Transform uiCanvas){
        uiCanvas.gameObject.SetActive(false);
    }

    
}


