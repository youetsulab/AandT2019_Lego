using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LegoColor
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

public class LegoBase : MonoBehaviour
{
  protected static readonly int DEPTH_CAMERA_WIDTH = 320;
  protected static readonly int DEPTH_CAMERA_HEIGHT = 240;
  protected static readonly int LANDSCAPE_MAP_HEIGHT = 32;
  protected static readonly int LANDSCAPE_MAP_WIDTH = 32;
  protected static readonly int CALIBRATION_DEPTH = 100;
  protected static readonly int NUM_CALIBRATION_POINT = 4;
  protected bool isCalibrated = false;
}
