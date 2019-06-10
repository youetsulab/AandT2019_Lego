using UnityEngine;

/*
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
*/

public static class LegoGenericData
{
  public static readonly int DEPTH_CAMERA_WIDTH = 640;
  public static readonly int DEPTH_CAMERA_HEIGHT = 480;
  public static readonly int LANDSCAPE_MAP_HEIGHT = 32;
  public static readonly int LANDSCAPE_MAP_WIDTH = 32;
  public static readonly int CALIBRATION_DEPTH = 100;
  public static readonly int NUM_CALIBRATION_POINT = 4;
}