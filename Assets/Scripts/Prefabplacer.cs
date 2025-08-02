using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PrefabPlacer : MonoBehaviour
{
    [Header("XR Ray Interactors")]
    public XRRayInteractor rightRayInteractor;
    public XRRayInteractor leftRayInteractor;

    [Header("Input Actions")]
    public InputActionProperty rightTrigger;
    public InputActionProperty leftTrigger;
    public InputActionProperty aButton; // Only for canceling placement

    private GameObject currentPreview;
    private bool isPlacing = false;

    private void OnEnable()
    {
        rightTrigger.action.Enable();
        leftTrigger.action.Enable();
        aButton.action.Enable();
    }

    private void OnDisable()
    {
        rightTrigger.action.Disable();
        leftTrigger.action.Disable();
        aButton.action.Disable();
    }

    void Update()
    {
        if (isPlacing && currentPreview != null)
        {
            // Update preview position
            if (GetRayHit(out RaycastHit hit))
            {
                Vector3 newPosition = hit.point;
                newPosition.y = 0.21f; // Adjust height as needed
                currentPreview.transform.position = newPosition;
            }

            // Place the prefab
            if (rightTrigger.action.WasPressedThisFrame() || leftTrigger.action.WasPressedThisFrame())
            {
                PlacePrefab();
            }

            // Cancel/delete the current preview prefab when A button is pressed
            if (aButton.action.WasPressedThisFrame())
            {
                CancelPrefabPlacement();
            }
        }
    }

    public void SelectModelPrefab(GameObject prefab)
    {
        if (prefab == null) return;

        // Clean up any existing preview
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        // Create new preview
        currentPreview = Instantiate(prefab);
        currentPreview.GetComponent<Collider>().enabled = false;
        isPlacing = true;
    }

    private void PlacePrefab()
    {
        if (currentPreview == null) return;

        // Enable collider and clear reference
        currentPreview.GetComponent<Collider>().enabled = true;
        currentPreview = null;
        isPlacing = false;
    }

    private void CancelPrefabPlacement()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
        isPlacing = false;
        Debug.Log("Prefab placement canceled");
    }

    private bool GetRayHit(out RaycastHit hit)
    {
        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out hit)) return true;
        if (leftRayInteractor.TryGetCurrent3DRaycastHit(out hit)) return true;

        hit = new RaycastHit();
        return false;
    }
}