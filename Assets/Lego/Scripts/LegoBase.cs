using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum LegoColor
{
    White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange
}
struct RawLegoPixelInfo
{
  public ushort depth;
  public Color color;
}

struct LandscapeCellInfo
{
  public int index;
  public LegoColor legoColor;
}

public class LegoBase : MonoBehaviour
{
  #region Constant Value
  private static readonly int DEPTH_CAMERA_WIDTH = 640;
  private static readonly int DEPTH_CAMERA_HEIGHT = 480;
  private static readonly int LANDSCAPE_MAP_HEIGHT = 32;
  private static readonly int LANDSCAPE_MAP_WIDTH = 32;
  public static readonly float MAX_DEPTH_NUM = 3975f;
  #endregion


  private int calibrationDepth;
  private static LegoBase instance_ = new LegoBase();
  public static List<Vector3> calibrationCoordinateAndDepth_ = new List<Vector3>();
  [SerializeField]
  public static bool isCalibrated = false;

  public static LegoBase Instance{
    get { return instance_; }
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
    if (calibrationArray.Count < 4) return;

    calibrationCoordinateAndDepth_.Add(calibrationArray[0]);
    calibrationCoordinateAndDepth_.Add(calibrationArray[1]);
    calibrationCoordinateAndDepth_.Add(calibrationArray[2]);
    calibrationCoordinateAndDepth_.Add(calibrationArray[3]);
    isCalibrated = true;
  }

  RawLegoPixelInfo[,] GetTexturedata()
  {
    KinectManager manager_;
    Texture2D colorTexture_;
    RawImage colorImage_ = null;

    manager_ = KinectManager.Instance;
    colorTexture_ = new Texture2D(DEPTH_CAMERA_WIDTH, DEPTH_CAMERA_HEIGHT, TextureFormat.RGBA32, false);

    Vector2 inverseY = new Vector2(1, -1);
    colorImage_.GetComponent<Transform>().localScale *= inverseY;

    colorImage_.texture = manager_.GetUsersClrTex();
    colorTexture_ = (Texture2D)colorImage_.texture;

    RawLegoPixelInfo[,] cameramap = new RawLegoPixelInfo[DEPTH_CAMERA_WIDTH, DEPTH_CAMERA_HEIGHT];

    for (int y = 0; y < DEPTH_CAMERA_HEIGHT; y++)
    {
      for (int x = 0; x < DEPTH_CAMERA_WIDTH; x++)
      {
        cameramap[x, y].depth = manager_.GetDepthForPixel(x, y);

        Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x, y));
        cameramap[x, y].color = colorTexture_.GetPixel((int)posColor.x, (int)posColor.y);
      }
    }

    return cameramap;
  }
}