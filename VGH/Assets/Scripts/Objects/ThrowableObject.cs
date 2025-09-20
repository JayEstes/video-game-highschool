using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableObject : MonoBehaviour
{
    private Rigidbody rigid = null;
    private Transform moveTo;
    private static float FollowForce = 500.0f;
    private static float MaxVelocity = 20.0f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!moveTo) return;

        var followDampening = 15.0f;

        Vector3 dir = (moveTo.position - rigid.position);
        Vector3 force = dir * FollowForce * Time.fixedDeltaTime;
        rigid.linearVelocity = Vector3.ClampMagnitude(rigid.linearVelocity + force, MaxVelocity);

        // Apply force with damping for smoothness
        rigid.linearVelocity = Vector3.ClampMagnitude(rigid.linearVelocity + force, MaxVelocity);
        rigid.linearVelocity *= (1f - followDampening * Time.fixedDeltaTime);
    }

    public void OnPickup(Transform picker)
    {
        moveTo = picker;
        rigid.useGravity = false;
        rigid.linearDamping = 5.5f;
        rigid.freezeRotation = true;
    }

    public void OnThrow(Vector3 force)
    {
        moveTo = null;
        rigid.useGravity = true;
        rigid.linearVelocity = force;
        rigid.linearDamping = 0.0f;
        rigid.freezeRotation = false;
    }
}
