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
    // Start is called before the first frame update
    void Start()
    {
        arena = mArena.GetComponent<Arena>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && selectThisCard)
        {
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = arena.grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            Tile tile = (Tile)arena.tilemap.GetTile(mousePos);
            if (tile != null && tile.Equals(arena.mTile))
            {
                selected = false;
                selectThisCard = false;
                this.GetComponent<Image>().color = Color.white;
                arena.hideRadius(mAreaShape, mRange);
                PlayerManager.singleton.Player.SetMovement(mousePos);
            }
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
}
