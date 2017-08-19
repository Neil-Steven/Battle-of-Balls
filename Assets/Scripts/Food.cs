using UnityEngine;

public class Food : Primitive, IEatable
{
    public bool isAlive { get; private set; }

    public float volume { get { return scale.x * scale.x * scale.x; } }


    public Food(Color color, float sideLength, Vector3 position)
        : base(PrimitiveType.Cube, color, sideLength * Vector3.one, position)
    {
        isAlive = true;
        gameObject.name = "Food";
    }


    public void BeEaten()
    {
        isAlive = false;
        Object.Destroy(gameObject);
    }
}
