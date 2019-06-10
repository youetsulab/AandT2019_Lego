using UnityEngine;

enum LegoColor
{
  White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange
}

public struct RawLegoPixelInfo
{
  ushort depth;
  Color color;
}

public struct LandscapeCellInfo
{
  int index;
  LegoColor legoColor;
}

public static class LegoGenericData
{
  public const int DEPTH_CAMERA_WIDTH = 320;
  public const int DEPTH_CAMERA_HEIGHT = 240;
  public const int DEPTH_CAMERA_RESOLUTION = DEPTH_CAMERA_WIDTH * DEPTH_CAMERA_HEIGHT;
  public const int LANDSCAPE_MAP_HEIGHT = 32;
  public const int LANDSCAPE_MAP_WIDTH = 32;
  public const int CALIBRATION_DEPTH = 100;
}