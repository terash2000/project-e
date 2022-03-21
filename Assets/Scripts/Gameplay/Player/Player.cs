using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MoveableSprite
{
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    public List<Vector3Int> AttackArea()
    {
        List<Vector3Int> area = new List<Vector3Int>();
        area.AddRange(Arena.singleton.getPosListNear(currentTile));
        return area;
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(name + " Game Object Left Clicked!");
        }
    }
}