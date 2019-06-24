using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
#endregion

public class LegoBase : MonoBehaviour
{
  #region Memeber Value
  [SerializeField]
  private RawImage colorImage_, debugImage1_, debugImage2_, debugImage3_;
  private KinectManager manager_;
  private List<LegoBlockInfo[,]> landscapeMapList_;
  private int rawLegoImageWidth_, rawLegoImageHeight_;
  private int createNumCount_;
  private LegoBlockInfo[,] currentLandscapeMap_ = new LegoBlockInfo[LegoData.LANDSCAPE_MAP_WIDTH,LegoData.LANDSCAPE_MAP_HEIGHT];
  private static readonly int MAX_CREATE_NUM = 60;
  private float timeLeft__1FPS_, timeLeft__15FPS_;
  #endregion

  protected void Start()
  {
    LegoData.CalibrationData.GetCalibrationData();
    rawLegoImageWidth_ = (int)Mathf.Abs(LegoData.CalibrationData.baseEdgeXY[0].x - LegoData.CalibrationData.baseEdgeXY[3].x);
    rawLegoImageHeight_ = (int)Mathf.Abs(LegoData.CalibrationData.baseEdgeXY[0].y - LegoData.CalibrationData.baseEdgeXY[3].y);
    landscapeMapList_ = new List<LegoBlockInfo[,]>();
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

    if (MAX_CREATE_NUM > createNumCount_)
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
    Texture2D debugTexture1 = new Texture2D(LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT, TextureFormat.RGBA32, false);
    Texture2D debugTexture2 = new Texture2D(LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT, TextureFormat.RGBA32, false);
    currentLandscapeMap_ = CalcLandscapeMapMode();

    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        Color color;
        if (currentLandscapeMap_[x, y].height == 0) color = Color.white;
        else
        {
          switch (currentLandscapeMap_[x, y].legoColor)
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

            case LegoColor.Yellow:
              color = Color.yellow;
              break;

            case LegoColor.None:
              color = Color.gray;
              break;

            default:
              color = Color.white;
              break;
          }
          debugTexture1.SetPixel(x, y, color);
        }

        switch (currentLandscapeMap_[x, y].height)
        {
          case 0:
            color = Color.white;
            break;

          case 1:
            color = Color.green;
            break;

          case 2:
            color = Color.yellow;
            break;

          case 3:
            color = Color.blue;
            break;

          case 4:
            color = Color.red;
            break;

          case 5:
            color = Color.black;
            break;

          default:
            color = Color.cyan;
            break;
        }
        debugTexture2.SetPixel(x, y, color);
      }
    }
    debugTexture1.Apply();
    debugTexture2.Apply();
    debugImage2_.texture = debugTexture1;
    debugImage3_.texture = debugTexture2;

    //[TODO] LegoBlockInfoの統計情報（色、高さ）を計算する。
    LegoBlockInfo[,] CalcLandscapeMapMode()
    {
      LegoBlockInfo[,] lsMap = new LegoBlockInfo[LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT];

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
            legoHeight[i] = landscapeMapList_[i][x, y].height;
            i++;
          }
          lsMap[x, y].legoColor = LegoGeneric.CalcMode(legoColor, Enum.GetNames(typeof(LegoColor)).Length);
          lsMap[x, y].height = LegoGeneric.CalcMode(legoHeight, LegoData.BUILDING_HIERARCHY_NUM);
        }
      }
      return lsMap;
    }
  }

  //LegoBlockInfo[,] CreateLandscapeMap()
  void CreateLandscapeMap()
  {
    #region Main
    RawLegoPixelInfo[,] rawLegoMap = GetTexturedata((Texture2D)colorImage_.texture);
    LegoBlockInfo[,] legoMap = ConvertRawLegoMap2LandscapeMap(rawLegoMap);
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
          cameramap[x, y].depth = manager_.GetDepthForPixel(x + (int)LegoData.CalibrationData.baseEdgeXY[0].x, y + (int)LegoData.CalibrationData.baseEdgeXY[0].y);
          cameramap[x, y].depth = (ushort)Mathf.Abs(cameramap[x, y].depth >> 3);

          Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x + (int)LegoData.CalibrationData.baseEdgeXY[0].x, y + (int)LegoData.CalibrationData.baseEdgeXY[0].y));
          cameramap[x, y].color = colorTexture.GetPixel((int)posColor.x, (int)posColor.y);

          texture.SetPixel(x, y, cameramap[x, y].color);
        }
      }
      texture.Apply();
      debugImage1_.texture = texture;
      return cameramap;
    }

    // Array(RawLegoPixelInfo) => Array(LegoBlockInfo)
    LegoBlockInfo[,] ConvertRawLegoMap2LandscapeMap(RawLegoPixelInfo[,] cameraMap)
    {
      LegoBlockInfo[,] landscapeMap = new LegoBlockInfo[LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT];
      int cellWidth = rawLegoImageWidth_ / LegoData.LANDSCAPE_MAP_WIDTH;
      int cellHeight = rawLegoImageHeight_ / LegoData.LANDSCAPE_MAP_HEIGHT;

      for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
      {
        for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
        {
          LegoColor[] cellColorMap = new LegoColor[cellHeight * cellWidth];
          int[] cellFloorMap = new int[cellWidth * cellHeight];
          for (int cy = 0; cy < cellHeight; cy++)
          {
            for (int cx = 0; cx < cellWidth; cx++)
            {
              cellColorMap[cy * cellWidth + cx] = DiscernColor(cameraMap[x * cellWidth + cx, y * cellHeight + cy].color);
              cellFloorMap[cy * cellWidth + cx] = DiscernLegoHeight(cameraMap[x * cellWidth + cx, y * cellHeight + cy].depth);
            }
          }
          landscapeMap[x, y].legoColor = LegoGeneric.CalcMode(cellColorMap, Enum.GetNames(typeof(LegoColor)).Length);
          landscapeMap[x, y].height = LegoGeneric.CalcMode(cellFloorMap, LegoData.BUILDING_HIERARCHY_NUM);
        }
      }

      return landscapeMap;
    }

    LegoColor DiscernColor(Color c)
    {
      HSV hsv = LegoGeneric.RGB2HSV(c);

      /*
      if (hsv.h == 0 && hsv.v == 0) return LegoColor.Black;
      if (225 <= hsv.h && hsv.h < 255) return LegoColor.Blue; //base : 240
      if (hsv.h < 15 || 345 <= hsv.h) return LegoColor.Red;   //base : 0
      if (75 <= hsv.h && hsv.h < 105) return LegoColor.Green; //base : 90
      */
      /*
      if (hsv.h == 0 && hsv.v == 0) return LegoColor.Black;
      if (165 <= hsv.h && hsv.h < 300) return LegoColor.Blue; //base : 240
      if (hsv.h < 45 || 300 <= hsv.h) return LegoColor.Red;   //base : 0
      if (45 <= hsv.h && hsv.h < 165) return LegoColor.Green; //base : 90

      return LegoColor.None;
      */
      LegoColor max = LegoGeneric.Max_rgb(c);
      if (c.r > 0.6f && c.g > 0.6f && c.b > 0.6f) return LegoColor.White;
      else return max;
    }

    int DiscernLegoHeight(ushort depth)
    {
      ushort baseDepth = LegoData.CalibrationData.baseCenterDepth;
      if (depth > baseDepth - 3) return 0;
      if (baseDepth - 3 >= depth && depth > baseDepth - 13) return 1;
      if (baseDepth - 13 >= depth && depth > baseDepth - 23) return 2;
      if (baseDepth - 23 >= depth && depth > baseDepth - 33) return 3;
      if (baseDepth - 33 >= depth && depth > baseDepth - 42) return 4;
      return 5;
    }
    #endregion
  }

  //色を仕分ける


  public void OnButtonClicked()
  {
    LegoData.legoMap = currentLandscapeMap_;

    //kinectは用済みなので削除する。また必要になる場合は削除せずに保持しておいたほうが良い可能性がある。
    GameObject mainCamera = GameObject.Find("Kinect Camera");
    SceneManager.MoveGameObjectToScene(mainCamera, SceneManager.GetActiveScene());
    SceneManager.LoadScene("Landscape");
  }
}


