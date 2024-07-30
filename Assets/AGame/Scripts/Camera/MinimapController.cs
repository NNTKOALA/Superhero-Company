using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] public Transform player;
    Camera _topDownCam;

    // Start is called before the first frame update
    void Start()
    {
        _topDownCam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var position = player.position;
        _topDownCam.transform.position = new Vector3(position.x, _topDownCam.transform.position.y, position.z);
    }
}
