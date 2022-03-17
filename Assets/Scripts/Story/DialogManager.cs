using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    public static DialogManager singleton;

    public string nextScene;
    public DialogNode root;
    private DialogNode current;
    [SerializeField] private QuoteText quoteObj;

    void Awake(){
        singleton = this;
    }

    void Start()
    {
        current = root;
        quoteObj.dialog = current.quote;
        quoteObj.StartTyping();
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
                quoteObj.dialog = current.quote;
                quoteObj.StartTyping();
            }
            else
            {
                SceneChanger.previousScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}