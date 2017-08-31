namespace Isometric.Core
{
    public interface IMilitaryObject
    {
        int LifePoints { get; set; }

        int GetDamageFor(IMilitaryObject enemy);
        
        ArmorType Armor { get; }

        IResources Loot { get; }

        void Destroy(IMilitaryObject destroyer);
    }
}