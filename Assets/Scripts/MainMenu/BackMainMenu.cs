using UnityEngine;

public class BackMainMenu : MonoBehaviour
{
    public void ReturnMainMenu()
    {
        SceneChanger.Instance.LoadScene("MainMenuScene");
    }
}
