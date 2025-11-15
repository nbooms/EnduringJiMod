using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class PostureBreakState : BossGeneralState {
    // Token: 0x06000793 RID: 1939 RVA: 0x000243B2 File Offset: 0x000225B2
    public override MonsterBase.States GetStateType() {
        return MonsterBase.States.PostureBreak;
    }

    // Token: 0x06000794 RID: 1940 RVA: 0x000243B9 File Offset: 0x000225B9
    public override void OnStateEnter() {
        base.OnStateEnter();
        base.monster.monsterCore.DisablePushAway();
        base.monster.Velocity = Vector2.zero;
        LootSpawner lootSpawner = this.postureBreakAmmoSpawner;
        if (lootSpawner == null) {
            return;
        }
        lootSpawner.Spawn();
    }

    // Token: 0x06000795 RID: 1941 RVA: 0x000243F1 File Offset: 0x000225F1
    public override void OnStateExit() {
        base.OnStateExit();
        base.monster.monsterCore.EnablePushAway();
        base.monster.HurtInterrupt.currentAccumulateDamage = 0f;
    }

    // Token: 0x0400061C RID: 1564
    public LootSpawner postureBreakAmmoSpawner;
}
