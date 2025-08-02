using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayGrabXZOnly : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public Transform attachPoint;

    private void Awake()
    {
        if (rayInteractor == null)
            rayInteractor = GetComponent<XRRayInteractor>();
    }

    private void Update()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Vector3 hitPos = hit.point;
            //hitPos.y = 0; // lock to XZ plane
            attachPoint.position = hitPos;
        }
    }
}
