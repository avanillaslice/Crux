public class Cannon : SingleFireWeaponBase
{
    // Initialize default values
    protected void Awake()
    {
        hasAnimation = false;
        CurrentFireRate = BaseFireRate;
        WeaponType = WeaponType.Primary;
    }

}
