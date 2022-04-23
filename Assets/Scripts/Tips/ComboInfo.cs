using TMPro;
using UnityEngine;

public class ComboInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _equation;
    private Color _red = new Color(0.5f, 0f, 0f);
    private Color _purple = new Color(0.5f, 0f, 0.5f);

    public void Init(Combo combo)
    {
        _equation.text = combo.Equation;
        switch (combo.Type)
        {
            case Combo.ReactionType.Combustion:
                _equation.color = _red;
                break;
            case Combo.ReactionType.Nuclear:
                _equation.color = _purple;
                break;
        }
    }
}
