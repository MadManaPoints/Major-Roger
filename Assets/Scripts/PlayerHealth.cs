using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
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

    }

    public void TakeDamage(int amount)
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
}
