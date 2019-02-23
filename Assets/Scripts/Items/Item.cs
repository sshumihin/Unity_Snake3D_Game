using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public GameObject Go;

    FieldController.Cell m_cell;
    public FieldController.Cell Cell
    {
        get { return m_cell; }

        set
        {
            m_cell = value;

            m_cell.Use(this);
        }
    }

    eDirections m_direction;
    internal eDirections Direction
    {
        get { return m_direction; }
        set
        {
            m_direction = value;

            if (Go != null) Go.transform.forward = Utilities.ConvertDirection(value);
        }
    }

    internal void SetPosition(Vector3 pos)
    {
        Go.transform.position = pos;
    }

    internal void Destroy()
    {
        Cell.Free();

        if (Go != null) GameObject.Destroy(Go);
    }
}
