/*
[TODO]
範囲がガバイ。
*/

using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Calibration : MonoBehaviour
{
  private KinectManager manager_;
  [SerializeField] RawImage depthImage_;
  [SerializeField, Range(0f, 1.0f)] float displayRange_;
  [SerializeField, Range(750f, 1000f)] float upperDisplayRange_;
  [SerializeField, Range(0f, 850f)] float lowerDisplayRange_;

  private Texture2D depthTexture_;
  ushort[] depthMap_;

  struct BasePixelInfo
  {
    ushort depth;
    int x, y;
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

    Vector2 inverseY = new Vector2(1, -1);
    depthImage_.GetComponent<Transform>().localScale *= inverseY;

    depthMap_ = new ushort[LegoGenericData.DEPTH_CAMERA_WIDTH * LegoGenericData.DEPTH_CAMERA_HEIGHT];

    depthTexture_ = new Texture2D(LegoGenericData.DEPTH_CAMERA_WIDTH, LegoGenericData.DEPTH_CAMERA_HEIGHT, TextureFormat.RGBA32, false);
    depthImage_.texture = depthTexture_;
  }

  void Update()
  {
    Texture2D colorTexture = null;

    if (!(manager_ && manager_.IsInitialized())) return;

    colorTexture = manager_.GetUsersClrTex();
    depthMap_ = manager_.GetRawDepthMap();

    ScanFrom4EndPoint(colorTexture);
    depthTexture_.Apply();
  }



  //左上から順番にではなく、4つの端点から中心に向かって走査する
  void ScanFrom4EndPoint(Texture2D colorTexture)
  {
    /*
    ScanFromLeftUp();
    ScanFromRightUp();
    ScanFromLeftLow();
    ScanFromRightLow();
    */
    ScanFrom4EndPoint_Body();

    //[TODO]
    //・読みやすいコードへのリファクタリング
    //・走査範囲の厳密化
    #region LocalFunction
    void SetPixelForXY(int x, int y)
    {
      Color col;
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
        |    1   |    2    |
        |------------------| <- horizontal center line 
        |    3   |    4    |
        |--------|---------|
                 ^
       vertical center line
        number = [j] value
        figure: display
        */

        for (int j = 0; j < 4; j++)
        {
          if (i < horizontalCenterLine)
          {
            switch (j)
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
                  SetPixelForXY(x, y);
                  x--; y++;
                }
                break;

              case 1:
                x = displayWidth - i;
                y = 0;
                while (x <= displayWidth)
                {
                  SetPixelForXY(x, y);
                  x++; y++;
                }
                break;

              case 2:
                x = i;
                y = displayHeight - 1;
                while (x >= 0)
                {
                  SetPixelForXY(x, y);
                  x--; y--;
                }
                break;

              case 3:
                x = displayWidth - 1 - i; 
                y = displayHeight - 1;

                while (x <= displayWidth)
                {
                  SetPixelForXY(x, y);
                  x++; y--;
                }
                break;
            }
          }
          else if (horizontalCenterLine <= i && i < verticalCenterLine)
          {
            switch (j)
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
                  SetPixelForXY(x, y);
                  x--; y++;
                }
                break;

              case 1:
                x = displayWidth - i; y = 0;

                while (x <= displayWidth - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y);
                  x++; y++;
                }
                break;

              case 2:
                x = i; y = displayHeight - 1;

                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y);
                  x--; y--;
                }
                break;

              case 3:
                x = (displayWidth - i - 1); y = displayHeight - 1;

                while (x <= displayWidth - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y);
                  x++; y--;
                }
                break;
            }

          }
          else if (horizontalCenterLine < i)
          {
            switch (j)
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
                  SetPixelForXY(x, y);
                  x--; y++;
                }
                break;

              case 1:
                x = verticalCenterLine; 
                y = i - verticalCenterLine;

                while (x <= displayWidth - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y);
                  x++; y++;
                }
                break;

              case 2:
                x = verticalCenterLine; 
                y = (displayHeight - 1) - (i - verticalCenterLine);
                while (x >= i - horizontalCenterLine)
                {
                  SetPixelForXY(x, y);
                  x--; y--;
                }
                break;

              case 3:
                x = verticalCenterLine; 
                y = (displayHeight - 1) - (i - verticalCenterLine);
                while (x <= displayWidth - (i - horizontalCenterLine))
                {
                  SetPixelForXY(x, y);
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
}