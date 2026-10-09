using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [Header("Cameras")]
    public CinemachineCamera
        followCam,
        targetLockCam,
        shoulderCam;
    public AimCamera aimCam;
    public List<CinemachineCamera> allCams = new List<CinemachineCamera>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        //UpdateInactiveCameras();
    }

    void UpdateInactiveCameras()
    {
        foreach (CinemachineCamera cam in allCams)
        {
            if (cam.Priority < 2)
            {
                cam.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}

