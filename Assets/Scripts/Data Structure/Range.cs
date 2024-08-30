using UnityEngine;

[System.Serializable]
public class RangeInt
{
    public int min, max;

    public RangeInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public int RandomValue()
    {
        return Random.Range(min, max);
    }
}

[System.Serializable]
public class RangeFloat
{
    public float min, max;

    public RangeFloat(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float RandomValue()
    {
        return Random.Range(min, max);
    }
}