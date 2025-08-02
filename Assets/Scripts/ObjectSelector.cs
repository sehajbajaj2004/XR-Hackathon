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
    public InputActionProperty aButton;

    [Header("Movement Components")]
    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;

    private GameObject selectedObject = null;
    private bool isObjectSelected = false;
    private bool triggerWasPressed = false;

    private void OnEnable()
    {
        leftTrigger.action.Enable();
        rightTrigger.action.Enable();
        leftJoystick.action.Enable();
        rightJoystick.action.Enable();
        aButton.action.Enable();
    }

    private void OnDisable()
    {
        leftTrigger.action.Disable();
        rightTrigger.action.Disable();
        leftJoystick.action.Disable();
        rightJoystick.action.Disable();
        aButton.action.Disable();
    }

    void Update()
    {
        bool triggerPressed = IsTriggerPressed(leftTrigger) || IsTriggerPressed(rightTrigger);

        // Toggle selection with trigger press
        if (!triggerWasPressed && triggerPressed)
        {
            if (!isObjectSelected)
                TrySelectObject();
            else
                DeselectObject();
        }

        triggerWasPressed = triggerPressed;

        if (isObjectSelected && selectedObject != null)
        {
            MoveObject();
            RotateObject();

            // Delete object with A button
            if (aButton.action != null && aButton.action.WasPressedThisFrame())
            {
                selectedObject.SetActive(false);
                DeselectObject(); // This will enable both providers
            }
        }
    }

    private bool IsTriggerPressed(InputActionProperty trigger)
    {
        return trigger.action != null && trigger.action.ReadValue<float>() > 0.5f;
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

                // Disable both providers when selecting
                SetMovementProvidersEnabled(false);
            }
        }
    }

    private void DeselectObject()
    {
        // Enable both providers when deselecting
        SetMovementProvidersEnabled(true);

        selectedObject = null;
        isObjectSelected = false;
    }

    private void SetMovementProvidersEnabled(bool enabled)
    {
        if (moveProvider != null)
        {
            moveProvider.enabled = enabled;
            Debug.Log($"Move provider {(enabled ? "enabled" : "disabled")}");
        }
        if (turnProvider != null)
        {
            turnProvider.enabled = enabled;
            Debug.Log($"Turn provider {(enabled ? "enabled" : "disabled")}");
        }
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