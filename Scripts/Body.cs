using Godot;

public partial class Body : CharacterBody2D
{
	[Export]
	public float SPEED = 300.0f;
	private AnimatedSprite2D _Animations;
	private Timer _Sit;
	private Timer _LayDown;
	private _ActionEnum _Action = _ActionEnum.Stand;
	private enum _ActionEnum { Stand, Sit, LayDown, Walking }
	private float Direction;

	public override void _Ready()
	{
		_Animations = GetNode<AnimatedSprite2D>("Animations");
		_Sit = GetNode<Timer>("Sit");
		_LayDown = GetNode<Timer>("LayDown");

		_Sit.WaitTime = 5.0;
		_Sit.OneShot = true;

		_LayDown.WaitTime = 10.0;
		_LayDown.OneShot = true;

		_Sit.Timeout += () =>
		{
			_Action = _ActionEnum.Sit;
			_Animations.Play("idle_sit");
			_LayDown.Start();
		};

		_LayDown.Timeout += () =>
		{
			_Action = _ActionEnum.LayDown;
			_Animations.Play("idle_sleep");
		};

		_Sit.Start();
		_Animations.Play("idle_front");

		base._Ready();
	}


	public override void _PhysicsProcess(double delta)
	{
		ProcessMovement();
		MoveAndSlide();
	}


	private void ProcessMovement()
	{
		Vector2 velocity = Velocity;

		Direction = Input.GetAxis("move_left", "move_right");

		velocity.X = Direction * SPEED;

		Velocity = velocity;

		_PlayAnimation();
	}
	private void _MovementDirection()
	{
		_Animations.FlipH = Direction switch
		{
			> 0 => false,
			< 0 => true,
			_ => _Animations.FlipH
		};
	}

	private void _SpriteAnimations()
	{
		if (Direction != 0)
		{
			_Sit.Stop();
			_LayDown.Stop();

			if (_Action != _ActionEnum.Walking)
			{
				_Action = _ActionEnum.Walking;
				_Animations.Play("cat_walk");
			}
		}
		else
		{
			if (_Action == _ActionEnum.Walking)
			{
				_Action = _ActionEnum.Stand;
				_Animations.Play("idle_front");
				_Sit.Start();
			}
		}
	}

	private void _PlayAnimation()
	{
		_MovementDirection();
		_SpriteAnimations();
	}
}
