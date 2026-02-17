using UnityEngine;

public class FlightControl : MonoBehaviour
{
    public FlightJoystick joystick;
    public Rigidbody planeRb;
    public float pitchSpeed = 50f;
    public float yawSpeed = 50f;

    bool controlActive = false;

    public void EnableControl()
    {
        controlActive = true;
        if (joystick != null) joystick.EnableJoystick();
    }

    public void DisableControl()
    {
        controlActive = false;
        if (joystick != null) joystick.DisableJoystick();
    }

    void FixedUpdate()
    {
        if (!controlActive || joystick == null || planeRb == null) return;

        Vector2 input = joystick.InputDirection;
        if (input.sqrMagnitude < 0.01f) return;

        // Pitch — вверх/вниз (инвертирован: тянем вниз — нос вверх)
        Vector3 pitchTorque = planeRb.transform.right * (-input.y * pitchSpeed);
        // Yaw — лево/право
        Vector3 yawTorque = planeRb.transform.up * (input.x * yawSpeed);

        planeRb.AddTorque(pitchTorque + yawTorque, ForceMode.Acceleration);
    }
}
