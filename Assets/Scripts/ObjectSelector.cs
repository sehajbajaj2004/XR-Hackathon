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

    [Header("UI Elements")]
    public GameObject hoverUI;      // UI shown on hover
    public GameObject selectionUI;  // UI shown on selection

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
        HandleHoverUI();

        bool triggerPressed = IsTriggerPressed(leftTrigger) || IsTriggerPressed(rightTrigger);

        // Toggle selection
        if (!triggerWasPressed && triggerPressed)
        {
            if (!isObjectSelected)
                TrySelectObject();
            else
                DeselectObject();
        }

        triggerWasPressed = triggerPressed;

        if (isObjectSelected)
        {
            MoveObject();
            RotateObject();

            if (aButton.action != null && aButton.action.WasPressedThisFrame())
            {
                gameObject.SetActive(false);
                DeselectObject();
            }
        }
    }

    private void HandleHoverUI()
    {
        RaycastHit hit;
        bool hoverDetected = false;

        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out hit) || leftRayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                hoverDetected = true;
            }
        }

        if (hoverUI != null)
            hoverUI.SetActive(hoverDetected);
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
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                isObjectSelected = true;
                SetMovementProvidersEnabled(false);

                if (selectionUI != null)
                    selectionUI.SetActive(true);

                if (hoverUI != null)
                    hoverUI.SetActive(false);
            }
        }
    }

    private void DeselectObject()
    {
        isObjectSelected = false;

        SetMovementProvidersEnabled(true);

        if (selectionUI != null)
            selectionUI.SetActive(false);

        if (hoverUI != null)
            hoverUI.SetActive(false);
    }

    private void SetMovementProvidersEnabled(bool enabled)
    {
        if (moveProvider != null)
        {

            //moveProvider.enabled = enabled;
            moveProvider.moveSpeed = enabled ?3: 0;
        }

        if (turnProvider != null)
            turnProvider.turnSpeed = enabled ? 80:0;
            //turnProvider.enabled = enabled;
    }

    private void MoveObject()
    {
        Vector2 moveInput = leftJoystick.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * Time.deltaTime * 1.5f;
        transform.position += move;
    }

    private void RotateObject()
    {
        Vector2 rotateInput = rightJoystick.action.ReadValue<Vector2>();
        float rotationSpeed = 90f;
        float rotationAmount = rotateInput.x * rotationSpeed * Time.deltaTime;

        transform.Rotate(0f, rotationAmount, 0f);
    }
}



//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.XR.Interaction.Toolkit;

//public class ObjectSelectorMover : MonoBehaviour
//{
//    [Header("Ray Interactors")]
//    public XRRayInteractor leftRayInteractor;
//    public XRRayInteractor rightRayInteractor;

//    [Header("Input Actions")]
//    public InputActionProperty leftTrigger;
//    public InputActionProperty rightTrigger;
//    public InputActionProperty leftJoystick;
//    public InputActionProperty rightJoystick;
//    public InputActionProperty aButton;

//    [Header("Movement Components")]
//    public ActionBasedContinuousMoveProvider moveProvider;
//    public ActionBasedContinuousTurnProvider turnProvider;

//    private GameObject selectedObject = null;
//    private bool isObjectSelected = false;
//    private bool triggerWasPressed = false;

//    private void OnEnable()
//    {
//        leftTrigger.action.Enable();
//        rightTrigger.action.Enable();
//        leftJoystick.action.Enable();
//        rightJoystick.action.Enable();
//        aButton.action.Enable();
//    }

//    private void OnDisable()
//    {
//        leftTrigger.action.Disable();
//        rightTrigger.action.Disable();
//        leftJoystick.action.Disable();
//        rightJoystick.action.Disable();
//        aButton.action.Disable();
//    }

//    void Update()
//    {
//        // Check if either trigger is pressed
//        bool triggerPressed = (leftTrigger.action != null && leftTrigger.action.ReadValue<float>() > 0.5f) ||
//                              (rightTrigger.action != null && rightTrigger.action.ReadValue<float>() > 0.5f);

//        // Toggle selection with trigger press
//        if (!triggerWasPressed && triggerPressed)
//        {
//            if (!isObjectSelected)
//            {
//                RaycastHit hit;
//                // Try to select an object using either ray interactor
//                if ((rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out hit)) ||
//                    (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out hit)))
//                {
//                    if (hit.collider != null && hit.collider.gameObject.CompareTag("Selectable"))
//                    {
//                        selectedObject = hit.collider.gameObject;
//                        isObjectSelected = true;

//                        // Disable movement providers when an object is selected
//                        if (moveProvider != null) moveProvider.enabled = false;
//                        if (turnProvider != null) turnProvider.enabled = false;
//                        Debug.Log("Movement providers disabled.");
//                    }
//                }
//            }
//            else // An object is currently selected, deselect it
//            {
//                selectedObject = null;
//                isObjectSelected = false;

//                // Enable movement providers when no object is selected
//                if (moveProvider != null) moveProvider.enabled = true;
//                if (turnProvider != null) turnProvider.enabled = true;
//                Debug.Log("Movement providers enabled.");
//            }
//        }
//        triggerWasPressed = triggerPressed;

//        if (isObjectSelected && selectedObject != null)
//        {
//            // Move the selected object using the left joystick
//            Vector2 moveInput = leftJoystick.action.ReadValue<Vector2>();
//            Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * Time.deltaTime * 1.5f;
//            selectedObject.transform.position += move;

//            // Rotate the selected object using the right joystick
//            Vector2 rotateInput = rightJoystick.action.ReadValue<Vector2>();
//            float rotationSpeed = 90f;
//            float rotationAmount = rotateInput.x * rotationSpeed * Time.deltaTime;
//            selectedObject.transform.Rotate(0f, rotationAmount, 0f);

//            // Delete object with A button
//            if (aButton.action != null && aButton.action.WasPressedThisFrame())
//            {
//                selectedObject.SetActive(false); // Consider Destroy(selectedObject) if permanent removal is desired
//                selectedObject = null;
//                isObjectSelected = false;

//                // Re-enable movement providers after deleting the object
//                if (moveProvider != null) moveProvider.enabled = true;
//                if (turnProvider != null) turnProvider.enabled = true;
//                Debug.Log("Movement providers enabled after deletion.");
//            }
//        }
//    }
//}