using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Arena mArena;
    //public PieceManager mPieceManager;

    void Start()
    {
        // Create the board
        mArena.Create();

        // Create pieces
        //mPieceManager.Setup(mBoard);
    }
}