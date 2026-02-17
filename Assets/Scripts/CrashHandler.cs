using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CrashHandler : MonoBehaviour
{
    public GameObject explosion;
    public GameObject distanceMarkerPrefab;
    public float markerSpawnDelay = 1f;
    public float markerOffsetForward = 2f;

    Rigidbody rb;
    bool crashed = false;
    float crashTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void OnCollisionEnter(Collision col)
    {
        if (crashed) return;
        crashed = true;

   
        crashTime = Time.unscaledTime;
        Instantiate(explosion, transform.position, Quaternion.identity);

        // Отключить управление в полёте
        var fc = FindObjectOfType<FlightControl>();
        if (fc != null) fc.DisableControl();

        // Остановить самолёт
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        StartCoroutine(PostCrashSequence());
    }

    IEnumerator PostCrashSequence()
    {
        // Ждём 1 секунду перед спавном столба
        yield return new WaitForSeconds(markerSpawnDelay);

        // Спавним дистанционный столб
        Vector3 markerPos = transform.position + transform.forward * markerOffsetForward;
        if (distanceMarkerPrefab != null)
        {
            Instantiate(distanceMarkerPrefab, markerPos, Quaternion.identity);
        }

        // Ждём ещё 1 секунду перед показом окна награды
        yield return new WaitForSeconds(1f);

        // Показать окно награды
        var rewardScreen = FindObjectOfType<RewardScreen>();
        if (rewardScreen != null)
        {
            rewardScreen.Show();
        }
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

