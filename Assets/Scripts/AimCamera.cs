using UnityEngine;

public class AimCamera : MonoBehaviour
{
    public static AimCamera Instance;
    [SerializeField] float xRotation;
    [SerializeField] float yRotation;
    [SerializeField] float rotationClamp = 30.0f;
    [SerializeField] float sens = 100f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    public void AimCameraControls(Vector2 aim)
    {
        // Get right stick/mouse movement on x and y axis
        float x = aim.x * Time.deltaTime * sens;
        float y = aim.y * Time.deltaTime * sens;

        yRotation += x;
        xRotation -= y;

        //Ensures the player can't look 360 degrees up and down
        xRotation = Mathf.Clamp(xRotation, -rotationClamp, rotationClamp);

        //rotate cam and player
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    public void FollowCameraControls()
    {
        // Follow camera to stop disorienting jump when cam switching 
        transform.rotation = Camera.main.transform.rotation;

        // Update rotation variables used while aiming to match current position
        xRotation = Camera.main.transform.localEulerAngles.x;
        yRotation = Camera.main.transform.localEulerAngles.y;
    }
}
