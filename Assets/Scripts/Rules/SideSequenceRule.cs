using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideSequenceRule : Rule
{
    private bool _alreadyTouchedBall = false;

    public override void Initiate(Ball ballReference)
    {
        base.Initiate(ballReference);

        ball = ballReference;

        ball.ballClick.AddListener(BallTouch);
        ball.LeftBounce.AddListener(SideTouched);
        ball.RightBounce.AddListener(SideTouched);
    }

    private void OnDestroy()
    {
        ball.ballClick.RemoveListener(BallTouch);
        ball.LeftBounce.RemoveListener(SideTouched);
        ball.RightBounce.RemoveListener(SideTouched);
    }

    private void SideTouched()
    {
        _alreadyTouchedBall = false;
        GameManager.Instance.IncrementScore();
    }

    private void BallTouch()
    {
        if (_alreadyTouchedBall)
        {
            GameManager.Instance.ResetScore();
        }

        _alreadyTouchedBall = true;
    }
}
