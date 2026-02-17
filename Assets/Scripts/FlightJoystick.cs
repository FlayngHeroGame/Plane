using UnityEngine;
using UnityEngine.EventSystems;

public class FlightJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle;
    public float joystickRadius = 100f;

    Vector2 inputDirection = Vector2.zero;
    bool isActive = false;

    public Vector2 InputDirection => isActive ? inputDirection : Vector2.zero;

    public void EnableJoystick()
    {
        isActive = true;
        if (background != null) background.gameObject.SetActive(false); // скрыт, пока не коснулись
    }

    public void DisableJoystick()
    {
        isActive = false;
        if (background != null) background.gameObject.SetActive(false);
    }

    void Start()
    {
        if (background != null) background.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive) return;
        if (background != null)
        {
            background.position = eventData.position;
            background.gameObject.SetActive(true);
        }
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isActive || background == null) return;

        Vector2 offset = eventData.position - (Vector2)background.position;
        offset = Vector2.ClampMagnitude(offset, joystickRadius);
        inputDirection = offset / joystickRadius;

        if (handle != null)
            handle.position = (Vector2)background.position + offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputDirection = Vector2.zero;
        if (handle != null && background != null)
            handle.position = background.position;
        if (background != null)
            background.gameObject.SetActive(false);
    }
}
