using UnityEngine;

[System.Serializable]
public class BallTouchRule : Rule
{
    public override void Initiate(Ball ballReference)
    {
        base.Initiate(ballReference);

        ball = ballReference;
        ball.ballClick.AddListener(BallTouch);
    }

    private void OnDestroy()
    {
        ball.ballClick.RemoveListener(BallTouch);
    }

    private void BallTouch()
    {
        GameManager.Instance.IncrementScore();
    }
}
