using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Image artworkImage;
    [SerializeField] private TextMeshProUGUI manaText;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = card.cardName;
        descriptionText.text = card.description;
        typeText.text = card.type.ToString();
        artworkImage.sprite = card.artwork;
        manaText.text = card.manaCost.ToString();
    }
}
