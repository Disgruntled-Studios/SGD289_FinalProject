using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraInfo
{
    public CinemachineCamera VCam { get; private set; }
    public bool IsCCTV { get; private set; }
    public Volume CCTVVolume { get; private set; }

    public CameraInfo(CinemachineCamera vCam, bool isCCTV, Volume cctvVolume = null)
    {
        VCam = vCam;
        IsCCTV = isCCTV;
        CCTVVolume = cctvVolume;
    }
}
