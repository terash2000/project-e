using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager singleton;

    public static List<DialogNode> nextRoot = new List<DialogNode>();
    private DialogNode current;
    [SerializeField] private QuoteText quoteObj;
    [SerializeField] private GameObject characterName;
    [SerializeField] private Image background;
    [SerializeField] private Image sprite;
    [SerializeField] private VerticalLayoutGroup choiceContainer;
    [SerializeField] private GameObject choicePrefab;
    private TextMeshProUGUI characterNameText;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        characterNameText = characterName.transform.Find("Character Text").gameObject.GetComponent<TextMeshProUGUI>();

        current = nextRoot[0];
        nextRoot.RemoveAt(0);
        UpdateText();
    }

    void Update()
    {
        if ((Input.GetMouseButtonUp(0) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Z)) &&
            !quoteObj.IsTyping()) Next();
    }

    private void Next()
    {
        if (current.child != null && current.child.Count != 0)
        {
            switch (current.type)
            {
                case NodeType.Basic:
                    ChangeNode(current.child[0]);
                    break;
                case NodeType.Choice:
                    break;
            }
        }
        else
        {
            SceneChanger.NextScene();
        }
    }

    private void ChangeNode(DialogNode node)
    {
        current = node;
        UpdateText();
    }

    private void UpdateText()
    {
        quoteObj.dialog = current.quote;
        quoteObj.StartTyping();

        // character name
        if (current.character != null)
        {
            characterNameText.SetText(current.character.name);
            characterName.SetActive(true);
        }
        else characterName.SetActive(false);

        // image
        if (current.background != null) background.sprite = current.background;
        if (current.sprite != null)
        {
            sprite.sprite = current.sprite;
            sprite.gameObject.SetActive(true);
        }
        else sprite.gameObject.SetActive(false);

        // choice
        for (int i = 0; i < choiceContainer.transform.childCount; i++)
        {
            Destroy(choiceContainer.transform.GetChild(i).gameObject);
        }

        if (current.type == NodeType.Choice)
        {
            for (int i = 0; i < current.choice.Count; i++)
            {
                GameObject choiceButton = Instantiate(choicePrefab, choiceContainer.transform);
                choiceButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = current.choice[i];
                DialogNode nextNode = current.child[i];
                UnityAction listener = () => ChangeNode(nextNode);
                choiceButton.GetComponent<Button>().onClick.AddListener(listener);
            }
        }
    }
}