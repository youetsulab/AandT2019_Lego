using UnityEngine;

enum LegoColor
{
  White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange
}

public struct RawLegoPixelInfo
{
  ushort depth_;
  Color color_;
}

public struct LandscapeCellInfo
{
  int index_;
  LegoColor legoColor_;
}