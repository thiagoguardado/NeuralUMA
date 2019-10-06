using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{

  public float speed = 10;

  // Update is called once per frame
  void Update()
  {
    RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
  }
}
