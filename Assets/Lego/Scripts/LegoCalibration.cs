using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoCalibration : LegoBase
{
	void Update()
	{
		if(Input.GetKey(KeyCode.UpArrow))
      MoveUpNeck();
  }

  void MoveUpNeck()
  {
    cameraAngle += 10;
    KinectWrapper.NuiCameraElevationSetAngle(cameraAngle);
    Debug.Log(cameraAngle);
  }
}
