using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ellipse
{
    private float x, y;

    Vector3 lastPosition;
    Transform characterTransform;

    private Vector3 dot1, dot2;

    public float Radius { get { return x; } }

    public Ellipse(Transform center, float radius = 1.0f)
    {
        characterTransform = center;

        x = radius;
        y = radius / 1.5f;
    }

    public void SetRadius(float radius)
    {
        x = radius;
        y = radius / 1.5f;
    }

    private void updateDot()
    {
        // 타원의 정의에 해당하는 두 점을 구한다.
        dot1 = characterTransform.localPosition;
        dot1.x -= (float)Math.Sqrt(x * x - y * y);
        dot2 = characterTransform.localPosition;
        dot2.x += (float)Math.Sqrt(x * x - y * y);

        lastPosition = characterTransform.localPosition;
    }

    public float GetDistance(Transform other)
    {
        if (characterTransform.localPosition != lastPosition)
        {
            updateDot();
        }

        // 두 점으로부터의 거리의 합을 구한다.
        float dist = 0;

        dist += Vector3.Distance(other.localPosition, dot1);
        dist += Vector3.Distance(other.localPosition, dot2);

        return dist;
    }

    public bool InEllipse(Transform other)
    {
        // 2x 보다 작으면 타원안에 포함된다.
        if (GetDistance(other) <= x * 2)
            return true;

        return false;
    }
}
