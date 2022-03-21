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
    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        arena = mArena.GetComponent<Arena>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && selected)
        {
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = arena.grid.WorldToCell(new Vector3(oriPos.x, oriPos.y, 0));
            Tile tile = (Tile)arena.tilemap.GetTile(mousePos);
            if (tile != null && tile.Equals(arena.mTile))
            {
                selected = false;
                this.GetComponent<Image>().color = Color.white;
                arena.hideRadius(mAreaShape, mRange);
                arena.mCharacter.GetComponent<MoveableSprite>().SetMovement(mousePos);
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
        selected = !selected;
    }
}
