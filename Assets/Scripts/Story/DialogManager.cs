using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviourSingleton<DialogManager>
{
    private static Stack<DialogNode> _nextRoot = new Stack<DialogNode>();

    [SerializeField] private QuoteText _quoteObj;
    [SerializeField] private GameObject _characterName;
    [SerializeField] private Image _background;
    [SerializeField] private Image _sprite;
    [SerializeField] private VerticalLayoutGroup _choiceContainer;
    [SerializeField] private GameObject _choicePrefab;
    [SerializeField] private Transform _canvas;
    private TextMeshProUGUI _characterNameText;
    private DialogNode _current;
    private bool _isPause;
    private bool _choiceClicked;

    public static Stack<DialogNode> NextRoot
    {
        get { return _nextRoot; }
        set { _nextRoot = value; }
    }

    void Start()
    {
        _characterNameText = _characterName.transform.Find("Character Text").gameObject.GetComponent<TextMeshProUGUI>();

        _current = NextRoot.Peek();
        NextRoot.Pop();
        Render();
    }

    void Update()
    {
        if ((Input.GetMouseButtonUp(0) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Z)) &&
            !_isPause &&
            !_choiceClicked &&
            Time.timeScale != 0)
        {
            Next();
        }

        _isPause = Time.timeScale == 0;
        _choiceClicked = false;
    }

    public void ShowPopup(string header, string cardname)
    {
        GameObject newPopup = Instantiate(CardCollection.Instance.NewCardPopup, _canvas);
        newPopup.GetComponent<NewCardPopup>().Init(header, cardname);
    }

    private void Next()
    {
        if (!_quoteObj.IsTyping())
        {
            if (_current.NextWave != null)
            {
                if (_current.Child != null && _current.Child.Count != 0)
                {
                    SceneChanger.Instance.NextSceneStack.Push("StoryScene");
                    DialogManager.NextRoot.Push(_current.Child[0]);
                }
                MonsterManager.Wave = _current.NextWave;
                SceneChanger.Instance.LoadScene("CombatScene");

            }
            else if (_current.Child != null && _current.Child.Count != 0)
            {
                switch (_current.Type)
                {
                    case NodeType.Basic:
                        ChangeNode(_current.Child[0]);
                        break;
                    case NodeType.Choice:
                        break;
                }
            }
            else
            {
                SceneChanger.Instance.NextScene();
            }
        }
        else
        {
            _quoteObj.SkipTyping();
        }
    }

    private void ChangeNode(DialogNode node)
    {
        _current = node;
        Render();
    }

    private void Render()
    {
        // action
        _current.Action.Trigger();

        // dialog
        if (_current.Quote != "")
        {
            _quoteObj.Dialog = _current.Quote;
            _quoteObj.StartTyping();

            // character name
            if (_current.Character != null)
            {
                _characterNameText.SetText(_current.Character.CharacterName);
                _characterName.SetActive(true);
            }
            else _characterName.SetActive(false);
        }

        // image
        if (_current.Background != null) _background.sprite = _current.Background;
        if (_current.Sprite != null)
        {
            _sprite.sprite = _current.Sprite;
            _sprite.gameObject.SetActive(true);
        }
        else _sprite.gameObject.SetActive(false);

        // choice
        for (int i = 0; i < _choiceContainer.transform.childCount; i++)
        {
            Destroy(_choiceContainer.transform.GetChild(i).gameObject);
        }

        if (_current.Type == NodeType.Choice)
        {
            for (int i = 0; i < _current.Choice.Count; i++)
            {
                GameObject choiceButton = Instantiate(_choicePrefab, _choiceContainer.transform);
                choiceButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = _current.Choice[i];
                DialogNode nextNode = _current.Child[i];
                UnityAction listener1 = () => ChangeNode(nextNode);
                UnityAction listener2 = () =>
                {
                    _choiceClicked = true;
                };
                choiceButton.GetComponent<Button>().onClick.AddListener(listener1);
                choiceButton.GetComponent<Button>().onClick.AddListener(listener2);
            }
        }
    }
}