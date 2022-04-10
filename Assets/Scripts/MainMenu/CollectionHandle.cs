using UnityEngine;

public class CollectionHandle : MonoBehaviour
{
    public void ToCardCollection()
    {
        SceneChanger.Instance.LoadScene("CollectionScene");
    }
}
