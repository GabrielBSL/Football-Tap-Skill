using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSideBounceRule : Rule
{
    public override void Initiate(Ball ballReference)
    {
        base.Initiate(ballReference);

        ball = ballReference;
        ball.ballClick.AddListener(BallTouch);

        ball.LeftBounce.AddListener(SideTouch);
        ball.RightBounce.AddListener(SideTouch);
    }

    private void OnDestroy()
    {
        ball.ballClick.RemoveListener(BallTouch);

        ball.LeftBounce.RemoveListener(SideTouch);
        ball.RightBounce.RemoveListener(SideTouch);
    }

    private void BallTouch()
    {
        GameManager.Instance.IncrementScore();
    }

    private void SideTouch()
    {
        GameManager.Instance.ResetScore();
    }
}
