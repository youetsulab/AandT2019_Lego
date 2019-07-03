public enum LegoColor
{
  White, Green, Blue, Red, Yellow, YellowishGreen, Brown, Black, Orange, None
}

[System.Serializable]
public struct LegoBlockInfo
{
  public LegoColor legoColor;
  public int height;
}