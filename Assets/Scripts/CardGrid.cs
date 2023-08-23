using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.UI;

public class CardGrid : MonoBehaviour
{
    List<CardSO> cardData;

    [SerializeField] private Transform templateCardItem;
    [SerializeField] private Transform container;

    private void Awake() {
        templateCardItem.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        cardData = GameManager.Instance.GetActiveCards();

        foreach(CardSO card in cardData) {
            var cardSpawned = LeanPool.Spawn(templateCardItem, container);
            cardSpawned.gameObject.SetActive(true);
            cardSpawned.GetComponent<CardItemUI>().SetCardSOData(card);
        }
    }
    
    
}
