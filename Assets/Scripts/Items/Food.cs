using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    FoodController m_controller;

    public Food(FoodController controller)
    {
        m_controller = controller;

        Go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    internal void EatFood()
    {
        Cell.Free();

        m_controller.FoodEated(this);
    }

}
