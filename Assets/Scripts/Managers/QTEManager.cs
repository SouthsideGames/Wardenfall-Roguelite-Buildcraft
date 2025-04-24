using UnityEngine;
using System;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance;

    public GameObject tapTargetPrefab;
    public Transform canvasTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

   public void StartQTE(int targets, float timePerTarget, Action onComplete, Action onFail)
    {
        int hits = 0;
        int needed = targets;
        bool failed = false;

        for (int i = 0; i < targets; i++)
        {
            Vector2 screenPos = new Vector2(
                UnityEngine.Random.Range(100, Screen.width - 100),
                UnityEngine.Random.Range(100, Screen.height - 100)
            );

            GameObject obj = Instantiate(tapTargetPrefab, canvasTransform);
            obj.transform.position = screenPos;

            obj.GetComponent<QTE_TapTarget>().Init(
                () => {
                    if (failed) return;
                    hits++;
                    if (hits == needed) onComplete?.Invoke();
                },
                () => {
                    if (failed) return;
                    failed = true;
                    onFail?.Invoke();
                    // Optional: clean up remaining targets
                    foreach (var t in GameObject.FindGameObjectsWithTag("QTE"))
                        Destroy(t);
                },
                timePerTarget
            );
        }
    }
}
