using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectSelectorMover : MonoBehaviour
{
    [Header("Ray Interactors")]
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    [Header("Input Actions")]
    public InputActionProperty leftTrigger;
    public InputActionProperty rightTrigger;
    public InputActionProperty leftJoystick;
    public InputActionProperty rightJoystick;

    [Header("Movement Components")]
    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;

    private GameObject selectedObject = null;
    private bool isObjectSelected = false;
    private bool triggerWasPressed = false;

    void Update()
    {
        bool triggerPressed = IsTriggerPressed(leftTrigger) || IsTriggerPressed(rightTrigger);

        if (!triggerWasPressed && triggerPressed)
        {
            // Toggle selection
            if (!isObjectSelected)
                TrySelectObject();
            else
                DeselectObject();
        }

        triggerWasPressed = triggerPressed;

        // Move or Rotate selected object
        if (isObjectSelected && selectedObject != null)
        {
            MoveObject();
            RotateObject();
        }
    }

    private bool IsTriggerPressed(InputActionProperty trigger)
    {
        return trigger.action != null && trigger.action.IsPressed();
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

                // Disable movement & turning
                if (moveProvider != null) moveProvider.enabled = false;
                if (turnProvider != null) turnProvider.enabled = false;
            }
        }
    }

    private void DeselectObject()
    {
        selectedObject = null;
        isObjectSelected = false;

        // Re-enable movement & turning
        if (moveProvider != null) moveProvider.enabled = true;
        if (turnProvider != null) turnProvider.enabled = true;
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
        float rotationSpeed = 90f;
        float rotationAmount = rotateInput.x * rotationSpeed * Time.deltaTime;

        selectedObject.transform.Rotate(0f, rotationAmount, 0f);
    }
}
