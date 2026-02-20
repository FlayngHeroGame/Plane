using UnityEngine;

public class FlightControl : MonoBehaviour
{
    public FlightJoystick joystick;
    public Rigidbody planeRb;

    [Header("Скорость поворота")]
    public float pitchSpeed = 15f;          // Было 50 — слишком быстро
    public float yawSpeed = 15f;            // Было 50 — слишком быстро

    [Header("Ограничения")]
    public float maxAngularSpeed = 1.5f;    // Макс. угловая скорость (рад/с) — не даёт раскрутиться
    public float angularDamping = 3f;       // Затухание вращения при отпускании джойстика

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

        if (input.sqrMagnitude > 0.01f)
        {
            // Pitch — вверх/вниз (инвертирован: тянем вниз — нос вверх)
            Vector3 pitchTorque = planeRb.transform.right * (input.y * pitchSpeed);
            // Yaw — лево/право
            Vector3 yawTorque = planeRb.transform.up * (input.x * yawSpeed);

            planeRb.AddTorque(pitchTorque + yawTorque, ForceMode.Acceleration);
        }

        // Затухание вращения — самолёт плавно прекращает вращаться
        // (работает всегда: и с джойстиком, и без)
        planeRb.angularVelocity = Vector3.Lerp(
            planeRb.angularVelocity,
            Vector3.zero,
            Time.fixedDeltaTime * angularDamping);

        // Ограничение максимальной угловой скорости — не даёт раскрутиться
        if (planeRb.angularVelocity.magnitude > maxAngularSpeed)
        {
            planeRb.angularVelocity = planeRb.angularVelocity.normalized * maxAngularSpeed;
        }
    }
}