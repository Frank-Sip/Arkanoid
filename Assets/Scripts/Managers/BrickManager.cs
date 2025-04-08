using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 5;
    [SerializeField] private float spacing = 0.3f;
    [SerializeField] private Vector2 startPoint = new Vector2(-7f, 4f);
    
    private float width;
    private float height;
    
    private static List<BrickController> bricks = new List<BrickController>();

    private void Awake()
    {
        BrickSO config = brickPrefab.GetComponent<BrickController>().brickConfig;
        width = config.width;
        height = config.height;
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = new Vector3(
                    startPoint.x + col * (width + spacing),
                    startPoint.y - row * (height + spacing),
                    0f
                );

                GameObject go = Instantiate(brickPrefab, pos, Quaternion.identity);
                BrickController controller = go.GetComponent<BrickController>();
                controller.UpdateBounds();
            }
        }
    }

    public static void Register(BrickController brick)
    {
        if (!bricks.Contains(brick))
        {
            bricks.Add(brick);
        }
    }

    public static void Unregister(BrickController brick)
    {
        bricks.Remove(brick);

        if (bricks.Count <= 0)
        {
            Debug.Log("You win!");
        }
    }

    public static List<BrickController> GetBricks()
    {
        return bricks;
    }
}