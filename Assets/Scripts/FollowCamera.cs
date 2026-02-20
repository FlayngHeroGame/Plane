using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Rigidbody targetRb;

    [Header("Направление трассы")]
    public Transform trackStart;               // Начало трассы (например, slingOrigin)
    public Transform trackEnd;                 // Конец трассы (например, endPoint)

    [Header("Дистанция")]
    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float minSpeed = 0f;
    public float maxSpeed = 50f;

    [Header("Высота")]
    public float heightOffset = 3f;

    [Header("Плавность")]
    public float positionSmoothTime = 0.4f;
    public float distanceSmoothTime = 0.5f;

    bool isFollowing = false;
    Vector3 velocity = Vector3.zero;
    float currentDistance;
    float distanceVelocity;
    bool initialized = false;
    Vector3 trackDirection;                    // Фиксированное направление трассы

    public void StartFollowing()
    {
        isFollowing = true;
    }

    void LateUpdate()
    {
        if (!isFollowing || target == null || targetRb == null) return;

        if (!initialized)
        {
            currentDistance = minDistance;

            // Вычисляем фиксированное направление трассы один раз
            if (trackStart != null && trackEnd != null)
            {
                trackDirection = (trackEnd.position - trackStart.position).normalized;
            }
            else
            {
                // Fallback: используем мировую ось Z
                trackDirection = Vector3.forward;
            }

            initialized = true;
        }

        // Плавное изменение дистанции в зависимости от скорости
        float speed = targetRb.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, t);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, distanceSmoothTime);

        // Камера всегда позади самолёта ВДОЛЬ ТРАССЫ (фиксированное направление)
        // НЕ зависит от поворотов самолёта
        Vector3 desiredPosition = target.position - trackDirection * currentDistance + Vector3.up * heightOffset;

        // Плавное движение камеры
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);

        // Камера всегда смотрит вперёд по трассе с небольшим наклоном к самолёту
        // Rotation фиксирован — не зависит от ориентации самолёта
        Vector3 lookTarget = target.position;
        Vector3 lookDirection = lookTarget - transform.position;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            // Фиксируем вращение: смотрим на самолёт, но без крена
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }
}