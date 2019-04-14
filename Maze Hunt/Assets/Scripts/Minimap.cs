using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform OVRCameraRig;
    public GameObject minimap;
    private Vector3 minimapCameraPosition;
    private Vector3 minimapPosition;
    private Quaternion minimapRotation;


    void LateUpdate ()
    {
        minimapCameraPosition = OVRCameraRig.position;
        minimapCameraPosition.y = transform.position.y;
        transform.position = minimapCameraPosition;

        minimapPosition = new Vector3(OVRCameraRig.transform.position.x, OVRCameraRig.transform.position.y, OVRCameraRig.transform.position.z);
        minimap.transform.position = minimapPosition + OVRCameraRig.transform.forward;
        //minimapBorder.transform.position = minimapPosition + player.transform.forward;

        minimapRotation = OVRCameraRig.transform.rotation;
        minimap.transform.rotation = minimapRotation;
        //minimapBorder.transform.rotation = minimapRotation;
    }
}
