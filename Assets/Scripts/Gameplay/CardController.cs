using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject mArena;
    [HideInInspector]
    public Arena arena;
    public int mRange;
    public AreaShape mAreaShape;
    public static bool selected = false;
    protected bool selectThisCard = false;
    private int manaCost = 1; //temp
    // Start is called before the first frame update
    void Start()
    {
        arena = mArena.GetComponent<Arena>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && selectThisCard && usable())
        {
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = arena.grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            if(onUse(mousePos)) PlayerData.mana -= manaCost;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selected) return;
        this.GetComponent<Image>().color = Color.yellow;
        arena.showRadius(mAreaShape, mRange);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selected) return;
        this.GetComponent<Image>().color = Color.white;
        arena.hideRadius(mAreaShape, mRange);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectThisCard)
        {
            selected = false;
            selectThisCard = false;
        }
        else if (!selected)
        {
            selected = true;
            selectThisCard = true;
        }
    }

    public virtual bool onUse(Vector3Int mousePos)
    {
        Tile tile = (Tile)arena.tilemap.GetTile(mousePos);
        if (tile != null && tile.Equals(arena.mTile) && MonsterManager.singleton.FindMonsterByTile(mousePos) == null)
        {
            selected = false;
            selectThisCard = false;
            this.GetComponent<Image>().color = Color.white;
            arena.hideRadius(mAreaShape, mRange);
            PlayerManager.singleton.Player.SetMovement(mousePos);
            return true;
        }
        return false;
    }

    public bool usable()
    {
        return manaCost <= PlayerData.mana;
    }
}
