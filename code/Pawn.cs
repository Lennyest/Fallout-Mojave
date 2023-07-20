using Sandbox;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sandbox;

public partial class Pawn : AnimatedEntity
{

	// All worn clothing items.
	public List<ClothingItem> Clothing = new();

	[Net, Predicted]
	public Weapon ActiveWeapon { get; set; }
	public bool Noclipping = false;
	
	[ClientInput]
	public Vector3 InputDirection { get; set; }

	[ClientInput]
	public Angles ViewAngles { get; set; }

	public Inventory inventory {get; set;}

	public ModelEntity Ragdoll;

	ClothingContainer clothing;

	public bool ThirdPerson;

	/// <summary>
	/// Position a player should be looking from in world space.
	/// </summary>
	[Browsable( false )]
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	[Browsable( false )]
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Rotation EyeLocalRotation { get; set; }

	public BBox Hull
	{
		get => new
		(
			new Vector3( -16, -16, 0 ),
			new Vector3( 16, 16, 64 )
		);
	}

	[BindComponent] public PawnUse UseController { get; }
	[BindComponent] public PawnMovement Controller { get; }
	[BindComponent] public PawnAnimator Animator { get; }


	// Third Person camera distance.
	private float CameraDistance { get; set; } = -25.0f;
	private float CameraDistanceMax = -5.0f;
	private float CameraDistanceMin = -30.0f;

	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );

	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		SetModel( "models/fallout/human/male/test/player.vmdl" );
		SetAnimGraph( "animgraphs/white_male.vanmgrph" );
		
		inventory = new();

		Health = 100;

		var item = new Item();
		item.Name = "Test Item";

		inventory.PickupItem(item);
		inventory.DropItem(item);

		ClothingManager clothingManager = new ClothingManager( this );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		//foreach (var item in Clothing)
		//	item.OnSpawn(this);
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		if (Health - info.Damage <= 0) 
		{
			Log.Info("dead moose meat");
			Die();
		}
	}

	public void Die()
	{
		// Create Ragdoll

		var ragdoll = new ModelEntity();
		ragdoll.SetModel( GetModelName() );
		ragdoll.CopyBodyGroups(this);
		ragdoll.CopyBonesFrom(this);
		ragdoll.CopyMaterialGroup(this);
		ragdoll.Position = Position;
		ragdoll.Rotation = Rotation;
		ragdoll.EnableAllCollisions = true;
		ragdoll.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		ragdoll.RenderColor = RenderColor;
		ragdoll.PhysicsEnabled = true;

		ragdoll.DeleteAsync( 10.0f );

		foreach (var item in Clothing)
			item.OnSpawn(this);

		this.Delete();
		Respawn();
	}

	public void SetActiveWeapon( Weapon weapon )
	{
		ActiveWeapon?.OnHolster();
		ActiveWeapon = weapon;
		ActiveWeapon.OnEquip( this );
	}

	public void Respawn()
	{
		Components.Create<PawnUse>();
		Components.Create<PawnMovement>();
		Components.Create<PawnAnimator>();

		SetActiveWeapon( new Pistol() );

		foreach (var item in Clothing)
			item.OnSpawn(this);
	}

	public override void Simulate( IClient cl )
	{
		if ( cl.Pawn == null || !cl.Pawn.IsValid ) return;

		SimulateRotation();
		Controller?.Simulate( cl );
		Animator?.Simulate();
		UseController?.Simulate();
		ActiveWeapon?.Simulate( cl );
		EyeLocalPosition = Vector3.Up * (64f * Scale);


		// Handles camera distance
		CameraDistance = CameraDistance + Input.MouseWheel * -1;

		Log.Info( CameraDistance );

		if (Input.Pressed(InputButton.PrimaryAttack) && Game.IsServer )
		{
			Ragdoll?.Delete();

			//Die();

			//var mdl = new ModelEntity();
			//mdl.SetModel( "models/fallout/world/remnantsbunker/bunker.vmdl" );
			//mdl.Position = cl.Pawn.Position + cl.Pawn.Rotation.Forward * 40f;
			//Ragdoll = mdl;

			Health -= 10;


			if (Health <= 0) Health = 100;
		}

		if (Input.Pressed(InputButton.SecondaryAttack))
		{
			Noclipping = !Noclipping;
		}

		if (Input.Pressed(InputButton.Reload)) 
		{
			ThirdPerson = !ThirdPerson;

			if (!ThirdPerson)
				Camera.FirstPersonViewer = this;
			else
				Camera.FirstPersonViewer = null;
		}
	}

	public override void BuildInput()
	{
		var cl = Client;
		var headPos = GetBoneTransform( GetBoneIndex( "Bip01_Head" ) ).Position;

		Camera.Rotation = ViewAngles.ToRotation(); // Rotation.Lerp( Camera.Rotation, ViewAngles.ToRotation(), Time.Delta * 50f );

		if ( ThirdPerson )
			Camera.Position = Vector3.Lerp(Camera.Position, headPos + cl.Pawn.Rotation.Forward * CameraDistance + cl.Pawn.Rotation.Up * 10f + cl.Pawn.Rotation.Right * 10, Time.Delta * 25);
		else
			Camera.Position = headPos + ViewAngles.Forward * 2f;

		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		if ( !ThirdPerson )
		{
			EnableHideInFirstPerson = true;
			Camera.FirstPersonViewer = this;
			foreach ( var child in Children )
			{
				child.EnableHideInFirstPerson = true;
			}
		}

		InputDirection = Input.AnalogMove;

		if ( Input.StopProcessing )
			return;

		var look = Input.AnalogLook;

		if ( ViewAngles.pitch > 90f || ViewAngles.pitch < -90f )
		{
			look = look.WithYaw( look.yaw * -1f );
		}

		var viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -89f, 89f );
		viewAngles.roll = 0f;
		ViewAngles = viewAngles.Normal;

	}

	public override void FrameSimulate( IClient cl )
	{
		SimulateRotation();

		if (Game.IsServer) return;

	}

	public TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
	{
		return TraceBBox( start, end, Hull.Mins, Hull.Maxs, liftFeet );
	}

	public TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var tr = Trace.Ray( start, end )
					.Size( mins, maxs )
					.WithAnyTags( "solid", "playerclip", "passbullets" )
					.Ignore( this )
					.Run();

		return tr;
	}

	float test;
	Vector3 zero = new Vector3(-0, -0, -0);
	protected void SimulateRotation()
	{
		var lastRotation = Rotation;

		EyeRotation = ViewAngles.ToRotation();
		Rotation = ViewAngles.WithPitch( 0f ).ToRotation();


		if (Time.Now >= test) 
		{
			var difference = Rotation.Difference(lastRotation, Rotation);
			var angles = difference.Angles();
			var velocity = Velocity;

			if (velocity.AlmostEqual(zero, 0.0001f)) 
			{

				//Log.Info(angles);

				if (angles.yaw > 1 || angles.yaw < -1) 
				{
					Log.Info( "rotating" );
					SetAnimParameter("Rotating", true);
				}
				// Moving
			}

			test = Time.Now + .1f;
		}

	}
}

