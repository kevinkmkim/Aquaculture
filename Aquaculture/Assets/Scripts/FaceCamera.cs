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
        mainCam = GetActiveCamera()?.transform;
    }

    void Update()
    {
        if (target == null || mainCam == null)
            return;

        Vector3 directionToCam = mainCam.position - target.position;
        directionToCam.y = 0f;

        if (directionToCam.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToCam);
            Quaternion finalRotation = Quaternion.Slerp(
                target.rotation,
                lookRotation,
                Time.deltaTime * rotationSpeed
            );

            target.rotation = Quaternion.Euler(0f, finalRotation.eulerAngles.y, 0f);
        }
    }

    private Camera GetActiveCamera()
    {
        Camera[] cameras = Camera.allCameras;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].enabled && cameras[i].gameObject.activeInHierarchy)
            {
                return cameras[i];
            }
        }
        return null;
    }
}
