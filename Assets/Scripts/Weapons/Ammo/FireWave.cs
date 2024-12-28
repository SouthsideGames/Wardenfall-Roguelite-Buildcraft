using System.Collections;
using UnityEngine;

public class FireWave : MonoBehaviour
{
    private float speed = 10f;
    private float range = 8f;
    private float burnDuration = 3f;
    private float burnInterval = 1f;
    private int burnDamage = 5;

    private Vector2 startPosition;

    [SerializeField] private FireParticleDamage particleDamage; 

    public void Setup(float _speed, float _range, float _burnDamage, float _burnDuration, float _burnInterval, float _width)
    {
        speed = _speed;
        range = _range;
        burnDamage = Mathf.RoundToInt(_burnDamage);
        burnDuration = _burnDuration;
        burnInterval = _burnInterval;

        startPosition = transform.position;
        transform.localScale = new Vector3(_width, 1, 1);

        // Pass burn settings to ParticleDamage
        if (particleDamage != null)
            particleDamage.SetupBurn(burnDamage, burnDuration, burnInterval);

        Destroy(gameObject, range / speed);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        if (Vector2.Distance(startPosition, transform.position) >= range)
            Destroy(gameObject);
    }
}
