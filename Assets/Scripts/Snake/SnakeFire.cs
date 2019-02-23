using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeFire
{
    internal bool IsFireAvailable { get; private set; }

    float m_timeCurrent = 0f;

    Bullet m_bullet;

    float m_timeSpan;

    Color m_color;

    public SnakeFire(Color col)
    {
        m_timeSpan = GameManager.FIRE_TIME_SPAN;

        m_color = col;
    }

    internal void Update()
    {
        m_timeCurrent += Time.deltaTime;
        if (m_timeCurrent >= m_timeSpan)
        {
            m_timeCurrent = 0f;
            IsFireAvailable = true;
        }

        if (m_bullet != null) m_bullet.Update();
    }

    internal void Fire(FieldController.Cell cell, eDirections dir)
    {
        //Debug.Log("Fire!");

        m_timeCurrent = 0f;
        IsFireAvailable = false;

        m_bullet = new Bullet(m_color) { Cell = cell, Direction = dir };
    }
}
