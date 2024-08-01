using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera _virtualCamera;
    public float camRotateSpeed = 5f;
    public float camZoomSpeed = 15f;
    public float minZoom = 20f;
    public float maxZoom = 60f;
    public bool drag = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 lastMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        initialPosition = _virtualCamera.transform.position;
        initialRotation = _virtualCamera.transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.chatBox.isFocused)
        {
            return;
        }

        RotateCamera();
        ZoomCamera();
        ResetCameraRotation();
    }

    // Rotate camera with right-mouse drag
    void RotateCamera()
    {
        if (InputManager.Instance.IsCamRotate())
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float rotationX = Mathf.Abs(mouseDelta.x) * camRotateSpeed * Time.deltaTime;
            float rotationY = -mouseDelta.y * camRotateSpeed * Time.deltaTime;

            if (mouseDelta.x < 0)
            {
                _virtualCamera.transform.Rotate(Vector3.down, rotationX, Space.World);
            }
            else
            {
                _virtualCamera.transform.Rotate(Vector3.up, rotationX, Space.World);
            }

            float newXRotation = _virtualCamera.transform.localEulerAngles.x + rotationY;

            newXRotation = Mathf.Clamp(newXRotation, 0f, 80f);

            _virtualCamera.transform.localEulerAngles = new Vector3(newXRotation, _virtualCamera.transform.localEulerAngles.y, 0);
        }
        lastMousePosition = Input.mousePosition;
    }

    //Zoom in/out with mouse-scoll wheel
    void ZoomCamera()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (_virtualCamera != null)
        {
            if (_virtualCamera.m_Lens.Orthographic)
            {
                float newZoom = _virtualCamera.m_Lens.OrthographicSize - scrollInput * camZoomSpeed;
                newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
                _virtualCamera.m_Lens.OrthographicSize = newZoom;
            }
            else
            {
                float newZoom = _virtualCamera.m_Lens.FieldOfView - scrollInput * camZoomSpeed;
                newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
                _virtualCamera.m_Lens.FieldOfView = newZoom;
            }
        }
    }

    //Reset camera rotation with R
    void ResetCameraRotation()
    {
        if (InputManager.Instance.IsResetCamPos())
        {
            _virtualCamera.transform.position = initialPosition;
            _virtualCamera.transform.rotation = initialRotation;
        }
    }
}
