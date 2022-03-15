using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.Find("Gold Quantity").gameObject.GetComponent<Text>().text 
            = PlayerData.gold.ToString();
    }
}
