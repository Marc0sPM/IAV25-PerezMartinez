using UnityEngine;
using System.IO;

public class MapLoader : MonoBehaviour
{
    public GameObject wallPrefab1;
    public GameObject wallPrefab2;
    public GameObject wallPrefab3;

    public GameObject intersection3Prefab;
    public GameObject intersection4Prefab;
    public GameObject turnPrefab;

    public GameObject endPrefab;
    public GameObject pillarPrefab;

    public GameObject obstaclePrefab;
    public GameObject floorPrefab;

    private int[,] map;

    public void LoadMap(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        int height = lines.Length;
        int width = lines[0].Length;
        map = new int[width, height];

        for (int j = 0; j < height; j++)
        {
            string line = lines[j];
            for (int i = 0; i < width; i++)
            {
                char c = line[i];

                map[i, j] = (c == '#') ? 1 : 0;

                // Instanciar suelo
                Instantiate(floorPrefab, new Vector3(i, 0, j), Quaternion.identity, transform);

                if (map[i, j] == 1)
                    InstantiateWall(i, j);
            }
        }
    }

    void InstantiateWall(int i, int j)
    {
        int top = GetMap(i, j + 1);
        int bottom = GetMap(i, j - 1);
        int left = GetMap(i - 1, j);
        int right = GetMap(i + 1, j);

        int sum = top + bottom + left + right;
        GameObject prefab = null;
        Quaternion rot = Quaternion.identity;

        switch (sum)
        {
            case 0:
                prefab = pillarPrefab;
                break;

            case 1:
                prefab = endPrefab;
                if (top == 1) rot = Quaternion.Euler(0, 180, 0);
                else if (right == 1) rot = Quaternion.Euler(0, 270, 0);
                else if (bottom == 1) rot = Quaternion.Euler(0, 0, 0);
                else if (left == 1) rot = Quaternion.Euler(0, 90, 0);
                break;

            case 2:
                if ((top == 1 && bottom == 1) || (left == 1 && right == 1))
                {
                    prefab = wallPrefab2;
                    if (left == 1 && right == 1)
                        rot = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    prefab = turnPrefab;
                    if (top == 1 && right == 1) rot = Quaternion.Euler(0, 0, 0);
                    else if (right == 1 && bottom == 1) rot = Quaternion.Euler(0, 90, 0);
                    else if (bottom == 1 && left == 1) rot = Quaternion.Euler(0, 180, 0);
                    else if (left == 1 && top == 1) rot = Quaternion.Euler(0, 270, 0);
                }
                break;

            case 3:
                prefab = intersection3Prefab;
                if (top == 0) rot = Quaternion.Euler(0, 180, 0);
                else if (right == 0) rot = Quaternion.Euler(0, 270, 0);
                else if (bottom == 0) rot = Quaternion.Euler(0, 0, 0);
                else if (left == 0) rot = Quaternion.Euler(0, 90, 0);
                break;

            case 4:
                prefab = intersection4Prefab;
                break;
        }

        if (prefab != null)
            Instantiate(prefab, new Vector3(i, 0, j), rot, transform);
    }

    int GetMap(int i, int j)
    {
        if (i < 0 || j < 0 || i >= map.GetLength(0) || j >= map.GetLength(1))
            return 0;
        return map[i, j];
    }
}
