using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class SnakeController
{
    internal Snake LastSnake
    {
        get
        {
            Snake last = null;
            for (int i = 0; i < m_snakes.Length; i++)
            {
                if (m_snakes[i].IsAlive)
                {
                    last = m_snakes[i];
                    break;
                }
            }

            return last;
        }
    }    

    FieldController m_field;

    Snake[] m_snakes;

    int m_snakeCount;

    public SnakeController(FieldController field)
    {
        m_field = field;

        m_snakeCount = GameManager.SNAKE_COUNT;
    }

    internal void StartGame()
    {
        InitSnakes();
    }

    private void InitSnakes()
    {
        m_snakes = new Snake[m_snakeCount];

        Color[] colors = GameManager.Instance.COLORS;

        int rnd = URandom.Range(0, colors.Length);


        for (int i = 0; i < m_snakes.Length; i++)
        {
            rnd++;
            rnd %= colors.Length;

            Color col = colors[rnd];

            m_snakes[i] = new Snake(m_field, i, col);
        }

    }

    internal void StopGame()
    {

    }

    internal eSnakeState UpdateSnakes()
    {
        for (int i = 0; i < m_snakes.Length; i++)
        {
            m_snakes[i].Update();
        }

        if (LastSnakeAlive())
        {
            Debug.Log("Last Snake!");
            return eSnakeState.COMPLETE;
        }

        return eSnakeState.PROGRESS;
    }

    private bool LastSnakeAlive()
    {
        int cnt = 0;
        for (int i = 0; i < m_snakes.Length; i++)
        {
            if (m_snakes[i].IsAlive) cnt++;
        }

        return cnt == 1;
    }

    internal void KillRandomSnake()
    {
        List<Snake> snakes = new List<Snake>();
        for (int i = 0; i < m_snakes.Length; i++)
        {
            if (m_snakes[i].IsAlive)
            {
                snakes.Add(m_snakes[i]);
            }
        }

        int rnd = URandom.Range(0, snakes.Count);
        snakes[rnd].Kill();
    }
}
