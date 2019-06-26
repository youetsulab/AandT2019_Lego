using UnityEngine;

public static class LegoObjects
{
  //Road
  public static GameObject road_straight, road_intersection_T, road_intersection_X, road_curve, road_stop, road_crossWalk;

  //Water
  public static GameObject water_river;

  public static void LoadGameObjects()
  {
    road_straight = (GameObject)Resources.Load("Road/Road_Straight");
    road_intersection_T = (GameObject)Resources.Load("Road/Road_Intersection_T");
    road_intersection_X = (GameObject)Resources.Load("Road/Road_Intersection_X");
    road_curve = (GameObject)Resources.Load("Road/Road_Curve");
    road_stop = (GameObject)Resources.Load("Road/Road_Stop");
    road_crossWalk = (GameObject)Resources.Load("Road/Road_Crosswalk");

    water_river = (GameObject)Resources.Load("Water/River");
  }
}
