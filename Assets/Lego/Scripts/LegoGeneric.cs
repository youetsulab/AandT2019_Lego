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

    switch (Max_rgb(rgb))
    {
      case LegoColor.Red:
        max = (int)rgb.r;
        break;

      case LegoColor.Blue:
        max = (int)rgb.b;
        break;

      case LegoColor.Green:
        max = (int)rgb.g;
        break;

      default:
        max = (int)rgb.r;
        break;
    }

    switch (Min_rgb(rgb))
    {
      case LegoColor.Red:
        min = (int)rgb.r;
        break;

      case LegoColor.Blue:
        min = (int)rgb.b;
        break;

      case LegoColor.Green:
        min = (int)rgb.g;
        break;

      default:
        min = (int)rgb.r;
        break;
    }

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
        if (min == (int)rgb.b) hsv.h = 60 * (int)(rgb.g - rgb.r) / (max - min) + 60;
        else if (min == (int)rgb.r) hsv.h = 60 * (int)(rgb.b - rgb.g) / (max - min) + 180;
        else if (min == (int)rgb.g) hsv.h = 60 * (int)(rgb.r - rgb.b) / (max - min) + 300;
        else Application.Quit();
      }

      if (hsv.h < 0) hsv.h += 360;
      else if (hsv.h > 360) hsv.h -= 360;
    }

    return hsv;


  }

  public static LegoColor Max_rgb(Color c)
  {
    if (c.r > c.g)
    {
      if (c.r > c.b) return LegoColor.Red;
      else return LegoColor.Blue;
    }
    else
    {
      if (c.g > c.b) return LegoColor.Green;
      else return LegoColor.Blue;
    }
  }

  public static LegoColor Min_rgb(Color c)
  {
    if (c.r < c.g)
    {
      if (c.r < c.b) return LegoColor.Red;
      else return LegoColor.Blue;
    }
    else
    {
      if (c.g < c.b) return LegoColor.Green;
      else return LegoColor.Blue;
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
