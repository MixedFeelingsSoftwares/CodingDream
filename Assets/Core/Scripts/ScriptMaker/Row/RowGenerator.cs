using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RowGenerator : MonoBehaviour
{
    #region Private Properties

    public int RowHeight = 1;

    private List<RectTransform> Lines { get; } = new List<RectTransform>();

    #endregion Private Properties

    #region Public Properties

    public static RowGenerator Instance { get; private set; }

    public int RowLength { get; private set; }

    #endregion Public Properties

    #region Private Methods

    private void Awake()
    {
        Instance = this;
    }

    private void MakeLineNumbers(int totalLines)
    {
        // Finds Parent with the tag 'lineRow'
        GameObject rows = GameObject.FindGameObjectWithTag("lineNumber");

        if (rows != null)
        {
            for (int i = 0; i < totalLines; i++)
            {
                // Make new Gameobject
                Transform rowNum = new GameObject($"lineNumber [{i + 1}]", typeof(RectTransform)).transform;

                // Sets tagged transform (gameobject) 'lineRow' as Parent
                rowNum.SetParent(rows.transform, false);

                // Get Rect Transform from Line Object
                RectTransform rTransform = rowNum.GetComponent<RectTransform>();
                rTransform.pivot = new Vector2(0.5f, 1.0f);

                // Anchors
                rTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rTransform.anchorMin = new Vector2(0.5f, 1.0f);

                // Add Image to Row line
                TextMeshProUGUI rowNumberText = rowNum.gameObject.AddComponent<TextMeshProUGUI>();
                rowNumberText.text = $"{(i + 1)}";
                rowNumberText.raycastTarget = false;
                rowNumberText.alignment = TextAlignmentOptions.CenterGeoAligned;
                rowNumberText.fontSize = 15.0f;
                rowNumberText.color = new Color(0, 0, 0);

                // Offsets (Pos Y)
                float offsetY = -rowNumberText.fontSize * (i + 1) + (rowNumberText.fontSize / 2);

                rTransform.offsetMax = new Vector2(0.5f, offsetY);
                rTransform.offsetMin = new Vector2(0.5f, offsetY);

                // Set height to desired height
                rTransform.sizeDelta = new Vector2(rowNumberText.fontSize * (i + 1).ToString().Length, rowNumberText.fontSize);

                // Add transform to list
            }
        }
    }

    private void OnEnable()
    {
        MakeLineNumbers(25);
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    #endregion Private Methods
}