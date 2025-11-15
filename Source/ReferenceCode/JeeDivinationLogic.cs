using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000826 RID: 2086
public class JeeDivinationLogic : MonoBehaviour {
    // Token: 0x06003339 RID: 13113 RVA: 0x000D2204 File Offset: 0x000D0404
    public void JeeDecide() {
        for (int i = 0; i < this.jeeEntries.Length; i++) {
            if (this.jeeEntries[i].BindingSequence == this.monster_jee.monsterCore.attackSequenceMoodule.getCurrentSequence()) {
                this.SingleSelected(this.jeeEntries[i], false);
                return;
            }
        }
    }

    // Token: 0x0600333A RID: 13114 RVA: 0x000D2260 File Offset: 0x000D0460
    public void HideIcon() {
        for (int i = 0; i < this.jeeEntries.Length; i++) {
            this.jeeEntries[i].HideIcon();
        }
    }

    // Token: 0x0600333B RID: 13115 RVA: 0x000D2290 File Offset: 0x000D0490
    public void ResetState() {
        this.path.MuteRotation = false;
        int num = UnityEngine.Random.Range(0, 3);
        List<MonsterStateGroupSequence> list = this.monster_jee.monsterCore.attackSequenceMoodule.SneakPeakNextQueue(this.monster_jee.PhaseIndex, 2, true);
        for (int i = 0; i < this.jeeEntries.Length; i++) {
            if (i == num) {
                this.jeeEntries[i].AssignBindingSequence(this.healthSequence);
            } else {
                int index = UnityEngine.Random.Range(0, list.Count);
                this.jeeEntries[i].AssignBindingSequence(list[index]);
                list.RemoveAt(index);
            }
            this.jeeEntries[i].gameObject.SetActive(true);
        }
    }

    // Token: 0x0600333C RID: 13116 RVA: 0x000D233B File Offset: 0x000D053B
    private void Awake() {
        this.Init();
    }

    // Token: 0x0600333D RID: 13117 RVA: 0x000D2344 File Offset: 0x000D0544
    private void Init() {
        for (int i = 0; i < this.jeeEntries.Length; i++) {
            this.BindJeeDivinationEntry(this.jeeEntries[i]);
        }
        this.BindHealthSequenceBinding(this.healthSequence);
    }

    // Token: 0x0600333E RID: 13118 RVA: 0x000D237E File Offset: 0x000D057E
    private void BindJeeDivinationEntry(JeeDivinationEntry entry) {
        entry.JumpSelectEvent.AddListener(delegate (JeeDivinationEntry e) {
            this.SingleSelected(e, true);
        });
    }

    // Token: 0x0600333F RID: 13119 RVA: 0x000D2397 File Offset: 0x000D0597
    private void BindHealthSequenceBinding(MonsterStateGroupSequence healthAttackSequence) {
        healthAttackSequence.LastStatePoppingEvent.AddListener(new UnityAction(this.AfterHealthSequenceLinkMoveToNormal));
    }

    // Token: 0x06003340 RID: 13120 RVA: 0x000D23B0 File Offset: 0x000D05B0
    private void AfterHealthSequenceLinkMoveToNormal() {
        this.monster_jee.monsterCore.attackSequenceMoodule.StartNewAttackSequence(this.monster_jee.PhaseIndex);
    }

    // Token: 0x06003341 RID: 13121 RVA: 0x000D23D4 File Offset: 0x000D05D4
    private void SingleSelected(JeeDivinationEntry entry, bool byPlayer) {
        if (byPlayer) {
            this.monster_jee.ChangeStateIfValid(this.InterruptState.GetStateType());
            this.path.MuteRotation = true;
            this.jeeDecideEntry = entry;
            for (int i = 0; i < this.jeeEntries.Length; i++) {
                if (this.jeeEntries[i] != entry) {
                    this.jeeEntries[i].NotJumppedAndDestroy();
                }
            }
            return;
        }
        this.jeeDecideEntry = entry;
    }

    // Token: 0x06003342 RID: 13122 RVA: 0x000D2446 File Offset: 0x000D0646
    public void ShowJeeEntry() {
        if (this.jeeDecideEntry != null) {
            this.jeeDecideEntry.ShowJeeIcon();
        }
    }

    // Token: 0x04002A73 RID: 10867
    [AutoChildren(false)]
    private JeeDivinationEntry[] jeeEntries;

    // Token: 0x04002A74 RID: 10868
    [AutoParent(true)]
    private MonsterBase monster_jee;

    // Token: 0x04002A75 RID: 10869
    public MonsterState InterruptState;

    // Token: 0x04002A76 RID: 10870
    public CirclePath path;

    // Token: 0x04002A77 RID: 10871
    public MonsterStateGroupSequence healthSequence;

    // Token: 0x04002A78 RID: 10872
    private JeeDivinationEntry jeeDecideEntry;
}

