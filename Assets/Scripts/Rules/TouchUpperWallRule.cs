using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchUpperWallRule : Rule
{
    public override void Initiate(Ball ballReference)
    {
        base.Initiate(ballReference);

        ball = ballReference;

        ball.UpBounce.AddListener(UpTouched);
    }

    private void OnDestroy()
    {
        ball.UpBounce.RemoveListener(UpTouched);
    }

    private void UpTouched()
    {
        GameManager.Instance.IncrementScore();
    }
}
