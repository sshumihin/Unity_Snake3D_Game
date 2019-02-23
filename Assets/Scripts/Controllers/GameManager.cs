using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public const float SNAKE_TIME_SPAN = 0.25f;

    public const float BULLET_TIME_SPAN = 0.1f;

    public const float FIRE_TIME_SPAN = 5f;

    public const int FOOD_COUNT = 5;

    public const int SNAKE_COUNT = 4;

    internal readonly Color[] COLORS = { Color.red, Color.blue, Color.green, Color.white, Color.yellow };

    FoodController m_food;

    internal FieldController Field { get; private set; }

    internal SnakeController Snakes { get; private set; }

    bool m_isGameOver;

    CameraMovement m_camMove;

    UICanvas m_canvas;

    private void Awake()
    {
        Field = new FieldController();

        m_food = new FoodController(Field);

        Snakes = new SnakeController(Field);

        m_camMove = Camera.main.GetComponent<CameraMovement>();

        m_canvas = FindObjectOfType<UICanvas>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Field.StartGame();
        m_food.StartGame();
        Snakes.StartGame();

        m_isGameOver = false;
    }

    private void Update()
    {
        if (m_isGameOver) return;

        if (Snakes.UpdateSnakes() == eSnakeState.COMPLETE)
        {
            m_isGameOver = true;
            CompleteGame();
            return;
        }
    }

    private void CompleteGame()
    {
        Debug.Log("Complete Game!");

        Field.StopGame();
        m_food.StopGame();
        Snakes.StopGame();

        m_camMove.ShowWinner(Snakes.LastSnake.Head.Go.transform);

        m_canvas.ShowPanel();
    }
}
