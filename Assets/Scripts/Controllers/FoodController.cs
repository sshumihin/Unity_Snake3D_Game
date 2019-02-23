using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController
{
    List<Food> m_foodList = new List<Food>();

    FieldController m_field;

    int m_foodCount;

    public FoodController(FieldController field)
    {
        m_field = field;

        m_foodCount = GameManager.FOOD_COUNT;
    }

    internal void StartGame()
    {
        for (int i = 0; i < m_foodCount; i++)
        {
            GenerateFood();
        }
    }

    internal void FoodEated(Food food)
    {
        food.Cell = m_field.GetRandomCell();
    }

    private void GenerateFood()
    {
        Food food = new Food(this);
        food.Cell = m_field.GetRandomCell();

        m_foodList.Add(food);
    }

    internal void StopGame()
    {
    }


}
