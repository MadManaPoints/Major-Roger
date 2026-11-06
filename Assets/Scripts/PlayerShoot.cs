using UnityEngine;
using System.Collections.Generic;


public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Transform bulletHolder; // Parent for all bullet objects
    public List<GameObject> pooledBullets;
    int numberToPool = 20;
    public List<GameObject> bazookaBullets;
    int bazookaPool = 3;
    float reloadCooldown = 1f;
    float targetTime = 0f;
    float totalTime = 0.15f;
    [SerializeField] Transform gun;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject bazookaBulletPrefab;
    Vector3 centerScreen = new Vector3(0.5f, 0.5f, 0f);
    Ray laser;
    Vector3 shootDirection;
    [SerializeField] GameObject bulletDirection;
    RaycastHit hit = new RaycastHit();
    float raycastDistance = 10f;
    LayerMask layerMask;
    [SerializeField] Vector3 offset; // Offset bullet direction when aim raycast doesn't hit anything


    void Awake()
    {
        layerMask = LayerMask.GetMask("Opp");
    }

    void Start()
    {
        // Loop through list of pooled objects,deactivating them and adding them to the list 
        pooledBullets = new List<GameObject>();
        for (int i = 0; i < numberToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(bulletPrefab);
            obj.SetActive(false);
            pooledBullets.Add(obj);
            obj.transform.SetParent(bulletHolder); // set as children of Gun
        }

        bazookaBullets = new List<GameObject>();
        for (int i = 0; i < bazookaPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(bazookaBulletPrefab);
            obj.SetActive(false);
            bazookaBullets.Add(obj);
            obj.transform.SetParent(bulletHolder);
        }
    }

    void Update()
    {
        // Cast a ray toward center of the screen
        laser = Camera.main.ViewportPointToRay(centerScreen);

        if (Physics.Raycast(laser, out hit, raycastDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            //Vector3 direction = (hit.point - gun.position).normalized;
            //bulletDirection.transform.position = gun.position + direction * 20f;
            bulletDirection.transform.position = hit.point;
        }
        else
        {
            // Position empty gameobject at end of the ray 
            if (PlayerController.Instance.isAiming)
            {
                bulletDirection.transform.position = gun.position + (laser.direction + offset) * 20f;

            }
            else
            {
                bulletDirection.transform.position = gun.position + gun.forward * 20f;
            }

        }
        //Debug.Log(laser.direction);

        // Get direction toward end of ray
        //shootDirection = (testDirection.transform.position - gun.position).normalized;
        //Debug.DrawRay(gun.position, shootDirection * 5f, Color.magenta);
    }

    public void Fire()
    {
        if (targetTime > 0f)
        {
            targetTime -= Time.deltaTime;
        }
        else
        {
            // Create bullet and face it toward direction of ray 
            GetPooledBullet().SetActive(true);

            //GameObject bullet = Instantiate(bulletPrefab, gun.position, Quaternion.identity);
            //bullet.transform.LookAt(bulletDirection.transform);

            targetTime = totalTime;
        }
    }

    public void Bazooka()
    {
        GetPooledBazookaBullet().SetActive(true);
    }

    // Source: https://learn.unity.com/tutorial/introduction-to-object-pooling
    GameObject GetPooledBullet()
    {
        for (int i = 0; i < numberToPool; i++)
        {
            if (!pooledBullets[i].activeInHierarchy)
            {
                pooledBullets[i].transform.position = gun.position;
                pooledBullets[i].transform.LookAt(bulletDirection.transform);
                return pooledBullets[i];
            }
        }
        return null;
    }

    GameObject GetPooledBazookaBullet()
    {
        for (int i = 0; i < bazookaPool; i++)
        {
            if (!bazookaBullets[i].activeInHierarchy)
            {
                bazookaBullets[i].transform.position = gun.position;
                bazookaBullets[i].transform.LookAt(bulletDirection.transform);
                return bazookaBullets[i];
            }
        }
        return null;
    }
}
