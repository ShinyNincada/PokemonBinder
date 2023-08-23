using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CardSO : ScriptableObject
{
    public string cardId;
    public string cardName;
    public string cardSetID;
    public string cardSetName;
    public TYPE cardType;
    public SUPERTYPE cardSuperTypes;
    public RARITY cardRarity;
    public float lowPrice;
    public float midPrice;
    public float highPrice;
    public string smallImageUrl;
    public string largeImageUrl;
    public Texture2D smallImage;
    public Texture2D largeImage;
}
