using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPiece
{
    int PieceType { get; }
    int PieceTeam { get; set; }
    float PieceHealth { get; set; }
    Vector2Int position { get; set; }
}
