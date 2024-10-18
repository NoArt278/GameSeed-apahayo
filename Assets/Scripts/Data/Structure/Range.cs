using UnityEngine;

[System.Serializable]
public class RangeInt
{
    public int Min;
    public int Max;

    public RangeInt(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public int RandomValue()
    {
        return Random.Range(Min, Max);
    }
}

[System.Serializable]
public class RangeFloat
{
    public float Min;
    public float Max;

    public RangeFloat(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public float RandomValue()
    {
        return Random.Range(Min, Max);
    }
}