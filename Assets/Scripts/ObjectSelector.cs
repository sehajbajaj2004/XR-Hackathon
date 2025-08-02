using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectSelectorMover : MonoBehaviour
{
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    public InputActionProperty leftTrigger;
    public InputActionProperty rightTrigger;
    public InputActionProperty leftJoystick;
    public InputActionProperty rightJoystick;

    private GameObject selectedObject = null;
    private bool isObjectSelected = false;

    void Update()
    {
        // Detect Trigger Press from either hand
        if (!isObjectSelected && (IsTriggerPressed(leftTrigger) || IsTriggerPressed(rightTrigger)))
        {
            TrySelectObject();
        }
        else if (isObjectSelected && (IsTriggerPressed(leftTrigger) || IsTriggerPressed(rightTrigger)))
        {
            DeselectObject();
        }

        // Move or Rotate selected object
        if (isObjectSelected && selectedObject != null)
        {
            MoveObject();
            RotateObject();
        }
    }

    private bool IsTriggerPressed(InputActionProperty trigger)
    {
        return trigger.action != null && trigger.action.WasPressedThisFrame();
    }

    private void TrySelectObject()
    {
        RaycastHit hit;

        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out hit) || leftRayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Selectable"))
            {
                selectedObject = hit.collider.gameObject;
                isObjectSelected = true;
            }
        }
    }

    private void DeselectObject()
    {
        selectedObject = null;
        isObjectSelected = false;
    }

    private void MoveObject()
    {
        Vector2 moveInput = leftJoystick.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * Time.deltaTime * 1.5f;
        selectedObject.transform.position += move;
    }

    private void RotateObject()
    {
        Vector2 rotateInput = rightJoystick.action.ReadValue<Vector2>();
        float rotationSpeed = 90f; // degrees per second
        float rotationAmount = rotateInput.x * rotationSpeed * Time.deltaTime;

        selectedObject.transform.Rotate(0f, rotationAmount, 0f);
    }
}
