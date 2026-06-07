using UnityEngine;
using System.Collections.Generic;


public class CircularOrbs : MonoBehaviour
{
    public List<GameObject> pooledOrbs;
    public GameObject orbToPool;
    public int numberToPool;
    bool playerDetected, orbsActive;
    float orbCooldown = 2.8f;
    void Start()
    {
        // Loop through list of pooled objects,deactivating them and adding them to the list 
        pooledOrbs = new List<GameObject>();
        for (int i = 0; i < numberToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(orbToPool);
            obj.transform.localEulerAngles = new Vector3(0f, i * 22.5f, 0f);
            obj.transform.position = transform.position;
            obj.SetActive(false);
            pooledOrbs.Add(obj);
            obj.transform.SetParent(this.transform); // set as children of Spawn Manager
        }
    }

    void Update()
    {
        if (playerDetected && !orbsActive)
        {
            orbsActive = true;
            OrbsActive();
        }

        if (orbsActive)
        {
            if (orbCooldown > 0f)
            {
                orbCooldown -= Time.deltaTime;
            }
            else
            {
                orbCooldown = 2.8f;
                orbsActive = false;
            }
        }
    }

    public void OrbsActive()
    {
        foreach (GameObject orb in pooledOrbs)
        {
            orb.transform.position = transform.position;
            orb.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerDetected = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
        }
    }
}
