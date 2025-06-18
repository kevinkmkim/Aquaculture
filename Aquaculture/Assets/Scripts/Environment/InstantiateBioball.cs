using UnityEngine;

public class InstantiateBioball : MonoBehaviour
{
    [Header("Bioball Settings")]
    public GameObject bioballPrefab;
    public int count = 100;
    public float radius = 0.5f;

    [Header("Y Offset Settings")]
    public float height = 0.05f;
    public float yOffsetRange = 0.05f;

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 localOffset = GenerateRandomLocalOffset();
            Vector3 worldPosition = transform.TransformPoint(localOffset);

            Instantiate(
                bioballPrefab,
                worldPosition,
                Quaternion.Euler(0, Random.Range(0f, 360f), 0),
                transform
            );
        }
    }

    Vector3 GenerateRandomLocalOffset()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        float x = distance * Mathf.Cos(angle);
        float z = distance * Mathf.Sin(angle);
        float y = height + Random.Range(-yOffsetRange, yOffsetRange);

        return new Vector3(x, y, z);
    }
}
