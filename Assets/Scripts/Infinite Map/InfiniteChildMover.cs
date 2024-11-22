 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteChildMover : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform playerTransform;

    [Header("SETTINGS:")]
    [SerializeField] private float mapChunkSize;
    [SerializeField] private float distanceThreshold = 1.5f;


    // Update is called once per frame
    void Update()
    {
        if(CameraManager.Instance.UseInfiniteMap)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                Vector3 distance = playerTransform.position - child.position;   
                float calculateDistanceThreshold = mapChunkSize * distanceThreshold;

                if(Mathf.Abs(distance.x) > calculateDistanceThreshold)
                child.position += Vector3.right * calculateDistanceThreshold * 2 * Mathf.Sign(distance.x);   

                if(Mathf.Abs(distance.y) > calculateDistanceThreshold)
                child.position += Vector3.up * calculateDistanceThreshold * 2 * Mathf.Sign(distance.y);   
            }    
        }
      
    }
}
