using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashHandler : MonoBehaviour
{
    public GameObject explosion;
    bool crashed = false;

    void OnCollisionEnter(Collision col)
    {
        if (crashed) return;
        
        crashed = true;
        Instantiate(explosion, transform.position, Quaternion.identity);
        Time.timeScale = 0f;
        
        // Перезагрузка сцены через 3 секунды (unscaled time)
        Invoke(nameof(RestartScene), 3f);
    }

    void Update()
    {
        // Возможность ручного перезапуска после краша
        if (crashed && Input.anyKeyDown)
        {
            RestartScene();
        }
    }

    void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
