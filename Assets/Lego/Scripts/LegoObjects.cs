using UnityEngine;

public static class LegoObjects
{
  //Road
  public static GameObject road_straight, road_intersection_T, road_intersection_X, road_curve, road_stop, road_crossWalk, road_bridge;

  //Building
  public static GameObject building_1, building_2;

  //Water
  public static GameObject water_river_straight_1, water_river_straight_2, water_river_curve;

  //Nature
  public static GameObject Nature_forest_1;

  public static void LoadGameObjects()
  {
    //Road
    road_straight = (GameObject)Resources.Load("Road/Road_Straight");
    road_intersection_T = (GameObject)Resources.Load("Road/Road_Intersection_T");
    road_intersection_X = (GameObject)Resources.Load("Road/Road_Intersection_X");
    road_curve = (GameObject)Resources.Load("Road/Road_Curve");
    road_stop = (GameObject)Resources.Load("Road/Road_Stop");
    road_crossWalk = (GameObject)Resources.Load("Road/Road_Crosswalk");
    road_bridge = (GameObject)Resources.Load("Road/Road_Bridge");

    //Building
    building_1 = (GameObject)Resources.Load("Building/Building_1");
    building_2 = (GameObject)Resources.Load("Building/Building_2");

    //Water
    water_river_straight_1 = (GameObject)Resources.Load("Water/Water_River_Straight_1");
    water_river_straight_2 = (GameObject)Resources.Load("Water/Water_River_Straight_2");
    water_river_curve = (GameObject)Resources.Load("Water/Water_River_Curve");

    //Nature
    Nature_forest_1 = (GameObject)Resources.Load("Nature/Forest_1");
  }
}
