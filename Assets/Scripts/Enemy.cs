using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject healthBarObj;
    [SerializeField] Image healthBar;
    RectTransform rt;
    int maxHealth = 290;
    int currentHealth = 290;

    void Start()
    {
        rt = healthBar.rectTransform;
        rt.sizeDelta = new Vector2(currentHealth, rt.sizeDelta.y);
    }

    void Update()
    {
        if (currentHealth == 0f)
        {
            healthBarObj.SetActive(false); // Turn off health bar
            transform.GetChild(0).SetParent(null); // Deparent orbs so current attack finishes before being destroyed
            Destroy(gameObject); // Die die die
        }
    }

    void TakeDamage(int amount)
    {
        if (currentHealth - amount > 0)
        {
            currentHealth -= amount;
        }
        else
        {
            currentHealth = 0;
        }

        rt.sizeDelta = new Vector2(currentHealth, rt.sizeDelta.y);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Bullet"))
        {
            col.gameObject.GetComponent<StandardBullet>().BulletHit();
            TakeDamage(1);
        }

        if (col.transform.CompareTag("Bazooka"))
        {
            col.gameObject.GetComponent<StandardBullet>().BulletHit();
            TakeDamage(150);
        }
    }
}
