using UnityEngine;

public class Primitive {

    protected GameObject gameObject;

    public Color color
    {
        get
        {
            if (gameObject.GetComponent<Renderer>().material.shader != Shader.Find("Unlit/Color"))
                return Color.white;

            return gameObject.GetComponent<Renderer>().material.color;
        }
        protected set
        {
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
            gameObject.GetComponent<Renderer>().material.color = value;
        }
    }

    public Vector3 scale
    {
        get { return gameObject.transform.localScale; }
        protected set {  gameObject.transform.localScale = value; }
    }

    public Vector3 position
    {
        get { return gameObject.transform.localPosition; }
        protected set { gameObject.transform.localPosition = value; }
    }


    public Primitive(PrimitiveType type, Color color, Vector3 scale, Vector3 position)
    {
        gameObject = GameObject.CreatePrimitive(type);
        this.color = color;
        this.scale = scale;
        this.position = position;
    }
}
