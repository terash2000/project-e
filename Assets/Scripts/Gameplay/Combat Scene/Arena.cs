using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Arena : MonoBehaviour
{
    public Tile mTile;

    public Tile mOriginalTile;
    [HideInInspector]
    public GridLayout grid;
    [HideInInspector]
    public Tilemap tilemap;
    [HideInInspector]
    public GameObject hexBorder;
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
}
