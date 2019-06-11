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

  // Update is called once per frame
  /*
  void Update()
  {
    Texture2D colorTexture = null;
    Color col;

    if (!(manager_ && manager_.IsInitialized())) return;

    colorTexture = manager_.GetUsersClrTex();
    depthMap_ = manager_.GetRawDepthMap();

    for (int y = 0; y < LegoGenericData.DEPTH_CAMERA_HEIGHT; y++)
    {
      for (int x = 0; x < LegoGenericData.DEPTH_CAMERA_WIDTH; x++)
      {
        int depthData = depthMap_[y * LegoGenericData.DEPTH_CAMERA_WIDTH + x] >> 3;
        float monoNum = (float)(depthData) / 3975f;
        if (lowerDisplayRange_ < depthData && depthData < upperDisplayRange_)
        {
          col = new Color(0, 0, 0, 255);
        }
        else if (monoNum >= displayRange_ && (colorTexture != null))
        {
          Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x, y));
          col = colorTexture.GetPixel((int)posColor.x, (int)posColor.y);
        }
        else
        {
          col = new Color(monoNum, monoNum, monoNum, 1.0f);
        }
        depthTexture_.SetPixel(x, y, col);

      }
    }
    depthTexture_.Apply();
  }
  */

  void Update()
  {
    Texture2D colorTexture = null;

    if (!(manager_ && manager_.IsInitialized())) return;

    colorTexture = manager_.GetUsersClrTex();
    depthMap_ = manager_.GetRawDepthMap();

    //画面の4分の1ずつ描画し、矩形の端点を見つける。
    //左上
    for (int i = 0; i < (LegoGenericData.DEPTH_CAMERA_WIDTH / 2 + LegoGenericData.DEPTH_CAMERA_HEIGHT / 2); i++)
    {

      /*
      この部分の描写
      ----------------------
      |**********          |
      |********            |
      |******              |
      |****                |
      |**                  |
      ----------------------
       */
      if (i < LegoGenericData.DEPTH_CAMERA_HEIGHT / 2)
      {
        int x = i, y = 0;

        while (x >= 0)
        {
          SetPixelForXY(x, y, colorTexture);

          x--; y++;
        }
      }

      /*
      この部分の描写
      ----------------------
      |          **********|
      |        **********  |
      |      **********    |
      |    **********      |
      |  **********        |
      ----------------------
       */
      else if (LegoGenericData.DEPTH_CAMERA_HEIGHT / 2 < i && i < LegoGenericData.DEPTH_CAMERA_WIDTH / 2)
      {
        int x = i, y = 0;

        while (x >= i - LegoGenericData.DEPTH_CAMERA_HEIGHT / 2)
        {
          SetPixelForXY(x, y, colorTexture);

          x--; y++;
        }
      }

      /*
      この部分の描写
      ----------------------
      |                    |
      |                  **|
      |                ****|
      |              ******|
      |           *********|
      ----------------------
       */
      else if (LegoGenericData.DEPTH_CAMERA_HEIGHT / 2 < i)
      {
        int x = LegoGenericData.DEPTH_CAMERA_WIDTH / 2, y = i - LegoGenericData.DEPTH_CAMERA_WIDTH / 2;

        while (x >= i - LegoGenericData.DEPTH_CAMERA_HEIGHT / 2)
        {
          SetPixelForXY(x, y, colorTexture);

          x--; y++;
        }
      }
      else
      {
        Application.Quit();
      }
    }
    depthTexture_.Apply();
  }

  void SetPixelForXY(int x, int y, Texture2D colorTexture)
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
}
