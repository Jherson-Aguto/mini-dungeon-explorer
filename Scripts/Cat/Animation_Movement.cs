using Godot;
using System;

public partial class Animation_Movement : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	private AnimatedSprite2D _animations;
	private Vector2 _velocity;
	private float _direction;
	private _Actions _currentAction = _Actions.Standing;
	private enum _Actions { Standing, Walking, Sitting, Sleeping }
	private Timer _sit;
	private Timer _Sleep;

	public override void _Ready()
	{
		_velocity = Velocity;

		_animations = GetNode<AnimatedSprite2D>("Animations");
		_animations.Play("cat_idle_stand");

		_sit = GetNode<Timer>("Sit");
		_Sleep = GetNode<Timer>("Sleep");

		_sit.WaitTime = 5;
		_sit.OneShot = true;
		_Sleep.WaitTime = 10;
		_Sleep.OneShot = true;

		_sit.Timeout += () =>
		{
			_currentAction = _Actions.Sitting;
			_animations.Play("cat_idle_sit");
			_Sleep.Start();
		};
		_Sleep.Timeout += () =>
		{
			_currentAction = _Actions.Sleeping;
			_animations.Play("cat_idle_sleep");
		};

		_sit.Start();

		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		Movement();
		Behavior();
		MoveAndSlide();
	}

	private void Movement()
	{
		_direction = Input.GetAxis("move_left", "move_right");

		_velocity.X = _direction * Speed;

		Velocity = _velocity;
	}


	private void Behavior()
	{
		if (_direction != 0)
		{
			_animations.FlipH = _direction < 0;
			if (_currentAction != _Actions.Walking)
			{
				StopIdleBehavior();
				_currentAction = _Actions.Walking;
				_animations.Play("cat_walk");
			}
		}
		else
		{
			if (_currentAction == _Actions.Walking)
			{
				_currentAction = _Actions.Standing;
				_animations.Play("cat_idle_stand");
				IdleBehavior();
			}
		}
	}

	private void StopIdleBehavior()
	{
		_sit.Stop();
		_Sleep.Stop();
	}

	private void IdleBehavior()
	{
		if (_currentAction != _Actions.Sitting)
			_sit.Start();
	}

}
