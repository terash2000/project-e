using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.Find("Gold Quantity").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text 
            = PlayerData.gold.ToString();
    }
}
