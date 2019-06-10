using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Calibration : MonoBehaviour
{
  private KinectManager manager_;
  [SerializeField] RawImage depthImage_;
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

    manager_ = KinectManager.Instance;

    Vector2 inverseY = new Vector2(1, -1);
    depthImage_.GetComponent<Transform>().localScale *= inverseY;

    depthMap_ = new ushort[LegoGenericData.DEPTH_CAMERA_WIDTH * LegoGenericData.DEPTH_CAMERA_HEIGHT];

    depthTexture_ = new Texture2D(LegoGenericData.DEPTH_CAMERA_WIDTH, LegoGenericData.DEPTH_CAMERA_HEIGHT, TextureFormat.RGBA32, false);
    depthImage_.texture = depthTexture_;
  }

  // Update is called once per frame
  void Update()
  {
    Color col;

    if (!(manager_ && manager_.IsInitialized())) return;

    depthMap_ = manager_.GetRawDepthMap();

    for (int y = 0; y < LegoGenericData.DEPTH_CAMERA_HEIGHT; y++)
    {
      for (int x = 0; x < LegoGenericData.DEPTH_CAMERA_WIDTH; x++)
      {
        float monoNum = (float)(depthMap_[y * LegoGenericData.DEPTH_CAMERA_WIDTH + x] >> 3) / 3975f;
        col = new Color(monoNum, monoNum, monoNum, 255);
        depthTexture_.SetPixel(x, y, col);
      }
    }
    depthTexture_.Apply();
  }
}
