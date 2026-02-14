using UnityEngine;
using TMPro;

public class UITDisplaysA : MonoBehaviour
{
    public TextMeshProUGUI nestText;

    void Update()
    {
        nestText.text = "Nest count: " + AntManager.Instance.nests.ToString();
    }
}
