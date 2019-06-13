using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//[TODO]
//キャリブレーションをしない選択をした場合でも、まだキャリブレーションがされていない場合はキャリブレーションを実行する。
public class Init : MonoBehaviour
{
  private LegoBase lego_ = LegoBase.Instance;
  [SerializeField]
  private Text text_;

  public void OnClickButton_yes()
  {
    text_.text = "Calibration画面へ移行します。";
    lego_.IsInitialized = true;
    StartCoroutine(DelayMethod(3.5f, () =>
    {
      SceneManager.LoadScene("Calibration");
    }));
  }

  public void OnClickButton_no()
  {
    lego_.IsInitialized = true;
    if (lego_.IsCalibrated)
    {
      text_.text = "Main画面へ移行します。";
      StartCoroutine(DelayMethod(3.5f, () =>
      {
        SceneManager.LoadScene("Main");
      }));
    }
    else
    {
      text_.text = "Calibration Dataがありません。\nCalibration画面に移行します。";
      StartCoroutine(DelayMethod(3.5f, () =>
      {
        SceneManager.LoadScene("Calibration");
      }));
    }
  }

  private IEnumerator DelayMethod(float waitTime, Action action)
  {
    yield return new WaitForSeconds(waitTime);
    action();
  }
}
