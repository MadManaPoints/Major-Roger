using UnityEngine;

public class ForwardOrb : MonoBehaviour
{
    float moveSpeed = 6f;
    float lifespan = 2.5f;

    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * moveSpeed);

        if (lifespan > 0f)
        {
            lifespan -= Time.deltaTime;
        }
        else
        {
            lifespan = 2.5f;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController player = col.gameObject.GetComponent<PlayerController>();
            if (player.canTakeDamage) player.TakeDamage(50);
            lifespan = 2.5f;
            gameObject.SetActive(false);
        }
    }
}
