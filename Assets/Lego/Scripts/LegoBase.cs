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
      calibrationDepthAverage_ += LegoData.calibrationCoordinateAndDepth[i].z;
    }
    calibrationDepthAverage_ = calibrationDepthAverage_ / 4;
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
}


