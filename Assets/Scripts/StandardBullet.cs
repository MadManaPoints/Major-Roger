using UnityEngine;

public class StandardBullet : MonoBehaviour
{
    float moveSpeed = 12f;
    float lifespan = 5f;

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }
}
