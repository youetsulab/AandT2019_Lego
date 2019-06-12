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
public class Calibration : MonoBehaviour
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
  private ushort[] depthMap_;
  private List<BasePixelInfo> basePixelMap_;
  private List<Vector3> baseCoordinateAndDepth;

  private struct BasePixelInfo
  {
    public int sectionNumber;
    public ushort depth;
    public int x, y;
  }

  // Start is called before the first frame update
  void Start()
  {
    if (depthImage_ == null)
    {
      Debug.LogError("[Depth Image] is not attaced.", depthImage_);
      Application.Quit();
    }

    manager_ = KinectManager.Instance;
    lego_ = LegoBase.GetInstance();

    Vector2 inverseY = new Vector2(1, -1);
    depthImage_.GetComponent<Transform>().localScale *= inverseY;

    depthMap_ = new ushort[LegoGenericData.DEPTH_CAMERA_WIDTH * LegoGenericData.DEPTH_CAMERA_HEIGHT];

    depthTexture_ = new Texture2D(LegoGenericData.DEPTH_CAMERA_WIDTH, LegoGenericData.DEPTH_CAMERA_HEIGHT, TextureFormat.RGBA32, false);
    depthImage_.texture = depthTexture_;

    basePixelMap_ = new List<BasePixelInfo>();
    baseCoordinateAndDepth = new List<Vector3>();
    timeLeft__1FPS_ = 1.0f;
    timeLeft__15FPS_ = 0.04f;
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

      Texture2D colorTexture = null;

      colorTexture = manager_.GetUsersClrTex();
      depthMap_ = manager_.GetRawDepthMap();

      ScanFrom4EndPoint(colorTexture);
      depthTexture_.Apply();
    }

    //1fps処理
    if (timeLeft__1FPS_ <= 0.0f)
    {
      timeLeft__1FPS_ = 1.0f;
      baseCoordinateAndDepth.Clear();
      baseCoordinateAndDepth.Add(CalcBaseDepthAverageAndPoint(0));
      baseCoordinateAndDepth.Add(CalcBaseDepthAverageAndPoint(1));
      baseCoordinateAndDepth.Add(CalcBaseDepthAverageAndPoint(2));
      baseCoordinateAndDepth.Add(CalcBaseDepthAverageAndPoint(3));
      Debug.Log("Coordinate0:" + baseCoordinateAndDepth[0].x + ", " + baseCoordinateAndDepth[0].y + " Depth value:" + baseCoordinateAndDepth[0].z);
      Debug.Log("Coordinate1:" + baseCoordinateAndDepth[1].x + ", " + baseCoordinateAndDepth[1].y + " Depth value:" + baseCoordinateAndDepth[1].z);
      Debug.Log("Coordinate2:" + baseCoordinateAndDepth[2].x + ", " + baseCoordinateAndDepth[2].y + " Depth value:" + baseCoordinateAndDepth[2].z);
      Debug.Log("Coordinate3:" + baseCoordinateAndDepth[3].x + ", " + baseCoordinateAndDepth[3].y + " Depth value:" + baseCoordinateAndDepth[3].z);
      basePixelMap_.Clear();
    }
  }

  private Vector3 CalcBaseDepthAverageAndPoint(int sectionNum)
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
    ScanFrom4EndPoint_Body();

    //[TODO]
    //・読みやすいコードへのリファクタリング
    //・走査範囲の厳密化
    #region LocalFunction
    void SetPixelForXY(int x, int y, int sectionNum)
    {
      Color col;
      BasePixelInfo pixel;
      int depthData = depthMap_[y * LegoGenericData.DEPTH_CAMERA_WIDTH + x] >> 3;

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
    }

    void ScanFrom4EndPoint_Body()
    {
      int x, y;
      int displayWidth = LegoGenericData.DEPTH_CAMERA_WIDTH;
      int displayHeight = LegoGenericData.DEPTH_CAMERA_HEIGHT;
      int horizontalCenterLine = displayHeight / 2;
      int verticalCenterLine = displayWidth / 2;

      for (int i = 0; i < (displayWidth / 2) + (LegoGenericData.DEPTH_CAMERA_HEIGHT / 2); i++)
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
                x = displayWidth - i;
                y = 0;
                while (x <= displayWidth)
                {
                  SetPixelForXY(x, y, section);
                  x++; y++;
                }
                break;

              case 2:
                x = i;
                y = displayHeight - 1;
                while (x >= 0)
                {
                  SetPixelForXY(x, y, section);
                  x--; y--;
                }
                break;

              case 3:
                x = displayWidth - 1 - i;
                y = displayHeight - 1;

                while (x <= displayWidth)
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
                x = displayWidth - i; y = 0;

                while (x <= displayWidth - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y, section);
                  x++; y++;
                }
                break;

              case 2:
                x = i; y = displayHeight - 1;

                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y, section);
                  x--; y--;
                }
                break;

              case 3:
                x = (displayWidth - i - 1); y = displayHeight - 1;

                while (x <= displayWidth - (i - horizontalCenterLine))
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

                while (x <= displayWidth - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y, section);
                  x++; y++;
                }
                break;

              case 2:
                x = verticalCenterLine;
                y = (displayHeight - 1) - (i - verticalCenterLine);
                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y, section);
                  x--; y--;
                }
                break;

              case 3:
                x = verticalCenterLine;
                y = (displayHeight - 1) - (i - verticalCenterLine);
                while (x <= displayWidth - (i - horizontalCenterLine))
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
    # endregion
  }

  public void CompleteCalibration()
  {
    lego_.PushCalibrationValue(baseCoordinateAndDepth);
    SceneManager.LoadScene("Main");
  }
}