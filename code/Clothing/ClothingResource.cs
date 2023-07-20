using System.Collections.Generic;
using Sandbox;
namespace Sandbox;

[GameResource("Armor Resource", "armor", "Describes an item of clothing and implicitly which other items it can be worn with."), Icon( "🦶" ) ]
public partial class ClothingResource : GameResource
{

    [Description("Display name of item."), Category("Basic Information")]
    public string Name {get; set;}
    [ResourceType("vmdl"), Category("Basic Information")]
    public string Model {get; set;}

    [Description("Displayed description on the item when looked at."), Category("Basic Information")]
    public string Description {get; set;}

    [Description("Damage threshhold, how much damage they must receive for it to deal damage to the wearer."), Category("Combat Information")]
    public int DT {get; set;}

    [Description("How much % Durability this item will have when initially found."), Category("Combat Information")]
    public int BaseDurability {get; set;}

    [Description("How much repairing will repair this item by per use."), Category("Combat Information")]
    public int DurabilityCost {get; set;}

    [ResourceType("sound"), Category("Sound Information")]
    public string FootstepLeft {get; set;}

    [ResourceType("sound"), Category("Sound Information")]
    public string FootstepRight {get; set;}

    [ResourceType("sound"), Category("Sound Information")]
    public string Jump {get; set;}

    [ResourceType("sound"), Category("Sound Information")]
    public string Land {get; set;}


    // Change this into multi select
    public enum HideParts {
        None = 0,
        Head = 1,
        Torso = 3,
        Arms = 4,
        Legs = 5,
        All = 6,
        AllButHead = 7,
    }

	[Category( "Basic Information" )]
	public HideParts HidePart { get; set; } = HideParts.None;
}
