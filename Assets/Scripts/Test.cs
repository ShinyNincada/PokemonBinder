using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private MeshRenderer targetMesh;
    // Start is called before the first frame update
    void Start()
    {
        
        WebRequest.AssignDownloadedImages("sm115");
    }

    
  
}
