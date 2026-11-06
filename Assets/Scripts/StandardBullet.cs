using UnityEngine;

public class StandardBullet : MonoBehaviour
{
    float moveSpeed = 30f;
    float lifespan = 3f;

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;

        if (lifespan > 0f)
        {
            lifespan -= Time.deltaTime;
        }
        else
        {
            lifespan = 3f;
            gameObject.SetActive(false);
        }
    }

    public void BulletHit()
    {
        lifespan = 3f;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Opp"))
        {
            PlayerController.Instance.UpdateATB(8f);
        }
    }
}
