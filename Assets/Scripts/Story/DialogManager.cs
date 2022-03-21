using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager singleton;

    public string nextScene;
    public DialogNode root;
    private DialogNode current;
    [SerializeField] private QuoteText quoteObj;
    [SerializeField] private GameObject characterName;
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

        current = root;
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
            SceneChanger.previousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(nextScene);
        }
    }

    private void ChangeNode(DialogNode node)
    {
        current = node;
        UpdateText();
    }

    private void UpdateText()
    {
        if (current.character != null)
        {
            characterNameText.SetText(current.character.name);
            characterName.SetActive(true);
        }
        else characterName.SetActive(false);

        quoteObj.dialog = current.quote;
        quoteObj.StartTyping();

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