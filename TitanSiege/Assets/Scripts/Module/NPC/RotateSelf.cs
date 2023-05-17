using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public bool IsCanRotate;
    private float xSpeed;
    private float x;
    // Start is called before the first frame update
    void Start()
    {
        IsCanRotate = true;
        xSpeed = 120f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1) && IsCanRotate) {
            //设置横向旋转距离
            x = Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            //设置纵向旋转距离
            //y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            transform.RotateAround(transform.position, Vector3.down, x);
        }

    }
   
    void OnEnable() {
        transform.localRotation = Quaternion.identity;
    }
}
