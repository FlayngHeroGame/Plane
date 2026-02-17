using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Rigidbody targetRb;

    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float minSpeed = 0f;
    public float maxSpeed = 50f;
    public float smoothSpeed = 5f;
    public float heightOffset = 3f;

    bool isFollowing = false;

    public void StartFollowing()
    {
        isFollowing = true;
    }

    void LateUpdate()
    {
        if (!isFollowing || target == null || targetRb == null) return;

        float speed = targetRb.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        float distance = Mathf.Lerp(minDistance, maxDistance, t);

        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * heightOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(target);
    }
}
