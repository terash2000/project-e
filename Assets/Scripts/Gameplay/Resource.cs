using TMPro;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private TextMeshProUGUI _healthText;
    private TextMeshProUGUI _goldText;

    void Start()
    {
        _healthText = transform.Find("Health").gameObject.GetComponent<TextMeshProUGUI>();
        _goldText = transform.Find("Gold Quantity").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        _healthText.text = PlayerData.Health.ToString() + '/' + PlayerData.MaxHealth.ToString();
        _goldText.text = PlayerData.Gold.ToString();
    }
}
