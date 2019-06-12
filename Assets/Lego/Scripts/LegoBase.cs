using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LegoColor
{
    White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange
}
public struct RawLegoPixelInfo
{
    public ushort depth;
    public Color color;
}

public struct LandscapeCellInfo
{
    int index;
    LegoColor legoColor;
}

public class LegoBase : MonoBehaviour
{
    protected static readonly int DEPTH_CAMERA_WIDTH = 320;
    protected static readonly int DEPTH_CAMERA_HEIGHT = 240;
    protected static readonly int LANDSCAPE_MAP_HEIGHT = 32;
    protected static readonly int LANDSCAPE_MAP_WIDTH = 32;
    protected static readonly int CALIBRATION_DEPTH = 100;
    protected static readonly int NUM_CALIBRATION_POINT = 4;
    protected bool isCalibrated = false;

    public RawLegoPixelInfo[,] Gettexturedata()
    {
        KinectManager manager_;
        Texture2D colorTexture_;
        RawImage colorImage_ = null;

        manager_ = KinectManager.Instance;
        colorTexture_ = new Texture2D(KinectWrapper.Constants.DepthImageWidth, KinectWrapper.Constants.DepthImageHeight, TextureFormat.RGBA32, false);

        Vector2 inverseY = new Vector2(1, -1);
        colorImage_.GetComponent<Transform>().localScale *= inverseY;

        colorImage_.texture = manager_.GetUsersClrTex();
        colorTexture_ = (Texture2D)colorImage_.texture;

        RawLegoPixelInfo[,] cameramap = new RawLegoPixelInfo[KinectWrapper.Constants.DepthImageWidth, KinectWrapper.Constants.DepthImageHeight];

        for (int y = 0; y < KinectWrapper.Constants.DepthImageHeight; y++)
        {
            for (int x = 0; x < KinectWrapper.Constants.DepthImageWidth; x++)
            {
                cameramap[x, y].depth = manager_.GetDepthForPixel(x, y);

                Vector2 posColor = manager_.GetColorMapPosForDepthPos(new Vector2(x, y));
                cameramap[x, y].color = colorTexture_.GetPixel((int)posColor.x, (int)posColor.y);
            }
        }

        return cameramap;
    }
}