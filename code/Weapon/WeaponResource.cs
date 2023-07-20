using Sandbox;
namespace Sandbox;

[GameResource("Weapon Resource", "weapon", "Describes an item of clothing and implicitly which other items it can be worn with.")]
public partial class WeaponResource : GameResource
{
    public string Name {get; set;}
    [ResourceType("vmdl")]
    public string Model {get; set;}
    public int Damage {get; set;}
    public int ReloadSpeed {get; set;}

    // Implement curve api here
    public int Recoil {get; set;}
    public int RPM {get; set;}
}
