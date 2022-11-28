using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float touchForce = 10;
    [SerializeField] private float offCenterForce = 2.5f;
    [SerializeField] private float rotationForce = 1000;

    [Header("Effect")]
    [SerializeField] private GameObject ballHitAnimator;

    private float ballEdge;

    #region events
    [HideInInspector] public UnityEvent ballClick;
    [HideInInspector] public UnityEvent LeftBounce;
    [HideInInspector] public UnityEvent RightBounce;
    [HideInInspector] public UnityEvent UpBounce;
    [HideInInspector] public UnityEvent DownTrigger;
    #endregion

    private Vector2 _ballInitialPosition;
    private Animator ballHit;

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out CircleCollider2D collider2D);

        ballEdge = collider2D.radius;
        _ballInitialPosition = transform.position;
        
        Instantiate(ballHitAnimator.gameObject).TryGetComponent(out ballHit);
        ballHit.gameObject.SetActive(false);
    }

    public void ApplyForce(Vector2 touchPoint)
    {
        if(rb.velocity.y > .1f)
        {
            return;
        }
        ballClick?.Invoke();
        AudioManager.instance.PlaySFX("ballKick");

        ballHit.transform.position = touchPoint;
        ballHit.gameObject.SetActive(true);
        ballHit.Play("hit", 0, 0);

        var difference = (Vector2)transform.position - touchPoint;
        Vector2 direction = new Vector2(difference.x, difference.y + (ballEdge / 2)).normalized;

        direction = new Vector2(direction.x, direction.y + (offCenterForce * (Mathf.Abs(difference.y) / ballEdge)));

        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * touchForce, ForceMode2D.Impulse);

        rb.angularVelocity = -difference.x * rotationForce;
    }

    public void ResetBall()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.isKinematic = true;

        transform.position = _ballInitialPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.instance.PlaySFX("ballBounce");

        if (collision.collider.CompareTag("Left"))
        {
            LeftBounce?.Invoke();
        }
        else if (collision.collider.CompareTag("Right"))
        {
            RightBounce?.Invoke();
        }
        else if (collision.collider.CompareTag("Up"))
        {
            UpBounce?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Down"))
        {
            if (GameManager.Instance.RemoveLife())
            {
                ResetBall();
                GameManager.Instance.ResetScore(false);
                AudioManager.instance.PlaySFX("refereeWhistle");
            }
        }
    }
}
