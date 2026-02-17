using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashHandler : MonoBehaviour
{
    public GameObject explosion;
    bool crashed = false;
    float crashTime;

    void OnCollisionEnter(Collision col)
    {
        if (crashed) return;
        
        crashed = true;
        crashTime = Time.unscaledTime;
        Instantiate(explosion, transform.position, Quaternion.identity);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!crashed) return;

        // Перезагрузка сцены через 3 секунды (unscaled time) или при нажатии пробела/ЛКМ
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Time.unscaledTime - crashTime >= 3f))
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

