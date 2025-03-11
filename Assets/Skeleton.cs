using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour, IPiece
{
    float _interval = 0.75f;

    public int PieceType { get; } = 1;
    [field: SerializeField] public int PieceTeam { get; set; } = 0;
    [field: SerializeField] public float PieceHealth { get; set; } = 12f;
    [field: SerializeField] public Vector2Int position { get; set; } = new Vector2Int(7, 0);

    float _time;

    void Awake()
    {
        _time = 0f;
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
        _time += Time.deltaTime;
        while (_time >= _interval)
        {
            string result = GetBestAction(position, BoardManager.GetEnemies(PieceTeam), BoardManager.GetWalls(), BoardManager.GetAllies(PieceTeam));
            Debug.Log(result);

            string[] resultParts = result.Split("Move to ");
            if (resultParts.Length > 1)
            {
                string newPosition = resultParts[1];
                GameObject element = GameObject.Find(newPosition);

                if (element != null)
                {
                    // Get the target world position
                    Vector3 targetPosition = element.transform.position;
                    targetPosition.y = transform.position.y; // keep current Y

                    // Start tweening to target position
                    StartCoroutine(TweenToPosition(targetPosition));

                    // Update piece position
                    string[] parts = newPosition.Replace("(", "").Replace(")", "").Split(',');
                    position = new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
                }
            }

            resultParts = result.Split("Attack enemy at");
            if (resultParts.Length > 1)
            {
                string[] parts = resultParts[1].Replace("(", "").Replace(")", "").Split(",");
                RotateObject(new Vector2Int(int.Parse(parts[0]).CompareTo(position.x), int.Parse(parts[1]).CompareTo(position.y)));
            }

            _time -= _interval;
        }
    }

    IEnumerator TweenToPosition(Vector3 targetPosition)
    {
        float timeElapsed = 0f;
        float moveDuration = _interval; // Time for the movement to take place (can be adjusted)

        Vector3 startingPosition = transform.position;

        while (timeElapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it finishes at the exact target position
        transform.position = targetPosition;
    }


    string GetBestAction(Vector2Int position, List<Vector2Int> enemyPositions, List<Vector2Int> wallPositions, List<Vector2Int> allyPositions)
    {
        int x = position.x, y = position.y;
        var enemies = new HashSet<Vector2Int>(enemyPositions);
        var walls = new HashSet<Vector2Int>(wallPositions);
        var allies = new HashSet<Vector2Int>(allyPositions);

        var attackDirections = new (int, int)[]
        {
            (1, 0), (-1, 0), (0, 1), (0, -1), // Horizontal & Vertical
            (1, 1), (-1, -1), (1, -1), (-1, 1) // Diagonal
        };
        var moveDirections = new (int, int)[]
        {
            (1, 0), (-1, 0), (0, 1), (0, -1), // Horizontal & Vertical
            (1, 1), (-1, -1), (1, -1), (-1, 1) // Diagonal
        };

        // Check attack possibilities
        foreach (var (dx, dy) in attackDirections)
        {
            int nx = x + dx, ny = y + dy;
            if (nx < 0 || nx >= 8 || ny < 0 || ny >= 8) continue;
            if (walls.Contains(new Vector2Int(nx, ny)) || allies.Contains(new Vector2Int(nx, ny))) {
                Debug.Log($"Ally at ({nx}, {ny})");
                continue; 
            } // Wall or Ally blocks attack
            if (enemies.Contains(new Vector2Int(nx, ny))) return $"Attack enemy at ({nx}, {ny})";
        }

        // BFS for shortest path to an attack position
        var queue = new Queue<(Vector2Int position, List<Vector2Int> path)>();
        var visited = new HashSet<Vector2Int> { position };
        queue.Enqueue((position, new List<Vector2Int> { position }));

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();
            int cx = current.x, cy = current.y;

            foreach (var (dx, dy) in attackDirections)
            {
                int nx = cx + dx, ny = cy + dy;
                if (nx < 0 || nx >= 8 || ny < 0 || ny >= 8) continue;
                if (walls.Contains(new Vector2Int(nx, ny)) || allies.Contains(new Vector2Int(nx, ny))) continue;
                if (enemies.Contains(new Vector2Int(nx, ny))) // Found an attack position
                {
                    RotateObject(new Vector2Int(path[1].x - path[0].x, path[1].y - path[0].y));
                    if (path.Count > 1) return $"Move to {path[1]}";
                    else return "No valid move";
                }
            }

            // Try moving to adjacent valid positions
            foreach (var (dx, dy) in moveDirections)
            {
                int nx = cx + dx, ny = cy + dy;
                if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && !walls.Contains(new Vector2Int(nx, ny)) && !enemies.Contains(new Vector2Int(nx, ny)) && !allies.Contains(new Vector2Int(nx, ny)))
                {
                    if (!visited.Contains(new Vector2Int(nx, ny)))
                    {
                        visited.Add(new Vector2Int(nx, ny));
                        var newPath = new List<Vector2Int>(path) { new Vector2Int(nx, ny) };
                        queue.Enqueue((new Vector2Int(nx, ny), newPath));
                    }
                }
            }
        }

        return "No valid move";
    }

    IEnumerator TweenToRotation(Quaternion targetRotation, float duration)
    {
        float timeElapsed = 0f;
        Quaternion startingRotation = transform.rotation;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it finishes at the exact target rotation
        transform.rotation = targetRotation;
    }

    public void RotateObject(Vector2Int direction)
    {
        float angle = Mathf.Atan2(direction.y, -direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        StartCoroutine(TweenToRotation(targetRotation, _interval / 4));
    }
}
