using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

public struct HSV
{
  //h:0-360 s:0-256 v:-256
  public int h;
  public int s;
  public int v;
}

public static class LegoGeneric
{

  //h:0-360 s:0f-1f v:0f-1f
  //[FIXME]
  public static HSV RGB2HSV(Color rgb)
  {
    rgb.r *= 255;
    rgb.g *= 255;
    rgb.b *= 255;

    HSV hsv = new HSV();
    int min, max;

    max = Max_rgb(rgb);
    min = Min_rgb(rgb);
    hsv.v = max;

    if (hsv.v == 0f) hsv.s = hsv.h = 0;
    else
    {
      hsv.s = 255 * (max - min) / max;

      //黒用の判別
      if ((Mathf.Abs(max - min) < 30 && max < 50) || max == min)
      {
        hsv.h = 0;
        hsv.s = 0;
        hsv.v = 0;
      }
      else
      {

        if (max == (int)rgb.r) hsv.h = 60 * (int)(rgb.b - rgb.g) / (max - min);
        else if (max == (int)rgb.g) hsv.h = 60 * (int)(rgb.r - rgb.b) / (max - min) + 120;
        else if (max == (int)rgb.b) hsv.h = 60 * (int)(rgb.g - rgb.r) / (max - min) + 240;
        else Application.Quit();
      }

      if (hsv.h < 0) hsv.h += 360;
      else if (hsv.h > 360) hsv.h -= 360;
    }

    return hsv;

    int Max_rgb(Color c)
    {
      if (c.r > c.g)
      {
        if (c.r > c.b) return (int)c.r;
        else return (int)c.b;
      }
      else
      {
        if (c.g > c.b) return (int)c.g;
        else return (int)c.b;
      }
    }

    int Min_rgb(Color c)
    {
      if (c.r < c.g)
      {
        if (c.r < c.b) return (int)c.r;
        else return (int)c.b;
      }
      else
      {
        if (c.g < c.b) return (int)c.g;
        else return (int)c.b;
      }
    }
  }

  public static T CalcMode<T>(T[] array, int numberOfType)
  {
    Dictionary<T, int> dictionary = new Dictionary<T, int>(numberOfType);

    foreach (var item in array)
    {
      if (dictionary.ContainsKey(item)) dictionary[item]++;
      else
      {
        dictionary.Add(item, 0);
      }
    }

    return dictionary.OrderBy(val => val.Value).First().Key;
  }

  public static IEnumerator DelayMethod(float waitTime, Action action)
  {
    yield return new WaitForSeconds(waitTime);
    action();
  }
}
