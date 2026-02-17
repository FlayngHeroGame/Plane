using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SlingshotVisual : MonoBehaviour
{
    public Transform leftAnchor;
    public Transform rightAnchor;
    public Transform projectile;   // ������

    LineRenderer lr;

    bool launched = false;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 3;
    }

    void Update()
    {
        if (launched) return;

        DrawBand();
    }

    void DrawBand()
    {
        lr.SetPosition(0, leftAnchor.position);
        lr.SetPosition(2, rightAnchor.position);

        Vector3 jitter = Random.insideUnitSphere * 0.02f;
        lr.SetPosition(1, projectile.position + jitter);
    }

    public void OnLaunch()
    {
        launched = true;
        lr.enabled = false; // ������� �������������
    }
}
