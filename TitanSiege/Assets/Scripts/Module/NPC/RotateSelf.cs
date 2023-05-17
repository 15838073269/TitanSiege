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
            //���ú�����ת����
            x = Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            //����������ת����
            //y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            transform.RotateAround(transform.position, Vector3.down, x);
        }

    }
   
    void OnEnable() {
        transform.localRotation = Quaternion.identity;
    }
}
