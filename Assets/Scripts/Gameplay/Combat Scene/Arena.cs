using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Arena : MonoBehaviour
{
    public static Arena singleton;

    public Tile mTile;
    public Tile mOriginalTile;
    [HideInInspector]
    public GridLayout grid;
    [HideInInspector]
    public Tilemap tilemap;
    [HideInInspector]
    public GameObject hexBorder;
    public GameObject mCharacter;

    void Awake(){
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponentInChildren<GridLayout>();
        tilemap = grid.GetComponentInChildren<Tilemap>();
        hexBorder = transform.Find("hexBorder").gameObject;
        BakeLineDebuger(hexBorder);
    }

    public static void BakeLineDebuger(GameObject lineObj)
    {
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        var meshFilter = lineObj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshFilter.sharedMesh = mesh;

        var meshRenderer = lineObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = lineRenderer.sharedMaterial;
        meshRenderer.sortingOrder = lineRenderer.sortingOrder;

        GameObject.Destroy(lineRenderer);
    }
    void OnMouseOver()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("1");
        Vector3 oriPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePos = grid.WorldToCell(new Vector3(oriPos.x,oriPos.y,0));
        Tile tile = (Tile)tilemap.GetTile(mousePos);
        if(tile.Equals(mTile))
        {
            hexBorder.GetComponent<MeshRenderer>().gameObject.SetActive(true);
            hexBorder.transform.position = grid.CellToWorld(mousePos);

        }
        else hexBorder.GetComponent<MeshRenderer>().gameObject.SetActive(false);
    }

    public void showRadius(AreaShape areaShape, int range)
    {
        //Vector3Int curPos = grid.WorldToCell(mCharacter.transform.position);
        Vector3Int curPos = mCharacter.GetComponent<MoveableSprite>().currentTile;
        List<Vector3Int> posList = getPosList(areaShape,range,curPos);
        foreach(Vector3Int pos in posList)
        {
            Debug.Log(pos.x+" "+pos.y);
        }
        //Debug.Log(posList.ToString());
        setTile(mTile, posList);
    }

    public void hideRadius(AreaShape areaShape, int range)
    {
        //Vector3Int curPos = grid.WorldToCell(mCharacter.transform.position);
        Vector3Int curPos = mCharacter.GetComponent<MoveableSprite>().currentTile;
        setTile(mOriginalTile, getPosList(areaShape,range,curPos));
    }

    public void setTile(Tile tile, List<Vector3Int> posList)
    {
        foreach(Vector3Int pos in posList)
        {
            Color color = tilemap.GetColor(new Vector3Int(pos.x,pos.y,0));
            tilemap.SetTile(new Vector3Int(pos.x,pos.y,0),tile);
            tilemap.SetTileFlags(new Vector3Int(pos.x,pos.y,0), TileFlags.None);
            tilemap.SetColor(new Vector3Int(pos.x,pos.y,0), color);
        }
    }

    public void setTileColor(Color color, List<Vector3Int> posList)
    {
        foreach(Vector3Int pos in posList)
        {
            tilemap.SetTileFlags(new Vector3Int(pos.x,pos.y,0), TileFlags.None);
            tilemap.SetColor(new Vector3Int(pos.x,pos.y,0), color);
        }
    }

    public List<Vector3Int> getPosList(AreaShape areaShape, int range, Vector3Int curPos)
    {
        switch(areaShape)
        {
            case AreaShape.Hexagon:
                return getPosListCircle(range, curPos);
            case AreaShape.Line:
                return getPosListLine(range, curPos);
            default:
                return getPosListNear(curPos);
        }
    }

    private List<Vector3Int> getPosListCircle(int range, Vector3Int curPos)
    {
        List<Vector3Int> posList = new List<Vector3Int>();
        int k=1;
        if(Mathf.Abs(curPos.y)%2==1)k=-1;
        for(int y=-range;y<=range;y++)
        {
            for(int x=-range;x<=range;x++)
            {
                int temp = x;
                if(Mathf.Abs(y)%2 == 1 && k*x>0) x+=k;
                if(Mathf.Abs(x)+Mathf.Abs(y)/2<=range){
                    x = temp;
                    posList.Add(new Vector3Int(x+curPos.x,y+curPos.y,curPos.z));
                }
                x = temp;
            }
        }
        return posList;
    }

    public List<Vector3Int> getPosListNear(Vector3Int curPos)
    {
        List<Vector3Int> posList = getPosListCircle(1, curPos);
        posList.Remove(curPos);
        return posList;
    }

    private List<Vector3Int> getPosListLine(int range, Vector3Int curPos)
    {
        List<Vector3Int> posListNear = getPosListNear(curPos);
        List<Vector3Int> posList = new List<Vector3Int>();
        posList.Add(curPos);
        for(int i=0;i<posListNear.Count;i++)
        {
            Vector3Int pos = posListNear[i];
            posList.Add(pos);
            for(int j=0;j<range-1;j++)
            {
                pos = getPosListNear(pos)[i];
                posList.Add(pos);
            }
        }
        return posList;
    }

}
