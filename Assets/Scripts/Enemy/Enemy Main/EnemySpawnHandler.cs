using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemySpawnHandler : MonoBehaviour
{
    private Enemy enemy;
    private EnemyMovement movement;
    private float spawnSize = 1.2f;
    private float spawnTime = 0.3f;
    private int numberOfLoops = 4;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        movement = GetComponent<EnemyMovement>();


    }

    public void SetSpawnValues(float size, float time, int loops)
    {
        spawnSize = size;
        spawnTime = time;
        numberOfLoops = loops;
    }

    public void BeginSpawn()
    {
        SetRenderersVisibility(false);
        movement.canMove = false;

        Vector3 targetScale = enemy.SpawnIndicator.transform.localScale * spawnSize;
        LeanTween.scale(enemy.SpawnIndicator.gameObject, targetScale, spawnTime)
            .setLoopPingPong(numberOfLoops)
            .setOnComplete(SpawnCompleted);
    }

    private void SetRenderersVisibility(bool visible)
    {
        enemy.SpriteRenderer.enabled = visible;
        enemy.SpawnIndicator.enabled = !visible;
    }

    private void SpawnCompleted()
    {
        SetRenderersVisibility(true);
        enemy.MarkAsSpawned();
        enemy.Collider.enabled = true;

        movement.StorePlayer(enemy.Character);
        movement.EnableMovement();
        enemy.OnSpawnCompleted?.Invoke();

        enemy.StartCoroutine(ApplyTraitsNextFrame());
    }

    private IEnumerator ApplyTraitsNextFrame()
    {
        yield return null;
        enemy.modifierHandler?.ApplyTraits();
    }
}
