using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardItemDisplay : MonoBehaviour
{
    MeshRenderer _meshRenderer;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        // gameObject.SetActive(false);
        var a = StartCoroutine(StartCAAO());
        
    }  

    IEnumerator StartCAAO(){
        yield return new WaitForSeconds(1);
    }


    private void OnEnable() {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        CardItemUI.OnCardSelected += CardItemUI_OnCardSelected;
    }

    private void GameManager_OnGameStateChanged(object sender, GameManager.OnGameStateChangedEventArgs e)
    {
        if(e.newState != GameState.SingleCard) {
            Hide();
        }
        else{
            Show();
        }
    }

    private void CardItemUI_OnCardSelected(object sender, CardItemUI.OnCardSelectedEventArgs e)
    {
        Show();
        SetDisplayImage(e.cardData);
        CardSelectedTween(e.selectedPosition);
    }

    public void Show(){
        gameObject.SetActive(true);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
    public void SetDisplayImage(CardSO cardData)
    {
        _meshRenderer.materials[1].SetTexture("_Base", cardData.largeImage);
    }

    public void CardSelectedTween(Vector2 selectedPosition)
    {
        Debug.Log("tesstCardItem");
        transform.position = selectedPosition;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(Vector3.zero, 1.5f))
            .Join(transform.DOScale(Vector3.one * 100, 2f));
    
    }
    private void OnDisable() {
        // CardItemUI.OnCardSelected -= CardItemUI_OnCardSelected;
        transform.localScale = Vector3.one;
    }
    

   
}
