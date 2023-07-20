using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox;
public class PawnAnimator : EntityComponent<Pawn>, ISingletonComponent
{
	private void ResetAll()
	{
		Entity.SetAnimParameter( "RunningBackwards", false );
		Entity.SetAnimParameter( "RunningRight", false );
		Entity.SetAnimParameter( "RunningLeft", false );
		Entity.SetAnimParameter( "WalkingBackwards", false );
		Entity.SetAnimParameter( "Walking", false );
		Entity.SetAnimParameter( "WalkingLeft", false );
		Entity.SetAnimParameter( "WalkingRight", false );
		Entity.SetAnimParameter( "Running", false );
		Entity.SetAnimParameter( "Jump", false );
		Entity.SetAnimParameter( "Sneak", false );
	}
	public void Simulate()
	{
		if (Entity == null || !Entity.IsValid) return;

		if ( Input.Down( InputButton.Reload) && Input.Down(InputButton.Flashlight) )
		{
			Entity.SetAnimParameter( "Reload", true );
		}
		
		if ( Input.Down( InputButton.Run ) && Input.Down( InputButton.Forward ) )
		{
			// Running
			ResetAll();
			Entity.SetAnimParameter( "Walking", true );
		}
		else if ( Input.Down( InputButton.Forward ) && !Input.Down( InputButton.Run ) )
		{
			// Walking
			ResetAll();
			Entity.SetAnimParameter( "Running", true );
		}
		else if ( Input.Down( InputButton.Back ) && !Input.Down( InputButton.Run ) )
		{
			// Walking backwards
			ResetAll();
			Entity.SetAnimParameter( "RunningBackwards", true );

		}
		else if ( Input.Down( InputButton.Back ) && Input.Down( InputButton.Run ) )
		{
			ResetAll();
			Entity.SetAnimParameter( "WalkingBackwards", true );
		}
		else if ( Input.Down( InputButton.Left ) && !Input.Down( InputButton.Run ))
		{
			ResetAll();
			Entity.SetAnimParameter( "RunningLeft", true );

		}
		else if ( Input.Down( InputButton.Right ) && !Input.Down( InputButton.Run ))
		{
			ResetAll();
			Entity.SetAnimParameter( "RunningRight", true );
		} 
		else if (Input.Down( InputButton.Right ) && Input.Down( InputButton.Run )) 
		{
			ResetAll();
			Entity.SetAnimParameter( "WalkingRight", true );
		}
		else if (Input.Down( InputButton.Left ) && Input.Down( InputButton.Run )) // Add check to see if they are in air 
		{
			ResetAll();
			Entity.SetAnimParameter( "WalkingLeft", true );
		}
		else if ( Input.Down( InputButton.Jump) ) 
		{
			ResetAll();
			Entity.SetAnimParameter( "Jump", true );
		}
		else if ( Input.Down( InputButton.Duck ))
		{
			ResetAll();
			Entity.SetAnimParameter("Sneak", true);
		}
		else
		{
			// We are not moving.
			ResetAll();
		}

		//if ( Entity.Controller.HasEvent( "jump" ) )
		//{
		//	helper.TriggerJump();
		//}
	}
}
