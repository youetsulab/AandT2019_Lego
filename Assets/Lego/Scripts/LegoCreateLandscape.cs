using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LandscapeType
{
  Building, River, Forest, Road, Crossroad
}

struct LandscapeLegoInfo
{
  public LandscapeType thisType, leftLegoType, rightLegoType, upLegoType, downLegoType;
}

public class LegoCreateLandscape : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  LandscapeLegoInfo[,] ConvertLegoBlockInfo2LandscapeInfo()
  {
    LegoBlockInfo[,] legoBlockMap = LegoData.legoMap;
    LandscapeLegoInfo[,] landscapeLegoMap = new LandscapeLegoInfo[LegoData.LANDSCAPE_MAP_WIDTH, LegoData.LANDSCAPE_MAP_HEIGHT];

    for (int y = 0; y < LegoData.LANDSCAPE_MAP_HEIGHT; y++)
    {
      for (int x = 0; x < LegoData.LANDSCAPE_MAP_WIDTH; x++)
      {
        if (x == 0 || y == 0)
        {
          if (x == 0 && y == 0)
          {
            landscapeLegoMap[x, y].thisType
          }
          else if (x == 0)
          {

          }
          else if (y == 0)
          {

          }
        }
        else
        {

        }

      }
    }

    return landscapeLegoMap;

    LandscapeType DiscriminateLegoType(LegoColor legoColor)
    {
      switch (legoColor)
      {
          
          default:
      }
    }
  }
}
