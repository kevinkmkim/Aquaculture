using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class ProximityCanvasFader : MonoBehaviour
{
    [SerializeField]
    private float activationDistance = 1f;

    [SerializeField]
    private float fadeSpeed = 2f;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform camTransform;

    private bool isActive = false;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        camTransform = GetActiveCamera()?.transform;

        canvas.enabled = false;
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, camTransform.position);
        bool shouldBeActive = distance <= activationDistance;

        if (shouldBeActive && !isActive)
        {
            canvas.enabled = true;
            isActive = true;
        }
        else if (!shouldBeActive && isActive && canvasGroup.alpha <= 0.01f)
        {
            canvas.enabled = false;
            isActive = false;
        }

        float targetAlpha = shouldBeActive ? 1f : 0f;
        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            targetAlpha,
            Time.deltaTime * fadeSpeed
        );
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
