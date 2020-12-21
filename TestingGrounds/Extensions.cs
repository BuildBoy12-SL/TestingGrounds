namespace TestingGrounds
{
    public static class Extensions
    {
        public static int ItemDur(this ItemType weapon)
        {
            switch (weapon)
            {
                case ItemType.GunCOM15:
                    return 12;
                case ItemType.GunE11SR:
                    return 18;
                case ItemType.GunProject90:
                    return 50;
                case ItemType.GunMP7:
                    return 35;
                case ItemType.GunLogicer:
                    return 100;
                case ItemType.GunUSP:
                    return 18;
                case ItemType.Ammo762:
                    return 25;
                case ItemType.Ammo9mm:
                    return 25;
                case ItemType.Ammo556:
                    return 25;
                default:
                    return 50;
            }
        }
    }
}