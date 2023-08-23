using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;


public static class WebRequest
{
    private class WebRequestMonoBehaviour : MonoBehaviour {}

    private static WebRequestMonoBehaviour webRequestMonoBehaviour;

    private static void Init(){
        if(webRequestMonoBehaviour == null){
            GameObject gameObject = new GameObject("WebRequests");
            webRequestMonoBehaviour = gameObject.AddComponent<WebRequestMonoBehaviour>();               
        }

    }

    private static void GetRawData(string url, Action<string> OnError, Action<string> OnSuccess){
        Init();
        webRequestMonoBehaviour.StartCoroutine(GetRawDataCO(url, OnError, OnSuccess));
    }    

    private static IEnumerator GetRawDataCO(string url, Action<string> OnError, Action<string> OnSuccess) {
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                OnError(request.error);
            }
            else{
                OnSuccess(request.downloadHandler.text);
            }
        }
    }

    private static void GetTexture2D(string url, Action<string> OnError, Action<Texture2D> OnSuccess) {
        Init();
        webRequestMonoBehaviour.StartCoroutine(GetTextureCO(url, OnError, OnSuccess));
    }


    //Same as Promise in javascript: resolve, rejectt
    private static IEnumerator GetTextureCO(string url, Action<string> OnError, Action<Texture2D> OnSuccess)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url)) {
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                OnError(request.error);
            }
            else{
                DownloadHandlerTexture dowloadedTexture = request.downloadHandler as DownloadHandlerTexture;
                OnSuccess(dowloadedTexture.texture);
            }
        }
    }

    private static void GetTextureFromWebp(string url, Action<string> OnError, Action<string> OnSuccess) {
        // GetTextureFromWebpCO(url, OnError, OnSuccess);
    }

    public static void FetchCardData(string url){
        Init();
        webRequestMonoBehaviour.StartCoroutine(DowloadDeckCO(url, 
            (string error) => {
                Debug.Log(error);
                // debugText.SetText($"Error: {error}");
            },
            (string success) => {
                JObject jsonObj = JObject.Parse(success);

                JArray data = jsonObj["data"] as JArray;
               
                foreach(JObject dataObj in data) {
                    CreateCardData(dataObj);
                }
        }));   
    }

    private static void CreateCardData(JObject cardObject)
    {
        // Create a new instance of the CardData ScriptableObject
        CardSO newCard = ScriptableObject.CreateInstance<CardSO>();

        // Set properties from JSON data
        newCard.cardId = cardObject["id"].ToString();
        newCard.cardName = cardObject["name"].ToString();
        newCard.cardSetID = cardObject["set"]["id"].ToString();
        newCard.cardSetName = cardObject["set"]["name"].ToString();
        if(cardObject.TryGetValue("types", out JToken typesToken)) {
            string typeString = typesToken[0].ToString();
            newCard.cardType = ParseCardType(typeString);
        }
        newCard.cardSuperTypes = ParseSuperTypes(cardObject["supertype"].ToString()); 
        newCard.cardRarity = ParseRarity(cardObject["rarity"].ToString());
        UpdateCardSOPrice(newCard, cardObject["tcgplayer"] as JObject);
        newCard.smallImageUrl = cardObject["images"]["small"].ToString();
        newCard.largeImageUrl = cardObject["images"]["large"].ToString();
        // Save the newCard instance as an asset
        string assetPath = $"Assets/Resources/CardSO/{newCard.cardSetID}/{newCard.cardId}.asset"; // Adjust the path
        CardSO existingCard  = UnityEditor.AssetDatabase.LoadAssetAtPath<CardSO>(assetPath);
        
        // Check if the asset already exists
        if (existingCard != null)
        {
            // Update the existing asset with the new values
            UnityEditor.EditorUtility.CopySerialized(newCard, existingCard);
            Debug.Log("Asset updated: " + assetPath);
        }
        else
        {
            //If the first time dowload -> dowload the assets from url image
            DowloadCardTexture2D(newCard);
            // Ensure the parent directory exists
            string directoryPath = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create the new asset
            UnityEditor.AssetDatabase.CreateAsset(newCard, assetPath);
            Debug.Log("Asset created: " + assetPath);
        }
    }

  
    private static IEnumerator DowloadDeckCO(string url, Action<string> OnError, Action<string> OnSuccess) {
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                OnError(request.error);
            }
            else{
                OnSuccess(request.downloadHandler.text);
            }
        }
    }

    private static RARITY ParseRarity(string rarityString)
    {
        // Convert the string to lowercase to handle case-insensitivity
        rarityString = rarityString.ToLower();

        switch (rarityString)
        {
            case "amazing rare":
                return RARITY.AmazingRare;
            case "classic collection":
                return RARITY.ClassicCollection;
            case "common":
                return RARITY.Common;
            case "double rare":
                return RARITY.DoubleRare;
            case "hyper rare":
                return RARITY.HyperRare;
            case "illustration rare":
                return RARITY.IllustrationRare;
            case "legend":
                return RARITY.LEGEND;
            case "promo":
                return RARITY.Promo;
            case "radiant rare":
                return RARITY.RadiantRare;
            case "rare":
                return RARITY.Rare;
            case "rare ace":
                return RARITY.RareACE;
            case "rare break":
                return RARITY.RareBREAK;
            case "rare holo":
                return RARITY.RareHolo;
            case "rare holo ex":
                return RARITY.RareHoloEX;
            case "rare holo gx":
                return RARITY.RareHoloGX;
            case "rare holo lv.x":
                return RARITY.RareHoloLVX;
            case "rare holo star":
                return RARITY.RareHoloStar;
            case "rare holo v":
                return RARITY.RareHoloV;
            case "rare holo vmax":
                return RARITY.RareHoloVMAX;
            case "rare holo vstar":
                return RARITY.RareHoloVSTAR;
            case "rare prime":
                return RARITY.RarePrime;
            case "rare prism star":
                return RARITY.RarePrismStar;
            case "rare rainbow":
                return RARITY.RareRainbow;
            case "rare secret":
                return RARITY.RareSecret;
            case "rare shining":
                return RARITY.RareShining;
            case "rare shiny":
                return RARITY.RareShiny;
            case "rare shiny gx":
                return RARITY.RareShinyGX;
            case "rare ultra":
                return RARITY.RareUltra;
            case "special illustration rare":
                return RARITY.SpecialIllustrationRare;
            case "trainer gallery rare holo":
                return RARITY.TrainerGalleryRareHolo;
            case "ultra rare":
                return RARITY.UltraRare;
            case "uncommon":
                return RARITY.Uncommon;
            default:
                Debug.LogWarning("Unknown rarity: " + rarityString);
                return RARITY.Common; // Or a default value of your choice
        }
    }

    private static SUPERTYPE ParseSuperTypes(string superText)
    {
        // Convert the string to lowercase to handle case-insensitivity
        superText = superText.ToLower();

        switch (superText)
        {
            case "energy":
                return SUPERTYPE.Energy;
            case "pokémon":
                return SUPERTYPE.Pokemon;
            case "trainer":
                return SUPERTYPE.Trainer;
            default:
                Debug.LogWarning("Unknown supertype: " + superText);
                return SUPERTYPE.Pokemon; // Or a default value of your choice
        }
    }

    private static TYPE ParseCardType(string typeText)
    {
        typeText = typeText.ToLower();
        
        switch (typeText)
        {
            case "colorless":
                return TYPE.Colorless;
            case "darkness":
                return TYPE.Darkness;
            case "dragon":
                return TYPE.Dragon;
            case "fairy":
                return TYPE.Fairy;
            case "fighting":
                return TYPE.Fighting;
            case "fire":
                return TYPE.Fire;
            case "grass":
                return TYPE.Grass;
            case "lightning":
                return TYPE.Lightning;
            case "metal":
                return TYPE.Metal;
            case "psychic":
                return TYPE.Psychic;
            case "water":
                return TYPE.Water;
            default:
                Debug.LogWarning("Unknown type: " + typeText);
                return TYPE.Colorless; // Or a default value of your choice
        }
    }

    private static void UpdateCardSOPrice(CardSO card, JObject tcgPlayer){
        // Access the "prices" object within the "tcgplayer" object
        JObject pricesObject = tcgPlayer["prices"] as JObject;

        // Access the first price object within the "prices" object
        JProperty firstPrice = pricesObject.Properties().FirstOrDefault();

        if (firstPrice != null)
        {
            JObject firstPriceObject = firstPrice.Value as JObject;

            // Access the "low", "mid", and "high" values within the first price object
            float lowPrice = float.Parse(firstPriceObject["low"].ToString());
            float midPrice = float.Parse(firstPriceObject["mid"].ToString());
            float highPrice = float.Parse(firstPriceObject["high"].ToString());

            // Now you can use these price values
            card.lowPrice = lowPrice;
            card.midPrice = midPrice;
            card.highPrice = highPrice;
        }
    }

    public static void DowloadCardTexture2D(CardSO cardData) {
        if (cardData.smallImageUrl != null) {
            GetTexture2D(cardData.smallImageUrl, (string fail) => {
                Debug.LogWarning($"Failed to dowload texture with URL: {cardData.smallImageUrl}");
            },
            (Texture2D success) => {
                string textureAssetPath = $"Assets/Resources/Texture2D/{cardData.cardSetID}/small/{cardData.cardId}_small.png"; // Adjust the path
                // Create the parent directory if it doesn't exist
                string parentDirectory = Path.GetDirectoryName(textureAssetPath);
                if (!Directory.Exists(parentDirectory))
                {   
                    Directory.CreateDirectory(parentDirectory);
                }

                
                if(!File.Exists(textureAssetPath)) {
                    // Save the texture as an asset
                    byte[] textureBytes = success.EncodeToPNG();
                    System.IO.File.WriteAllBytes(textureAssetPath, textureBytes);

                    Debug.Log($"Saved small image as asset: {textureAssetPath}");
                    UnityEditor.AssetDatabase.Refresh();
                }
                else{
                    Debug.LogWarning("File existed, no enCode");
                }
            });
            }

        if(cardData.largeImageUrl != null) {
              GetTexture2D(cardData.largeImageUrl, (string fail) => {
                Debug.LogWarning($"Failed to dowload texture with URL: {cardData.largeImageUrl}");
            },
            (Texture2D success) => {
                string textureAssetPath = $"Assets/Resources/Texture2D/{cardData.cardSetID}/large/{cardData.cardId}_large.png"; // Adjust the path
                // Create the parent directory if it doesn't exist
                string parentDirectory = Path.GetDirectoryName(textureAssetPath);
                if (!Directory.Exists(parentDirectory))
                {
                    Directory.CreateDirectory(parentDirectory);
                }

                if(!File.Exists(textureAssetPath)) {
                    // Save the texture as an asset
                    byte[] textureBytes = success.EncodeToPNG();
                    System.IO.File.WriteAllBytes(textureAssetPath, textureBytes);
                    Debug.Log($"Saved large image as asset: {textureAssetPath}");
                    // Refresh the asset database to recognize the changes
                    UnityEditor.AssetDatabase.Refresh();
                }
                else{
                    Debug.LogWarning("File existed, no enCode");
                }
            
                });
            }

    }


    public static void AssignDownloadedImages(string setID)
    {
        CardSO[] allCards = Resources.LoadAll<CardSO>($"CardSO/{setID}/");
        foreach (CardSO card in allCards)
        {
            string smallTexturePath = $"Texture2D/{card.cardSetID}/small/{card.cardId}_small";
            string largeTexturePath = $"Texture2D/{card.cardSetID}/large/{card.cardId}_large";
            Texture2D smallTexture = Resources.Load<Texture2D>(smallTexturePath);
            Texture2D largeTexture = Resources.Load<Texture2D>(largeTexturePath);

            if (smallTexture != null)
            {
                card.smallImage = smallTexture;
                UnityEditor.EditorUtility.SetDirty(card); // Mark the ScriptableObject as dirty
                Debug.Log($"Assigned small texture to card {card.cardId}");
            }
            else
            {
                Debug.LogWarning($"Texture not found for card {card.cardId}");
            }

            if (largeTexture != null)
            {
                card.largeImage = largeTexture;
                UnityEditor.EditorUtility.SetDirty(card); // Mark the ScriptableObject as dirty
                Debug.Log($"Assigned large texture to card {card.cardId}");
            }
            else
            {
                Debug.LogWarning($"Texture not found for card {card.cardId}");
            }
        }

        // Save changes to ScriptableObjects
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
}
