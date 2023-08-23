using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonBottomNavUI : MonoBehaviour
{
    [SerializeField] private Button backwardButton;

    // Start is called before the first frame update
    void Start()
    {
        backwardButton.onClick.AddListener(() => {
            GameManager.Instance.ChangeStateBackward();
        });
    }

   
}
