/**********************************************************
・コメントの「#」はGitHubのイシュー番号に対応するものです。
[YouetusLab](https://github.com/youetsulab/AandT2019_Lego/issues/3)
**********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeCreate : MonoBehaviour
{
  private RawLegoPixelInfo[,] rawLegoMap_;
  private RawLegoPixelInfo[,] trimedLegoMap_;
  private LandscapeCellInfo[,] landscapeMap_;
  private Vector2[] diagonalCalibrationPoint_;

  // Start is called before the first frame update
  void Start()
  {
    diagonalCalibrationPoint_ = new Vector2[2];
    InitializeMap();
  }

  void InitializeMap()
  {
    rawLegoMap_ = new RawLegoPixelInfo[LegoGenericData.DEPTH_CAMERA_WIDTH, LegoGenericData.DEPTH_CAMERA_HEIGHT];

    landscapeMap_ = new LandscapeCellInfo[LegoGenericData.LANDSCAPE_MAP_WIDTH, LegoGenericData.LANDSCAPE_MAP_HEIGHT];
  }

  //#3-1. 
  void FindRectDiagonalPoint()
  {
    Vector2[] calibrationPoint = new Vector2[LegoGenericData.CALIBRATION_POINT_NUM];
  }

  //#3-2.
  void TrimRectangleMap()
  {

  }

  //#3-3.
  void Transform_RawLegoMap2LandscapeMap()
  {

  }

  //#3-4.
  void CreateLandscapeMap()
  {

  }
}
