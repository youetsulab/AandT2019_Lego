using UnityEngine;
using UnityEngine.UI;

public class LegoCreateTex : MonoBehaviour
{
  [SerializeField]
  private RawImage legoColorImage_, legoHeightImage_;

  public void CreateTexture(LegoBlockInfo[,] legoBrockMap)
  {
    Texture2D colorTexuture = new Texture2D(LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT, TextureFormat.RGBA32, false);
    Texture2D heightTexuture = new Texture2D(LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT, TextureFormat.RGBA32, false);

    CreateLandScapeColorTexture(legoBrockMap, ref colorTexuture);
    CreateLandScapeHeightTexture(legoBrockMap, ref heightTexuture);

    legoColorImage_.texture = colorTexuture;
    legoHeightImage_.texture = heightTexuture;
  }

  void CreateLandScapeColorTexture(LegoBlockInfo[,] legoBrockMap, ref Texture2D texture)
  {
    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        Color color;
        if (legoBrockMap[x, y].height == 0) color = Color.white;
        else
        {
          switch (legoBrockMap[x, y].legoColor)
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
          texture.SetPixel(x, y, color);
        }
      }
    }
    texture.Apply();
  }

  void CreateLandScapeHeightTexture(LegoBlockInfo[,] legoBrockMap, ref Texture2D texture)
  {
    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        Color color;
        if (legoBrockMap[x, y].height == 0) color = Color.white;
        else
        {
          switch (legoBrockMap[x, y].height)
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
        }
        texture.SetPixel(x, y, color);
      }
    }
    texture.Apply();
  }
}
