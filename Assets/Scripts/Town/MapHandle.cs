using UnityEngine;

public class MapHandle : MonoBehaviour
{
    public void ToMap()
    {
        SceneChanger.Instance.LoadScene("MapScene");
    }
}
