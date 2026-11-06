using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AttackMenu : MonoBehaviour
{
    public static AttackMenu Instance;
    PlayerController player;
    [SerializeField] GameObject attackList;
    [Header("Attacks")]
    public bool menuOpen;
    int selectionIndex = 0;
    public List<TextMeshProUGUI> abilities = new List<TextMeshProUGUI>();

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        player = PlayerController.Instance;
    }

    void Update()
    {
        if (!menuOpen) return;

        UpdateTextColor();
    }

    public void ToggleMenu()
    {
        menuOpen = !menuOpen;

        Time.timeScale = menuOpen ? 0.05f : 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (menuOpen)
        {
            selectionIndex = 0;
            attackList.SetActive(true);
        }
        else
        {
            attackList.SetActive(false);
        }
    }

    public void ConfirmSelection()
    {
        if (selectionIndex == 0) player.SpecialAttack();
        ToggleMenu();
    }

    public void UpdateSelection(int requestedMove)
    {
        int newIndex = selectionIndex + requestedMove;
        if (newIndex < 0 || newIndex > abilities.Count - 1) return;
        else selectionIndex = newIndex;
    }
    void UpdateTextColor()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (i == selectionIndex && abilities[i].color != Color.white) abilities[i].color = Color.white;
            else if (i != selectionIndex && abilities[i].color != Color.black) abilities[i].color = Color.black;
        }
    }
}
