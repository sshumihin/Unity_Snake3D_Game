using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

public class FieldController
{
    const int FIELD_SIZE = 15;

    public class Cell
    {
        public Item Item;

        public Vector3 WorldPosition { get; }

        public int[] FieldPosition { get; }

        public Cell(Vector3 wPos, int[] fPos)
        {
            WorldPosition = wPos;

            FieldPosition = fPos;
        }

        internal void Use(Item it)
        {
            Item = it;

            Item.SetPosition(WorldPosition);
        }

        internal void Free()
        {
            Item = null;
        }
    }

    Cell[,,] m_field = new Cell[FIELD_SIZE, FIELD_SIZE, FIELD_SIZE];

    public FieldController()
    {
        InitField();
    }


    private void InitField()
    {
        for (int z = 0; z < FIELD_SIZE; z++)
        {
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                for (int x = 0; x < FIELD_SIZE; x++)
                {
                    m_field[x, y, z] = new Cell(new Vector3(x, y, z), new int[3] { x, y, z });
                }
            }
        }
    }

    internal void StartGame()
    {
    }

    internal void StopGame()
    {

    }

    internal Cell GetRandomCell()
    {
        Cell cell = null;

        while (cell == null)
        {
            Cell c = m_field[URandom.Range(0, FIELD_SIZE), URandom.Range(0, FIELD_SIZE), URandom.Range(0, FIELD_SIZE)];

            if (c.Item == null)
            {
                cell = c;
            }
        }

        return cell;
    }

    internal Cell GetNextCell(Cell cell, eDirections dir)
    {
        switch (dir)
        {
            case eDirections.RIGHT:
                if (cell.FieldPosition[0] + 1 < FIELD_SIZE)
                {
                    return m_field[cell.FieldPosition[0] + 1, cell.FieldPosition[1], cell.FieldPosition[2]];
                }
                break;
            case eDirections.LEFT:

                if (cell.FieldPosition[0] > 0)
                {
                    return m_field[cell.FieldPosition[0] - 1, cell.FieldPosition[1], cell.FieldPosition[2]];
                }
                break;
            case eDirections.UP:

                if (cell.FieldPosition[1] + 1 < FIELD_SIZE)
                {
                    return m_field[cell.FieldPosition[0], cell.FieldPosition[1] + 1, cell.FieldPosition[2]];
                }
                break;
            case eDirections.DOWN:
                if (cell.FieldPosition[1] > 0)
                {
                    return m_field[cell.FieldPosition[0], cell.FieldPosition[1] - 1, cell.FieldPosition[2]];
                }
                break;
            case eDirections.FORWARD:
                if (cell.FieldPosition[2] + 1 < FIELD_SIZE)
                {
                    return m_field[cell.FieldPosition[0], cell.FieldPosition[1], cell.FieldPosition[2] + 1];
                }
                break;
            case eDirections.BACKWARD:
                if (cell.FieldPosition[2] > 0)
                {
                    return m_field[cell.FieldPosition[0], cell.FieldPosition[1], cell.FieldPosition[2] - 1];
                }
                break;
        }

        return null;
    }

    internal Cell GetRandomNeigborCell(Cell cell)
    {
        List<eDirections> dirs = Utilities.GetDirectionsList();

        Cell ncell = null;
        while (ncell == null && dirs.Count > 0)
        {
            int rnd = URandom.Range(0, dirs.Count);
            eDirections dir = dirs[rnd];

            ncell = GetNextCell(cell, dir);

            dirs.Remove(dir);
        }

        return ncell;
    }

    internal bool IsAlienStraightAhead(Cell cell, eDirections dir, int idx)
    {
        switch (dir)
        {
            case eDirections.FORWARD:
                if (cell.FieldPosition[2] < FIELD_SIZE - 1)
                {
                    for (int i = cell.FieldPosition[2] + 1; i < FIELD_SIZE; i++)
                    {
                        Cell c = m_field[cell.FieldPosition[0], cell.FieldPosition[1], i];
                        if (c.Item == null) continue;

                        if (SnakeIsAlien(c, idx)) return true;
                    }
                }
                break;
            case eDirections.BACKWARD:

                for (int i = 0; i < cell.FieldPosition[2]; i++)
                {
                    Cell c = m_field[cell.FieldPosition[0], cell.FieldPosition[1], i];
                    if (c.Item == null) continue;

                    if (SnakeIsAlien(c, idx)) return true;
                }
                break;
            case eDirections.RIGHT:
                if (cell.FieldPosition[0] < FIELD_SIZE - 1)
                {
                    for (int i = cell.FieldPosition[0] + 1; i < FIELD_SIZE; i++)
                    {
                        Cell c = m_field[i, cell.FieldPosition[1], cell.FieldPosition[2]];
                        if (c.Item == null) continue;

                        if (SnakeIsAlien(c, idx)) return true;
                    }
                }
                break;
            case eDirections.LEFT:
                for (int i = 0; i < cell.FieldPosition[0]; i++)
                {
                    Cell c = m_field[i, cell.FieldPosition[1], cell.FieldPosition[2]];
                    if (c.Item == null) continue;

                    if (SnakeIsAlien(c, idx)) return true;
                }
                break;
            case eDirections.UP:
                if (cell.FieldPosition[1] < FIELD_SIZE - 1)
                {
                    for (int i = cell.FieldPosition[1] + 1; i < FIELD_SIZE; i++)
                    {
                        Cell c = m_field[cell.FieldPosition[0], i, cell.FieldPosition[2]];
                        if (c.Item == null) continue;

                        if (SnakeIsAlien(c, idx)) return true;
                    }
                }
                break;
            case eDirections.DOWN:
                for (int i = 0; i < cell.FieldPosition[1]; i++)
                {
                    Cell c = m_field[cell.FieldPosition[0], i, cell.FieldPosition[2]];
                    if (c.Item == null) continue;

                    if (SnakeIsAlien(c, idx)) return true;
                }
                break;

        }

        return false;
    }

    private bool SnakeIsAlien(Cell c, int idx)
    {
        if (c.Item is Snake.SnakeBody)
        {
            Snake.SnakeBody body = c.Item as Snake.SnakeBody;
            if (body.HostSnake.Idx != idx) return true;
        }

        return false;
    }

    internal Tuple<FieldController.Cell, eDirections> GetFoodCellDirection(Cell cell, eDirections excludeDir)
    {
        Tuple<FieldController.Cell, eDirections> result = null;

        List<eDirections> dirs = Utilities.GetDirectionsList();
        dirs.Remove(excludeDir);

        for (int i = 0; i < dirs.Count; i++)
        {
            Cell cellNext = cell;
            while (cellNext != null)
            {
                cellNext = GetNextCell(cellNext, dirs[i]);
                if (cellNext != null && cellNext.Item != null && cellNext.Item is Food)
                {
                    result = Tuple.Create(cellNext, dirs[i]);
                    break;
                }
            }
        }

        return result;
    }

    internal Cell GetRandomNeigborCell(Cell cell, eDirections excludeDir)
    {
        List<eDirections> newDirs = new List<eDirections>();

        eDirections[] dirs = Utilities.GetDirectionsArray();
        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i] == excludeDir) continue;
            if (dirs[i] == Utilities.GetOppositeDirection(excludeDir)) continue;

            Cell ncell = GetNextCell(cell, dirs[i]);
            if (ncell == null) continue;

            Item item = ncell.Item;
            if (item != null && (item is Snake.SnakeBody || item is Bullet)) continue;

            newDirs.Add(dirs[i]);
        }

        Cell result = GetRandomCell(cell, newDirs);

        return result;
    }

    private Cell GetRandomCell(Cell cell, List<eDirections> dirs)
    {
        Cell result = null;
        if (dirs.Count > 0)
        {
            int rnd = URandom.Range(0, dirs.Count);
            eDirections dir = dirs[rnd];

            result = GetNextCell(cell, dir);
        }

        return result;
    }
}
