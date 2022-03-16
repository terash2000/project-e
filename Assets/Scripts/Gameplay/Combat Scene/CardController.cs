using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Tile mTile;

    public Tile mOriginalTile;

    public Tilemap mArena;
    public GridLayout mGrid;
    public GameObject mCharacter;
    public int mRange;
    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    private void showRadius()
    {
        Vector3Int curPos = mGrid.WorldToCell(mCharacter.transform.position);
        int k=1;
        if(Mathf.Abs(curPos.y)%2==1)k=-1;
        for(int x=-mRange;x<=mRange;x++)
        {
            for(int y=-mRange;y<=mRange;y++)
            {
                int temp = x;
                if(Mathf.Abs(y)%2 == 1 && k*x>0) x+=k;
                if(Mathf.Abs(x)+Mathf.Abs(y)/2<=mRange){
                    x = temp;
                    mArena.SetTile(new Vector3Int(x+curPos.x,y+curPos.y,0),mTile);
                }
                x = temp;
            }
        }
    }

    private void unShowRadius()
    {
        Vector3Int curPos = mGrid.WorldToCell(mCharacter.transform.position);
        int k=1;
        if(Mathf.Abs(curPos.y)%2==1)k=-1;
        for(int x=-mRange;x<=mRange;x++)
        {
            for(int y=-mRange;y<=mRange;y++)
            {
                int temp = x;
                if(Mathf.Abs(y)%2 == 1 && k*x>0) x+=k;
                if(Mathf.Abs(x)+Mathf.Abs(y)/2<=mRange){
                    x = temp;
                    mArena.SetTile(new Vector3Int(x+curPos.x,y+curPos.y,0),mOriginalTile);
                    Debug.Log("Yes");
                }
                x = temp;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0)&&selected){
            Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mousePos = mGrid.WorldToCell(new Vector3(oriPos.x,oriPos.y,0));
            Tile tile = (Tile)mArena.GetTile(mousePos);
            if(tile.Equals(mTile))
            {
                mCharacter.GetComponent<TopDownController>().setMovement(mGrid.CellToWorld(mousePos));
                selected = false;
                this.GetComponent<Image>().color = Color.white;
                unShowRadius();
            }
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = Color.yellow;
        showRadius();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(selected) return;
        this.GetComponent<Image>().color = Color.white;
        unShowRadius();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = !selected;
    }
}
