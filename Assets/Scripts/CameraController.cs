using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance; //THIS

    Transform followTransform;

    float keyboardNavigationSpeed = 0.1f;

    Vector3 newPosition;
    float newZoom;


    float moveTime = 10f;
    float zoomAmount = 1f;
    float maxZoom = 15f;
    float minZoom = 1f;

    void Start()
    {
        instance = this;
        newPosition = transform.position;
        newZoom = 5f;
    }


    // Update is called once per frame
    void Update()
    {
        if (followTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, followTransform.position, Time.deltaTime * moveTime);
        }
        HandleKeyboardInput();
        HandleMouseInput();
        Navigate();
    }

    void StopFollowing() { followTransform = null; }
    public void SetFollowing(Transform transform) { followTransform = transform; }


    void HandleMouseInput()
    {
        //CAMERA ZOOM IN/OUT
        if (Input.GetAxis("Mouse ScrollWheel") < 0) //MOUSE SCROLLING
        {
            newZoom = Camera.main.orthographicSize + zoomAmount;
            if(newZoom < maxZoom && newZoom > minZoom)
            {
                Camera.main.orthographicSize = newZoom;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            newZoom = Camera.main.orthographicSize - zoomAmount;
            if (newZoom < maxZoom && newZoom > minZoom)
            {
                Camera.main.orthographicSize = newZoom;
            }
        }
    }

    void HandleKeyboardInput()
    {

        //CAMERA MOVEMENT WASD + ARROW KEYS
        if (Input.GetKey(KeyCode.UpArrow))
        {
            StopFollowing();
            newPosition += (transform.up * keyboardNavigationSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            StopFollowing();
            newPosition += (transform.up * -keyboardNavigationSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            StopFollowing();
            newPosition += (transform.right * -keyboardNavigationSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            StopFollowing();
            newPosition += (transform.right * keyboardNavigationSpeed);
        }
    }

    void Navigate()
    {
        //SMOOTHING
        if (!followTransform) transform.position = newPosition;

        //cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraTransform.transform.position, Time.deltaTime * moveTime);
    }

}
