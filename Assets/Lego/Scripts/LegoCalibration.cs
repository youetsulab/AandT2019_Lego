/*
[TODO]
範囲がガバイ。
*/

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

struct BasePixelInfo
{
  public int sectionNumber;
  public ushort depth;
  public int x, y;
}

public class LegoCalibration : MonoBehaviour
{
  private float timeLeft__1FPS_, timeLeft__15FPS_, timeLeft__12FPM_;
  private KinectManager manager_;
  [SerializeField] RawImage depthImage_;
  [SerializeField] private int upperBasePixelDepthValue_ = 420;
  [SerializeField] private int lowerBasePixelDepthValue_ = 415;
  [SerializeField] private Text currentlyCalibrationText;
  private Texture2D depthTexture_;
  private Texture2D colorTexture_;
  private ushort[] depthMap_;
  private List<BasePixelInfo> basePixelMap_;
  private Vector3[] baseXYAndDepth_;
  private List<Vector3[]> baseXYAndDepthList_;
  private Vector3 baseCenter_;
  private int progressFlag_;
  private int currentlyCalibratedHierarchy_;


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
    baseXYAndDepthList_ = new List<Vector3[]>();
    baseXYAndDepth_ = new Vector3[4];
    timeLeft__1FPS_ = 1.0f;
    timeLeft__15FPS_ = 0.04f;
    timeLeft__12FPM_ = 5.0f;

    progressFlag_ = 0;
    currentlyCalibratedHierarchy_ = 5;

    currentlyCalibrationText.text = currentlyCalibratedHierarchy_ + "段目のキャリブレーション";
  }

  void Update()
  {
    timeLeft__1FPS_ -= Time.deltaTime;
    timeLeft__15FPS_ -= Time.deltaTime;
    timeLeft__12FPM_ -= Time.deltaTime;

    if (!(manager_ && manager_.IsInitialized())) return;

    //15fps処理
    if (timeLeft__15FPS_ <= 0.0f)
    {
      timeLeft__15FPS_ = 0.04f;

      colorTexture_ = manager_.GetUsersClrTex();
      depthMap_ = manager_.GetRawDepthMap();

      ScanFrom4EndPoint(colorTexture_);
      if (progressFlag_ == 2) depthTexture_.SetPixel((int)baseCenter_.x, (int)baseCenter_.y, new Color(0, 0, 0, 255));
      depthTexture_.Apply();
    }

    //1fps処理
    if (timeLeft__1FPS_ <= 0.0f)
    {
      timeLeft__1FPS_ = 1.0f;

      switch (progressFlag_)
      {
        case 0:
          for (int i = 0; i < 4; i++)
          {
            baseXYAndDepth_[i] = CalcEdgeBaseDepthAverage_And_Point(i);
            Debug.Log("XY" + i + ":" + (int)baseXYAndDepth_[i].x + ", " + (int)baseXYAndDepth_[i].y + " Depth value:" + (int)baseXYAndDepth_[i].z);
          }
          CalcCenterBaseDepth_And_Point();
          Debug.Log("Center XY:" + (int)baseCenter_.x + ", " + (int)baseCenter_.y + "Depth value:" + (int)baseCenter_.z);
          break;

        case 1:
          if (float.IsNaN(baseXYAndDepth_[0].x) || float.IsNaN(baseXYAndDepth_[1].x) || float.IsNaN(baseXYAndDepth_[2].x) || float.IsNaN(baseXYAndDepth_[3].x) || float.IsNaN(baseXYAndDepth_[0].y) || float.IsNaN(baseXYAndDepth_[1].y) || float.IsNaN(baseXYAndDepth_[2].y) || float.IsNaN(baseXYAndDepth_[3].y))
          {
            Debug.Log("キャリブレーションに失敗しました。");
            progressFlag_--;
          }
          else
          {
            baseXYAndDepthList_.Add(baseXYAndDepth_);
            currentlyCalibratedHierarchy_--;
            if (currentlyCalibratedHierarchy_ < 0)
            {
              currentlyCalibrationText.text = "色のデバッグ";
              progressFlag_++;
            }
            else
            {
              currentlyCalibrationText.text = currentlyCalibratedHierarchy_ + "段目のキャリブレーション";
              progressFlag_--;
            }
          }
          break;

        case 2:
          Debug.Log("Color");
          Vector2[] vecXY = new Vector2[4];
          Color[] colorXY = new Color[4];
          HSV[] hsvXY = new HSV[4];
          for (int i = 0; i < 4; i++)
          {
            vecXY[i] = manager_.GetColorMapPosForDepthPos(new Vector2(baseXYAndDepth_[i].x, baseXYAndDepth_[i].y));
            colorXY[i] = colorTexture_.GetPixel((int)vecXY[i].x, (int)vecXY[i].y);
            hsvXY[i] = LegoGeneric.RGB2HSV(colorXY[i]);
            //Debug.Log("XY" + i + ":" + colorXY[i].r + ", " + colorXY[i].g + ", " + colorXY[i].b + ", " + colorXY[i].a);
            Debug.Log("XY" + i + ":" + " H:" + hsvXY[i].h + ", S:" + hsvXY[i].s + ", V:" + hsvXY[i].v);
          }
          Debug.Log("Center XY:" + (int)baseCenter_.x + ", " + (int)baseCenter_.y + "Depth value:" + (depthMap_[(int)baseCenter_.y * LegoData.DEPTH_CAMERA_WIDTH + (int)baseCenter_.x] >> 3));
          Color centerColor = colorTexture_.GetPixel((int)baseCenter_.x, (int)baseCenter_.y);
          HSV centerHsv = LegoGeneric.RGB2HSV(centerColor);
          Debug.Log("Center " + " R:" + centerColor.r + ", G:" + centerColor.g + ", B:" + centerColor.b);
          Debug.Log("Center " + " H:" + centerHsv.h + ", S:" + centerHsv.s + ", V:" + centerHsv.v);
          break;


        case 3:
          LegoData.CalibrationData.PushCalibrationData(baseXYAndDepth_, baseCenter_);
          SceneManager.LoadScene("Main");
          break;

        default:
          LegoData.CalibrationData.PushCalibrationData(baseXYAndDepth_, baseCenter_);
          SceneManager.LoadScene("Main");
          break;
      }
    }
    /*
    //12FPM処理
    if(timeLeft__12FPM_ < 0)
    {
      timeLeft__12FPM_ = 5.0f;

      byte[] pngData = depthTexture_.EncodeToPNG();

      string filePath = EditorUtility.SaveFilePanel("Save Texture", "", depthTexture_.name + ".png", "png");

      if(filePath.Length > 0)
      {
        File.WriteAllBytes(filePath, pngData);
      }
    }    
    */
    basePixelMap_.Clear();
  }

  //4つの点からなる2つの直線の交点を求め、座標と深度を計算する。
  //http://imagingsolution.blog.fc2.com/blog-entry-137.html
  //リンク切れの場合:/Documents/4点からなる交点の求め方
  private void CalcCenterBaseDepth_And_Point()
  {
    if (float.IsNaN(baseXYAndDepth_[0].x) || float.IsNaN(baseXYAndDepth_[1].x) || float.IsNaN(baseXYAndDepth_[2].x) || float.IsNaN(baseXYAndDepth_[3].x)) return;
    if (float.IsNaN(baseXYAndDepth_[0].y) || float.IsNaN(baseXYAndDepth_[1].y) || float.IsNaN(baseXYAndDepth_[2].y) || float.IsNaN(baseXYAndDepth_[3].y)) return;

    float s1 = ((baseXYAndDepth_[3].x - baseXYAndDepth_[1].x) * (baseXYAndDepth_[0].y - baseXYAndDepth_[1].y) - (baseXYAndDepth_[3].y - baseXYAndDepth_[1].y) * (baseXYAndDepth_[0].x - baseXYAndDepth_[1].x)) * 0.5f;
    s1 = Mathf.Abs(s1);
    float s2 = ((baseXYAndDepth_[3].x - baseXYAndDepth_[1].x) * (baseXYAndDepth_[1].y - baseXYAndDepth_[2].y) - (baseXYAndDepth_[3].y - baseXYAndDepth_[1].y) * (baseXYAndDepth_[1].x - baseXYAndDepth_[2].x)) * 0.5f;
    s2 = Mathf.Abs(s2);

    baseCenter_.x = baseXYAndDepth_[0].x + (baseXYAndDepth_[1].x - baseXYAndDepth_[0].x) * s1 / (s1 + s2);
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

      if (lowerBasePixelDepthValue_ < depthData && depthData < upperBasePixelDepthValue_)
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
    progressFlag_ -= 2;
    if (progressFlag_ < 0) progressFlag_ = 0;
  }
}