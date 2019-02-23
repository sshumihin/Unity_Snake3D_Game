using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Snake
{
    const int SIZE_MIN = 3;

    public class SnakeBody : Item
    {
        internal Snake HostSnake { get; private set; }

        public SnakeBody(Snake snake, string name)
        {
            HostSnake = snake;

            Go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Go.name = name;
        }
    }

    internal SnakeBody Head
    {
        get
        {
            return m_bodies[0];
        }
    }

    internal bool IsAlive { get; private set; }

    internal int Idx { get; private set; }

    const string m_nameTemplate = "Snake_{0}_body_{1}";

    Color m_color;

    FieldController m_field;

    List<SnakeBody> m_bodies = new List<SnakeBody>();

    float m_timeCurrent;

    FieldController.Cell m_targetFoodCell;

    SnakeFire m_snakeFire;

    float m_timeSpan;

    public Snake(FieldController field, int idx, Color col)
    {
        m_field = field;

        Idx = idx;

        m_color = col;

        m_timeSpan = GameManager.SNAKE_TIME_SPAN;

        IsAlive = true;

        CreateSnake();

        SetColor();

        m_snakeFire = new SnakeFire(m_color);
    }

    private void CreateSnake()
    {
        FieldController.Cell cell = null;
        for (int i = 0; i < SIZE_MIN; i++)
        {
            SnakeBody body = new SnakeBody(this, string.Format(m_nameTemplate, Idx, i));
            if (i == 0)
            {
                cell = m_field.GetRandomCell();
                if (cell == null)
                {
                    Debug.LogError("Snake Error!");
                    continue;
                }

                body.Cell = cell;

                //set direction
                body.Direction = Utilities.GetRandomDirection();
            }
            else
            {
                eDirections dir = m_bodies[i - 1].Direction;
                eDirections nextDir = Utilities.GetOppositeDirection(dir);

                FieldController.Cell bcell = m_field.GetNextCell(cell, nextDir);
                if (bcell == null)
                {
                    //need other cell
                    Debug.LogWarning("Snake Error!");

                    bcell = m_field.GetRandomNeigborCell(cell, nextDir);
                }

                body.Cell = bcell;
                body.Direction = Utilities.GetNextCellDirection(bcell.FieldPosition, cell.FieldPosition);

                cell = body.Cell;
            }

            m_bodies.Add(body);
        }
    }



    void SetColor()
    {
        for (int i = 0; i < m_bodies.Count; i++)
        {
            Renderer r = m_bodies[i].Go.GetComponent<Renderer>();

            r.material.color = (i == 0) ? m_color * 0.5f : m_color;
        }
    }

    internal void Update()
    {
        if (!IsAlive) return;

        bool timeToMove = false;
        m_timeCurrent += Time.deltaTime;
        if (m_timeCurrent >= m_timeSpan)
        {
            m_timeCurrent = 0f;
            timeToMove = true;
        }

        if (timeToMove)
        {
            //Debug.Log("Can Move!");
            SnakeBody head = m_bodies[0];
            eDirections dir = head.Direction;

            if (m_targetFoodCell == null)
            {
                dir = TryChangeDirection(dir);
            }

            FieldController.Cell cell = GetNextCell(head.Cell, dir);
            if (cell == null)
            {
                DoDeath();
            }
            else if (cell.Item != null)
            {
                if (cell.Item is Food)
                {
                    Food food = cell.Item as Food;
                    food.EatFood();

                    m_targetFoodCell = null;

                    AddSection();
                }
                else if (cell.Item is SnakeBody || cell.Item is Bullet)
                {
                    FieldController.Cell ncell = m_field.GetRandomNeigborCell(head.Cell, dir);
                    if (ncell == null)
                    {
                        Debug.Log("Snake is Dead! " + Idx);
                        DoDeath();
                    }
                    else
                    {
                        cell = ncell;
                    }
                }
            }

            if (IsAlive)
            {
                UpdateSections(cell);

                //TODO check line and fire
                TryKillAlienSnake();

                TryCatchFood();
            }
        }

        if (m_snakeFire != null) m_snakeFire.Update();
    }

    private void TryKillAlienSnake()
    {
        SnakeBody head = m_bodies[0];
        eDirections dir = head.Direction;
        if (m_field.IsAlienStraightAhead(head.Cell, dir, Idx))
        {
            if (m_snakeFire != null && m_snakeFire.IsFireAvailable)
            {
                FieldController.Cell nextcell = m_field.GetNextCell(head.Cell, dir);
                if (nextcell != null)
                {
                    m_snakeFire.Fire(nextcell, dir);
                }
            }
        }
    }


    private void TryCatchFood()
    {
        if (m_targetFoodCell != null) return;

        SnakeBody head = m_bodies[0];
        eDirections exDir = Utilities.GetOppositeDirection(head.Direction);

        Tuple<FieldController.Cell, eDirections> target = m_field.GetFoodCellDirection(head.Cell, exDir);

        if (target != null && target.Item1 != null)
        {
            m_targetFoodCell = target.Item1;
            head.Direction = target.Item2;
        }
    }

    private FieldController.Cell GetNextCell(FieldController.Cell cell, eDirections dir)
    {
        FieldController.Cell result = m_field.GetNextCell(cell, dir);
        if (result == null)
        {
            //Debug.Log("Cell is null!");
            result = m_field.GetRandomNeigborCell(cell, dir);
            if (result == null)
            {
                //no more empty cells
                //Debug.LogError("Cell is null!");
            }
        }

        return result;
    }

    private eDirections TryChangeDirection(eDirections dir)
    {
        float rnd = URandom.value;
        if (rnd < 0.2f)
        {
            dir = Utilities.GetRandomDirection(dir);
        }

        return dir;
    }

    private void UpdateSections(FieldController.Cell cell)
    {
        FieldController.Cell pcell = m_bodies[0].Cell;
        FieldController.Cell ncell = cell;
        for (int i = 0; i < m_bodies.Count; i++)
        {
            SnakeBody body = m_bodies[i];

            pcell = body.Cell;

            body.Cell.Free();
            body.Cell = ncell;

            body.Direction = Utilities.GetNextCellDirection(pcell.FieldPosition, ncell.FieldPosition);

            ncell = pcell;
        }
    }

    void AddSection()
    {
        SnakeBody body = new SnakeBody(this, string.Format(m_nameTemplate, Idx, m_bodies.Count - 1));
        SnakeBody last = m_bodies[m_bodies.Count - 1];
        eDirections dir = last.Direction;
        FieldController.Cell bcell = m_field.GetNextCell(last.Cell, Utilities.GetOppositeDirection(dir));
        if (bcell == null)
        {
            //need other cell
            Debug.LogWarning("Snake Error!");

            bcell = m_field.GetRandomNeigborCell(last.Cell, dir);
        }

        body.Cell = bcell;
        body.Direction = dir;

        m_bodies.Add(body);

        SetColor();
    }

    void RemoveSection()
    {
        SnakeBody body = m_bodies[m_bodies.Count - 1];
        m_bodies.RemoveAt(m_bodies.Count - 1);

        body.Cell.Free();
        body.Go.AddComponent<Rigidbody>();
    }

    internal void Damage()
    {
        if (m_bodies.Count <= SIZE_MIN)
        {
            Debug.Log("Snake is Dead! " + Idx);
            DoDeath();
            return;
        }

        RemoveSection();
    }

    void DoDeath()
    {
        IsAlive = false;

        for (int i = 0; i < m_bodies.Count; i++)
        {
            m_bodies[i].Cell.Free();
            m_bodies[i].Go.AddComponent<Rigidbody>();
        }
    }

    internal void Destroy()
    {
        for (int i = 0; i < m_bodies.Count; i++)
        {
            SnakeBody body = m_bodies[i];
            if (body.Go != null)
            {
                GameObject.Destroy(body.Go);
            }

        }
    }

    internal void Kill()
    {
        DoDeath();
    }
}
