using Sandbox;
namespace Sandbox;
public partial class ClothingItem : AnimatedEntity
{

	public enum HideParts
	{
		None = 0,
		Torso = 1,
		Legs = 2,
		Arms = 3,
		Head = 4,
		Hair = 5,
		Accessories = 6,
	}

    public Pawn Pawn => Owner as Pawn;
    // Model of item when created
	public virtual string ModelPath => null;

    // Item name, display name
    public virtual string ClothingName => null;

    // Inventory sizes
    public virtual byte Width => 0;
    public virtual byte Height => 0;

    public ClothingItem() {
        Pawn?.Clothing.Add(this);
    }

    ~ClothingItem()
    {
        Pawn?.Clothing.Remove(this);
    } 

    // What slot the item should be taking.
    //public virtual ClothingManager.Slot Slot => ClothingManager.Slot.None;

    // Called when the player respawns while they have this in their inventory.
    public virtual void OnSpawn(Pawn pawn) {}
    // Called when the player dies while having this item in their inventory.
    public virtual void OnDeath(Pawn pawn) {}
}
