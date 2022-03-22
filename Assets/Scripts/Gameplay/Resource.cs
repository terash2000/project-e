using TMPro;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI goldText;

    void Start()
    {
        healthText = transform.Find("Health").gameObject.GetComponent<TextMeshProUGUI>();
        goldText = transform.Find("Gold Quantity").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        healthText.text = PlayerData.health.ToString() + '/' + PlayerData.maxHealth.ToString();
        goldText.text = PlayerData.gold.ToString();
    }
}
