using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
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

    #region Private Methods

    public enum LineDirection
    {
        X = 0,

        Y = 1,
    }

    private StartEnd ConvertDynamicX(int current, int max)
    {
        var stEnd = new StartEnd()
        {
            Start = Camera.main.ScreenToWorldPoint(
             new Vector3(Screen.width, 0, 0)).x / max * current * -1,
            End = Camera.main.ScreenToWorldPoint(
             new Vector3(Screen.width, 0, 0)).x / max * current
        };
        return stEnd;
    }

    private StartEnd ConvertStartEndX()
    {
        var stEnd = new StartEnd()
        {
            Start = Camera.main.ScreenToWorldPoint(
             new Vector3(Screen.width, 0, 0)).x * -1,

            End = Camera.main.ScreenToWorldPoint(
             new Vector3(Screen.width, 0, 0)).x
        };
        return stEnd;
    }

    private StartEnd ConvertStartEndY()
    {
        var stEnd = new StartEnd()
        {
            Start = Camera.main.ScreenToWorldPoint(
             new Vector3(0, 0, Screen.height)).y * -1,

            End = Camera.main.ScreenToWorldPoint(
             new Vector3(0, 0, Screen.height)).y
        };
        return stEnd;
    }

    private Vector3 ConvertToGridX(bool Start, int current, int max)
    {
        return Start ?

            new Vector3(
             ConvertDynamicX(current, max).End,
             ConvertStartEndY().Start,
             0)
             :
             new Vector3(
             ConvertDynamicX(current, max).End,
             ConvertStartEndY().End,
             0);
    }

    private void createLine(string Name, Vector3 Start, Vector3 End)
    {
        if (GridParent == null) { return; }

        GameObject obj = new GameObject(Name);

        // Sets parent
        obj.transform.SetParent(GridParent.transform, true);

        // Creates lineRenderer
        LineRenderer line = obj.AddComponent<LineRenderer>();

        line.startWidth = 0.05f;
        line.positionCount = 2;

        line.SetPosition(0, Start);

        line.SetPosition(1, End);

        GridParent.name = $"Total Lines - {GridParent.transform.childCount}";
    }

    // Start is called before the first frame update
    private void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GenerateGrid()
    {
        GridParent = new GameObject("GridParent");
        {
            for (int x = -TotalHorizontal; x < TotalHorizontal; x++)
            {
                for (int y = -TotalVertical; y < TotalVertical; y++)
                {
                    Debug.Log($"Line_At_{x + 1}");

                    createLine($"Line_At_{x + 1}",
                        ConvertToGridX(true, x, TotalHorizontal),
                        ConvertToGridX(false, x, TotalHorizontal));
                }
            }
        }
    }

    public void GenerateLine(LineDirection dir, int current, int max)
    {
    }

    private class StartEnd
    {
        #region Public Properties

        public float End { get; set; }

        public float Start { get; set; }

        #endregion Public Properties
    }

    #endregion Private Methods
}