using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class RenameAndOrganizeCubes : MonoBehaviour
{
    public Transform parent; // Assign the parent containing all cubes in the Inspector

    void Start()
    {
        if (parent == null)
        {
            Debug.LogError("Parent not assigned!");
            return;
        }

        RenameCubes();
        OrganizeHierarchy();
    }

    void RenameCubes()
    {
        Transform[] cubes = parent.GetComponentsInChildren<Transform>();
        List<Transform> cubeList = new List<Transform>();

        foreach (Transform cube in cubes)
        {
            if (cube != parent)
                cubeList.Add(cube);
        }

        int gridWidth = 8;  // Number of cubes per row
        int gridHeight = 8; // Total rows (adjust if needed)

        foreach (Transform cube in cubeList)
        {
            string name = cube.name;
            int number;
            if (int.TryParse(name.Replace("Cube.", ""), out number))
            {
                number -= 1;  // Fix offset issue

                int x = gridWidth - 1 - (number / gridHeight); // Flip X-Axis (Right to Left)
                int y = gridHeight - 1 - (number % gridWidth); // Flip Y-Axis (Top to Bottom)

                cube.name = $"({x}, {y})";
            }
        }

        Debug.Log("Cubes renamed successfully!");
    }



void OrganizeHierarchy()
{
    Transform[] cubes = parent.GetComponentsInChildren<Transform>();
    List<Transform> cubeList = new List<Transform>();

    foreach (Transform cube in cubes)
    {
        if (cube != parent)
            cubeList.Add(cube);
    }

    // Sort by (y, x) to get the correct bottom-to-top, left-to-right order
    cubeList = cubeList.OrderBy(cube =>
    {
        string[] parts = cube.name.Replace("(", "").Replace(")", "").Split(',');
        int x = int.Parse(parts[0].Trim());
        int y = int.Parse(parts[1].Trim());
        return (y * 100 + x); // Sorting primarily by y, then x
    }).ToList();

    // Reparent cubes in sorted order
    foreach (Transform cube in cubeList)
    {
        cube.SetSiblingIndex(cubeList.IndexOf(cube)); // Ensures proper order in hierarchy
    }

    Debug.Log("Cubes organized successfully in the hierarchy!");
}
}
