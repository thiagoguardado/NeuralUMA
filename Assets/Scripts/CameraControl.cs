using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
  public float speed;
  public Transform objectToRotate;

  void Update()
  {
    if (Input.touches.Length > 0)
    {
      if (Input.GetTouch(0).phase == TouchPhase.Moved)
      {
        objectToRotate.rotation *= Quaternion.AngleAxis(-speed * Input.GetTouch(0).deltaPosition.x * Time.deltaTime, Vector3.up);
      }
    }
  }
}
