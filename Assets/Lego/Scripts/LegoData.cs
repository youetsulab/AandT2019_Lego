using UnityEngine;
using System.Collections.Generic;

public static class LegoData
{
  #region Local Class
  internal static class CalibrationData
  {
    public static Vector3[] calibrationXYAndDepth = new Vector3[4];
    public static Vector3 calibrationCenter = new Vector3();
    internal static bool HasCalibrationData()
    {
      if (PlayerPrefs.HasKey("Init")) return true;
      else return false;
    }

    internal static void DeleteCalibarationData()
    {
      PlayerPrefs.DeleteAll();
    }

    internal static void GetCalibrationData()
    {
      calibrationXYAndDepth[0] = new Vector3(PlayerPrefs.GetFloat("X0"), PlayerPrefs.GetFloat("Y0"), PlayerPrefs.GetFloat("D0"));
      calibrationXYAndDepth[0] = new Vector3(PlayerPrefs.GetFloat("X1"), PlayerPrefs.GetFloat("Y1"), PlayerPrefs.GetFloat("D1"));
      calibrationXYAndDepth[0] = new Vector3(PlayerPrefs.GetFloat("X2"), PlayerPrefs.GetFloat("Y2"), PlayerPrefs.GetFloat("D2"));
      calibrationXYAndDepth[0] = new Vector3(PlayerPrefs.GetFloat("X3"), PlayerPrefs.GetFloat("Y3"), PlayerPrefs.GetFloat("D3"));
    }

    internal static void SetCalibrationData()
    {
      DeleteCalibarationData();
      PlayerPrefs.SetInt("Init", 1);
      PlayerPrefs.SetFloat("X0", calibrationXYAndDepth[0].x);
      PlayerPrefs.SetFloat("X1", calibrationXYAndDepth[1].x);
      PlayerPrefs.SetFloat("X2", calibrationXYAndDepth[2].x);
      PlayerPrefs.SetFloat("X3", calibrationXYAndDepth[3].x);
      PlayerPrefs.SetFloat("Y0", calibrationXYAndDepth[0].y);
      PlayerPrefs.SetFloat("Y1", calibrationXYAndDepth[1].y);
      PlayerPrefs.SetFloat("Y2", calibrationXYAndDepth[2].y);
      PlayerPrefs.SetFloat("Y3", calibrationXYAndDepth[3].y);
      PlayerPrefs.SetFloat("D0", calibrationXYAndDepth[0].z);
      PlayerPrefs.SetFloat("D1", calibrationXYAndDepth[1].z);
      PlayerPrefs.SetFloat("D2", calibrationXYAndDepth[2].z);
      PlayerPrefs.SetFloat("D3", calibrationXYAndDepth[3].z);
      PlayerPrefs.Save();
    }

    public static void PushCalibrationData(List<Vector3> calibrationArray)
    {
      if (calibrationArray.Count < 4) return;

      calibrationXYAndDepth[0] = calibrationArray[0];
      calibrationXYAndDepth[1] = calibrationArray[1];
      calibrationXYAndDepth[2] = calibrationArray[2];
      calibrationXYAndDepth[3] = calibrationArray[3];
      isCalibrated = true;
      SetCalibrationData();
    }
  }
  #endregion

  #region Constant Value
  public static readonly int DEPTH_CAMERA_WIDTH = 640;
  public static readonly int DEPTH_CAMERA_HEIGHT = 480;
  public static readonly int LANDSCAPE_MAP_HEIGHT = 32;
  public static readonly int LANDSCAPE_MAP_WIDTH = 32;
  public static readonly float MAX_DEPTH_NUM = 3975f;
  #endregion

  public static bool isCalibrated = false;
  public static bool isInitialized = false;


}
