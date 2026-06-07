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
            col.gameObject.GetComponent<PlayerController>().TakeDamage();
            gameObject.SetActive(false);
        }
    }
}
