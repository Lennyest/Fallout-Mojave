namespace Sandbox;

public class PawnUse : EntityComponent<Pawn>
{
    public void Simulate()
    {
		if (Input.Pressed(InputButton.Use))
		{
			var start = Entity.GetBoneTransform(Entity.GetBoneIndex("Bip01_Head")).Position;
			var end = Entity.Position + Entity.ViewAngles.Normal.Forward * 100;
			var tr = Trace.Ray( start, end ).Run();

			DebugOverlay.Line(start, end, Color.Red, 1);
				
			Log.Info(tr.Entity);
		}
    }
}
