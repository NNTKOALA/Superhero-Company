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

    // Start is called before the first frame update
    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
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
    }

    // Rotate camera with right-mouse drag
    void RotateCamera()
    {
        if (Input.GetMouseButton(1))
        {
            transform.eulerAngles += camRotateSpeed * new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        }
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
}
