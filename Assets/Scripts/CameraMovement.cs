using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    bool m_isMoving;

    Camera m_cam;

    int m_zoom = 20;

    float m_speed = 5f;

    private void Awake()
    {
        m_cam = GetComponent<Camera>();
    }
    
    void Update()
    {
        if (!m_isMoving) return;

        m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, m_zoom, Time.deltaTime * m_speed);
    }

    internal void ShowWinner(Transform target)
    {
        m_isMoving = true;

        transform.LookAt(target);
    }
}
