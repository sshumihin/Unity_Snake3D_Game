using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Item
{
    public bool IsAlive { get; private set; }

    FieldController m_field;

    float m_timeCurrent;

    float m_timeSpan;

    Color m_color;

    public Bullet(Color col)
    {
        m_color = col;

        m_timeSpan = GameManager.BULLET_TIME_SPAN;

        Go = GameObject.Instantiate(Resources.Load<GameObject>("Bullet"));

        SetColor();

        m_field = GameManager.Instance.Field;

        IsAlive = true;
    }

    private void SetColor()
    {
        foreach (Transform tr in Go.transform)
        {
            Renderer r = tr.GetComponent<Renderer>();
            if (r == null) continue;

            r.material.color = m_color;
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
            FieldController.Cell cell = m_field.GetNextCell(Cell, Direction);
            if (cell == null)
            {
                IsAlive = false;
            }
            else if (cell.Item != null)
            {
                if (cell.Item is Snake.SnakeBody)
                {
                    Snake.SnakeBody body = cell.Item as Snake.SnakeBody;
                    Snake snake = body.HostSnake;
                    snake.Damage();

                    IsAlive = false;
                }
            }

            if (IsAlive)
            {
                Cell = cell;
            }
            else
            {
                Destroy();
            }
        }
    }
}
