using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawnPoint : MonoBehaviour
{
    public Image crossHair;
    public CinemachineVirtualCamera aimVirtualCamera;

    public void AssignCameraAndCrosshair(ref Image crosshair, ref CinemachineVirtualCamera camera)
    {
        crosshair = this.crossHair;
        camera = this.aimVirtualCamera;
    }
}
