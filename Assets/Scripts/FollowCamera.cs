using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Rigidbody targetRb;

    [Header("Дистанция")]
    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float minSpeed = 0f;
    public float maxSpeed = 50f;

    [Header("Высота")]
    public float heightOffset = 3f;

    [Header("Плавность")]
    public float positionSmoothTime = 0.4f;   // Время сглаживания позиции (секунды)
    public float rotationSmoothSpeed = 3f;     // Скорость сглаживания поворота
    public float distanceSmoothTime = 0.5f;    // Время сглаживания дистанции

    bool isFollowing = false;
    Vector3 velocity = Vector3.zero;           // Для SmoothDamp
    float currentDistance;
    float distanceVelocity;                    // Для SmoothDamp дистанции
    bool initialized = false;

    public void StartFollowing()
    {
        isFollowing = true;
    }

    void LateUpdate()
    {
        if (!isFollowing || target == null || targetRb == null) return;

        // Инициализация: камера стартует с текущей позиции без рывка
        if (!initialized)
        {
            currentDistance = minDistance;
            initialized = true;
        }

        // Плавное изменение дистанции в зависимости от скорости
        float speed = targetRb.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, t);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, distanceSmoothTime);

        // Целевая позиция камеры
        Vector3 desiredPosition = target.position - target.forward * currentDistance + Vector3.up * heightOffset;

        // Плавное движение камеры (SmoothDamp вместо Lerp — нет рывков)
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);

        // Плавный поворот вместо жёсткого LookAt
        Vector3 lookDirection = target.position - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }
}