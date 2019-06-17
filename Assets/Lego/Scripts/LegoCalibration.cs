/*
[TODO]
範囲がガバイ。
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

struct BasePixelInfo
{
  public int sectionNumber;
  public ushort depth;
  public int x, y;
}

struct HSV
{
  //h:0-360 s:0-256 v:-256
  public int h;
  public int s;
  public int v;
}

public class LegoCalibration : MonoBehaviour
{
  private float timeLeft__1FPS_;
  private float timeLeft__15FPS_;
  private KinectManager manager_;
  private LegoBase lego_;
  [SerializeField] RawImage depthImage_;
  [SerializeField, Range(750f, 1000f)] float upperDisplayRange_;
  [SerializeField, Range(0f, 850f)] float lowerDisplayRange_;
  private static readonly int upperBasePixelDepthValue_ = 860;
  private static readonly int lowerBasePixelDepthValue_ = 840;
  private Texture2D depthTexture_;
  private Texture2D colorTexture_;
  private ushort[] depthMap_;
  private List<BasePixelInfo> basePixelMap_;
  private List<Vector3> baseXYAndDepth_;
  private Vector3 baseCenter_;
  private int progressFlag_;


  // Start is called before the first frame update
  void Start()
  {
    if (depthImage_ == null)
    {
      Debug.LogError("[Depth Image] is not attaced.", depthImage_);
      Application.Quit();
    }

    manager_ = KinectManager.Instance;

    Vector2 inverseY = new Vector2(1, -1);
    depthImage_.GetComponent<Transform>().localScale *= inverseY;

    depthMap_ = new ushort[LegoData.DEPTH_CAMERA_WIDTH * LegoData.DEPTH_CAMERA_HEIGHT];

    depthTexture_ = new Texture2D(LegoData.DEPTH_CAMERA_WIDTH, LegoData.DEPTH_CAMERA_HEIGHT, TextureFormat.RGBA32, false);
    depthImage_.texture = depthTexture_;

    basePixelMap_ = new List<BasePixelInfo>();
    baseXYAndDepth_ = new List<Vector3>();
    timeLeft__1FPS_ = 1.0f;
    timeLeft__15FPS_ = 0.04f;

    progressFlag_ = 0;
  }

  void Update()
  {
    timeLeft__1FPS_ -= Time.deltaTime;
    timeLeft__15FPS_ -= Time.deltaTime;

    if (!(manager_ && manager_.IsInitialized())) return;

    //15fps処理
    if (timeLeft__15FPS_ <= 0.0f)
    {
      timeLeft__15FPS_ = 0.04f;

      colorTexture_ = manager_.GetUsersClrTex();
      depthMap_ = manager_.GetRawDepthMap();

      ScanFrom4EndPoint(colorTexture_);
      depthTexture_.Apply();
    }

    //1fps処理
    if (timeLeft__1FPS_ <= 0.0f)
    {
      timeLeft__1FPS_ = 1.0f;
      switch (progressFlag_)
      {
        case 0:
          baseXYAndDepth_.Clear();
          for (int i = 0; i < 4; i++)
          {
            baseXYAndDepth_.Add(CalcEdgeBaseDepthAverage_And_Point(i));
            Debug.Log("XY" + i + ":" + (int)baseXYAndDepth_[i].x + ", " + (int)baseXYAndDepth_[i].y + " Depth value:" + (int)baseXYAndDepth_[i].z);
          }
          CalcCenterBaseDepth_And_Point();
          Debug.Log("Center XY:" + baseCenter_.x + ", " + baseCenter_.y + "Depth value:" + baseCenter_.z);
          break;

        case 1:
          Debug.Log("Color");
          Vector2[] vecXY = new Vector2[4];
          Color[] colorXY = new Color[4];
          HSV[] hsvXY = new HSV[4];
          for (int i = 0; i < 4; i++)
          {
            vecXY[i] = manager_.GetColorMapPosForDepthPos(new Vector2(baseXYAndDepth_[i].x, baseXYAndDepth_[i].y));
            colorXY[i] = colorTexture_.GetPixel((int)vecXY[i].x, (int)vecXY[i].y);
            hsvXY[i] = RGB2HSV(colorXY[i]);
            Debug.Log("XY" + i + ":" + colorXY[i].r + ", " + colorXY[i].g + ", " + colorXY[i].b + ", " + colorXY[i].a);
            Debug.Log("XY" + i + ":" + hsvXY[i].h + ", " + hsvXY[i].s + ", " + hsvXY[i].v);
          }
          break;

        case 2:
          LegoData.CalibrationData.PushCalibrationData(baseXYAndDepth_, baseCenter_);
          SceneManager.LoadScene("Main");
          break;

        default:
          LegoData.CalibrationData.PushCalibrationData(baseXYAndDepth_, baseCenter_);
          SceneManager.LoadScene("Main");
          break;
      }
    }
    basePixelMap_.Clear();
  }

  //h:0-360 s:0f-1f v:0f-1f
  private HSV RGB2HSV(Color rgb)
  {
    rgb.r *= 255;
    rgb.g *= 255;
    rgb.b *= 255;

    HSV hsv = new HSV();
    int min, max;

    max = Max_rgb(rgb);
    min = Min_rgb(rgb);
    hsv.v = max;

    if (hsv.v == 0f) hsv.s = hsv.h = 0;
    else
    {
      hsv.s = 255 * (max - min) / max;

      if (Mathf.Abs(max - min) < 30)
      {
        hsv.h = 0;
        hsv.s = 0;
        hsv.v = 255;
      }
      else
      {

        if (max == (int)rgb.r) hsv.h = 60 * (int)(rgb.b - rgb.g) / (max - min);
        else if (max == (int)rgb.g) hsv.h = 60 * (int)(rgb.r - rgb.b) / (max - min) + 120;
        else if (max == (int)rgb.b) hsv.h = 60 * (int)(rgb.g - rgb.r) / (max - min) + 240;
        else Application.Quit();
      }

      if (hsv.h < 0) hsv.h += 360;
      else if (hsv.h > 360) hsv.h -= 360;
    }

    return hsv;

    int Max_rgb(Color c)
    {
      if (c.r > c.g)
      {
        if (c.r > c.b) return (int)c.r;
        else return (int)c.b;
      }
      else
      {
        if (c.g > c.b) return (int)c.g;
        else return (int)c.b;
      }
    }

    int Min_rgb(Color c)
    {
      if (c.r < c.g)
      {
        if (c.r < c.b) return (int)c.r;
        else return (int)c.b;
      }
      else
      {
        if (c.g < c.b) return (int)c.g;
        else return (int)c.b;
      }
    }
  }


  //4つの点からなる2つの直線の交点を求め、座標と深度を計算する。
  //http://imagingsolution.blog.fc2.com/blog-entry-137.html
  private void CalcCenterBaseDepth_And_Point()
  {
    if (float.IsNaN(baseXYAndDepth_[0].x) || float.IsNaN(baseXYAndDepth_[1].x) || float.IsNaN(baseXYAndDepth_[2].x) || float.IsNaN(baseXYAndDepth_[3].x)) return;
    if (float.IsNaN(baseXYAndDepth_[0].y) || float.IsNaN(baseXYAndDepth_[1].y) || float.IsNaN(baseXYAndDepth_[2].y) || float.IsNaN(baseXYAndDepth_[3].y)) return;

    float s1 = ((baseXYAndDepth_[3].x - baseXYAndDepth_[1].x) * (baseXYAndDepth_[0].y - baseXYAndDepth_[1].y) - (baseXYAndDepth_[3].y - baseXYAndDepth_[1].y) * (baseXYAndDepth_[0].x - baseXYAndDepth_[1].x)) * 0.5f;
    s1 = Mathf.Abs(s1);
    float s2 = ((baseXYAndDepth_[3].x - baseXYAndDepth_[1].x) * (baseXYAndDepth_[1].y - baseXYAndDepth_[2].y) - (baseXYAndDepth_[3].y - baseXYAndDepth_[1].y) * (baseXYAndDepth_[1].x - baseXYAndDepth_[2].x)) * 0.5f;
    s2 = Mathf.Abs(s2);

    baseCenter_.x = baseXYAndDepth_[0].x + (baseXYAndDepth_[2].x - baseXYAndDepth_[0].x) * s1 / (s1 + s2);
    baseCenter_.y = baseXYAndDepth_[0].y + (baseXYAndDepth_[2].y - baseXYAndDepth_[0].y) * s1 / (s1 + s2);
    baseCenter_.z = depthMap_[(int)baseCenter_.y * LegoData.DEPTH_CAMERA_WIDTH + (int)baseCenter_.x] >> 3;
  }

  //4つの端点の座標と深度を計算する。
  private Vector3 CalcEdgeBaseDepthAverage_And_Point(int sectionNum)
  {
    //x,y: coordinate, z: average of depth value
    float depth = 0, x = 0, y = 0;
    int pixelNum = 0;

    if (basePixelMap_ == null) return new Vector3(0, 0, 0);

    foreach (var item in basePixelMap_)
    {
      if (item.sectionNumber == sectionNum)
      {
        depth += item.depth;
        x += item.x;
        y += item.y;
        pixelNum++;
      }
    }
    return new Vector3(x / pixelNum, y / pixelNum, depth / pixelNum);
  }

  //左上から順番にではなく、4つの端点から中心に向かって走査する
  private void ScanFrom4EndPoint(Texture2D colorTexture)
  {
    ScanFrom4EndPoint();

    //[TODO]
    //・読みやすいコードへのリファクタリング
    //・走査範囲の厳密化
    #region LocalFunction
    void SetPixelForXY(int x, int y, int sectionNum)
    {
      Color col;
      BasePixelInfo pixel;
      int depthData = depthMap_[y * LegoData.DEPTH_CAMERA_WIDTH + x] >> 3;

      if (lowerDisplayRange_ < depthData && depthData < upperDisplayRange_)
      {
        col = new Color(0, 0, 0, 255);
      }
      else
      {
        Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x, y));
        col = colorTexture.GetPixel((int)posColor.x, (int)posColor.y);
      }
      depthTexture_.SetPixel(x, y, col);

      if (lowerBasePixelDepthValue_ <= depthData && depthData <= upperBasePixelDepthValue_)
      {
        pixel.sectionNumber = sectionNum;
        pixel.depth = (ushort)depthData;
        pixel.x = x;
        pixel.y = y;
        basePixelMap_.Add(pixel);
      }

      if (progressFlag_ > 0)
      {
        for (int i = 0; i < 4; i++)
        {
          depthTexture_.SetPixel((int)baseXYAndDepth_[i].x, (int)baseXYAndDepth_[i].y, new Color(1, 0, 0, 1));
        }
      }
    }

    void ScanFrom4EndPoint()
    {
      int x, y;
      int horizontalCenterLine = LegoData.DEPTH_CAMERA_HEIGHT / 2;
      int verticalCenterLine = LegoData.DEPTH_CAMERA_WIDTH / 2;

      for (int i = 0; i < (LegoData.DEPTH_CAMERA_WIDTH / 2) + (LegoData.DEPTH_CAMERA_HEIGHT / 2); i++)
      {
        /*
        |--------|---------|
        |    0   |    1    |
        |------------------| <- horizontal center line 
        |    2   |    3    |
        |--------|---------|
                 ^
       vertical center line
        number = [j] value
        figure: display
        */

        for (int section = 0; section < 4; section++)
        {
          if (i < horizontalCenterLine)
          {
            switch (section)
            {
              /*
              case of j = 1:
              This step is scanning display as shown below.
              |--------------------|
              |********            |
              |******              |
              |****                |
              |**                  |
              |--------------------|
              */
              case 0:
                x = i; y = 0;
                while (x >= 0)
                {
                  SetPixelForXY(x, y, section);
                  x--; y++;
                }
                break;

              case 1:
                x = LegoData.DEPTH_CAMERA_WIDTH - i;
                y = 0;
                while (x <= LegoData.DEPTH_CAMERA_WIDTH)
                {
                  SetPixelForXY(x, y, section);
                  x++; y++;
                }
                break;

              case 2:
                x = i;
                y = LegoData.DEPTH_CAMERA_HEIGHT - 1;
                while (x >= 0)
                {
                  SetPixelForXY(x, y, section);
                  x--; y--;
                }
                break;

              case 3:
                x = LegoData.DEPTH_CAMERA_WIDTH - 1 - i;
                y = LegoData.DEPTH_CAMERA_HEIGHT - 1;

                while (x <= LegoData.DEPTH_CAMERA_WIDTH)
                {
                  SetPixelForXY(x, y, section);
                  x++; y--;
                }
                break;
            }
          }
          else if (horizontalCenterLine <= i && i < verticalCenterLine)
          {
            switch (section)
            {
              /*
              case of j = 1:
              This step is scanning display as shown below.
              |--------------------|
              |            ********|
              |          ********  |
              |        ********    |
              |      ********      |
              |--------------------|
              */
              case 0:
                x = i; y = 0;
                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y, section);
                  x--; y++;
                }
                break;

              case 1:
                x = LegoData.DEPTH_CAMERA_WIDTH - i; y = 0;

                while (x <= LegoData.DEPTH_CAMERA_WIDTH - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y, section);
                  x++; y++;
                }
                break;

              case 2:
                x = i; y = LegoData.DEPTH_CAMERA_HEIGHT - 1;

                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y, section);
                  x--; y--;
                }
                break;

              case 3:
                x = (LegoData.DEPTH_CAMERA_WIDTH - i - 1); y = LegoData.DEPTH_CAMERA_HEIGHT - 1;

                while (x <= LegoData.DEPTH_CAMERA_WIDTH - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y, section);
                  x++; y--;
                }
                break;
            }

          }
          else if (horizontalCenterLine < i)
          {
            switch (section)
            {
              /*
              case of j = 1:
              This step is scanning display as shown below.
              |--------------------|
              |                    |
              |                  **|
              |                ****|
              |              ******|
              |--------------------|
              */
              case 0:
                x = verticalCenterLine - 1;
                y = i - verticalCenterLine;
                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y, section);
                  x--; y++;
                }
                break;

              case 1:
                x = verticalCenterLine;
                y = i - verticalCenterLine;

                while (x <= LegoData.DEPTH_CAMERA_WIDTH - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y, section);
                  x++; y++;
                }
                break;

              case 2:
                x = verticalCenterLine;
                y = (LegoData.DEPTH_CAMERA_HEIGHT - 1) - (i - verticalCenterLine);
                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y, section);
                  x--; y--;
                }
                break;

              case 3:
                x = verticalCenterLine;
                y = (LegoData.DEPTH_CAMERA_HEIGHT - 1) - (i - verticalCenterLine);
                while (x <= LegoData.DEPTH_CAMERA_WIDTH - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y, section);
                  x++; y--;
                }
                break;
            }
          }
          else
          {
            Application.Quit();
          }
        }
      }
    }
    #endregion
  }

  public void NextFlag()
  {
    progressFlag_++;
  }

  public void PreviousFlag()
  {
    progressFlag_--;
    if (progressFlag_ < 0) progressFlag_ = 0;
  }
}