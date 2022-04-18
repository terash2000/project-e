using UnityEngine;
using UnityEngine.UI;

public class GoldChecker : MonoBehaviour
{
    public int GoldNeeded;
    void Update()
    {
        GetComponent<Button>().interactable = PlayerData.Gold >= GoldNeeded;
    }
}
