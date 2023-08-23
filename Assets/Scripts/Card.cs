using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu()]
public class Card : ScriptableObject
{
    public CardSO cardSO;
    public int encounterCount;
    public int ownedCount;
}

public enum SUPERTYPE{
    Energy,
    Pokemon,
    Trainer
}

public enum TYPE{
    Colorless,
    Darkness,
    Dragon,
    Fairy,
    Fighting,
    Fire,
    Grass,
    Lightning,
    Metal,
    Psychic,
    Water
}
public enum RARITY
{
    AmazingRare,
    ClassicCollection,
    Common,
    DoubleRare,
    HyperRare,
    IllustrationRare,
    LEGEND,
    Promo,
    RadiantRare,
    Rare,
    RareACE,
    RareBREAK,
    RareHolo,
    RareHoloEX,
    RareHoloGX,
    RareHoloLVX,
    RareHoloStar,
    RareHoloV,
    RareHoloVMAX,
    RareHoloVSTAR,
    RarePrime,
    RarePrismStar,
    RareRainbow,
    RareSecret,
    RareShining,
    RareShiny,
    RareShinyGX,
    RareUltra,
    SpecialIllustrationRare,
    TrainerGalleryRareHolo,
    UltraRare,
    Uncommon
}