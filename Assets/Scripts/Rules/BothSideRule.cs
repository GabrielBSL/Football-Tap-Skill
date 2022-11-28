using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BothSideRule : Rule
{
    private int _wallsInSequence = 0;

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
        _wallsInSequence++;

        if(_wallsInSequence == 2)
        {
            GameManager.Instance.IncrementScore();
        }
    }

    private void BallTouch()
    {
        _wallsInSequence = 0;
    }
}
