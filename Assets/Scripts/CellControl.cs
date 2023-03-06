using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellControl : MonoBehaviour
{
    private GameManager _manager;
    private void Start()
    {
        _manager = GameManager.instance;
    }

    private void OnMouseDown()
    {
        _manager.MarkToCell(this.gameObject);
    }
}
