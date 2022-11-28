using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSideWallRule : Rule
{
    public override void Initiate(Ball ballReference)
    {
        base.Initiate(ballReference);

        ball = ballReference;

        ball.LeftBounce.AddListener(SideTouched);
        ball.RightBounce.AddListener(SideTouched);
    }

    private void OnDestroy()
    {
        ball.LeftBounce.RemoveListener(SideTouched);
        ball.RightBounce.RemoveListener(SideTouched);
    }

    private void SideTouched()
    {
        GameManager.Instance.IncrementScore();
    }
}
