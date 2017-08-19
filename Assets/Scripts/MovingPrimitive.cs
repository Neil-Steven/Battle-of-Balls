using UnityEngine;
using GlobalSettings;

public abstract class MovingPrimitive : Primitive
{
	protected enum State
	{
		Idle,
		Move
	}

	protected State state = State.Idle;
	protected Plane plane;

	public new Vector3 position
	{
		get { return base.position; }
		protected set 
        {
            RestrictPosition(ref value);
            base.position = value;
		}
	}

	protected float velocity;
	protected Vector3 direction;
	protected Vector3 move_direction;

	/// <summary>
	/// 球的加速度(v = at, t固定)
	/// </summary>
	protected float acceleration
	{
		get
		{
			if (direction != Vector3.zero)
				return maxVelocity / BallData.AccelerateTime;
			else
				return -maxVelocity / BallData.AccelerateTime;
		}
	}

	/// <summary>
	/// 当前球的最大速度
	/// </summary>
	protected float maxVelocity { get { return 20f - scale.x * 0.1f; } }

	abstract public float volume { get; set; }



	protected MovingPrimitive(PrimitiveType type, Color color, Vector3 scale, Plane plane, Vector3 position)
		: base(type, color, scale, position)
	{
		this.plane = plane;
	}

	abstract protected void RestrictPosition(ref Vector3 position);

	protected void UpdatePosition()
	{
		if (direction != Vector3.zero)
			move_direction = direction;
		if (velocity == 0)
			move_direction = Vector3.zero;

		velocity += acceleration * Time.deltaTime;       // v = v0 + at

		if (velocity > maxVelocity)
			velocity = maxVelocity;
		else if (velocity < 0)
			velocity = 0;

		// 如果没有目标地点，则将小球视为处在Idle状态
		if (direction == Vector3.zero)
			state = State.Idle;
		else
			state = State.Move;

		position += move_direction.normalized * velocity * Time.deltaTime;
	}

	public void MoveTowards(Vector3 direction)
	{
		this.direction = direction;
	}

	public void StopMoving()
	{
		direction = Vector3.zero;
	}
}
