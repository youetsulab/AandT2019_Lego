using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LandscapeType_OverView
{
  Building, Water, Nature, Road, Spaces,
}

enum LandscapeType_Details
{                                                                                                           //LandscapeType_Overview
  House, Shop, Skyscraper/*TokyoTower*/,                                                                    //Building
  River_Straight, River_Curve, River_Intersection_T, Sea, Fountain,                                         //Water
  Forest, Park,                                                                                             //Nature
  Road_Straight, Road_Curve, Road_Intersection_T, Road_Intersection_X, Road_Stop, Road_CrossWalk, Bridge,   //Road
  Space                                                                                                     //Spaces
}

enum Direction
{
  North, South, East, West
}

class LandscapeLegoInfo
{
  public/*private*/ LandscapeType_OverView myType_Detail_OverView;
  public LandscapeType_OverView north, south, east, west;
  public/*private*/ LandscapeType_Details myType_Detail;
  public int height;
  public/*private*/ Direction myDirection;

    public LandscapeLegoInfo(LegoColor lc)
  {
    SetLegoType_OverView(lc);
        myType_Detail = LandscapeType_Details.Space;
    north = south = east = west = LandscapeType_OverView.Spaces;
    myDirection = Direction.North;
    height = 0;
  }

  void SetLegoType_OverView(LegoColor legoColor)
  {
    switch (legoColor)
    {
      case LegoColor.Blue:
        myType_Detail_OverView = LandscapeType_OverView.Water;
        break;

      case LegoColor.Green:
        myType_Detail_OverView = LandscapeType_OverView.Nature;
        break;

      case LegoColor.Red:
        myType_Detail_OverView = LandscapeType_OverView.Building;
        break;

      case LegoColor.White:
        myType_Detail_OverView = LandscapeType_OverView.Road;
        break;

      /*case LegoColor.None:
        myType_Detail_OverView = LandscapeType_OverView.Spaces;
        break;*/

      default:
        myType_Detail_OverView = LandscapeType_OverView.Spaces;
        break;
    }
  }
}

public class LegoCreateLandscape : MonoBehaviour
{
  private LandscapeLegoInfo[,] landscapeLegoMap_ = new LandscapeLegoInfo[LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT];
  private LegoCreateTex legoCreateTex_;
  // Start is called before the first frame update
  void Start()
  {
    LegoBlockInfo[,] legoBlockMap = JsonHelper_TwodimensionalArray.LoadJson<LegoBlockInfo>("savedata2.json");
    legoCreateTex_ = gameObject.GetComponent<LegoCreateTex>();
    legoCreateTex_.CreateTexture(legoBlockMap);
        ConvertLegoBlockInfo2LandscapeInfo(legoBlockMap);
        UpdateLandscapeMap();
  }

  void ConvertLegoBlockInfo2LandscapeInfo(LegoBlockInfo[,] legoBlockMap)
  {
    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        landscapeLegoMap_[x, y] = new LandscapeLegoInfo(legoBlockMap[x, y].legoColor);
        landscapeLegoMap_[x, y].height = legoBlockMap[x, y].height;
        if (x == 0 || y == 0)
        {
          if (x == 0 && y == 0) continue;
          else if (x == 0) landscapeLegoMap_[x, y].north = landscapeLegoMap_[x, y - 1].myType_Detail_OverView;
          else if (y == 0) landscapeLegoMap_[x, y].west = landscapeLegoMap_[x - 1, y].myType_Detail_OverView;
        }
        else
        {
          landscapeLegoMap_[x, y].west = landscapeLegoMap_[x - 1, y].myType_Detail_OverView;
          landscapeLegoMap_[x - 1, y].east = landscapeLegoMap_[x, y].myType_Detail_OverView;
          landscapeLegoMap_[x, y].north = landscapeLegoMap_[x, y - 1].myType_Detail_OverView;
          landscapeLegoMap_[x, y - 1].south = landscapeLegoMap_[x, y].myType_Detail_OverView;
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
    void UpdateLandscapeMap()
    {
        //・建造物の向きの判定
        //・交差点・T字路、立体交差の判定
        //・橋の判定
        //・建造物の連立、高さによるタイプ判定
        //・川、海、池、噴水等の判定
        //・孤立している場合の種類判定
        //

        for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
        {
            for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
            {
                if (landscapeLegoMap_[x, y].height != 0)
                {
                    switch (landscapeLegoMap_[x, y].myType_Detail_OverView)
                    {
                        case LandscapeType_OverView.Building:
                            landscapeLegoMap_[x, y].myType_Detail = SetBuildingDetails(landscapeLegoMap_[x, y]);
                            landscapeLegoMap_[x, y].myDirection = SetBuildingDirection(landscapeLegoMap_[x, y]);
                            break;

                        case LandscapeType_OverView.Water:
                            landscapeLegoMap_[x, y].myType_Detail = SetWaterDetails(landscapeLegoMap_[x, y]);
                            SetSeaDetails(x, y);
                            landscapeLegoMap_[x, y].myDirection = SetWaterDirection(landscapeLegoMap_[x, y]);
                            break;

                        case LandscapeType_OverView.Nature:
                            landscapeLegoMap_[x, y].myType_Detail = SetNatureDetails(landscapeLegoMap_[x, y]);
                            landscapeLegoMap_[x, y].myDirection = Direction.North;
                            break;

                        case LandscapeType_OverView.Road:
                            landscapeLegoMap_[x, y].myType_Detail = SetRoadDetails(landscapeLegoMap_[x, y]);
                            landscapeLegoMap_[x, y].myDirection = SetRoadDirection(landscapeLegoMap_[x, y]);
                            break;

                        case LandscapeType_OverView.Spaces:
                            landscapeLegoMap_[x, y].myType_Detail = LandscapeType_Details.Space;
                            landscapeLegoMap_[x, y].myDirection = Direction.North;
                            break;

                        default:
                            break;
                    }
                }

                if (landscapeLegoMap_[x, y].myType_Detail_OverView != LandscapeType_OverView.Spaces)
                {
                    Debug.Log("x:" + x + " y:" + y + " Detail:" + landscapeLegoMap_[x, y].myType_Detail + " Direction:" + landscapeLegoMap_[x, y].myDirection);
                }
            }
        }
    }

    LandscapeType_Details SetBuildingDetails(LandscapeLegoInfo landscapeLegoMap)
    {
        if (landscapeLegoMap.height >= 2 && landscapeLegoMap.height < 5)
            return LandscapeType_Details.Shop;
        else if(landscapeLegoMap.height == 5)
            return LandscapeType_Details.Skyscraper;
        else
            return LandscapeType_Details.House;
    }

    Direction SetBuildingDirection(LandscapeLegoInfo landscapeLegoMap)
    {
        if (landscapeLegoMap.north == LandscapeType_OverView.Road)
            return Direction.North;
        else if (landscapeLegoMap.south == LandscapeType_OverView.Road)
            return Direction.South;
        else if (landscapeLegoMap.east == LandscapeType_OverView.Road)
            return Direction.East;
        else if (landscapeLegoMap.west == LandscapeType_OverView.Road)
            return Direction.West;
        else
            return Direction.North;
    }

    LandscapeType_Details SetWaterDetails(LandscapeLegoInfo landscapeLegoMap)
    {
        if (landscapeLegoMap.north == LandscapeType_OverView.Water && landscapeLegoMap.height == 1)
        {
            if (landscapeLegoMap.south == LandscapeType_OverView.Water)
            {
                if (landscapeLegoMap.east == LandscapeType_OverView.Water || landscapeLegoMap.west == LandscapeType_OverView.Water)
                    return LandscapeType_Details.River_Intersection_T;//n & s & e == 1 or n & s & w == 1 or n & s & e & w == 1

                return LandscapeType_Details.River_Straight;//n & s == 1
            }
            else if (landscapeLegoMap.east == LandscapeType_OverView.Water && landscapeLegoMap.west == LandscapeType_OverView.Water)
                return LandscapeType_Details.River_Intersection_T;//n & e & w == 1
            else if (landscapeLegoMap.east == LandscapeType_OverView.Water || landscapeLegoMap.west == LandscapeType_OverView.Water)
                return LandscapeType_Details.River_Curve;//n & e == 1 or n & w == 1

            return LandscapeType_Details.River_Straight;//n == 1
        }
        else if (landscapeLegoMap.south == LandscapeType_OverView.Water)
        {
            if (landscapeLegoMap.east == LandscapeType_OverView.Water && landscapeLegoMap.west == LandscapeType_OverView.Water)
                return LandscapeType_Details.River_Intersection_T;//s & e & w == 1
            else if (landscapeLegoMap.east == LandscapeType_OverView.Water || landscapeLegoMap.west == LandscapeType_OverView.Water)
                return LandscapeType_Details.River_Curve;//s & e == 1 or s & w == 1

            return LandscapeType_Details.River_Straight;//s == 1
        }
        /*else if ((landscapeLegoMap.north == LandscapeType_OverView.Water || landscapeLegoMap.south == LandscapeType_OverView.Water || landscapeLegoMap.east == LandscapeType_OverView.Water
                 || landscapeLegoMap.west == LandscapeType_OverView.Water) && landscapeLegoMap.height == 2)
            return LandscapeType_Details.Sea;*/
        else if (landscapeLegoMap.height >= 2)
            return LandscapeType_Details.Fountain;
        else
            return LandscapeType_Details.River_Straight;//e == 1 or e & w == 1 or w == 1 or 0
    }

    void SetSeaDetails(int x, int y)
    {
        if (landscapeLegoMap_[x, y].myType_Detail == LandscapeType_Details.River_Curve || landscapeLegoMap_[x, y].myType_Detail == LandscapeType_Details.River_Intersection_T)
        {
            if (landscapeLegoMap_[x - 1, y].myType_Detail == LandscapeType_Details.River_Curve || landscapeLegoMap_[x - 1, y].myType_Detail == LandscapeType_Details.River_Intersection_T 
                || landscapeLegoMap_[x, y - 1].myType_Detail == LandscapeType_Details.River_Curve || landscapeLegoMap_[x, y - 1].myType_Detail == LandscapeType_Details.River_Intersection_T 
                || landscapeLegoMap_[x + 1, y].myType_Detail == LandscapeType_Details.River_Curve || landscapeLegoMap_[x + 1, y].myType_Detail == LandscapeType_Details.River_Intersection_T 
                || landscapeLegoMap_[x, y + 1].myType_Detail == LandscapeType_Details.River_Curve || landscapeLegoMap_[x, y + 1].myType_Detail == LandscapeType_Details.River_Intersection_T)
                landscapeLegoMap_[x, y].myType_Detail = LandscapeType_Details.Sea;
        }
    }

    Direction SetWaterDirection(LandscapeLegoInfo landscapeLegoMap)
    {
        if (landscapeLegoMap.north == LandscapeType_OverView.Water)
            return Direction.North;
        else if (landscapeLegoMap.south == LandscapeType_OverView.Water)
            return Direction.South;
        else if (landscapeLegoMap.east == LandscapeType_OverView.Water)
            return Direction.East;
        else if (landscapeLegoMap.west == LandscapeType_OverView.Water)
            return Direction.West;
        else
            return Direction.North;
    }

    LandscapeType_Details SetNatureDetails(LandscapeLegoInfo landscapeLegoMap)
    {
        if (landscapeLegoMap.north != LandscapeType_OverView.Nature && landscapeLegoMap.south != LandscapeType_OverView.Nature && landscapeLegoMap.east != LandscapeType_OverView.Nature
            && landscapeLegoMap.west != LandscapeType_OverView.Nature)
            return LandscapeType_Details.Park;
        else
            return LandscapeType_Details.Forest;
    }

    LandscapeType_Details SetRoadDetails(LandscapeLegoInfo landscapeLegoMap)
    {
        if ((landscapeLegoMap.north == LandscapeType_OverView.Water && landscapeLegoMap.south == LandscapeType_OverView.Water) || (landscapeLegoMap.east == LandscapeType_OverView.Water
            && landscapeLegoMap.west == LandscapeType_OverView.Water))
            return LandscapeType_Details.Bridge;
        else if (landscapeLegoMap.north == LandscapeType_OverView.Road)
        {
            if (landscapeLegoMap.south == LandscapeType_OverView.Road)
            {
                if (landscapeLegoMap.east == LandscapeType_OverView.Road && landscapeLegoMap.west == LandscapeType_OverView.Road)
                    return LandscapeType_Details.Road_Intersection_X;//n & s & e & w == 1
                else if (landscapeLegoMap.east == LandscapeType_OverView.Road || landscapeLegoMap.west == LandscapeType_OverView.Road)
                    return LandscapeType_Details.Road_Intersection_T;//n & s & e == 1 or n & s & w == 1

                return LandscapeType_Details.Road_Straight;//n & s == 1
            }
            else if (landscapeLegoMap.east == LandscapeType_OverView.Road && landscapeLegoMap.west == LandscapeType_OverView.Road)
                return LandscapeType_Details.Road_Intersection_T;//n & e & w == 1
            else if (landscapeLegoMap.east == LandscapeType_OverView.Road || landscapeLegoMap.west == LandscapeType_OverView.Road)
                return LandscapeType_Details.Road_Curve;//n & e == 1 or n & w == 1

            return LandscapeType_Details.Road_Straight;//n == 1
        }
        else if (landscapeLegoMap.south == LandscapeType_OverView.Road)
        {
            if (landscapeLegoMap.east == LandscapeType_OverView.Road && landscapeLegoMap.west == LandscapeType_OverView.Road)
                return LandscapeType_Details.Road_Intersection_T;//s & e & w == 1
            else if (landscapeLegoMap.east == LandscapeType_OverView.Road || landscapeLegoMap.west == LandscapeType_OverView.Road)
                return LandscapeType_Details.Road_Curve;//s & e == 1 or s & w == 1

            return LandscapeType_Details.Road_Straight;//s == 1
        }
        else
            return LandscapeType_Details.Road_Straight;
    }

    Direction SetRoadDirection(LandscapeLegoInfo landscapeLegoMap)
    {
        if (landscapeLegoMap.north == LandscapeType_OverView.Road)
            return Direction.North;
        else if (landscapeLegoMap.south == LandscapeType_OverView.Road)
            return Direction.South;
        else if (landscapeLegoMap.east == LandscapeType_OverView.Road)
            return Direction.East;
        else if (landscapeLegoMap.west == LandscapeType_OverView.Road)
            return Direction.West;
        else
            return Direction.North;
    }
}
