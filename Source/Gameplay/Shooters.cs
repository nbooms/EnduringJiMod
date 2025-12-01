using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnduringJiMod.Gameplay;

using static EnduringJiGlobalReferences;
public class ProjectileShooters {
    private string projectileShootersPath = "MonsterCore/Animator(Proxy)/Animator/LogicRoot";

    //Projectile Shooter Paths
    //Sword Blizzard Paths
    private string swordBlizzardR1Path = "SwordBlizzard/PathShootCore_SwordBlizzard_Right1/ProjectileShooter";
    private string swordBlizzardL2Path = "SwordBlizzard/PathShootCore_SwordBlizzard_Left2/ProjectileShooter";
    private string swordBlizzardR3Path = "SwordBlizzard/PathShootCore_SwordBlizzard_Right3/ProjectileShooter";
    private string swordBlizzardL4Path = "SwordBlizzard/PathShootCore_SwordBlizzard_Left4/ProjectileShooter";
    private string swordBlizzardR5Path = "SwordBlizzard/PathShootCore_SwordBlizzard_Right1/ProjectileShooter";

    //Projectile Shooter Objects
    //Sword Blizzard Projectile Shooters
    GameObject PS_SwordBlizzard_Right1 = null;
    GameObject PS_SwordBlizzard_Left2 = null;
    GameObject PS_SwordBlizzard_Right3 = null;
    GameObject PS_SwordBlizzard_Left4 = null;
    GameObject PS_SwordBlizzard_Right5 = null;

    public void GetProjectileShooterObjects() {
        //Sword Blizzard
        PS_SwordBlizzard_Right1 = GameObject.Find($"{JI_BOSS_PATH}/{projectileShootersPath}/{swordBlizzardR1Path}");
        PS_SwordBlizzard_Left2 = GameObject.Find($"{JI_BOSS_PATH}/{projectileShootersPath}/{swordBlizzardL2Path}");
        PS_SwordBlizzard_Right3 = GameObject.Find($"{JI_BOSS_PATH}/{projectileShootersPath}/{swordBlizzardR3Path}");
        PS_SwordBlizzard_Left4 = GameObject.Find($"{JI_BOSS_PATH}/{projectileShootersPath}/{swordBlizzardL4Path}");
        PS_SwordBlizzard_Right5 = GameObject.Find($"{JI_BOSS_PATH}/{projectileShootersPath}/{swordBlizzardR5Path}");
    }
}
