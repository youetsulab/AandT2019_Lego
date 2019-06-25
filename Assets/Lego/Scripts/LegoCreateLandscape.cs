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
    //ConvertLegoBlockInfo2LandscapeInfo();
    JsonHelper_TwodimensionalArray.LoadJson<int>("s.json");
  }

  void ConvertLegoBlockInfo2LandscapeInfo()
  {
    //LegoBlockInfo[,] legoBlockMap = LegoData.legoMap;
    LegoBlockInfo[,] legoBlockMap = JsonHelper_TwodimensionalArray.LoadJson<LegoBlockInfo>("s.json");

    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        landscapeLegoMap_[x, y] = new LandscapeLegoInfo(legoBlockMap[x, y].legoColor);
        landscapeLegoMap_[x, y].height = legoBlockMap[x, y].height;
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

  /// <summary>
  /// 景観作成
  /// ・各オブジェクトの向きを指定
  /// ・各オブジェクトの種類の詳細を指定
  /// ・高さによる種類の変化を設定
  /// ・他オブジェクトとの隣接関係を設定
  /// 例：
  /// ・橋や道路、交差点など、高さが違う場合は立体交差は歩道橋など
  /// ・１マスのみの場合や孤立している場合のことも考える。
  /// ・２マス分の建物
  /// ・川や海、高さが違う場合は滝など
  /// ・水のマスが１マスのみの場合は噴水など
  /// </summary>
  void CreateLandscape()
  {
  }
}
