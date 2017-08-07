using UnityEngine;

public class Player : IController
{
    Ball owner;

    public Player(Ball owner)
    {
        this.owner = owner;
    }

    public void Control()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(h, 0, v);
        owner.MoveTowards(direction);
    }
}
