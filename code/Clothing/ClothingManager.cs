using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	class ClothingManager
	{
		public enum Slot
		{
			Head,
			Torso,
			Arms,
			Legs,
			None,
		}
		List<string> ClothingOptions = new() {  };

		public ClothingManager() {}

		public static void ShowAllParts(Pawn pawn)
		{
			pawn.SetBodyGroup( "torso", 1 );
			pawn.SetBodyGroup( "limbs", 1 );
			pawn.SetBodyGroup( "head", 1 );
		}
		public static void HideAllParts( Pawn pawn )
		{
			pawn.SetBodyGroup( "head", 0 );
			pawn.SetBodyGroup( "limbs", 0 );
			pawn.SetBodyGroup( "torso", 0 );
			pawn.SetBodyGroup( "head", 0 );
		}

		public ClothingManager(Pawn pawn) 
		{
			//CreateClothing(pawn, "models/fallout/human/clothing/enclavecoat/coat.vmdl");
			//CreateClothing(pawn, "models/fallout/human/powerarmor/vetranger.vmdl");
			//ShowAllParts( pawn );
		}

		public void CreateBasicClothing(Pawn pawn)
		{
			// Underwear, basic arms, head

			Log.Info( "Created defaults");
		}

		public static void CreateClothing(Pawn pawn, string Model, bool Bonemerge = true, bool bNoRemove = false)
		{
			var clothing = new AnimatedEntity();
			clothing.SetModel( Model );
			clothing.SetParent( pawn, Bonemerge );
			clothing.Tags.Add( "clothing" );

			if ( bNoRemove )
				clothing.Tags.Add( "noremove" );

			Log.Info( "Created clothing : " + Model + " and parented to " + pawn.Name );
		}


		public static void EquipResourceAsset(Pawn pawn, ClothingResource resource)
		{
			CreateClothing( pawn, resource.Model );

			var hideparts = resource.HidePart;

			switch ( hideparts )
			{
				case ClothingResource.HideParts.None:
					Log.Info( "showing all parts" );
					ShowAllParts( pawn ); 
					break;

				case ClothingResource.HideParts.All:
					Log.Info( "hid all parts" );
					ShowAllParts( pawn );
					break;

				case ClothingResource.HideParts.AllButHead:
					HideAllParts( pawn );
					pawn.SetBodyGroup( "head", 1 );
					Log.Info( "shown all parts but head" );
					break;

				default: 
					break;
			}
		}

		[ConCmd.Server( "clothing_add" )]
		public static void WearClothing( string parameter )
		{
			var pawn = ConsoleSystem.Caller.Pawn as Pawn;
			if ( pawn == null ) { Log.Info( "No pawn." ); return; }

			var resource = ResourceLibrary.Get<ClothingResource>( $"armor/{parameter}.armor" );

			if (resource == null)
			{
				Log.Info( "Failed to find clothing, ensure you have reloaded the gamemode if you have added a new asset." );
				return;
			}

			Log.Info( "creating clothing for asset : " + resource.Name );
			
			EquipResourceAsset(pawn, resource);
		}

		[ConCmd.Server( "clothing_clear" )]
		public static void ClearClothing(  )
		{
			var pawn = ConsoleSystem.Caller.Pawn as Pawn;
			if ( pawn == null ) { Log.Info( "No pawn." ); return; }

			var children = pawn.Children;

			if ( children == null ) return;

			foreach (var child in children)
			{
				if ( !child.IsValid ) continue;

				if (child.Tags.Has("clothing") && !child.Tags.Has("noremove"))
				{
					Log.Info( "Deleted clothing: " + child.Name);
					child.Delete();
				}
			}

			Log.Info( "Finished deleting clothing." );
			Log.Info( "Showing all parts." );

			ShowAllParts( pawn );
		}

		[ConCmd.Server( "clothing_bg" )]
		public static void ClothingSetBG(int group, int num)
		{
			var pawn = ConsoleSystem.Caller.Pawn as Pawn;
			if ( pawn == null ) { Log.Info( "No pawn." ); return; }


			pawn.SetBodyGroup( group, num );
		}

		[ConCmd.Server( "clothing_bgs" )]
		public static void ClothingSetBG( string group, int num )
		{
			var pawn = ConsoleSystem.Caller.Pawn as Pawn;
			if ( pawn == null ) { Log.Info( "No pawn." ); return; }


			pawn.SetBodyGroup( group, num );
		}
	}
}
