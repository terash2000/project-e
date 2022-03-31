using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int mRange;
    public AreaShape mAreaShape;
    public AreaShape mTargetShape;
    public static bool selected = false;
    protected bool selectThisCard = false;
    private bool mouseOver = false;
    protected int damage; //temp
    private int manaCost = 1; //temp
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && selectThisCard && Usable() && !mouseOver)
        {
            this.GetComponent<Image>().color = Color.white;
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = Arena.Instance.grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            if (OnUse(mousePos)) PlayerData.Mana -= manaCost;
            selected = false;
            selectThisCard = false;
            Arena.Instance.SelectedCard = null;
            Arena.Instance.HideRadius(mAreaShape, mRange);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        if (selected) return;
        this.GetComponent<Image>().color = Color.yellow;
        Arena.Instance.SelectedCard = this;
        Arena.Instance.ShowRadius(mAreaShape, mTargetShape, mRange);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        if (selected) return;
        this.GetComponent<Image>().color = Color.white;
        Arena.Instance.SelectedCard = null;
        Arena.Instance.HideRadius(mAreaShape, mRange);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectThisCard)
        {
            selected = false;
            selectThisCard = false;
            Arena.Instance.SelectedCard = null;
        }
        else if (!selected)
        {
            selected = true;
            selectThisCard = true;
            Arena.Instance.SelectedCard = this;

            this.GetComponent<Image>().color = Color.yellow;
            Arena.Instance.ShowRadius(mAreaShape, mTargetShape, mRange);
        }
    }

    public virtual bool OnUse(Vector3Int mousePos)
    {
        Tile tile = (Tile)Arena.Instance.tilemap.GetTile(mousePos);
        if (tile != null && Arena.Instance.AreaPosList.Contains(mousePos) && MonsterManager.Instance.FindMonsterByTile(mousePos) == null)
        {
            Arena.Instance.HideRadius(mAreaShape, mRange);
            PlayerManager.Instance.Player.SetMovement(mousePos);
            return true;
        }
        return false;
    }

    public bool Usable()
    {
        return manaCost <= PlayerData.Mana;
    }

    public int GetDamage()
    {
        return damage;
    }
}
