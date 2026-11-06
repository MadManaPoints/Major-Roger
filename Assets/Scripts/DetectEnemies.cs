using UnityEngine;
using System.Collections.Generic;

public class DetectEnemies : MonoBehaviour
{
    public List<Transform> enemies = new List<Transform>();
    void Start()
    {

    }

    void Update()
    {

    }

    // Get nearest enemy, courtesty of: https://discussions.unity.com/t/clean-est-way-to-find-nearest-object-of-many-c/409917
    Transform ClosestOpp(Transform[] opps)
    {
        Transform min = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Transform opp in opps)
        {
            float dist = Vector3.Distance(opp.position, currentPos);
            if (dist < minDist)
            {
                min = opp;
                minDist = dist;
            }
        }

        return min;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Opp") && !col.isTrigger)
        {
            enemies.Add(col.transform);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.transform.CompareTag("Opp") && !col.isTrigger)
        {
            enemies.Remove(col.transform);
        }
    }
}
