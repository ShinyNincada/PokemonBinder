using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler<OnGameStateChangedEventArgs> OnGameStateChanged;

    public class OnGameStateChangedEventArgs : EventArgs{
        public GameState oldState;
        public GameState newState;
        public OnGameStateChangedEventArgs(GameState newState) { this.newState = newState;} 

        public OnGameStateChangedEventArgs(GameState oldState, GameState newState) {
            this.oldState = oldState;
            this.newState = newState;
        }
    }

    public static GameManager Instance { get; private set; }
    [SerializeField] GameState state;
    [SerializeField] private List<CardSO> multiCards;
    [SerializeField] private CardSO singleCard;

    // [SerializeField] CardItemDisplay displayItem;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else{
            Debug.LogWarning("Duplicated Instances");
        }

        multiCards = new List<CardSO>(Resources.LoadAll<CardSO>($"CardSO/sm115/"));
    }

    private void Start() {
        CardItemUI.OnCardSelected += CardItemUI_OnCardSelected;
        GameInput.Instance.OnBackwardAction += GameInput_OnBackwardAction;

        ChangeGameState(GameState.MultiCard);
    }

    private void GameInput_OnBackwardAction(object sender, EventArgs e)
    {
        // Debug.Log("tesst");
    }

    public void ChangeGameState(GameState newState) {
        state = newState;
        OnGameStateChanged?.Invoke(this, new OnGameStateChangedEventArgs(newState));
    }

    private void CardItemUI_OnCardSelected(object sender, CardItemUI.OnCardSelectedEventArgs e)
    {
        singleCard = e.cardData;
        ChangeGameState(GameState.SingleCard);
    }

    public void ChangeStateBackward(){
        int currentStateIndex = (int) state;
        currentStateIndex--;
        ChangeGameState((GameState) currentStateIndex);
    }
    public List<CardSO> GetActiveCards(){
        return multiCards;
    }

    public CardSO GetActiveCard(){
        return singleCard;
    }
}


public enum GameState {
    Collection,
    MultiCard,
    SingleCard,
}