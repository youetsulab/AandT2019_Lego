using UnityEngine;

public static class LegoObjects
{
  private static bool isLoaded = false;

  public static bool IsLoaded{
    get{return isLoaded;}
  }
  //Road
  public static GameObject road_straight, road_intersection_T, road_intersection_X, road_curve, road_stop, road_crossWalk, bridge;

  //Building
  public static GameObject building_1, building_2;
  public static GameObject eiffelTower;

  //Water
  public static GameObject river_straight, river_curve, river_intersection_T;

  //Nature
  public static GameObject forest_1;

  //Space
  public static GameObject space;

  public static void LoadGameObjects()
  {
    //Road
    road_straight = (GameObject)Resources.Load("Road/Road_Straight");
    road_intersection_T = (GameObject)Resources.Load("Road/Road_Intersection_T");
    road_intersection_X = (GameObject)Resources.Load("Road/Road_Intersection_X");
    road_curve = (GameObject)Resources.Load("Road/Road_Curve");
    road_stop = (GameObject)Resources.Load("Road/Road_Stop");
    road_crossWalk = (GameObject)Resources.Load("Road/Road_Crosswalk");
    bridge = (GameObject)Resources.Load("Road/Bridge");

    //Building
    building_1 = (GameObject)Resources.Load("Building/Building_1");
    building_2 = (GameObject)Resources.Load("Building/Building_2");
    eiffelTower = (GameObject)Resources.Load("Building/EiffelTower");

    //Water
    river_straight = (GameObject)Resources.Load("Water/River_Straight");
    river_curve = (GameObject)Resources.Load("Water/River_Curve");
    river_intersection_T = (GameObject)Resources.Load("Water/River_Intersection_T");

    //Nature
    forest_1 = (GameObject)Resources.Load("Nature/Forest_1");

    //Space
    space = (GameObject)Resources.Load("Space/Space");

    isLoaded = true;
  }
}
