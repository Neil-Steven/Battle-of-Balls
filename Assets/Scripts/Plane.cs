using UnityEngine;

public class Plane : Primitive
{
    public Plane(Color color, float width, float height)
        : base(PrimitiveType.Cube, color, new Vector3(width, 1, height), Vector3.zero)
    {
        gameObject.name = "Plane";
    }
}
