using System;
using System.Collections;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScreenshotCapture : MonoBehaviour
{
    [Header("Screenshot Settings")]
    public KeyCode screenshotKey = KeyCode.Space;
    public int superSize = 1;
    public bool includeUI = true;
    public string folderName = "Screenshots";

    [Header("File Settings")]
    public string filePrefix = "Screenshot";
    public ImageFormat imageFormat = ImageFormat.PNG;
    public bool addTimestamp = true;
    public bool addResolution = true;

    [Header("Camera Settings")]
    public Camera targetCamera;
    public bool useSpecificCamera = false;

    [Header("Advanced Settings")]
    public bool showCaptureNotification = true;
    public float notificationDuration = 2f;
    public bool playSound = false;
    public AudioClip captureSound;

    [Header("Burst Mode")]
    public bool enableBurstMode = false;
    public KeyCode burstModeKey = KeyCode.F11;
    public int burstCount = 5;
    public float burstInterval = 0.5f;

    private string savePath;
    private AudioSource audioSource;
    private bool isTakingBurst = false;

    public enum ImageFormat
    {
        PNG,
        JPG
    }

    void Start()
    {
#if UNITY_EDITOR
        savePath = Path.Combine(Application.dataPath, folderName);
#else
        savePath = Path.Combine(Application.persistentDataPath, folderName);
#endif
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        if (playSound && captureSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = captureSound;
            audioSource.playOnAwake = false;
        }

        if (targetCamera == null)
        {
            targetCamera = GetActiveCamera();
            if (targetCamera == null)
                targetCamera = FindObjectOfType<Camera>();
        }

        Debug.Log($"Screenshots will be saved to: {savePath}");
    }

    void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            TakeScreenshot();
        }

        if (enableBurstMode && Input.GetKeyDown(burstModeKey) && !isTakingBurst)
        {
            StartCoroutine(TakeBurstScreenshots());
        }
    }

    public void TakeScreenshot()
    {
        if (useSpecificCamera && targetCamera != null)
        {
            StartCoroutine(CaptureFromCamera());
        }
        else
        {
            StartCoroutine(CaptureFullScreen());
        }
    }

    IEnumerator CaptureFullScreen()
    {
        yield return new WaitForEndOfFrame();

        string filename = GenerateFilename();
        string fullPath = Path.Combine(savePath, filename);

        try
        {
            ScreenCapture.CaptureScreenshot(fullPath, superSize);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            if (showCaptureNotification)
            {
                StartCoroutine(ShowCaptureNotification($"Screenshot saved: {filename}"));
            }

            if (playSound && audioSource != null)
            {
                audioSource.Play();
            }

            Debug.Log($"Screenshot saved to: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save screenshot: {e.Message}");
        }
    }

    IEnumerator CaptureFromCamera()
    {
        if (targetCamera == null)
        {
            Debug.LogError("Target camera is null!");
            yield break;
        }

        yield return new WaitForEndOfFrame();

        int width = Screen.width * superSize;
        int height = Screen.height * superSize;

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        RenderTexture currentRT = targetCamera.targetTexture;

        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        targetCamera.targetTexture = currentRT;
        RenderTexture.active = null;

        string filename = GenerateFilename();
        string fullPath = Path.Combine(savePath, filename);

        try
        {
            byte[] data = GetImageData(screenshot);
            File.WriteAllBytes(fullPath, data);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            if (showCaptureNotification)
            {
                StartCoroutine(ShowCaptureNotification($"Screenshot saved: {filename}"));
            }

            if (playSound && audioSource != null)
            {
                audioSource.Play();
            }

            Debug.Log($"Camera screenshot saved to: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save camera screenshot: {e.Message}");
        }

        DestroyImmediate(screenshot);
        renderTexture.Release();
    }

    IEnumerator TakeBurstScreenshots()
    {
        isTakingBurst = true;

        if (showCaptureNotification)
        {
            StartCoroutine(ShowCaptureNotification($"Taking {burstCount} burst screenshots..."));
        }

        for (int i = 0; i < burstCount; i++)
        {
            if (useSpecificCamera && targetCamera != null)
            {
                yield return StartCoroutine(CaptureFromCamera());
            }
            else
            {
                yield return StartCoroutine(CaptureFullScreen());
            }

            if (i < burstCount - 1)
            {
                yield return new WaitForSeconds(burstInterval);
            }
        }

        if (showCaptureNotification)
        {
            StartCoroutine(
                ShowCaptureNotification($"Burst complete! {burstCount} screenshots saved.")
            );
        }

        isTakingBurst = false;
    }

    string GenerateFilename()
    {
        string filename = filePrefix;

        if (addTimestamp)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            filename += "_" + timestamp;
        }

        if (addResolution)
        {
            int width = Screen.width * superSize;
            int height = Screen.height * superSize;
            filename += $"_{width}x{height}";
        }

        filename += imageFormat == ImageFormat.PNG ? ".png" : ".jpg";

        return filename;
    }

    byte[] GetImageData(Texture2D texture)
    {
        switch (imageFormat)
        {
            case ImageFormat.PNG:
                return texture.EncodeToPNG();
            case ImageFormat.JPG:
                return texture.EncodeToJPG(85);
            default:
                return texture.EncodeToPNG();
        }
    }

    IEnumerator ShowCaptureNotification(string message)
    {
        Debug.Log($"[SCREENSHOT] {message}");
        yield return new WaitForSeconds(notificationDuration);
    }

    public void TakeScreenshotPublic()
    {
        TakeScreenshot();
    }

    public void SetSuperSize(int size)
    {
        superSize = Mathf.Max(1, size);
    }

    public void SetImageFormat(ImageFormat format)
    {
        imageFormat = format;
    }

    public string GetSavePath()
    {
        return savePath;
    }

    void OnValidate()
    {
        superSize = Mathf.Max(1, superSize);
        burstCount = Mathf.Max(1, burstCount);
        burstInterval = Mathf.Max(0.1f, burstInterval);
        notificationDuration = Mathf.Max(0.5f, notificationDuration);
    }

    void OnGUI()
    {
        if (showCaptureNotification && Event.current.type == EventType.Repaint)
        {
            string instructions = $"Press {screenshotKey} for screenshot";
            if (enableBurstMode)
            {
                instructions += $" | {burstModeKey} for burst mode";
            }

            GUI.color = new Color(1, 1, 1, 0.7f);
            GUI.Label(new Rect(10, 10, 400, 20), instructions);
            GUI.color = Color.white;
        }
    }

    private Camera GetActiveCamera()
    {
        Camera[] cameras = Camera.allCameras;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].enabled && cameras[i].gameObject.activeInHierarchy)
                return cameras[i];
        }
        return null;
    }
}
