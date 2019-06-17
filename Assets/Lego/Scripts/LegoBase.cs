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
  #region Memeber Value
  [SerializeField]
  private RawImage colorImage_;
  private KinectManager manager_;
  private float calibrationDepthAverage_;
  #endregion

  protected void Start()
  {
    calibrationDepthAverage_ = 0;
    LegoData.CalibrationData.GetCalibrationData();

    for (int i = 0; i < 4; i++)
    {
      calibrationDepthAverage_ += LegoData.CalibrationData.calibrationXYAndDepth[i].z;
    }
    calibrationDepthAverage_ = calibrationDepthAverage_ / 4;
    Debug.Log("Depth Average:" + calibrationDepthAverage_);
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

  LandscapeCellInfo[,] CreateLandscapeMap()
  {
    RawLegoPixelInfo[,] rawLegoMap = GetTexturedata((Texture2D)colorImage_.texture);
    return ConvertRawLegoMap2LandscapeMap(rawLegoMap);

    #region Local Method
    //Color Texture taken by kinect Camera => Array(RawLegoPixelInfo)
    RawLegoPixelInfo[,] GetTexturedata(Texture2D colorTexture)
    {
      RawLegoPixelInfo[,] cameramap = new RawLegoPixelInfo[LegoData.DEPTH_CAMERA_WIDTH, LegoData.DEPTH_CAMERA_HEIGHT];

      manager_ = KinectManager.Instance;

      for (int y = 0; y < LegoData.DEPTH_CAMERA_HEIGHT; y++)
      {
        for (int x = 0; x < LegoData.DEPTH_CAMERA_WIDTH; x++)
        {
          cameramap[x, y].depth = manager_.GetDepthForPixel(x, y);

          Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x, y));
          cameramap[x, y].color = colorTexture.GetPixel((int)posColor.x, (int)posColor.y);
        }
      }
      return cameramap;
    }

    /*
    // Array(RawLegoPixelInfo) => Array(RawLegoPixelInfo)(キャリブレーションした4点内に切り取る)
    Vector2 TrimRawLegoMap(RawLegoPixelInfo[,] legoMap)
    {
      return new Vector2(1, 1);
    }
    */

    // Array(RawLegoPixelInfo) => Array(LandscapeCellInfo)
    LandscapeCellInfo[,] ConvertRawLegoMap2LandscapeMap(RawLegoPixelInfo[,] legoMap)
    {
      Vector3 startVertex = LegoData.CalibrationData.calibrationXYAndDepth[0];
      Vector3 endVertex = LegoData.CalibrationData.calibrationXYAndDepth[3];
      int width = (int)endVertex.x - (int)startVertex.x;
      int height = (int)endVertex.y - (int)startVertex.y;
      int cellWidth = width / 32;
      int cellHeight = height / 32;

      /*
      for (int y = 0; y < 32; y++)
      {
        for (int x = 0; x < 32; x++)
        {
          for (int cy = 0; cy < cellHeight; cy++)
          {
            for (int cx = 0; cx < cellWidth; cx++)
            {

            }
          }
        }
      }
      */

      return new LandscapeCellInfo[1, 1];
    }
    #endregion
  }

  public void OnButtonClicked()
  {
    CreateLandscapeMap();
  }
}


