using UnityEngine;

/// <summary>
/// 在感知范围内的球。
/// </summary>
public struct SensibleBall
{
    public float radius { get; private set; }
    public Vector3 position { get; private set; }
    public float distance { get; private set; }
    public bool exists { get; private set; }

    public SensibleBall(float radius, Vector3 position, float distance)
    {
        this.radius = radius;
        this.position = position;
        this.distance = distance;
        exists = false;
    }

    public void Set(float radius, Vector3 position, float distance)
    {
        this.radius = radius;
        this.position = position;
        this.distance = distance;
        exists = true;
    }

    public void Reset()
    {
        radius = 0;
        position = Vector3.zero;
        distance = float.MaxValue;
        exists = false;
    }
}
