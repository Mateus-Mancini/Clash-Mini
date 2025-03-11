using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IPiece
{
    public int PieceType { get; } = 1;
    [field: SerializeField] public int PieceTeam { get; set; } = 1;
    [field: SerializeField] public float PieceHealth { get; set; } = 30f;
    [field: SerializeField] public Vector2Int position { get; set; }

    void Awake()
    {
        GameObject element = GameObject.Find(position.ToString());
        if (element != null)
        {
            Vector3 worldPosition = element.transform.position;
            float currentY = transform.position.y;
            transform.position = new Vector3(worldPosition.x, currentY, worldPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
