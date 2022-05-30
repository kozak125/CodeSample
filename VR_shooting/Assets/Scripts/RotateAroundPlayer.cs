using UnityEngine;

public class RotateAroundPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    private Vector3 offset;
    private Vector3 previousHeadRotation;

    private void Start()
    {
        offset = transform.position - cameraTransform.position;
        previousHeadRotation = GvrVRHelpers.GetHeadRotation().eulerAngles;
    }

    private void LateUpdate()
    {
        offset = CalculateOffset();
        
        transform.position = Vector3.Lerp(transform.position, cameraTransform.position + offset, 0.1f);
        transform.LookAt(cameraTransform);
    }

    private Vector3 CalculateOffset()
    {
        var angle = GvrVRHelpers.GetHeadRotation().eulerAngles.y - previousHeadRotation.y;
        previousHeadRotation = GvrVRHelpers.GetHeadRotation().eulerAngles;

        return Quaternion.AngleAxis(angle, Vector3.up) * offset;
    }
}
