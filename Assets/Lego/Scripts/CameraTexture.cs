using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

namespace doRA.LegoLand.CameraTexture
{
  public class CameraTexture : MonoBehaviour
  {
    const int DEPTH_CAMERA_WIDTH = 640;
    const int DEPTH_CAMERA_HEIGHT = 480;
    const int DEPTH_CAMERA_RESOLUTION = DEPTH_CAMERA_WIDTH * DEPTH_CAMERA_HEIGHT;
    [SerializeField] RawImage colorImage_;
    [SerializeField] RawImage depthImage_;
    [SerializeField, Range(0f, 1.0f)] float displayRange_;
    private Texture2D depthTexture_;
    ushort[] depthMap_;

    // Start is called before the first frame update
    void Start()
    {
      if (depthImage_ == null)
      {
        Debug.LogError("Depth image is not attaced.", depthImage_);
        Application.Quit();
      }

      if (colorImage_ == null)
      {
        Debug.LogError("Color image is not attaced.", colorImage_);
        Application.Quit();
      }

      Vector2 inverseY = new Vector2(1, -1);
      colorImage_.GetComponent<Transform>().localScale *= inverseY;
      depthImage_.GetComponent<Transform>().localScale *= inverseY;

      depthMap_ = new ushort[DEPTH_CAMERA_RESOLUTION];

      depthTexture_ = new Texture2D(DEPTH_CAMERA_WIDTH, DEPTH_CAMERA_HEIGHT, TextureFormat.RGBA32, false);
      depthImage_.texture = depthTexture_;
    }

    // Update is called once per frame
    void Update()
    {
      KinectManager manager = KinectManager.Instance;
      Texture2D colorTexture = null;
      Color col;

      if (!(manager && manager.IsInitialized())) return;

      if (colorImage_ && (colorImage_.texture == null))
      {
        colorImage_.texture = manager.GetUsersClrTex();
      }

      colorTexture = (Texture2D)colorImage_.texture;

      depthMap_ = manager.GetRawDepthMap();

      for (int y = 0; y < DEPTH_CAMERA_HEIGHT; y++)
      {
        for (int x = 0; x < DEPTH_CAMERA_WIDTH; x++)
        {
          float monoNum = (float)(depthMap_[y * DEPTH_CAMERA_WIDTH + x] >> 3) / 3975f;
          if (monoNum > displayRange_ && (colorTexture != null))
          {
            Vector2 posColor = manager.GetColorMapPosForDepthPos(new Vector2(x, y));
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
  }
}
