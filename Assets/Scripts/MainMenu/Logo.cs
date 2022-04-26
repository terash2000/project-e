using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    public TextAsset ElementData;
    private List<string> _symbols;
    private List<string> _names;
    private List<Color> _colors;
    private List<string> _groups;
    private int _curElement;
    // Start is called before the first frame update
    void Start()
    {
        ImportCSV();
        _curElement = 0;
    }

    public void ImportCSV()
    {
        string[] elements = ElementData.text.Split(new string[] { "\n" }, System.StringSplitOptions.None);
        _symbols = new List<string>();
        _names = new List<string>();
        _colors = new List<Color>();
        _groups = new List<string>();
        for (int i = 1; i < elements.Length - 1; i++)
        {
            string[] atts = elements[i].Split(new string[] { "," }, System.StringSplitOptions.None);
            _symbols.Add(atts[1].Trim('\"'));
            _names.Add(atts[2].Trim('\"'));
            _groups.Add(atts[atts.Length - 2].Trim('\"'));
            switch (atts[atts.Length - 2].Trim('\"'))
            {
                case "Nonmetal":
                    _colors.Add(new Color(255 / 255f, 255 / 255f, 191 / 255f));
                    break;
                case "Noble gas":
                    _colors.Add(new Color(255 / 255f, 229 / 255f, 191 / 255f));
                    break;
                case "Alkali metal":
                    _colors.Add(new Color(255 / 255f, 198 / 255f, 198 / 255f));
                    break;
                case "Alkaline earth metal":
                    _colors.Add(new Color(212 / 255f, 212 / 255f, 255 / 255f));
                    break;
                case "Metalloid":
                    _colors.Add(new Color(226 / 255f, 239 / 255f, 191 / 255f));
                    break;
                case "Post-transition tetal":
                    _colors.Add(new Color(191 / 255f, 255 / 255f, 191 / 255f));
                    break;
                case "Transition metal":
                    _colors.Add(new Color(191 / 255f, 223 / 255f, 255 / 255f));
                    break;
                case "Lanthanide":
                    _colors.Add(new Color(179 / 255f, 255 / 255f, 255 / 255f));
                    break;
                case "Actinide":
                    _colors.Add(new Color(198 / 255f, 255 / 255f, 236 / 255f));
                    break;
                default:
                    _colors.Add(new Color(255 / 255f, 255 / 255f, 191 / 255f));
                    break;
            }
        }
    }

    public void NextElement()
    {
        if (_curElement >= 118)
        {
            GameObject.Find("Elements").gameObject.SetActive(false);
            GameObject.Find("E").GetComponent<Button>().enabled = true;
            _curElement = 0;
            return;
        }
        Debug.Log(_groups[_curElement]);
        GameObject.Find("Symbol").GetComponent<TextMeshProUGUI>().text = _symbols[_curElement];
        GameObject.Find("Name").GetComponent<TextMeshProUGUI>().text = _names[_curElement];
        GameObject.Find("AtomicNumber").GetComponent<TextMeshProUGUI>().text = (_curElement + 1).ToString();
        GameObject.Find("Elements").GetComponent<Image>().color = _colors[_curElement];
        _curElement++;
    }


}
