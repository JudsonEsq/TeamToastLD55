using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public CameraController CameraController;
    public float GroundMovementSpeed = 15f;
    public float GroundMovementAcceleration = 200f;

    [Header("Gravity")]
    public float Gravity = 0.5f;
    public float TerminalVelocity = 60;
    public float CurrentGravity => _velocity.y - Mathf.Lerp(_velocity.y, -TerminalVelocity, Gravity);

    public Vector3 WishDirection { get; private set; }
    private Vector3 _velocity;
    readonly Vector3 _plane = new Vector3(1, 0, 1);

    void Update()
    {
        float x = Input.GetAxis("Horizontal Movement");
        float z = Input.GetAxis("Vertical Movement");
        _rawDirectionalMove = new Vector2(x, z);
        Debug.Log(_rawDirectionalMove);

        WishDirection = GetWishDirection();
        GroundMove(WishDirection, _velocity, GroundMovementAcceleration,
        GroundMovementSpeed);
        rb.MovePosition(rb.position + _velocity * Time.deltaTime);
    }

    Vector2 _rawDirectionalMove;
    Vector3 GetWishDirection () {
        return Vector3
        .Cross(
        (_rawDirectionalMove.x*-CameraController.PlanarForward +
        _rawDirectionalMove.y*CameraController.PlanarRight).normalized,
        new Vector3(0,1,0)).normalized;
    }

    void GroundMove (Vector3 dir, Vector3 vel, float accel, float cap) {
        vel = ApplyFriction(vel, 0.3f);
        ApplyGravity();
        _velocity = Accelerate(vel, dir, accel, cap);
    }

    Vector3 ApplyFriction (Vector3 currentVelocity, float friction) {
        return currentVelocity*(1/(friction + 1));
    }

    Vector3 Accelerate (Vector3 currentVelocity, Vector3 wishDirection, float accelerationRate,
                        float accelerationLimit) {
        float speed = Vector3.Dot(Vector3.Scale(_plane, currentVelocity),
        Vector3.Scale(wishDirection, _plane).normalized);
        float speedGain = accelerationRate*Time.deltaTime;

        if (speed + speedGain > accelerationLimit)
            speedGain = Mathf.Clamp(accelerationLimit - speed, 0, accelerationLimit);

        return currentVelocity + wishDirection*speedGain;
    }
    void ApplyGravity () {
        _velocity.y -= CurrentGravity*Time.fixedDeltaTime;
    }
}