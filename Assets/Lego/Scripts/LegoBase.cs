using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region Struct define
public enum LegoColor
{
  White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange
}
internal struct RawLegoPixelInfo
{
  public ushort depth;
  public Color color;
}

internal struct LandscapeCellInfo
{
  public int index;
  public LegoColor legoColor;
}
#endregion

public class LegoBase : MonoBehaviour
{
  #region Local Class
  internal static class CalibrationData
  {
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
      instance_.calibrationCoordinateAndDepth_[0] = new Vector3(PlayerPrefs.GetFloat("X0"), PlayerPrefs.GetFloat("Y0"), PlayerPrefs.GetFloat("D0"));
      instance_.calibrationCoordinateAndDepth_[0] = new Vector3(PlayerPrefs.GetFloat("X1"), PlayerPrefs.GetFloat("Y1"), PlayerPrefs.GetFloat("D1"));
      instance_.calibrationCoordinateAndDepth_[0] = new Vector3(PlayerPrefs.GetFloat("X2"), PlayerPrefs.GetFloat("Y2"), PlayerPrefs.GetFloat("D2"));
      instance_.calibrationCoordinateAndDepth_[0] = new Vector3(PlayerPrefs.GetFloat("X3"), PlayerPrefs.GetFloat("Y3"), PlayerPrefs.GetFloat("D3"));
    }

    internal static void SetCalibrationData()
    {
      DeleteCalibarationData();
      PlayerPrefs.SetInt("Init", 1);
      PlayerPrefs.SetFloat("X0", instance_.calibrationCoordinateAndDepth_[0].x);
      PlayerPrefs.SetFloat("X1", instance_.calibrationCoordinateAndDepth_[1].x);
      PlayerPrefs.SetFloat("X2", instance_.calibrationCoordinateAndDepth_[2].x);
      PlayerPrefs.SetFloat("X3", instance_.calibrationCoordinateAndDepth_[3].x);
      PlayerPrefs.SetFloat("Y0", instance_.calibrationCoordinateAndDepth_[0].y);
      PlayerPrefs.SetFloat("Y1", instance_.calibrationCoordinateAndDepth_[1].y);
      PlayerPrefs.SetFloat("Y2", instance_.calibrationCoordinateAndDepth_[2].y);
      PlayerPrefs.SetFloat("Y3", instance_.calibrationCoordinateAndDepth_[3].y);
      PlayerPrefs.SetFloat("D0", instance_.calibrationCoordinateAndDepth_[0].z);
      PlayerPrefs.SetFloat("D1", instance_.calibrationCoordinateAndDepth_[1].z);
      PlayerPrefs.SetFloat("D2", instance_.calibrationCoordinateAndDepth_[2].z);
      PlayerPrefs.SetFloat("D3", instance_.calibrationCoordinateAndDepth_[3].z);
      PlayerPrefs.Save();
    }
  }
  #endregion

  #region Constant Value
  private static readonly int DEPTH_CAMERA_WIDTH = 640;
  private static readonly int DEPTH_CAMERA_HEIGHT = 480;
  private static readonly int LANDSCAPE_MAP_HEIGHT = 32;
  private static readonly int LANDSCAPE_MAP_WIDTH = 32;
  public static readonly float MAX_DEPTH_NUM = 3975f;
  #endregion

  #region Memeber Value
  [SerializeField]
  private RawImage colorImage_;
  private KinectManager manager_;
  private static LegoBase instance_ = new LegoBase();
  private Vector3[] calibrationCoordinateAndDepth_ = new Vector3[4];
  private float calibrationDepthAverage_;
  private bool isCalibrated_ = false;
  private bool isInitialized_ = false;
  #endregion

  #region Accessor
  public static LegoBase Instance
  {
    get { return instance_; }
  }

  public bool IsCalibrated
  {
    get { return isCalibrated_; }
  }

  public bool IsInitialized
  {
    get { return isInitialized_; }
    set { isInitialized_ = value; }
  }
  #endregion

  protected void Start()
  {
    calibrationDepthAverage_ = 0;
    if (CalibrationData.HasCalibrationData())
    {
      CalibrationData.GetCalibrationData();
      instance_.isCalibrated_ = true;

      for (int i = 0; i < 4; i++)
      {
        instance_.calibrationDepthAverage_ += instance_.calibrationCoordinateAndDepth_[i].z;
      }
      instance_.calibrationDepthAverage_ = instance_.calibrationDepthAverage_ / 4;
    }

    if (!instance_.isInitialized_) SceneManager.LoadScene("Init");
  }

  void Update()
  {
    manager_ = KinectManager.Instance;

    if (!(manager_ && manager_.IsInitialized())) return;

    if (colorImage_ && (colorImage_.texture == null))
    {
      colorImage_.texture = manager_.GetUsersClrTex();
    }
  }

  private LegoBase() { }

  RawLegoPixelInfo[,] GetTexturedata(Texture2D colorTexture)
  {
    RawLegoPixelInfo[,] cameramap = new RawLegoPixelInfo[DEPTH_CAMERA_WIDTH, DEPTH_CAMERA_HEIGHT];

    manager_ = KinectManager.Instance;

    for (int y = 0; y < DEPTH_CAMERA_HEIGHT; y++)
    {
      for (int x = 0; x < DEPTH_CAMERA_WIDTH; x++)
      {
        cameramap[x, y].depth = manager_.GetDepthForPixel(x, y);

        Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x, y));
        cameramap[x, y].color = colorTexture.GetPixel((int)posColor.x, (int)posColor.y);
      }
    }

    return cameramap;
  }

  #region Function defines (Be Called by External)
  //From Calibration.cs
  public void PushCalibrationData(List<Vector3> calibrationArray)
  {
    if (calibrationArray.Count < 4) return;

    instance_.calibrationCoordinateAndDepth_[0] = calibrationArray[0];
    instance_.calibrationCoordinateAndDepth_[1] = calibrationArray[1];
    instance_.calibrationCoordinateAndDepth_[2] = calibrationArray[2];
    instance_.calibrationCoordinateAndDepth_[3] = calibrationArray[3];
    instance_.isCalibrated_ = true;
    CalibrationData.SetCalibrationData();
  }
  #endregion
}


