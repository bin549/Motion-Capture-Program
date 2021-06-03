using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CameraController : MonoBehaviour
{
    public AppSettings appSettings;
    public Avatar avatar;
    public Transform avatarTransform;
    private Vector3 offsetPosition;
    private bool isRotating = false;
    public float distance = 7;
    public float scrollSpeed = 30;
    public float rotateSpeed = 30;

    private void Awake()
    {
        appSettings = GameObject.FindObjectOfType<AppSettings>();
    }

    private void Start()
    {
        avatar = appSettings.GetAvatar();
        avatarTransform = avatar.gameObject.transform;
        transform.LookAt(avatarTransform.position);
        offsetPosition = transform.position - avatarTransform.position;
    } 

    private void Update()
    {
        transform.position = offsetPosition + avatarTransform.position;
        RotateView();
        ScrollViewMouse();
        ScrollViewArrow();
    }

    private void ScrollViewArrow()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(avatarTransform.transform.position, Vector3.up, rotateSpeed * 5 * Time.deltaTime);
            offsetPosition = transform.position - avatarTransform.transform.position;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(avatarTransform.transform.position, Vector3.up, -rotateSpeed * 5 * Time.deltaTime);
            offsetPosition = transform.position - avatarTransform.transform.position;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            distance = offsetPosition.magnitude;
            distance += (float)0.2 * -scrollSpeed;
            distance = Mathf.Clamp(distance, 1, 10);
            offsetPosition = offsetPosition.normalized * distance;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            distance = offsetPosition.magnitude;
            distance += (float)0.2 * scrollSpeed;
            distance = Mathf.Clamp(distance, 1, 10);
            offsetPosition = offsetPosition.normalized * distance;
        }
    }

    private void ScrollViewMouse()
    {
        distance = offsetPosition.magnitude;
        distance += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;
        distance = Mathf.Clamp(distance, 5, 10);
        offsetPosition = offsetPosition.normalized * distance;
    }

    private void RotateView()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            transform.RotateAround(avatarTransform.position, avatarTransform.up, rotateSpeed * Input.GetAxis("Mouse X"));
            Vector3 originalPos = transform.position;
            Quaternion originalRotation = transform.rotation;
            //transform.RotateAround(avatarTransform.position, avatarTransform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));
            float x = transform.eulerAngles.x;
            if (x < 10 || x > 80)
            {
                transform.position = originalPos;
                transform.rotation = originalRotation;
            }
        }
        offsetPosition = transform.position - avatarTransform.position;
    }
}
