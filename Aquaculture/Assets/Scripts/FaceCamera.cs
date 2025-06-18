using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float rotationSpeed = 5f;

    private Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        if (target == null || mainCam == null)
            return;

        // Get the direction from target to camera, but ignore the y-axis (only affect rotation around Y)
        Vector3 directionToCam = mainCam.position - target.position;
        directionToCam.y = 0f; // Ignore Y-axis to prevent tilting

        if (directionToCam.sqrMagnitude > 0.01f) // Ensure there's some distance
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToCam);
            Quaternion finalRotation = Quaternion.Slerp(
                target.rotation,
                lookRotation,
                Time.deltaTime * rotationSpeed
            );

            // Only update Y rotation of target
            target.rotation = Quaternion.Euler(0f, finalRotation.eulerAngles.y, 0f);
        }
    }
}
