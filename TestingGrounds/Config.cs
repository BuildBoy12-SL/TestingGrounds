namespace TestingGrounds
{
    using Exiled.API.Interfaces;
    using System.ComponentModel;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("Amount of ammo a shot someone using ItemGun will consume.")]
        public int ItemGunConsumedAmmo { get; set; } = 1;
    }
}