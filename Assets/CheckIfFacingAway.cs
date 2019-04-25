using UnityEngine;
using UnityEngine.Events;

public class CheckIfFacingAway : MonoBehaviour
{
    public Transform target;
    public UnityEvent OnFacedAway;
    public UnityEvent OnFacingAway;
    public UnityEvent OnFacedTowards;
    public UnityEvent OnFacingTowards;

    private bool facingAway;
    private bool facingTowards;

    void Update()
    {
        // If this object is facing away from the target object
        if (Vector3.Dot(transform.forward, target.forward) < 0f)
        {
            facingTowards = false;
            
            // Trigger any appropriate events
            if (!facingAway)
            {
                OnFacedAway.Invoke();
                facingAway = true;
            }

            OnFacingAway.Invoke();
        }
        // If this object is facing torwards from the target object
        else
        {
            facingAway = false;
            
            // Trigger any appropriate events
            if (!facingTowards)
            {
                OnFacedTowards.Invoke();
                facingTowards = true;
            }

            OnFacingTowards.Invoke();
        }
    }
}
