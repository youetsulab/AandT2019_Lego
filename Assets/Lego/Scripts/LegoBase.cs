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
  #region Constant Value
  protected static readonly int DEPTH_CAMERA_WIDTH = 640; 
  protected static readonly int DEPTH_CAMERA_HEIGHT = 480;
  protected static readonly int LANDSCAPE_MAP_HEIGHT = 32;
  protected static readonly int LANDSCAPE_MAP_WIDTH = 32;
  protected static readonly int CALIBRATION_DEPTH = 100;
  protected static readonly int NUM_CALIBRATION_POINT = 4;
  public static readonly float MAX_DEPTH_NUM = 3975f;
  #endregion


  private static LegoBase instance_ = new LegoBase();
  public static List<Vector3> calibrationCoordinateAndDepth_ = new List<Vector3>();
  [SerializeField]
  public static bool isCalibrated = false;

  public static LegoBase GetInstance()
  {
    return instance_;
  }

  //[TODO]
  //キャリブレーションされた値が保存されているかを確認して、ある場合は取り出す。ない場合はキャリブレーションをする。
  protected void Start()
	{
    /*
    if(!PlayerPrefs.HasKey("Init")){
      SetKey();
    }
    */
    SceneManager.LoadScene("Init");
  }

	void Update()
	{

	}

  //[TODO]
  //キャリブレーションした値を保存する。
  private void StoreCalibrationValue()
  {

  }

  //外部から呼び出される。
  public void PushCalibrationValue(List<Vector3> calibrationArray)
  {
    if(calibrationArray.Count < 4) return;

    calibrationCoordinateAndDepth_.Add(calibrationArray[0]);
    calibrationCoordinateAndDepth_.Add(calibrationArray[1]);
    calibrationCoordinateAndDepth_.Add(calibrationArray[2]);
    calibrationCoordinateAndDepth_.Add(calibrationArray[3]);
    isCalibrated = true;
  }
}
