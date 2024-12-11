using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float speed = 25f;
    public float borderThickness = 5f;
    public float scrollSpeed = 300f;
    PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (PV.IsMine)
        {
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >=
            Screen.height - borderThickness)
        {
            pos.z += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= borderThickness)
        {
            pos.z -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >=
            Screen.width - borderThickness)
        {
            pos.x += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= borderThickness)
        {
            pos.x -= speed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * 100f * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, 30, 120);
        pos.x = Mathf.Clamp(pos.x, -380, 380);
        pos.z = Mathf.Clamp(pos.z, -540, 300);

        transform.position = pos;
    }
}
