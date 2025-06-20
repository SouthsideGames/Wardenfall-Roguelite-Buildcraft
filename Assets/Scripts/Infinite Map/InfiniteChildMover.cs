using UnityEngine;

public class InfiniteChildMover : MonoBehaviour
{
    [Header("ELEMENT")]
    [SerializeField] private Transform playerTransform;

    [Header("SETTINGS")]
    [SerializeField] private float mapChunkSize;
    [SerializeField] private float distanceThreshold = 1.5f;

    // Update is called once per frame
    void Update()
    {
        MoveChildren();
    }
    
    private void MoveChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 distance = playerTransform.position - child.position;
            float calculatedDistanceThreshold = mapChunkSize * distanceThreshold;

            if (Mathf.Abs(distance.x) > calculatedDistanceThreshold)
                child.position += Vector3.right * calculatedDistanceThreshold * 2 * Mathf.Sign(distance.x);
                
            
            if (Mathf.Abs(distance.y) > calculatedDistanceThreshold)
                child.position += Vector3.up * calculatedDistanceThreshold * 2 * Mathf.Sign(distance.y);
        }
    }
}
