using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnduringJiMod;

using static EnduringJiGlobalReferences;

internal class Attacks {
    private string jiBossAttacksPath = "States/Attacks";

    private string _1_DivinationFreeZone = "[1]Divination Free Zone"; //Spin 3 options in a circle, will select a divination at random after a couple of seconds
    private string _2_FlyingProjectiles = "[2][Short]Flying Projectiles"; //Fires 2 swords, attack yi, loop back, attack Yi
    private string _3_BlackHoleAttack = "[3][Finisher]BlackHoleAttack"; //(Phase 2) Places large black hole that spawns swords
    private string _4_SetLaserAltarEnvironment = "[4][Altar]Set Laser Altar Environment"; //(Phase 2) Places altar that spawns 1 circle and sucks in infinite
    private string _5_SuckSword = "[5][Short]SuckSword 往內"; //Shoot two swords up in the air (missing Yi) that loop back around to attack him
    private string _6_Teleport3SwordSmash = "[6][Finisher]Teleport3Sword Smash 下砸"; //(Phase 2) Single sword that arcs up, into teleport, into big smash (usually follows two [Short] attacks)
    private string _7_SwordBlizzard = "[7][Finisher]SwordBlizzard"; //Shoots 5 swords, alternating direction, from above Yi
    private string _8_DivinationFreeZone_EndLessLoop = "[8]Divination Free Zone_EndLessLoop"; //Maybe it's just like [1] but runs forevber??
    private string _9_GroundSword = "[9][Short]GroundSword"; //Pulls sword out of ground from behind Yi, attacks him, loops back around, and attacks again.
    private string _10_SmallBlackHole = "[10][Altar]SmallBlackHole"; //Spawns the small black hole
    private string _11_ShorFlyingSword = "[11][Short]ShorFlyingSword"; //Throws a single sword from the air, straight at Yi
    private string _12_QuickHorizontalDoubleSword = "[12][Short]QuickHorizontalDoubleSword"; //Two swords shot in quick succession next to Yi
    private string _13_QuickTeleportSword = "[13][Finisher]QuickTeleportSword 危戳"; //Shoots 1 sword horizontally, hits Yi, teleports to sword on other side, dashes at Yi
    private string _14_LaserAltarCircle = "[14][Altar]Laser Altar Circle"; //(Phase 1) Places alter that spawns 2 circles
    private string _15_HealthAltar = "[15][Altar]Health Altar"; //Places health pot (altar)
    private string _16_DivinationJumpKicked = "[16]Divination JumpKicked"; //Maybe it's when a divination is selected??
    //NOTE: The "Three Sword" divination just spawns 3 of the [short] attacks

    //Attack States
    BossGeneralState _1_DivinationFreeZone_BGS = null;
    BossGeneralState _2_FlyingProjectiles_BGS = null;
    BossGeneralState _3_BlackHoleAttack_BGS = null;
    BossGeneralState _4_SetLaserAltarEnvironment_BGS = null;
    BossGeneralState _5_SuckSword_BGS = null;
    BossGeneralState _6_Teleport3SwordSmash_BGS = null;
    BossGeneralState _7_SwordBlizzard_BGS = null;
    BossGeneralState _8_DivinationFreeZone_EndLessLoop_BGS = null;
    BossGeneralState _9_GroundSword_BGS = null;
    BossGeneralState _10_SmallBlackHole_BGS = null;
    BossGeneralState _11_ShorFlyingSword_BGS = null;
    BossGeneralState _12_QuickHorizontalDoubleSword_BGS = null;
    BossGeneralState _13_QuickTeleportSword_BGS = null;
    BossGeneralState _14_LaserAltarCircle_BGS = null;
    BossGeneralState _15_HealthAltar_BGS = null;
    BossGeneralState _16_DivinationJumpKicked_BGS = null;

    public void GetAttackGameObjects() {
        _1_DivinationFreeZone_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_1_DivinationFreeZone}").GetComponent<BossGeneralState>();
        _2_FlyingProjectiles_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_2_FlyingProjectiles}").GetComponent<BossGeneralState>();
        _3_BlackHoleAttack_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_3_BlackHoleAttack}").GetComponent<BossGeneralState>();
        _4_SetLaserAltarEnvironment_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_4_SetLaserAltarEnvironment}").GetComponent<BossGeneralState>();
        _5_SuckSword_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_5_SuckSword}").GetComponent<BossGeneralState>();
        _6_Teleport3SwordSmash_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_6_Teleport3SwordSmash}").GetComponent<BossGeneralState>();
        _7_SwordBlizzard_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_7_SwordBlizzard}").GetComponent<BossGeneralState>();
        _8_DivinationFreeZone_EndLessLoop_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_8_DivinationFreeZone_EndLessLoop}").GetComponent<BossGeneralState>();
        _9_GroundSword_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_9_GroundSword}").GetComponent<BossGeneralState>();
        _10_SmallBlackHole_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_10_SmallBlackHole}").GetComponent<BossGeneralState>();
        _11_ShorFlyingSword_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_11_ShorFlyingSword}").GetComponent<BossGeneralState>();
        _12_QuickHorizontalDoubleSword_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_12_QuickHorizontalDoubleSword}").GetComponent<BossGeneralState>();
        _13_QuickTeleportSword_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_13_QuickTeleportSword}").GetComponent<BossGeneralState>();
        _14_LaserAltarCircle_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_14_LaserAltarCircle}").GetComponent<BossGeneralState>();
        _15_HealthAltar_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_15_HealthAltar}").GetComponent<BossGeneralState>();
        _16_DivinationJumpKicked_BGS = GameObject.Find($"{JI_BOSS_PATH}/{jiBossAttacksPath}/{_16_DivinationJumpKicked}").GetComponent<BossGeneralState>();
    }
}
