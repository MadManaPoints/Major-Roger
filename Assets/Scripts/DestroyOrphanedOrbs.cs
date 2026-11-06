using UnityEngine;

public class DestroyOrphanedOrbs : MonoBehaviour
{
    float targetTime = 2.5f;
    void Update()
    {
        if (targetTime > 0f)
        {
            if (transform.parent == null)
                targetTime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
