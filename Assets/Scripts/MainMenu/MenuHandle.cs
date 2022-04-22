using UnityEngine;

public class MenuHandle : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _optionMenu;

    public void OpenMainMenu()
    {
        _mainMenu.SetActive(true);
        _optionMenu.SetActive(false);
    }

    public void OpenOptionMenu()
    {
        _optionMenu.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void ToCardCollection()
    {
        SceneChanger.Instance.LoadScene("CollectionScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
