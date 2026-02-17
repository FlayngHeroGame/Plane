using UnityEngine;

public class CrashHandler : MonoBehaviour
{
    public GameObject explosion;

    void OnCollisionEnter(Collision col)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Time.timeScale = 0f;
    }
}
