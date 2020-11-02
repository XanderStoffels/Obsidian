﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Obsidian.API
{
    public interface IPlayer : ILiving
    {
        public string Username { get; }
        public Guid Uuid { get; }
        public IServer Server { get; }
        public bool IsOperator { get; }

        public Gamemode Gamemode { get; set; }
        public Hand MainHand { get; set; }

        public bool Sleeping { get; set; }
        public bool Sneaking { get; set; }
        public bool Sprinting { get; set; }
        public bool FlyingWithElytra { get; set; }
        public bool InHorseInventory { get; set; }

        public short AttackTime { get; set; }
        public short DeathTime { get; set; }
        public short HurtTime { get; set; }
        public short SleepTimer { get; set; }
        public short CurrentSlot { get; set; }

        public int Ping { get; }
        public int Dimension { get; set; }
        public int FoodLevel { get; set; }
        public int FoodTickTimer { get; set; }
        public int XpLevel { get; set; }
        public int XpTotal { get; set; }

        public double HeadY { get; }

        public float AdditionalHearts { get; set; }
        public float FallDistance { get; set; }
        public float FoodExhastionLevel { get; set; }
        public float FoodSaturationLevel { get; set; }

        public ICollection<string> Permissions { get; }

        public Task SendMessageAsync(string message, sbyte position = 0, Guid? sender = null);
        public Task KickAsync(string reason);

        public Task SetGamemodeAsync(Gamemode gamemode);
    }
}