using UnityEngine;
using UnityEngine.UI;
using System;

#region Struct define
public enum LegoColor
{
  White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange, None
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
  public int floor;
}
#endregion

public class LegoBase : MonoBehaviour
{
  #region Memeber Value
  [SerializeField]
  private RawImage colorImage_, debugImage_;
  private KinectManager manager_;
  private int rawLegoImageWidth_, rawLegoImageHeight_;
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

    rawLegoImageWidth_ = (int)Mathf.Abs(LegoData.CalibrationData.calibrationXYAndDepth[0].x - LegoData.CalibrationData.calibrationXYAndDepth[3].x);
    rawLegoImageHeight_ = (int)Mathf.Abs(LegoData.CalibrationData.calibrationXYAndDepth[0].y - LegoData.CalibrationData.calibrationXYAndDepth[3].y);
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
    //指定したエリア内のレゴデータを取得する
    RawLegoPixelInfo[,] GetTexturedata(Texture2D colorTexture)
    {
      RawLegoPixelInfo[,] cameramap = new RawLegoPixelInfo[rawLegoImageWidth_, rawLegoImageHeight_];
      Texture2D texture = new Texture2D(rawLegoImageWidth_, rawLegoImageHeight_, TextureFormat.RGBA32, false);

      manager_ = KinectManager.Instance;

      for (int y = 0; y < rawLegoImageHeight_; y++)
      {
        for (int x = 0; x < rawLegoImageWidth_; x++)
        {
          cameramap[x, y].depth = manager_.GetDepthForPixel(x + (int)LegoData.CalibrationData.calibrationXYAndDepth[0].x, y + (int)LegoData.CalibrationData.calibrationXYAndDepth[0].y);

          Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x + (int)LegoData.CalibrationData.calibrationXYAndDepth[0].x, y + (int)LegoData.CalibrationData.calibrationXYAndDepth[0].y));
          cameramap[x, y].color = colorTexture.GetPixel((int)posColor.x, (int)posColor.y);

          texture.SetPixel(x, y, cameramap[x, y].color);
        }
      }
      texture.Apply();
      debugImage_.texture = texture;
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
    LandscapeCellInfo[,] ConvertRawLegoMap2LandscapeMap(RawLegoPixelInfo[,] cameraMap)
    {
      LandscapeCellInfo[,] landscapleMap = new LandscapeCellInfo[32, 32];
      int cellWidth = rawLegoImageWidth_ / 32;
      int cellHeight = rawLegoImageHeight_ / 32;

      for (int y = 0; y < 32; y++)
      {
        for (int x = 0; x < 32; x++)
        {
          LegoColor[] cellColorInfo = new LegoColor[cellHeight * cellWidth];
          int[] cellFloorInfo = new int[cellWidth * cellHeight];
          for (int cy = 0; cy < cellWidth; cy++)
          {
            for (int cx = 0; cx < cellWidth; cx++)
            {
              cellColorInfo[cy * cellWidth + cellHeight] = DiscriminateColor(cameraMap[x * cellWidth + cx, y * cellHeight + cy].color);
              cellFloorInfo[cy * cellWidth + cellHeight] = DiscriminateLegoHeight(cameraMap[x * cellWidth + cx, y * cellHeight + cy].depth);
            }
          }
          landscapleMap[x, y].legoColor = LegoGeneric.CalcMode(cellColorInfo, Enum.GetNames(typeof(LegoColor)).Length);
          landscapleMap[x, y].floor = LegoGeneric.CalcMode(cellFloorInfo, 6);
        }
      }

      return new LandscapeCellInfo[1, 1];
    }

    LegoColor DiscriminateColor(Color c)
    {
      HSV hsv = LegoGeneric.RGB2HSV(c);

      if (hsv.h == 0 && hsv.v == 255) return LegoColor.White;
      if (230 < hsv.h && hsv.h < 250) return LegoColor.Blue;
      if (hsv.h < 10 || 340 < hsv.h) return LegoColor.Red;
      if (70 < hsv.h && hsv.h < 110) return LegoColor.Green;
      return LegoColor.None;
    }

    int DiscriminateLegoHeight(ushort depth)
    {
      if(depth < calibrationDepthAverage_) return 5;
      if(calibrationDepthAverage_ <= depth && depth < calibrationDepthAverage_ + 10) return 4;
      if(calibrationDepthAverage_ + 10 <= depth && depth < calibrationDepthAverage_ + 20) return 3;
      if(calibrationDepthAverage_ + 20 <= depth && depth < calibrationDepthAverage_ + 30) return 2;
      if(calibrationDepthAverage_ + 30 <= depth && depth < calibrationDepthAverage_ + 40) return 1;
      return 0;
    }
    #endregion
  }

  //色を仕分ける


  public void OnButtonClicked()
  {
    CreateLandscapeMap();
  }
}


