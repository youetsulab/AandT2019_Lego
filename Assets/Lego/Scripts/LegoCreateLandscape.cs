using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LandscapeType_OverView
{
  Building, Water, Nature, Road, Spaces,
}
enum LandscapeType_Details
{                         //LandscapeType_Overview
  House, Shop, Landmark,  //Building
  River, Sea,             //Water
  Forest, Park,           //Nature
  Road, Bridge, Crossroad,//Road 
  Space                   //Spaces
}

enum Direction
{
  North, South, East, West
}

class LandscapeLegoInfo
{
  private LandscapeType_OverView own;
  public LandscapeType_OverView north, south, east, west;
  private LandscapeType_Details myType;
  public int height;
  private Direction myDirection;

  #region Accessor
  public LandscapeType_OverView Own
  {
    get { return own; }
  }
  #endregion

  public LandscapeLegoInfo(LegoColor lc)
  {
    SetLegoType_OverView(lc);
    north = south = east = west = LandscapeType_OverView.Spaces;
    myDirection = Direction.North;
    height = 0;
  }

  void SetLegoType_OverView(LegoColor legoColor)
  {
    switch (legoColor)
    {
      case LegoColor.Blue:
        own = LandscapeType_OverView.Water;
        break;

      case LegoColor.Green:
        own = LandscapeType_OverView.Nature;
        break;

      case LegoColor.Red:
        own = LandscapeType_OverView.Building;
        break;

      case LegoColor.White:
        own = LandscapeType_OverView.Road;
        break;

      default:
        own = LandscapeType_OverView.Spaces;
        break;
    }
  }
}

public class LegoCreateLandscape : MonoBehaviour
{
  private LandscapeLegoInfo[,] landscapeLegoMap_ = new LandscapeLegoInfo[LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT];
  // Start is called before the first frame update
  void Start()
  {
    ConvertLegoBlockInfo2LandscapeInfo();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void ConvertLegoBlockInfo2LandscapeInfo()
  {
    LegoBlockInfo[,] legoBlockMap = LegoData.legoMap;

    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        landscapeLegoMap_[x, y] = new LandscapeLegoInfo(legoBlockMap[x, y].legoColor);
        landscapeLegoMap_[x, y].height = legoBlockMap[x, y].floor;
        if (x == 0 || y == 0)
        {
          if (x == 0 && y == 0) continue;
          else if (x == 0) landscapeLegoMap_[x, y].north = landscapeLegoMap_[x, y - 1].Own;
          else if (y == 0) landscapeLegoMap_[x, y].west = landscapeLegoMap_[x - 1, y].Own;
        }
        else
        {
          landscapeLegoMap_[x, y].west = landscapeLegoMap_[x - 1, y].Own;
          landscapeLegoMap_[x - 1, y].east = landscapeLegoMap_[x, y].Own;
          landscapeLegoMap_[x, y].north = landscapeLegoMap_[x, y - 1].Own;
          landscapeLegoMap_[x, y - 1].south = landscapeLegoMap_[x, y].Own;
        }
      }
    }
  }
}
