using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationX = 0F;
    float rotationY = 0F;
    Quaternion originalRotation;
    void Update()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }

        MoveCamera();

        if (Input.GetMouseButtonDown(0))
        {     
            //transform.Translate(0.0f, 1.0f, 0.0f, Space.Self);
        }
    }

    void Start()
    {
        // Make the rigid body not change rotation
        //if (rigidbody)
         //   rigidbody.freezeRotation = true;
        originalRotation = transform.localRotation;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
         angle += 360F;
        if (angle > 360F)
         angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public float ScrollSpeed = 1.0f;
    public float ScrollWidth = 1.0f;
    public float MaxCameraHeight = 5.0f;
    public float MinCameraHeight = -5.0f;

    private void MoveCamera()
    {
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);
        
        //Move the GameObject
        if (Input.GetKey("a"))
        {
            movement.x -= ScrollSpeed;
        }
        if (Input.GetKey("s"))
        {
            movement.z -= ScrollSpeed;

        }
        if (Input.GetKey("d"))
        {
            movement.x += ScrollSpeed;
        }
        if (Input.GetKey("w"))
        {

            movement.z += ScrollSpeed;
        }

        //horizontal camera movement
        if (xpos >= 0 && xpos < ScrollWidth)
        {
            movement.x -= ScrollSpeed;
        }
        else if (xpos <= Screen.width && xpos > Screen.width - ScrollWidth)
        {
            movement.x += ScrollSpeed;
        }

        //vertical camera movement
        if (ypos >= 0 && ypos < ScrollWidth)
        {
            movement.z -= ScrollSpeed;
        }
        else if (ypos <= Screen.height && ypos > Screen.height - ScrollWidth)
        {
            movement.z += ScrollSpeed;
        }

        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;
        //away from ground movement
        movement.y -= ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        //calculate desired camera position based on received input
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        if (destination.y > MaxCameraHeight)
        {
            destination.y = MaxCameraHeight;
        }
        else if (destination.y < MinCameraHeight)
        {
            destination.y = MinCameraHeight;
        }

        //if a change in position is detected perform the necessary update
        if (destination != origin)
        {
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ScrollSpeed);
        }
    }
}
