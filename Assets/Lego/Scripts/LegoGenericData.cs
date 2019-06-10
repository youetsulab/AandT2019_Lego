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
