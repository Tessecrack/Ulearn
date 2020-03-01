using System;

namespace Inheritance.MapObjects
{
    interface IOwnerChecker
    {
        void OwnerEqualsPlayerId(Player player);
    }
    interface ICheckerArmy
    {
        bool CheckPlayerCanBeatArmy(Player player);
    }
    interface IConsumerTreasure 
    {
        void ConsumerTreasure(Player player);
    }

    public class Dwelling : IOwnerChecker
    {
        public int Owner { get; set; } //владелец
        public void OwnerEqualsPlayerId(Player player) { this.Owner = player.Id; }
    }

    public class Mine : IOwnerChecker, ICheckerArmy, IConsumerTreasure
    {
        public int Owner { get; set; } // владелец
        public Army Army { get; set; } // армия
        public Treasure Treasure { get; set; } // сокровище
        public bool CheckPlayerCanBeatArmy(Player player) => player.CanBeat(Army);
        public void OwnerEqualsPlayerId(Player player) { if (player.CanBeat(Army)) this.Owner = player.Id; }
        public void ConsumerTreasure(Player player) { if (player.CanBeat(Army)) player.Consume(Treasure); }
    }

    public class Creeps : ICheckerArmy, IConsumerTreasure
    {
        public Army Army { get; set; } // армия
        public Treasure Treasure { get; set; } // сокровище
        public bool CheckPlayerCanBeatArmy(Player player) => player.CanBeat(Army);
        public void ConsumerTreasure(Player player) { if (player.CanBeat(Army)) player.Consume(Treasure); }

    }

    public class Wolfs : ICheckerArmy
    {
        public Army Army { get; set; } // армия волЧАР
        public bool CheckPlayerCanBeatArmy(Player player) => !(player.CanBeat(Army));
    }

    public class ResourcePile : IConsumerTreasure
    {
        public Treasure Treasure { get; set; } // сокровище
        public void ConsumerTreasure(Player player) { player.Consume(Treasure); }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IOwnerChecker)
            {
                var obj = mapObject as IOwnerChecker;
                obj.OwnerEqualsPlayerId(player);
            }
            if (mapObject is IConsumerTreasure)
            {
                var obj = mapObject as IConsumerTreasure;
                obj.ConsumerTreasure(player);
            }
            if (mapObject is ICheckerArmy)
            {
                var obj = mapObject as ICheckerArmy;
                if (!obj.CheckPlayerCanBeatArmy(player)) 
                    player.Die();
            }
        }
    }
}
