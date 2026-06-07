using UnityEngine;

public class WheelVisualSync : MonoBehaviour
{
    public WheelCollider targetWheelCollider;
    public Transform visualWheelMesh;

    void Update()
    {
        Vector3 position;
        Quaternion rotation;
        targetWheelCollider.GetWorldPose(out position, out rotation);

        visualWheelMesh.position = position;
        visualWheelMesh.rotation = rotation;
    }
}