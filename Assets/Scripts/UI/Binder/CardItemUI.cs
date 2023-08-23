using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CardItemUI : MonoBehaviour, IPointerClickHandler
{
    public static event EventHandler<OnCardSelectedEventArgs> OnCardSelected;
    public class OnCardSelectedEventArgs : EventArgs {
        public CardSO cardData;
        public Vector2 selectedPosition;

        public OnCardSelectedEventArgs(CardSO cardData, Vector2 selectedPosition){
            this.cardData = cardData;
            this.selectedPosition = Camera.main.ScreenToWorldPoint(selectedPosition);
        }
    }
    [SerializeField] CardSO cardSO;
    Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void SetCardSOData(CardSO cardData){
        cardSO = cardData;
        _image.sprite = Texture2DToSprite(cardSO.smallImage);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       #region observer
            OnCardSelected?.Invoke(this, new OnCardSelectedEventArgs(cardSO, eventData.pressPosition));
       #endregion
    }

    Sprite Texture2DToSprite(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        return Sprite.Create(texture, rect, pivot);
    }
}
