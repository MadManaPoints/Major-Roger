using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    float targetTime = 0f;
    float totalTime = 0.25f;
    [SerializeField] Transform gun;
    [SerializeField] GameObject bulletPrefab;
    Vector3 centerScreen = new Vector3(0.5f, 0.5f, 0f);
    Ray laser;
    Vector3 shootDirection;
    [SerializeField] GameObject testDirection;

    void Start()
    {
    }

    void Update()
    {
        // Cast a ray toward center of the screen
        laser = Camera.main.ViewportPointToRay(centerScreen);

        // Position empty gameobject at end of the ray 
        testDirection.transform.position = gun.position + laser.direction * 20f;
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
            GameObject bullet = Instantiate(bulletPrefab, gun.position, Quaternion.identity);
            bullet.transform.LookAt(testDirection.transform);
            targetTime = totalTime;
        }
    }
}
