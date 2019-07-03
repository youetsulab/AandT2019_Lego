using UnityEngine;
using System.Collections.Generic;
using System;



public static class LegoData
{
  #region Local Class
  internal static class CalibrationData
  {
    public static Vector2[] baseEdgeXY = new Vector2[4];
    public static List<ushort[]> baseEdgeDepthList = new List<ushort[]>();
    public static Vector2 baseCenterXY = new Vector2();
    public static ushort baseCenterDepth;
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
      baseEdgeXY[0] = new Vector2(PlayerPrefs.GetFloat("X0"), PlayerPrefs.GetFloat("Y0"));
      baseEdgeXY[1] = new Vector2(PlayerPrefs.GetFloat("X1"), PlayerPrefs.GetFloat("Y1"));
      baseEdgeXY[2] = new Vector2(PlayerPrefs.GetFloat("X2"), PlayerPrefs.GetFloat("Y2"));
      baseEdgeXY[3] = new Vector2(PlayerPrefs.GetFloat("X3"), PlayerPrefs.GetFloat("Y3"));
      baseCenterXY = new Vector2(PlayerPrefs.GetFloat("CX"), PlayerPrefs.GetFloat("CY"));
      baseCenterDepth = (ushort)PlayerPrefs.GetInt("CD");

      for (int y = 0; y < 4; y++)
      {
        ushort[] depthArray = new ushort[4];
        for (int x = 0; x < 4; x++)
        {
          string str = "D" + y + x;
          depthArray[x] = (ushort)PlayerPrefs.GetInt(str);
        }
        baseEdgeDepthList.Add(depthArray);
      }
    }

    internal static void SetCalibrationData()
    {
      DeleteCalibarationData();
      PlayerPrefs.SetInt("Init", 1);
      PlayerPrefs.SetFloat("X0", baseEdgeXY[0].x);
      PlayerPrefs.SetFloat("X1", baseEdgeXY[1].x);
      PlayerPrefs.SetFloat("X2", baseEdgeXY[2].x);
      PlayerPrefs.SetFloat("X3", baseEdgeXY[3].x);
      PlayerPrefs.SetFloat("Y0", baseEdgeXY[0].y);
      PlayerPrefs.SetFloat("Y1", baseEdgeXY[1].y);
      PlayerPrefs.SetFloat("Y2", baseEdgeXY[2].y);
      PlayerPrefs.SetFloat("Y3", baseEdgeXY[3].y);
      PlayerPrefs.SetFloat("CX", baseCenterXY.x);
      PlayerPrefs.SetFloat("CY", baseCenterXY.y);

      PlayerPrefs.SetInt("D00", baseEdgeDepthList[0][0]);
      PlayerPrefs.SetInt("D01", baseEdgeDepthList[0][1]);
      PlayerPrefs.SetInt("D02", baseEdgeDepthList[0][2]);
      PlayerPrefs.SetInt("D03", baseEdgeDepthList[0][3]);
      PlayerPrefs.SetInt("D10", baseEdgeDepthList[1][0]);
      PlayerPrefs.SetInt("D11", baseEdgeDepthList[1][1]);
      PlayerPrefs.SetInt("D12", baseEdgeDepthList[1][2]);
      PlayerPrefs.SetInt("D13", baseEdgeDepthList[1][3]);
      PlayerPrefs.SetInt("D20", baseEdgeDepthList[2][0]);
      PlayerPrefs.SetInt("D21", baseEdgeDepthList[2][1]);
      PlayerPrefs.SetInt("D22", baseEdgeDepthList[2][2]);
      PlayerPrefs.SetInt("D23", baseEdgeDepthList[2][3]);
      PlayerPrefs.SetInt("D30", baseEdgeDepthList[3][0]);
      PlayerPrefs.SetInt("D31", baseEdgeDepthList[3][1]);
      PlayerPrefs.SetInt("D32", baseEdgeDepthList[3][2]);
      PlayerPrefs.SetInt("D33", baseEdgeDepthList[3][3]);
      PlayerPrefs.SetInt("CD", baseCenterDepth);
      PlayerPrefs.Save();
    }

    public static void PushCalibrationData(Vector2[] eXY, List<ushort[]> eDepthList, Vector2 cXY, ushort cDepth)
    {
      if (eXY.Length < 4) return;

      baseEdgeXY[0] = eXY[0];
      baseEdgeXY[1] = eXY[1];
      baseEdgeXY[2] = eXY[2];
      baseEdgeXY[3] = eXY[3];
      baseEdgeDepthList = eDepthList;
      baseCenterXY = cXY;
      baseCenterDepth = cDepth;
      isCalibrated = true;
      SetCalibrationData();
    }
  }
  #endregion

  #region Constant Value
  public static readonly int DEPTH_CAMERA_WIDTH = 640;
  public static readonly int DEPTH_CAMERA_HEIGHT = 480;
  public static readonly int LANDSCAPE_MAP_HEIGHT = 16;
  public static readonly int LANDSCAPE_MAP_WIDTH = 16;
  public static readonly int LANDSCAPE_OBJECT_WIDTH = 10;
  public static readonly int LANDSCAPE_OBJECT_HEIGHT = 10;
  public static readonly float MAX_DEPTH_NUM = 3975f;
  public static readonly int BUILDING_HIERARCHY_NUM = 6;
  public static readonly string SAVE_FILE_PATH = "/Lego/SaveData/";
  #endregion

  public static bool isCalibrated = false;
  public static bool isInitialized = false;
  public static LegoBlockInfo[,] legoMap = new LegoBlockInfo[LANDSCAPE_MAP_WIDTH, LANDSCAPE_MAP_HEIGHT];
}
