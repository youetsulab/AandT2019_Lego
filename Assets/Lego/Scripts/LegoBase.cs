using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LegoColor
{
  White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange
}
public struct RawLegoPixelInfo
{
  ushort depth;
  Color color;
}

public struct LandscapeCellInfo
{
  int index;
  LegoColor legoColor;
}

public class LegoBase : MonoBehaviour
{
  protected static readonly int DEPTH_CAMERA_WIDTH = 640;
  protected static readonly int DEPTH_CAMERA_HEIGHT = 480;
  protected static readonly int LANDSCAPE_MAP_HEIGHT = 32;
  protected static readonly int LANDSCAPE_MAP_WIDTH = 32;
  protected static readonly int CALIBRATION_DEPTH = 100;
  protected static readonly int NUM_CALIBRATION_POINT = 4;
  protected bool isCalibrated = false;
  protected int cameraAngle;

  #region Accessor
  public bool IsCalibrated {
		get{ return isCalibrated; }
	}
  #endregion

  protected void Start()
	{
		KinectWrapper.NuiCameraElevationGetAngle(out cameraAngle);
    Debug.Log("Camera Angle :" + cameraAngle);

		if(!isCalibrated){

    }
  }

	void Update()
	{

	}
}
