using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LegoColor
{
  White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange
}

public struct RawLegoMapInfo
{
  ushort depth_;
  Color color_;
}

public struct LandscapeMapInfo
{
  int index_;
  LegoColor legoColor_;
}

public class LandscapeCreate : MonoBehaviour
{
  private int LANDSCAPE_MAP_WIDTH = 32;
  private int LANDSCAPE_MAP_HEIGHT = 32;
  private KinectManager manager_;
  private RawLegoMapInfo[,] RawLegoMap_;
  private LandscapeMapInfo[,] LandscapeMap_;

  // Start is called before the first frame update
  void Start()
  {
    RawLegoMap_ = new RawLegoMapInfo[KinectWrapper.GetDepthWidth(), KinectWrapper.GetDepthHeight()];
    LandscapeMap_ = new LandscapeMapInfo[LANDSCAPE_MAP_WIDTH, LANDSCAPE_MAP_HEIGHT];
  }

  // Update is called once per frame
  void Update()
  {

  }
}
