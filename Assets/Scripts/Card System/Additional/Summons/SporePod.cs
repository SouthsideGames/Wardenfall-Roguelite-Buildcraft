using System.Collections;
using UnityEngine;

public class SporePod : MonoBehaviour
{
    [SerializeField] private GameObject gasCloudPrefab;
    [SerializeField] private float releaseInterval = 3f;
    [SerializeField] private float effectRadius = 2f;

    private float activeDuration;

    public void Initialize(float duration)
    {
        activeDuration = duration;
        StartCoroutine(ReleaseGasRoutine());
        Destroy(gameObject, activeDuration);
    }

    private IEnumerator ReleaseGasRoutine()
    {
        float elapsed = 0f;

        while (elapsed < activeDuration)
        {
            SpawnGasCloud();
            yield return new WaitForSeconds(releaseInterval);
            elapsed += releaseInterval;
        }
    }

    private void SpawnGasCloud()
    {
        Instantiate(gasCloudPrefab, transform.position, Quaternion.identity);
    }
}
