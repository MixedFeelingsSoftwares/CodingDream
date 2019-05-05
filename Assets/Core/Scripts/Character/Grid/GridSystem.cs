using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPathing
{
}

public class GridSystem : MonoBehaviour
{
    #region Public Fields

    public GameObject GridParent;

    public bool isVisible = false;

    [Range(0.0f, 1.0f)]
    public float Opacity = 0.5f;

    [Header("Total lines Horizontal Lines"), Space(10)]
    public int TotalHorizontal = 20;

    [Header("Total lines Vertical Lines"), Space(10)]
    public int TotalVertical = 20;

    #endregion Public Fields

    #region Public Methods

    private Transform[] transforms;

    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        GridParent = new GameObject("GridParent");
        {
            for (int x = 0; x < TotalHorizontal; x++)
            {
                for (int y = 0; y < TotalVertical; y++)
                {
                    float xPos = MathUtilities.PosByCoordinate(MathUtilities.Direction.X, true).x;
                    float yPos = MathUtilities.PosByCoordinate(MathUtilities.Direction.Y, true).y;

                    // Width of X
                    float xWidth = MathUtilities.PosByCoordinate(MathUtilities.Direction.X, false).x;

                    // Width of Y
                    float yHeight = MathUtilities.PosByCoordinate(MathUtilities.Direction.Y, false).y;

                    float xP = xWidth / TotalHorizontal;
                    float yP = yHeight / TotalVertical;

                    GameObject obj = new GameObject("Grideroni");

                    obj.transform.SetParent(GridParent.transform);
                    obj.transform.localScale = new Vector3(1, 1, 1);

                    obj.transform.position = new Vector3(xP*x, yP*y, 0);
                }
            }
        }
    }

    #endregion Public Methods
}

public class MathUtilities
{
    #region Public Enums

    public enum Direction
    {
        X = 0,

        Y = 1,

        Z = 2
    }

    #endregion Public Enums

    #region Private Methods

    public static Vector3 PosByCoordinate(Direction dir, bool start)
    {
        Vector3 pos = Vector3.zero;

        switch (dir)
        {
            case Direction.X:
                if (start)
                {
                    pos = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, 0, 0);
                }
                else
                {
                    pos = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x, 0, 0);
                }
                break;

            case Direction.Y:
                if (start)
                {
                    pos = new Vector3(0, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y, 0);
                }
                else
                {
                    pos = new Vector3(0, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y, 0);
                }
                break;

            case Direction.Z:
                if (start)
                {
                    pos = new Vector3(0, 0, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).z);
                }
                else
                {
                    pos = new Vector3(0, 0, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Screen.width)).z);
                }
                break;
        }

        return pos;
    }

    #endregion Private Methods
}