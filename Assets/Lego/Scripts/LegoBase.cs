using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
  private RawImage colorImage_, debugImage1_, debugImage2_;
  private KinectManager manager_;
  private List<LandscapeCellInfo[,]> landscapeMapList_;
  private int rawLegoImageWidth_, rawLegoImageHeight_;
  private bool isCreateLandscapeMap_;
  private int createNumCount_;
  private static readonly int MAX_CREATE_NUM = 60;
  private float timeLeft__1FPS_, timeLeft__15FPS_;
  #endregion

  protected void Start()
  {
    LegoData.CalibrationData.GetCalibrationData();
    rawLegoImageWidth_ = (int)Mathf.Abs(LegoData.CalibrationData.calibrationXYAndDepth[0].x - LegoData.CalibrationData.calibrationXYAndDepth[3].x);
    rawLegoImageHeight_ = (int)Mathf.Abs(LegoData.CalibrationData.calibrationXYAndDepth[0].y - LegoData.CalibrationData.calibrationXYAndDepth[3].y);
    isCreateLandscapeMap_ = false;
    landscapeMapList_ = new List<LandscapeCellInfo[,]>();
    createNumCount_ = 0;

    timeLeft__15FPS_ = 0.04f;
    timeLeft__1FPS_ = 1.0f;
  }

  void Update()
  {
    manager_ = KinectManager.Instance;

    if (!(manager_ && manager_.IsInitialized())) return;

    if (colorImage_ && (colorImage_.texture == null))
    {
      colorImage_.texture = manager_.GetUsersClrTex();
    }

    if (isCreateLandscapeMap_ && (MAX_CREATE_NUM > createNumCount_))
    {
      CreateLandscapeMap();
      createNumCount_++;
    }

    if (createNumCount_ >= MAX_CREATE_NUM)
    {
      CreateLandScapeTexture();
      landscapeMapList_.Clear();
      createNumCount_ = 0;
    }
  }

  void CreateLandScapeTexture()
  {
    Texture2D debugTexture = new Texture2D(LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT, TextureFormat.RGBA32, false);
    LandscapeCellInfo[,] landscapeMap = CalcLandscapeMapMode();

    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        Color color;
        if (landscapeMap[x, y].floor == 0) color = Color.white;
        else
        {
          switch (landscapeMap[x, y].legoColor)
          {
            case LegoColor.Black:
              color = Color.black;
              break;

            case LegoColor.Red:
              color = Color.red;
              break;

            case LegoColor.Blue:
              color = Color.blue;
              break;

            case LegoColor.Green:
              color = Color.green;
              break;

            case LegoColor.None:
              color = Color.yellow;
              break;

            default:
              color = Color.white;
              break;
          }
          debugTexture.SetPixel(x, y, color);
        }
      }
    }
    debugTexture.Apply();
    debugImage2_.texture = debugTexture;

    //[TODO] LandscapeCellInfoの統計情報（色、高さ）を計算する。
    LandscapeCellInfo[,] CalcLandscapeMapMode()
    {
      LandscapeCellInfo[,] lsMap = new LandscapeCellInfo[LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT];

      for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
      {
        for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
        {
          int i = 0;
          LegoColor[] legoColor = new LegoColor[landscapeMapList_.Count];
          int[] legoHeight = new int[landscapeMapList_.Count];
          while (i < landscapeMapList_.Count)
          {
            legoColor[i] = landscapeMapList_[i][x, y].legoColor;
            legoHeight[i] = landscapeMapList_[i][x, y].floor;
            i++;
          }
          lsMap[x, y].legoColor = LegoGeneric.CalcMode(legoColor, Enum.GetNames(typeof(LegoColor)).Length);
          lsMap[x, y].floor = LegoGeneric.CalcMode(legoHeight, LegoData.BUILDING_HIERARCHY_NUM);
        }
      }
      return lsMap;
    }
  }

  //LandscapeCellInfo[,] CreateLandscapeMap()
  void CreateLandscapeMap()
  {
    #region Main
    RawLegoPixelInfo[,] rawLegoMap = GetTexturedata((Texture2D)colorImage_.texture);
    LandscapeCellInfo[,] legoMap = ConvertRawLegoMap2LandscapeMap(rawLegoMap);
    landscapeMapList_.Add(legoMap);
    #endregion

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
          cameramap[x, y].depth = (ushort)Mathf.Abs(cameramap[x, y].depth >> 3);

          Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x + (int)LegoData.CalibrationData.calibrationXYAndDepth[0].x, y + (int)LegoData.CalibrationData.calibrationXYAndDepth[0].y));
          cameramap[x, y].color = colorTexture.GetPixel((int)posColor.x, (int)posColor.y);

          texture.SetPixel(x, y, cameramap[x, y].color);
        }
      }
      texture.Apply();
      debugImage1_.texture = texture;
      return cameramap;
    }

    // Array(RawLegoPixelInfo) => Array(LandscapeCellInfo)
    LandscapeCellInfo[,] ConvertRawLegoMap2LandscapeMap(RawLegoPixelInfo[,] cameraMap)
    {
      LandscapeCellInfo[,] landscapeMap = new LandscapeCellInfo[LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT];
      int cellWidth = rawLegoImageWidth_ / LegoData.LANDSCAPE_MAP_WIDTH;
      int cellHeight = rawLegoImageHeight_ / LegoData.LANDSCAPE_MAP_HEIGHT;

      for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
      {
        for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
        {
          LegoColor[] cellColorInfo = new LegoColor[cellHeight * cellWidth];
          int[] cellFloorInfo = new int[cellWidth * cellHeight];
          for (int cy = 0; cy < cellWidth; cy++)
          {
            for (int cx = 0; cx < cellWidth; cx++)
            {
              cellColorInfo[cy * cellWidth + cx] = DiscriminateColor(cameraMap[x * cellWidth + cx, y * cellHeight + cy].color);
              cellFloorInfo[cy * cellWidth + cx] = DiscriminateLegoHeight(cameraMap[x * cellWidth + cx, y * cellHeight + cy].depth);
            }
          }
          landscapeMap[x, y].legoColor = LegoGeneric.CalcMode(cellColorInfo, Enum.GetNames(typeof(LegoColor)).Length);
          landscapeMap[x, y].floor = LegoGeneric.CalcMode(cellFloorInfo, LegoData.BUILDING_HIERARCHY_NUM);
        }
      }

      return landscapeMap;
    }

    LegoColor DiscriminateColor(Color c)
    {
      HSV hsv = LegoGeneric.RGB2HSV(c);

      if (hsv.h == 0 && hsv.v == 0) return LegoColor.Black;
      if (225 <= hsv.h && hsv.h < 255) return LegoColor.Blue; //base : 240
      if (hsv.h < 15 || 345 <= hsv.h) return LegoColor.Red;   //base : 0
      if (75 <= hsv.h && hsv.h < 105) return LegoColor.Green; //base : 90
      return LegoColor.None;
    }

    int DiscriminateLegoHeight(ushort depth)
    {
      ushort baseDepth = (ushort)LegoData.CalibrationData.calibrationCenter.z;
      if (depth > baseDepth - 5) return 0;
      if (baseDepth - 5 >= depth && depth > baseDepth - 10) return 1;
      if (baseDepth - 10 >= depth && depth > baseDepth - 20) return 2;
      if (baseDepth - 20 >= depth && depth > baseDepth - 30) return 3;
      if (baseDepth - 30 >= depth && depth > baseDepth - 40) return 4;
      return 5;
    }
    #endregion
  }

  //色を仕分ける


  public void OnButtonClicked()
  {
    isCreateLandscapeMap_ = !isCreateLandscapeMap_;
  }
}


