using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerATB : MonoBehaviour
{
    [SerializeField] List<Image> ATB = new List<Image>();
    RectTransform[] rt = new RectTransform[3];
    float currentProgress = 0f;
    float maxProgress = 300f;
    void Start()
    {
        for (int i = 0; i < ATB.Count; i++)
        {
            rt[i] = ATB[i].rectTransform;
            rt[i].sizeDelta = new Vector2(0f, rt[i].sizeDelta.y);
        }
    }

    void Update()
    {
        if (currentProgress < maxProgress)
        {
            currentProgress += Time.deltaTime * 5f;
        }
        else
        {
            if (currentProgress != maxProgress) currentProgress = maxProgress;
        }

        float clampedProgress1 = Mathf.Clamp(currentProgress, 0f, 100f);
        float clampedProgress2 = Mathf.Clamp(currentProgress, 100f, 200f);
        float clampedProgress3 = Mathf.Clamp(currentProgress, 200f, 300f);

        float ATB1 = Map(clampedProgress1, 0f, 100f, 0f, 90f);
        float ATB2 = Map(clampedProgress2, 100f, 200f, 0f, 90f);
        float ATB3 = Map(clampedProgress3, 200f, 300f, 0f, 90f);

        rt[0].sizeDelta = new Vector2(ATB1, rt[0].sizeDelta.y);
        rt[1].sizeDelta = new Vector2(ATB2, rt[1].sizeDelta.y);
        rt[2].sizeDelta = new Vector2(ATB3, rt[2].sizeDelta.y);

        if (Input.GetKeyDown(KeyCode.T)) AbilityTest();
    }

    void AbilityTest()
    {
        if (currentProgress >= 100f)
        {
            currentProgress -= 100f;
        }
        else
        {
            currentProgress = 0f;
        }
    }

    float Map(float value, float minA, float maxA, float minB, float maxB)
    {
        float range = maxA - minA;
        float valuePercent = (value - minA) / range;

        float newRange = maxB - minB;

        return valuePercent * newRange + minB;
    }
}
