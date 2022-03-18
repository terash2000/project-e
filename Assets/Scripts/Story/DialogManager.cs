using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    public static DialogManager singleton;

    public string nextScene;
    public DialogNode root;
    private DialogNode current;
    [SerializeField] private QuoteText quoteObj;
    [SerializeField] private GameObject characterName;
    private TextMeshProUGUI characterNameText;

    void Awake(){
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
        if (Input.GetMouseButtonUp(0) ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.Z) && 
                !quoteObj.IsTyping()){
            if (current.child)
            {
                current = current.child;
                UpdateText();
            }
            else
            {
                SceneChanger.previousScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(nextScene);
            }
        }
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
    }
}