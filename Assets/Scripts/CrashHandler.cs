using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CrashHandler : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject explosionPrefab;
    public GameObject distanceMarkerPrefab;
    
    [Header("Настройки крушения")]
    public float hardLandingSpeed = 10f;       // Вертикальная скорость для жёсткого приземления
    public float hardLandingAngle = 45f;       // Угол наклона для жёсткого приземления
    public float stopSpeedThreshold = 0.5f;    // Скорость при которой считаем что остановился
    public float markerOffsetForward = 2f;
    public float groundCheckDistance = 1.5f;   // Дистанция рейкаста для проверки земли
    
    [Header("Детали самолёта (для разлёта при крушении)")]
    public GameObject[] planeParts;            // Корпус, крылья, винт, хвост — дочерние объекты
    public float partExplosionForce = 300f;
    public float partExplosionRadius = 5f;
    
    [Header("Визуал целого самолёта")]
    public GameObject planeVisual;             // Визуальная модель целого самолёта (скрывается при крушении)
    
    Rigidbody rb;
    bool launched = false;
    bool crashed = false;
    bool stopped = false;
    bool wasInAir = false;      // Был ли самолёт в воздухе (после трамплина)
    float launchTime;           // Время запуска самолёта
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Спрятать детали, показать целый самолёт
        SetPartsActive(false);
    }
    
    // Вызывается из PlaneLaunch при запуске
    public void OnLaunched()
    {
        launched = true;
        launchTime = Time.time;
    }
    
    void FixedUpdate()
    {
        if (!launched || stopped) return;
        
        // Защитная задержка — не проверять остановку первые 3 секунды после запуска
        if (Time.time - launchTime < 3f) 
        {
            // Но продолжаем проверять wasInAir
            bool groundedEarly = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
            if (!groundedEarly)
                wasInAir = true;
            return;
        }
        
        // Проверяем: самолёт в воздухе?
        bool grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        
        if (!grounded)
            wasInAir = true;
        
        // Проверяем остановку: только если уже был в воздухе И скорость стала маленькой
        // (не crashed — crashed обрабатывает HardCrash отдельно)
        if (!crashed && wasInAir && rb.linearVelocity.magnitude < stopSpeedThreshold)
        {
            stopped = true;
            StartCoroutine(StopSequence());
        }
    }
    
    void OnCollisionEnter(Collision col)
    {
        if (!launched || crashed) return;
        
        // Если самолёт ещё НЕ был в воздухе — это качение по земле, игнорируем
        if (!wasInAir) return;
        
        // Самолёт был в воздухе и коснулся чего-то — проверяем тип приземления
        float verticalSpeed = Mathf.Abs(rb.linearVelocity.y);
        
        // Угол наклона носа (pitch) — вычисляем угол между forward вектором и горизонталью
        // Используем угол от Vector3.up и вычитаем 90 градусов чтобы получить pitch
        float angle = Mathf.Abs(Vector3.Angle(transform.forward, Vector3.up) - 90f);
        
        // Жёсткое приземление?
        if (verticalSpeed > hardLandingSpeed || angle > hardLandingAngle)
        {
            HardCrash();
        }
        // Мягкое — ничего не делаем, самолёт продолжает катиться
    }
    
    void HardCrash()
    {
        if (crashed) return;
        crashed = true;
        
        // Отключить управление
        var fc = FindObjectOfType<FlightControl>();
        if (fc != null) fc.DisableControl();
        
        // Скрыть целый самолёт, показать детали
        if (planeVisual != null) planeVisual.SetActive(false);
        SetPartsActive(true);
        
        // Разбросать детали
        foreach (var part in planeParts)
        {
            if (part == null) continue;
            
            // Отцепить от родителя
            part.transform.SetParent(null);
            
            // Добавить Rigidbody если нет
            Rigidbody partRb = part.GetComponent<Rigidbody>();
            if (partRb == null) partRb = part.AddComponent<Rigidbody>();
            
            // Добавить Collider если нет
            if (part.GetComponent<Collider>() == null)
                part.AddComponent<BoxCollider>();
            
            // Взрывная сила
            partRb.AddExplosionForce(partExplosionForce, transform.position, partExplosionRadius);
        }
        
        // Спавн взрыва
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        
        // Остановить самолёт-основу
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
    
    // Вызывается когда скорость стала очень маленькой
    IEnumerator StopSequence()
    {
        // Отключить управление если ещё не отключено
        var fc = FindObjectOfType<FlightControl>();
        if (fc != null) fc.DisableControl();
        
        // Если не было жёсткого крушения — просто остановить
        if (!crashed)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        // Ждём 1 секунду, потом спавним столбик
        yield return new WaitForSeconds(1f);
        
        Vector3 markerPos = transform.position + transform.forward * markerOffsetForward;
        if (distanceMarkerPrefab != null)
            Instantiate(distanceMarkerPrefab, markerPos, Quaternion.identity);
        
        // Ещё 1 секунда, потом RewardPanel
        yield return new WaitForSeconds(1f);
        
        var rewardScreen = FindObjectOfType<RewardScreen>();
        if (rewardScreen != null)
            rewardScreen.Show();
    }
    
    void SetPartsActive(bool active)
    {
        if (planeParts == null) return;
        foreach (var part in planeParts)
        {
            if (part != null) part.SetActive(active);
        }
    }
}

