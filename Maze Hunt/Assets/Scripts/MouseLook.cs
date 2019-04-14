using UnityEngine;

// Enables Maze Hunt to be played in the Unity editor. Make sure that the Mouse Look (Script) component is enabled in the Unity editor under Player/OVRCameraRig under the hierarchy tab.
// This script component should be disabled when testing on the Samsung Gear VR touchpad inputs are treated as mouse inputs
// If you want to view the maze from the Scene view while playing, press escape and drag the Game window out of the way. Turn off fog in the Lighting Tab (Window/Rendering/Lighting Settings) under Other Settings.
// Left click on the mouse to move in the direction you're looking
public class MouseLook : MonoBehaviour
{

    Vector2 mouseLook;
    Vector2 smoothVector;
    public float sensitivity = 1.0f;
    public float smoothing = 2.0f;
    private int minY = -70;
    private int maxY = 80;
    GameObject playerCamera;

    // Lock the cursor in the game window. Press esc to leave. Set the player camera to the gameobject the script is attached to.
	void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = this.transform.parent.gameObject;	
	}

    // Get and scale mouse input and update the player camera appropriately
    void Update()
    {
        // Get mouse input and scale it to the direction it moves
        Vector2 mouseDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDirection = Vector2.Scale(mouseDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        // Use Linear interpolation to smooth mouse movement
        smoothVector.x = Mathf.Lerp(smoothVector.x, mouseDirection.x, 1f / smoothing);
        smoothVector.y = Mathf.Lerp(smoothVector.y, mouseDirection.y, 1f / smoothing);
        mouseLook += smoothVector;

        // Restricts where the player can look so they cannt flip the camera the wrong way around.  
        mouseLook.y = Mathf.Clamp(mouseLook.y, minY, maxY); 

        // Change the player camera rotation to match the new values
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        playerCamera.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, playerCamera.transform.up);
    }
}
