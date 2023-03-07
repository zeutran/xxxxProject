using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private InputField inputFieldHorizontal;
    [SerializeField] private InputField inputFieldVertical;
    [SerializeField] private Sprite[] cellSprites; // 0 --> empty, 1 --> x
    public int[,] CellMatrix; // This is the matrix that holds the values of the cells
    public GameObject[,] CellObjectMatrix; // This is the matrix that holds the objects of the cells

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform centerPoint;

    private int _horizontalCellValues, _verticalCellValues = 0;
    private int _horizontalLenght, _verticalLenght = 0;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        instance = this;
        panel.SetActive(true);
    }

    #region Prepare Map

    public void CallGenerateMap() // This is the method that is called when the button is pressed & creates the map
    {
        panel.SetActive(false);
        _horizontalLenght = int.Parse(inputFieldHorizontal.text);
        _verticalLenght = int.Parse(inputFieldVertical.text);
        GenerateMap(_horizontalLenght, _verticalLenght);
    }

    private void GenerateMap(int horizontal, int vertical) // This is the method that actually generates the map
    {
        CellMatrix = new int[horizontal, vertical];
        CellObjectMatrix = new GameObject[horizontal, vertical];
        // Code to generate the map
        for (int h = 0; h < horizontal; h++) // This is the loop that creates the map horizontally
        {
            for (int v = 0; v < vertical; v++) // This is the loop that creates the map vertically
            {
                GameObject cell = Instantiate(cellPrefab, cellParent);
                cell.transform.position = new Vector3(h, v, 0);
                cell.name = h + "-" + v;
                CellMatrix[h, v] = 1;
                CellObjectMatrix[h, v] = cell;
            }
        }

        CalculateMapCenterPoint(horizontal, vertical);
    }

    private void CalculateMapCenterPoint(int horizontal,
        int vertical) // This is the method that calculates the center point of the map
    {
        float horizontalValue = (float)(horizontal - 1) / 2;
        float verticalValue = (float)(vertical - 1) / 2;

        centerPoint.position =
            new Vector3(horizontalValue, verticalValue, 0); // This is the position of the center point
        CameraFocusAtCenter();
    }

    private void CameraFocusAtCenter() // This is the method that focuses the camera on the center point of the map
    {
        Camera.main.transform.position = new Vector3(centerPoint.position.x, centerPoint.position.y, -10);
        int difference = _horizontalLenght - _verticalLenght;
        
        if (difference > 0)
            Camera.main.orthographicSize = _horizontalLenght + 1f + .15f * _horizontalLenght; 
        else
            Camera.main.orthographicSize = _verticalLenght + 1f + .15f * _verticalLenght;
        
    }

    #endregion

    #region Game Play

    public void MarkToCell(GameObject cell) // This is the method that is called when the cell is clicked
    {
        // Code to mark the cell
        string[] cellName = cell.name.Split('-'); // This is the code that splits the name of the cell for values

        int horizontalIndex = Int32.Parse(cellName[0]); // This is the code that converts to integer horizontal index


        int verticalIndex = Int32.Parse(cellName[1]); // This is the code that converts to integer vertical index
        CellMatrix[horizontalIndex, verticalIndex] = 2;
        SpriteRenderer spriteRenderer = cell.GetComponent<SpriteRenderer>();
        cell.transform.localScale = new Vector3(.2f, .2f, 1f);
        spriteRenderer.sprite = cellSprites[1];

        ControlMatch();
    }

    private void ControlMatch() // This is the method that controls the match
    {
        // Code to control the match
        _horizontalCellValues = 0;
        _verticalCellValues = 0;

        for (int h = 0; h < _horizontalLenght; h++)
        {
            for (int v = 0; v < _verticalLenght; v++)
            {
                if (CellMatrix[h, v] == 2) // This is the condition that checks if the cell is marked
                {
                    _verticalCellValues++; // This is the code that increases the value of the cell
                    if (_verticalCellValues == 3) // This is the condition that checks if the cell is marked 3 times
                    {
                        for (int i = v; i >= v - 2; i--)
                        {
                            CellObjectMatrix[h, i].GetComponent<SpriteRenderer>().sprite = cellSprites[0];
                            CellObjectMatrix[h, i].transform.localScale = new Vector3(1f, 1f, 1f);
                            CellMatrix[h, i] = 1;
                            ControlMatch();
                        }

                        _verticalCellValues = 0;
                    }
                }
                else // This is the condition that checks if the cell is not marked
                {
                    _verticalCellValues = 0;
                }
            }
        }

        for (int v = 0; v < _verticalLenght; v++)
        {
            for (int h = 0; h < _horizontalLenght; h++)
            {
                if (CellMatrix[h, v] == 2) // This is the condition that checks if the cell is marked
                {
                    _horizontalCellValues++; // This is the code that increases the value of the cell
                    if (_horizontalCellValues == 3) // This is the condition that checks if the cell is marked 3 times
                    {
                        for (int i = h; i >= h - 2; i--)
                        {
                            CellObjectMatrix[i, v].GetComponent<SpriteRenderer>().sprite = cellSprites[0];
                            CellObjectMatrix[i, v].transform.localScale = new Vector3(1f, 1f, 1f);
                            CellMatrix[i, v] = 1;
                            ControlMatch();
                        }

                        _horizontalCellValues = 0;
                    }
                }
                else // This is the condition that checks if the cell is not marked
                {
                    _horizontalCellValues = 0;
                }
            }
        }
    }

    #endregion
}