using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    public static List<Wall> Walls = new List<Wall>();
    public static List<IPiece> Pieces = new List<IPiece>();

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (var behaviour in allBehaviours)
        {
            if (behaviour is IPiece piece)
            {
                if (piece is Wall wall)
                {
                    Walls.Add(wall);
                } else
                {
                    Pieces.Add(piece);
                }
            }
        }

        Debug.Log($"Found {Pieces.Count} pieces.");
        Debug.Log($"Found {Walls.Count} walls.");
    }

    public static List<Vector2Int> GetEnemies(int pieceTeam)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        foreach (var piece in Pieces)
        {
            if (piece.PieceTeam != pieceTeam)
            {
                positions.Add(piece.position);
            }
        }

        return positions;
    }

    public static List<Vector2Int> GetAllies(int pieceTeam)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        foreach (var piece in Pieces)
        {
            if (piece.PieceTeam == pieceTeam)
            {
                positions.Add(piece.position);
            }
        }

        return positions;
    }

    public static List<Vector2Int> GetWalls()
    {
        List<Vector2Int> walls = new List<Vector2Int>();

        foreach (var wall in Walls)
        {
            walls.Add(wall.position);
        }

        return walls;
    }
}
