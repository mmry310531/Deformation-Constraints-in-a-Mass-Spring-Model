using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forced_Color : MonoBehaviour
{
    public Color color;

    public Spring springCS;
    public float red = 0f;
    public float green = 0f;
    public float blue = 0f;
    public float alpha = 1f;
    public Renderer render;
    void Start()
    {
        springCS = this.GetComponent<Spring>();
        render = this.GetComponent<Renderer>();
    }

    void Update()
    {
        if (springCS.ABSForce > 12f)
        {
            if (red < 1f)
                red += 0.1f;
            if (blue > 0f)
                blue -= 0.1f;
        }
        else
        {
            if (red > 0f)
                red -= 0.1f;
            if (blue < 1f)
                blue += 0.1f;
        }
        color = new Color(red, green, blue, alpha);
        render.material.color = color;
    }
}
