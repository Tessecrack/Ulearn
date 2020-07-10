// Вставьте сюда финальное содержимое файла Task.cs
using System;

namespace Inheritance.MapObjects
{
    interface IOwner
    {
        int Owner { get; set; }
    }
	
    interface IArmy
    {
        Army Army { get; set; }
    }
	
    interface ITreasure
    {
        Treasure Treasure { get; set; }
    }

    public class Dwelling : IOwner
    {
        public int Owner { get; set; }
    }

    public class Mine : IOwner, IArmy, ITreasure
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : IArmy, ITreasure
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolfs : IArmy
    {
        public Army Army { get; set; }
    }

    public class ResourcePile : ITreasure
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IArmy armyObj)
                if (!player.CanBeat(armyObj.Army))
                {
                    player.Die();
                    return;
                }
            if (mapObject is IOwner ownerObj)
                ownerObj.Owner = player.Id;
            if (mapObject is ITreasure treasureObj)
                player.Consume(treasureObj.Treasure);
        }
    }
}
