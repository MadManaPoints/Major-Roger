using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackMenu : MonoBehaviour
{
    public static AttackMenu Instance;
    [SerializeField] GameObject attackList;
    [Header("Attacks")]
    public bool menuOpen;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    void Update()
    {
        if (menuOpen && !attackList.activeInHierarchy)
        {
            attackList.SetActive(true);
        }
        else if (!menuOpen && attackList.activeInHierarchy)
        {
            attackList.SetActive(false);
        }
    }

    public void ToggleMenu()
    {
        menuOpen = !menuOpen;

        Time.timeScale = menuOpen ? 0.05f : 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
