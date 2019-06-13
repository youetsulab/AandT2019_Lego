using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//[TODO]
//キャリブレーションをしない選択をした場合でも、まだキャリブレーションがされていない場合はキャリブレーションを実行する。
public class Init : MonoBehaviour
{
  public void OnClickButton_yes()
  {
    SceneManager.LoadScene("Calibration");
  }

  public void OnClickButton_no()
  {

  }
}
