using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LegoInit : MonoBehaviour
{
  [SerializeField]
  private Text text_;

  void Start()
  {
    if (LegoData.CalibrationData.HasCalibrationData())
    {
      LegoData.isCalibrated = true;
    }

    if (!LegoObjects.IsLoaded) LegoObjects.LoadGameObjects();
  }

  public void OnClickButton_yes()
  {
    text_.text = "Calibration画面へ移行します。";
    LegoData.isInitialized = true;
    StartCoroutine(LegoGeneric.DelayMethod(3.5f, () =>
    {
      SceneManager.LoadScene("Calibration");
    }));
  }

  public void OnClickButton_no()
  {
    LegoData.isInitialized = true;
    if (LegoData.isCalibrated)
    {
      text_.text = "Main画面へ移行します。";
      StartCoroutine(LegoGeneric.DelayMethod(3.5f, () =>
      {
        SceneManager.LoadScene("Main");
      }));
    }
    else
    {
      text_.text = "Calibration Dataがありません。\nCalibration画面に移行します。";
      StartCoroutine(LegoGeneric.DelayMethod(3.5f, () =>
      {
        SceneManager.LoadScene("Calibration");
      }));
    }
  }
}
