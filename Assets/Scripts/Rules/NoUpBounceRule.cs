using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoUpBounceRule : Rule
{
    public override void Initiate(Ball ballReference)
    {
        base.Initiate(ballReference);

        ball = ballReference;

        ball.UpBounce.AddListener(UpTouched);
        ball.ballClick.AddListener(BallTouch);
    }

    private void OnDestroy()
    {
        ball.UpBounce.RemoveListener(UpTouched);
        ball.ballClick.RemoveListener(BallTouch);
    }

    private void UpTouched()
    {
        GameManager.Instance.ResetScore();
    }

    private void BallTouch()
    {
        GameManager.Instance.IncrementScore();
    }
}
