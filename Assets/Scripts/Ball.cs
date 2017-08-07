﻿using UnityEngine;
using GlobalSettings;

public class Ball : MovingPrimitive, IEatable
{
	public bool isAlive { get; private set; }

	IController controller;

    public float radius
    {
        get
        {
            return scale.x / 2;
        }
        private set
        {
            scale = 2 * value * Vector3.one;
        }
    }

    override protected void RestrictPosition(ref Vector3 position)
    {
        if (position.x <= -PlaneData.Width / 2 + radius)
            position.Set(-PlaneData.Width / 2 + radius, 0, position.z);

        if (position.x + radius >= PlaneData.Width / 2)
            position.Set(PlaneData.Width / 2 - radius, 0, position.z);

        if (position.z <= -PlaneData.Height / 2 + radius)
            position.Set(position.x, 0, -PlaneData.Height / 2 + radius);

        if (position.z + radius >= PlaneData.Height / 2)
            position.Set(position.x, 0, PlaneData.Height / 2 - radius);
    }

    override public float volume
    {
        get
        {
            return 4.0f / 3 * Mathf.PI * Mathf.Pow(radius, 3);
        }
        set
        {
            radius = Mathf.Pow(0.75f * value / Mathf.PI, 1.0f / 3);
        }
    }


    /// <summary>
    /// 实例化一个球。
    /// </summary>
    /// <param name="color">球的颜色</param>
    /// <param name="radius">球的半径</param>
    /// <param name="density">球的密度</param>
    /// <param name="plane">球所处的平面</param>
    /// <param name="position">球的生成位置</param>
    /// <param name="isPlayer">是否为玩家</param>
    /// <param name="senseDistance">感知半径，对玩家无效，若为非法值则置为默认值</param>
    /// <param name="senseInterval">感知间隔，对玩家无效，若为非法值则置为默认值</param>
    public Ball(Color color, float radius, Plane plane, Vector3 position, bool isPlayer, float senseDistance = 0, float senseInterval = 0)
        : base(PrimitiveType.Sphere, color, 2 * radius * Vector3.one, plane, position)
    {
        isAlive = true;

        if (isPlayer)
        {
            controller = new Player(this);
            gameObject.name = "Player";
        }
        else
        {
            controller = new AI(this, senseDistance, senseInterval);
            gameObject.name = "Enemy";
        }
    }


    public void Update()
    {
        controller.Control();
        UpdatePosition();
    }


    public float AwayFrom(Ball other)
    {
        return Vector3.Distance(position, other.position) - radius - other.radius;
    }


    public void Eat(IEatable other)
    {
        volume += other.volume;

        // 如果是AI操控，那吃完球后应该停止继续移动，等待下一次感知
        if (controller is AI && other is Ball)
            StopMoving();
    }


    public void BeEaten()
    {
        isAlive = false;
        Object.Destroy(gameObject);
    }
}
