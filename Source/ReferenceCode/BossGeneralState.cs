using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RCGMaker.Core;
using RCGMaker.Core.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020004D3 RID: 1235
public class BossGeneralState : MonsterState, IResetter, IBeforePrefabSaveCallbackReceiver {
    // Token: 0x1700038D RID: 909
    // (get) Token: 0x06001C3B RID: 7227 RVA: 0x000717E2 File Offset: 0x0006F9E2
    private bool hasStateType {
        get {
            return this.stateTypeScriptable != null;
        }
    }

    // Token: 0x06001C3C RID: 7228 RVA: 0x000717F0 File Offset: 0x0006F9F0
    protected override void OnDestroy() {
        base.OnDestroy();
        UnityEvent shootEvent = this.ShootEvent;
        if (shootEvent != null) {
            shootEvent.RemoveAllListeners();
        }
        UnityEvent shoot2Event = this.Shoot2Event;
        if (shoot2Event != null) {
            shoot2Event.RemoveAllListeners();
        }
        this.ShootEvent = null;
        this.Shoot2Event = null;
    }

    // Token: 0x1700038E RID: 910
    // (get) Token: 0x06001C3D RID: 7229 RVA: 0x00071828 File Offset: 0x0006FA28
    private LinkNextMoveStateWeight currentInterruptLinkMoveSet {
        get {
            if (this.linkInterruptMoveConditionalWeights.Count == 0) {
                return null;
            }
            if (this.linkInterruptMoveConditionalWeights.Count > base.monster.PhaseIndex) {
                return this.linkInterruptMoveConditionalWeights[base.monster.PhaseIndex];
            }
            return this.linkInterruptMoveConditionalWeights[0];
        }
    }

    // Token: 0x06001C3E RID: 7230 RVA: 0x0007187F File Offset: 0x0006FA7F
    private void Awake() {
        if (this.state.ToString().Contains("Attack")) {
            this.IsAttackState = true;
        }
    }

    // Token: 0x06001C3F RID: 7231 RVA: 0x000718A8 File Offset: 0x0006FAA8
    public void PrepareQueue() {
        if (this.testWeight && this.testWeight.isActiveAndEnabled) {
            this.LinkMoveOptionCount = this.testWeight.GetComponent<LinkNextMoveStateWeight>().EnqueueAttacks(this.QueuedAttacks);
            return;
        }
        if (this.linkNextMoveStateWeights.Count > 0) {
            LinkNextMoveStateWeight linkNextMoveStateWeight;
            if (base.monster.PhaseIndex >= this.linkNextMoveStateWeights.Count) {
                List<LinkNextMoveStateWeight> list = this.linkNextMoveStateWeights;
                linkNextMoveStateWeight = list[list.Count - 1];
            } else {
                linkNextMoveStateWeight = this.linkNextMoveStateWeights[base.monster.PhaseIndex];
            }
            this.LinkMoveOptionCount = linkNextMoveStateWeight.EnqueueAttacks(this.QueuedAttacks);
            return;
        }
        List<MonsterBase.States> list2 = new List<MonsterBase.States>();
        for (int i = 0; i < this.linkedStateTypes.Count; i++) {
            list2.Insert(UnityEngine.Random.Range(0, list2.Count + 1), this.linkedStateTypes[i]);
        }
        this.QueuedAttacks.AddRange(list2);
        this.LinkMoveOptionCount = this.linkedStateTypes.Count;
    }

    // Token: 0x06001C40 RID: 7232 RVA: 0x000719AC File Offset: 0x0006FBAC
    private MonsterBase.States FetchQueuedAttack() {
        if (this.lastMonsterPhase != base.monster.PhaseIndex) {
            this.QueuedAttacks.Clear();
        }
        this.lastMonsterPhase = base.monster.PhaseIndex;
        if (this.QueuedAttacks.Count <= 0 || this.QueuedAttacks.Count < this.LinkMoveOptionCount) {
            this.PrepareQueue();
            if (this.QueuedAttacks.Count == 0) {
                return MonsterBase.States.Skip;
            }
        }
        for (int i = 0; i < this.QueuedAttacks.Count; i++) {
            MonsterBase.States states = this.QueuedAttacks[i];
            if (states == MonsterBase.States.Undefined || states == MonsterBase.States.Skip) {
                this.QueuedAttacks.RemoveAt(i);
                return MonsterBase.States.Skip;
            }
            if (base.monster.FindState(states) != null) {
                MonsterState monsterState = base.monster.FindState(states);
                if (monsterState.IsValid) {
                    BossGeneralState x = monsterState as BossGeneralState;
                    if (!(x == null)) {
                        SingletonBehaviour<CoolDownTokenManager>.Instance.RhythmQueue.RegisterCheck(x);
                    }
                    this.QueuedAttacks.RemoveAt(i);
                    return states;
                }
            }
        }
        return MonsterBase.States.Undefined;
    }

    // Token: 0x06001C41 RID: 7233 RVA: 0x00071AB2 File Offset: 0x0006FCB2
    public override MonsterBase.States GetStateType() {
        if (this.stateTypeScriptable && this.stateTypeScriptable.overrideStateType != MonsterBase.States.Undefined) {
            return this.stateTypeScriptable.overrideStateType;
        }
        return this.state;
    }

    // Token: 0x1700038F RID: 911
    // (get) Token: 0x06001C42 RID: 7234 RVA: 0x00071AE0 File Offset: 0x0006FCE0
    public TeleportBinding teleportBinding {
        get {
            return base.monster.monsterCore.teleportBinding;
        }
    }

    // Token: 0x06001C43 RID: 7235 RVA: 0x00071AF2 File Offset: 0x0006FCF2
    public override void OnCreateMapping(MonoBehaviour context) {
        base.OnCreateMapping(context);
    }

    // Token: 0x06001C44 RID: 7236 RVA: 0x00071AFB File Offset: 0x0006FCFB
    public void Stun() {
        if (this._monster.fsm.HasState(this.stunState)) {
            this._monster.fsm.ChangeState(this.stunState, false);
        }
    }

    // Token: 0x06001C45 RID: 7237 RVA: 0x00071B2C File Offset: 0x0006FD2C
    public override bool AdditionalValidCheck() {
        return true;
    }

    // Token: 0x17000390 RID: 912
    // (get) Token: 0x06001C46 RID: 7238 RVA: 0x00071B2F File Offset: 0x0006FD2F
    private LinkMoveCountMaxProvider currentLinkMoveProvider {
        get {
            if (!this.myLinkMoveMaxProvider) {
                return this.bindLinkMoveMaxProvider;
            }
            return this.myLinkMoveMaxProvider;
        }
    }

    // Token: 0x06001C47 RID: 7239 RVA: 0x00071B4C File Offset: 0x0006FD4C
    public void ResetCoolDownCheck() {
        if (this.StateEndStateCoolDownProvider && this.StateEndStateCoolDownProvider.isActiveAndEnabled) {
            if (base.monster.lastAttackSensor != null) {
                base.monster.lastAttackSensor.ReplaceCoolDown(this.StateEndStateCoolDownProvider.FetchCoolDown);
            }
        } else if (base.monster.lastAttackSensor) {
            base.monster.lastAttackSensor.ResetCoolDown();
        }
        this.bindSensor = null;
        this.bindLinkMoveMaxProvider = null;
    }

    // Token: 0x06001C48 RID: 7240 RVA: 0x00071BD4 File Offset: 0x0006FDD4
    private void LinkMoveInterruptConditional() {
        if (this.currentInterruptLinkMoveSet == null) {
            return;
        }
        List<AttackWeight> stateWeightList = this.currentInterruptLinkMoveSet.stateWeightList;
        if (stateWeightList == null || stateWeightList.Count == 0) {
            return;
        }
        for (int i = 0; i < stateWeightList.Count; i++) {
            MonsterBase.States stateType = stateWeightList[i].StateType;
            if (stateWeightList[i].weight != 0) {
                BossGeneralState bossGeneralState = base.monster.ChangeStateIfValid(stateType) as BossGeneralState;
                if (bossGeneralState) {
                    bossGeneralState.bindSensor = this.bindSensor;
                    this.bindSensor = null;
                    if (bossGeneralState.state == MonsterBase.States.Dead) {
                        base.monster.deathType = MonsterBase.DeathType.SelfDestruction;
                    }
                    return;
                }
            }
        }
    }

    // Token: 0x06001C49 RID: 7241 RVA: 0x00071C7C File Offset: 0x0006FE7C
    private bool LinkTooLongCheck() {
        LinkMoveCountMaxProvider linkMoveCountMaxProvider = this.currentLinkMoveProvider;
        if (this.linkMoveExtend != null && this.linkMoveExtend.IsExtend(base.monster.PhaseIndex)) {
            return false;
        }
        if (linkMoveCountMaxProvider == null) {
            linkMoveCountMaxProvider = base.GetComponentInParent<LinkMoveCountMaxProvider>();
        }
        return linkMoveCountMaxProvider && base.monster.CurrentLinkMoveCount >= linkMoveCountMaxProvider.CurrentMaxCount;
    }

    // Token: 0x06001C4A RID: 7242 RVA: 0x00071CE6 File Offset: 0x0006FEE6
    private void FetchGroupingNode() {
        this.groupingNodes = base.GetComponentsInChildren<LinkMoveGroupingNode>();
    }

    // Token: 0x06001C4B RID: 7243 RVA: 0x00071CF4 File Offset: 0x0006FEF4
    private bool LinkMove() {
        if (Player.i.health.IsDead) {
            return false;
        }
        if (base.monster.postureSystem.IsMonsterEmptyPosture) {
            return false;
        }
        if (base.monster.IsUnderPlayerControl) {
            return false;
        }
        if (this.LinkTooLongCheck()) {
            this.bindLinkMoveMaxProvider = null;
            return false;
        }
        if (this.groupingNodes == null && this.linkNextMoveStateWeights.Count == 0 && this.attackQueue == null && (base.monster.monsterCore.attackSequenceMoodule == null || !base.monster.monsterCore.attackSequenceMoodule.isActiveAndEnabled)) {
            return false;
        }
        MonsterBase.States states;
        if (base.monster.monsterCore.attackSequenceMoodule && base.monster.monsterCore.attackSequenceMoodule.isActiveAndEnabled) {
            MonsterState nextLinkMove = base.monster.monsterCore.attackSequenceMoodule.GetNextLinkMove(base.monster.PhaseIndex);
            if (nextLinkMove == null) {
                return false;
            }
            states = nextLinkMove.GetStateType();
        } else if (this.groupingNodes != null && this.groupingNodes.Length != 0) {
            states = this.groupingNodes[this.linkMoveGroupIndex].FetchQueuedAttack(base.monster.PhaseIndex);
            if (states == MonsterBase.States.Undefined) {
                this.groupingNodes[this.linkMoveGroupIndex].Clear();
                states = this.groupingNodes[this.linkMoveGroupIndex].FetchQueuedAttack(base.monster.PhaseIndex);
            }
            this.linkMoveGroupIndex++;
        } else if (this.attackQueue != null) {
            states = this.attackQueue.FetchQueuedAttack(base.monster.PhaseIndex);
            if (states == MonsterBase.States.Undefined) {
                this.attackQueue.QueuedAttacks.Clear();
                states = this.attackQueue.FetchQueuedAttack(base.monster.PhaseIndex);
            }
        } else {
            states = this.FetchQueuedAttack();
        }
        if (states == MonsterBase.States.Skip) {
            return false;
        }
        if (states == MonsterBase.States.Undefined) {
            this.QueuedAttacks.Clear();
            states = this.FetchQueuedAttack();
            if (states == MonsterBase.States.Undefined) {
                return false;
            }
        }
        MonsterState monsterState = base.monster.ChangeStateIfValid(states);
        BossGeneralState bossGeneralState = monsterState as BossGeneralState;
        if (bossGeneralState == null) {
            if (monsterState.GetStateType() == MonsterBase.States.Engaging) {
                (monsterState as StealthEngaging).ForceRunToPlayer = true;
            }
        } else {
            if (this.myLinkMoveMaxProvider) {
                bossGeneralState.bindLinkMoveMaxProvider = this.myLinkMoveMaxProvider;
            } else if (this.bindLinkMoveMaxProvider) {
                bossGeneralState.bindLinkMoveMaxProvider = this.bindLinkMoveMaxProvider;
            }
            bossGeneralState.bindSensor = this.bindSensor;
        }
        if (bossGeneralState != this) {
            this.bindSensor = null;
            this.bindLinkMoveMaxProvider = null;
        }
        base.monster.LinkMoveAdd();
        return true;
    }

    // Token: 0x06001C4C RID: 7244 RVA: 0x00071F88 File Offset: 0x00070188
    public override void AnimationEvent(AnimationEvents.AnimationEvent e) {
        if (!base.IsPlayingBindingAnimation()) {
            return;
        }
        if (e == AnimationEvents.AnimationEvent.JeeQTELoop) {
            base.monster.ChangeStateIfValid(MonsterBase.States.Attack8);
        } else if (e == AnimationEvents.AnimationEvent.CheckPlayerToClose) {
            if (Vector3.Distance(Player.i.transform.position, base.monster.transform.position) < 50f) {
                base.monster.monsterContext.CheckToCloseReturn = this.GetStateType();
                base.monster.ChangeStateIfValid(this.ToCloseTransitionState);
                return;
            }
        } else if (e == AnimationEvents.AnimationEvent.InterruptRandomCheck) {
            if (base.monster.monsterCore.interrupter.InterruptCheck()) {
                this.Done();
                return;
            }
        } else {
            if (e == AnimationEvents.AnimationEvent.LinkMoveInterrupt) {
                this.LinkMoveInterruptConditional();
                return;
            }
            if (e == AnimationEvents.AnimationEvent.LinkNextMoveBreak) {
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f) {
                    Debug.Log("LinkNextMoveBreak");
                    this.LinkMove();
                    return;
                }
            } else if (e == AnimationEvents.AnimationEvent.LinkNextMovePhase3) {
                if (base.monster.PhaseIndex == 2) {
                    Debug.Log("LinkMovePhase3");
                    this.LinkMove();
                    return;
                }
            } else if (e == AnimationEvents.AnimationEvent.LinkNextMovePhase2) {
                if (base.monster.PhaseIndex > 0) {
                    Debug.Log("LinkMovePhase2");
                    this.LinkMove();
                    return;
                }
            } else {
                if (e == AnimationEvents.AnimationEvent.LinkNextMove) {
                    this.LinkMove();
                    return;
                }
                if (e == AnimationEvents.AnimationEvent.Done) {
                    base.monster.LinkMoveReset();
                    if (this._interruptTurnAround) {
                        this._interruptTurnAround = false;
                        base.monster.TurnAround();
                        return;
                    }
                    if (this.exitState == MonsterBase.States.Dead) {
                        base.monster.deathType = MonsterBase.DeathType.SelfDestruction;
                    }
                    if (base.monster.monsterContext.CheckToCloseReturn != MonsterBase.States.Undefined) {
                        base.monster.monsterContext.CheckToCloseReturn = MonsterBase.States.Undefined;
                        base.monster.ChangeStateIfValid(base.monster.monsterContext.CheckToCloseReturn);
                    } else {
                        if (base.monster.IsUnderPlayerControl) {
                            base.monster.ChangeStateIfValid(MonsterBase.States.WanderingIdle, MonsterBase.States.WanderingIdle);
                            return;
                        }
                        base.monster.ChangeStateIfValid(this.exitState, MonsterBase.States.Engaging);
                    }
                }
            }
        }
        base.AnimationEvent(e);
        for (int i = 0; i < this.animationEventReceivers.Count; i++) {
            this.animationEventReceivers[i].OnAnimationEvent(e);
        }
        if (e == AnimationEvents.AnimationEvent.CheckFacing) {
            this._monster.FacePlayer();
            this.UpdatePlayerRad();
        }
        if (e == AnimationEvents.AnimationEvent.InterruptFacing_TargetLeft) {
            bool flag = SingletonBehaviour<GameCore>.Instance.player.transform.position.x > base.monster.transform.position.x;
            if (flag && base.monster.Facing == Facings.Left) {
                if (this.LinkMove()) {
                    return;
                }
                SingletonBehaviour<CoolDownTokenManager>.Instance.RhythmQueue.ForceClear();
                base.monster.ChangeStateIfValid(this.exitState);
                return;
            } else if (!flag && base.monster.Facing == Facings.Right) {
                if (this.LinkMove()) {
                    return;
                }
                SingletonBehaviour<CoolDownTokenManager>.Instance.RhythmQueue.ForceClear();
                base.monster.ChangeStateIfValid(this.exitState);
                return;
            }
        }
        if (e == AnimationEvents.AnimationEvent.InterruptIfTooFar && Mathf.Abs(SingletonBehaviour<GameCore>.Instance.player.transform.position.x - base.monster.transform.position.x) > this.TooFarValue) {
            base.monster.ChangeStateIfValid(MonsterBase.States.Engaging);
            return;
        }
        if (e == AnimationEvents.AnimationEvent.InterruptIfOnSlope && base.monster.isOnSlope) {
            base.monster.ChangeStateIfValid(MonsterBase.States.Engaging);
            return;
        }
        if (e == AnimationEvents.AnimationEvent.InterruptFacing_TargetRight) {
            bool flag2 = SingletonBehaviour<GameCore>.Instance.player.transform.position.x > base.monster.transform.position.x;
            if (!flag2 && base.monster.Facing == Facings.Left) {
                base.monster.ChangeStateIfValid(this.exitState);
                return;
            }
            if (flag2 && base.monster.Facing == Facings.Right) {
                base.monster.ChangeStateIfValid(this.exitState);
                return;
            }
        } else {
            if (e == AnimationEvents.AnimationEvent.ForceFacingLeft) {
                base.monster.Facing = Facings.Left;
                return;
            }
            if (e == AnimationEvents.AnimationEvent.ForceFacingRight) {
                base.monster.Facing = Facings.Right;
                return;
            }
            if (e == AnimationEvents.AnimationEvent.CheckFacingInverse) {
                this._monster.DontFacePlayer();
            } else if (e == AnimationEvents.AnimationEvent.EvaluateContinuous) {
                this.IsContinuousEvaluateMoveScale = true;
            } else if (e == AnimationEvents.AnimationEvent.EvaluateTurnOff) {
                this.IsContinuousEvaluateMoveScale = false;
            } else if (e == AnimationEvents.AnimationEvent.Evaluate) {
                base.monster.monsterCore.movescaler.EvaluatePosition();
            } else if (e == AnimationEvents.AnimationEvent.ResetEvaluate) {
                this.moveScaler.ClearEvaluate();
            }
        }
        if (e == AnimationEvents.AnimationEvent.EnableGravity) {
            this._monster.HasGravity = true;
        }
        if (e == AnimationEvents.AnimationEvent.DisableGravity) {
            this._monster.HasGravity = false;
            this._monster.Velocity = new Vector2(this._monster.VelX, 0f);
        }
        if (e == AnimationEvents.AnimationEvent.Shoot) {
            UnityEvent shootEvent = this.ShootEvent;
            if (shootEvent != null) {
                shootEvent.Invoke();
            }
        }
        if (e == AnimationEvents.AnimationEvent.Shoot2) {
            UnityEvent shoot2Event = this.Shoot2Event;
            if (shoot2Event != null) {
                shoot2Event.Invoke();
            }
        }
        if (e == AnimationEvents.AnimationEvent.Out_Sourcing) {
            Debug.LogError("Use Boss Phase Change State!!!");
        }
        if (e == AnimationEvents.AnimationEvent.Teleport && this.teleportBinding != null) {
            this.teleportBinding.Teleport();
        }
        if (e == AnimationEvents.AnimationEvent.DangerHint) {
            Debug.LogError("Old DangerHint Spec");
        }
    }

    // Token: 0x06001C4D RID: 7245 RVA: 0x00072494 File Offset: 0x00070694
    public override Vector3 OnAnimationMove(Vector3 delta) {
        if (base.monster.IsUnderPlayerControl) {
            return base.OnAnimationMove(delta);
        }
        if (this.moveScaler) {
            float num = this.moveScaler.AnimatorMoved(delta.x * (float)base.monster.Facing);
            delta.x *= num;
        }
        if (this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayer || this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayerAtStart || this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayerAtStartAbovePlayerGroundY) {
            Vector3 point = delta;
            delta = Quaternion.Euler(0f, 0f, this.radToPlayer * 57.29578f) * point;
            if (base.monster.Facing == Facings.Left) {
                delta = -delta;
            }
            if (this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayerAtStartAbovePlayerGroundY) {
                float num2 = Player.i.FightingReferenceY;
                if (Player.i.transform.position.y < num2) {
                    num2 = Player.i.transform.position.y;
                }
                if (delta.y < 0f && base.monster.transform.position.y - 32f < num2) {
                    delta.y = 0f;
                }
            }
            if (this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayer) {
                float num3 = Vector2.Distance(base.monster.transform.position, Player.i.CenterAnchor.transform.position);
                float num4 = 8f;
                if (num3 < num4) {
                    delta = delta.normalized * num3 / 2f;
                }
            }
        } else {
            BossGeneralState.MoveDirectionRemap moveDirectionRemap = this.directionRemap;
        }
        if (base.monster.monsterCore.teleportBinding.GetTeleportScheme != TeleportBinding.TeleportScheme.ToPlayerPositionWithTween) {
            return base.OnAnimationMove(delta);
        }
        if (base.monster.monsterCore.teleportBinding.OnTween) {
            return Vector3.zero;
        }
        return base.OnAnimationMove(delta);
    }

    // Token: 0x06001C4E RID: 7246 RVA: 0x00072670 File Offset: 0x00070870
    public override void OnStateUpdate() {
        base.OnStateUpdate();
        Vector3 position = SingletonBehaviour<GameCore>.Instance.player.transform.position;
        Vector3 position2 = base.monster.transform.position;
        base.monster.advance.preferUp = (position.y > position2.y);
        base.monster.advance.preferDown = (position.y < position2.y);
        if (base.monster.IsPredictAttackSensorEnabled && base.monster.predictSensorActivator.InComboAttackCheck()) {
            Debug.Log("[BossGeneralState] Predict to Attack ", base.gameObject);
            return;
        }
        if (this.CheckInterruptTurnAround()) {
            return;
        }
        if (this.CanDefend && base.monster.DefendCheck()) {
            return;
        }
        if (this.AutoDoneIfCollisionWall && this._monster.CheckColInDir(base.monster.Velocity + base.monster.animator.velocity, this._monster.solid_layer, 3f)) {
            this.Done();
            return;
        }
        if (this.ExitIfPlayerBehind && this.IsPlayerBehind) {
            this.Done();
        }
        if (this.ClearSpeedWithDecay) {
            base.monster.VelX = Calc.Approach(this._monster.VelX, 0f, this.RunReduce * RCGTime.deltaTime);
            if (base.monster is FlyingMonster) {
                base.monster.VelY = Calc.Approach(this._monster.VelY, 0f, this.RunReduce * RCGTime.deltaTime);
            }
        }
        if (this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayer) {
            Vector3 vector = SingletonBehaviour<GameCore>.Instance.player.CenterAnchor.transform.position - base.monster.transform.position;
            vector.z = 0f;
            this.radToPlayer = Mathf.Atan2(vector.y, vector.x);
        }
    }

    // Token: 0x17000391 RID: 913
    // (get) Token: 0x06001C4F RID: 7247 RVA: 0x00072863 File Offset: 0x00070A63
    private AnimationMoveScaler moveScaler {
        get {
            return base.monster.monsterCore.movescaler;
        }
    }

    // Token: 0x06001C50 RID: 7248 RVA: 0x00072878 File Offset: 0x00070A78
    public override void OnStateExit() {
        if (this.IsAttackState) {
            this.ResetCoolDownCheck();
        }
        if (this.damageOnTimeList.Count > 0) {
            base.monster.ResetAggressiveness();
        }
        if (base.monster.monsterCore.SetZOrder) {
            base.monster.monsterCore.sortingGroup.sortingOrder = base.monster.GetSortingOrderOffset(false);
        }
        this.CancelMyTask();
        if (this.moveScaler) {
            this.moveScaler.ClearEvaluate();
        }
        this.IsContinuousEvaluateMoveScale = false;
        if (this._monster.monsterCore.AbstractRoot != null && this.statusTimer > 0.1f && this._monster.monsterCore.AbstractRoot.transform.localScale.x < 0f) {
            base.monster.FlipFacing();
        }
        if (this.SpeedRemain) {
            this._monster.Velocity += base.monster.AnimationVelocity;
        } else if (this.ClearSpeedWhenExit) {
            this._monster.VelX = 0f;
        }
        base.OnStateExit();
    }

    // Token: 0x17000392 RID: 914
    // (get) Token: 0x06001C51 RID: 7249 RVA: 0x000729A8 File Offset: 0x00070BA8
    private bool IsPlayerBehind {
        get {
            return base.monster.GetFacingToPlayer() != base.monster.Facing;
        }
    }

    // Token: 0x06001C52 RID: 7250 RVA: 0x000729C8 File Offset: 0x00070BC8
    public override void OnStateEnter() {
        base.OnStateEnter();
        Player i = Player.i;
        this.linkMoveGroupIndex = 0;
        this._interruptTurnAround = false;
        if (base.monster.monsterCore.SetZOrder) {
            base.monster.monsterCore.sortingGroup.sortingOrder = base.monster.GetSortingOrderOffset(true);
            this.AddTask(delegate {
                if (base.monster != null) {
                    base.monster.monsterCore.sortingGroup.sortingOrder = base.monster.GetSortingOrderOffset(false);
                }
            }, 1f);
        }
        if (this.IsPlayerBehind && this.AutoFlipAround && !base.monster.IsUnderPlayerControl) {
            if (base.monster.NoTurnAround) {
                Debug.LogError("NoTurnAround", this);
            } else {
                base.monster.FlipFacing();
            }
        } else if (this.IsPlayerBehind && this.AutoTurnAround && !base.monster.IsUnderPlayerControl && base.monster.TurnAround()) {
            return;
        }
        MonsterStateAnimationTransition component = base.GetComponent<MonsterStateAnimationTransition>();
        if (component) {
            if (this.stateTypeScriptable) {
                base.monster.animator.CrossFadeInFixedTime(this.stateTypeScriptable.stateName, component.duration);
            } else {
                base.monster.animator.CrossFadeInFixedTime(this.BindingAnimation, component.duration);
            }
            base.monster.RegisterCrossFade(component.duration);
        } else if (this.stateTypeScriptable) {
            base.PlayAnimation(this.stateTypeScriptable.animationStateName, true);
        } else {
            base.PlayAnimation(this.BindingAnimation, true);
        }
        if (this.EnterClearSpeed && !this.ClearSpeedWithDecay) {
            this._monster.Velocity = Vector2.zero;
        }
        if (this.OverideAnimationSpeed) {
            this._monster.animator.SetFloat("AnimationSpeed", this.AnimationSpeed);
        }
        if (base.monster.IsDead()) {
            base.monster.monsterCore.AnimationSpeed = 1f;
        }
        if (this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayer || this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayerAtStart || this.directionRemap == BossGeneralState.MoveDirectionRemap.DirectToPlayerAtStartAbovePlayerGroundY) {
            Vector3 vector = SingletonBehaviour<GameCore>.Instance.player.CenterAnchor.transform.position - base.monster.transform.position;
            vector.z = 0f;
            this.radToPlayer = Mathf.Atan2(vector.y, vector.x);
        }
    }

    // Token: 0x06001C53 RID: 7251 RVA: 0x00072C18 File Offset: 0x00070E18
    public void UpdatePlayerRad() {
        Vector3 vector = SingletonBehaviour<GameCore>.Instance.player.CenterAnchor.transform.position - base.monster.transform.position;
        vector.z = 0f;
        this.radToPlayer = Mathf.Atan2(vector.y, vector.x);
    }

    // Token: 0x06001C54 RID: 7252 RVA: 0x00072C77 File Offset: 0x00070E77
    public override void OnSpriteUpdate() {
    }

    // Token: 0x06001C55 RID: 7253 RVA: 0x00072C79 File Offset: 0x00070E79
    public override void OnShield() {
        if (this.ShieldingAttack != MonsterBase.States.Undefined && this._monster.fsm.HasState(this.ShieldingAttack)) {
            this._monster.fsm.ChangeState(this.ShieldingAttack, false);
        }
    }

    // Token: 0x06001C56 RID: 7254 RVA: 0x00072CB2 File Offset: 0x00070EB2
    public void Start() {
        this.animationEventReceivers = new List<IAnimationEventReceiver>();
        this.animationEventReceivers.AddRange(base.GetComponents<IAnimationEventReceiver>());
    }

    // Token: 0x06001C57 RID: 7255 RVA: 0x00072CD0 File Offset: 0x00070ED0
    public bool CheckInterruptTurnAround() {
        if (!this.HasInterruptTurnaround) {
            return false;
        }
        if (!this._interruptTurnAround && this.StateTimer > this.CanInterruptStartTime && this.StateTimer < this.InterruptJumpToTime && this.IsPlayerBehind) {
            this._interruptTurnAround = true;
            base.monster.animator.Play(this.BindingAnimation, 0, this.InterruptJumpToTime / this.AnimationLength);
            return true;
        }
        return false;
    }

    // Token: 0x06001C58 RID: 7256 RVA: 0x00072D44 File Offset: 0x00070F44
    public void EnterLevelReset() {
        List<LinkNextMoveStateWeight> list = new List<LinkNextMoveStateWeight>();
        this.linkNextMoveStateWeights.Clear();
        this.linkInterruptMoveConditionalWeights.Clear();
        if (this.groupingNodes != null && this.groupingNodes.Length != 0) {
            return;
        }
        base.GetComponentsInChildren<LinkNextMoveStateWeight>(true, list);
        foreach (LinkNextMoveStateWeight linkNextMoveStateWeight in list) {
            if (linkNextMoveStateWeight.gameObject.activeSelf) {
                if (linkNextMoveStateWeight.type == LinkMoveSetType.NextMove) {
                    this.linkNextMoveStateWeights.Add(linkNextMoveStateWeight);
                } else if (linkNextMoveStateWeight.type == LinkMoveSetType.InterruptConditional) {
                    this.linkInterruptMoveConditionalWeights.Add(linkNextMoveStateWeight);
                }
            }
        }
    }

    // Token: 0x06001C59 RID: 7257 RVA: 0x00072DFC File Offset: 0x00070FFC
    public void ExitLevelAndDestroy() {
    }

    // Token: 0x06001C5A RID: 7258 RVA: 0x00072DFE File Offset: 0x00070FFE
    public void OnBeforePrefabSave() {
    }

    // Token: 0x040016B6 RID: 5814
    [Header("自訂State類別:")]
    public MonsterBase.States state;

    // Token: 0x040016B7 RID: 5815
    public List<MonsterBase.States> linkedStateTypes;

    // Token: 0x040016B8 RID: 5816
    public List<MonsterBase.States> QueuedAttacks = new List<MonsterBase.States>();

    // Token: 0x040016B9 RID: 5817
    public List<LinkNextMoveStateWeight> linkNextMoveStateWeights = new List<LinkNextMoveStateWeight>();

    // Token: 0x040016BA RID: 5818
    public List<LinkNextMoveStateWeight> linkInterruptMoveConditionalWeights = new List<LinkNextMoveStateWeight>();

    // Token: 0x040016BB RID: 5819
    public MonsterStateQueue attackQueue;

    // Token: 0x040016BC RID: 5820
    [AutoChildren(false)]
    private TestStateWeight testWeight;

    // Token: 0x040016BD RID: 5821
    private int LinkMoveOptionCount;

    // Token: 0x040016BE RID: 5822
    private int lastMonsterPhase;

    // Token: 0x040016BF RID: 5823
    public MonsterBase.States ToCloseTransitionState;

    // Token: 0x040016C0 RID: 5824
    public BossGeneralState.BridgeState PreviousBridge;

    // Token: 0x040016C1 RID: 5825
    public float RandomAnimationEnterPointMin;

    // Token: 0x040016C2 RID: 5826
    public float RandomAnimationEnterPointMax;

    // Token: 0x040016C3 RID: 5827
    [Header("For Enter")]
    public bool EnterClearSpeed;

    // Token: 0x040016C4 RID: 5828
    public bool ClearSpeedWithDecay;

    // Token: 0x040016C5 RID: 5829
    public float RunReduce = 100f;

    // Token: 0x040016C6 RID: 5830
    [Header("For Shoot")]
    public UnityEvent ShootEvent;

    // Token: 0x040016C7 RID: 5831
    [Header("For Shoot2")]
    public UnityEvent Shoot2Event;

    // Token: 0x040016C8 RID: 5832
    [Header("For Stun")]
    public MonsterBase.States stunState = MonsterBase.States.Stun;

    // Token: 0x040016C9 RID: 5833
    [Header("For Dash")]
    public bool AutoDoneIfCollisionWall;

    // Token: 0x040016CA RID: 5834
    [Header("Exit")]
    public bool SpeedRemain;

    // Token: 0x040016CB RID: 5835
    public AttackSensor bindSensor;

    // Token: 0x040016CC RID: 5836
    [Auto(false)]
    private LinkMoveCountMaxProvider myLinkMoveMaxProvider;

    // Token: 0x040016CD RID: 5837
    private LinkMoveCountMaxProvider bindLinkMoveMaxProvider;

    // Token: 0x040016CE RID: 5838
    [AutoParent(true)]
    private StateCoolDownProvider StateEndStateCoolDownProvider;

    // Token: 0x040016CF RID: 5839
    [Auto(false)]
    private LinkMoveExtendProvider linkMoveExtend;

    // Token: 0x040016D0 RID: 5840
    [AutoChildren(false)]
    [PreviewInInspector]
    private LinkMoveGroupingNode[] groupingNodes;

    // Token: 0x040016D1 RID: 5841
    private int linkMoveGroupIndex;

    // Token: 0x040016D2 RID: 5842
    private bool IsContinuousEvaluateMoveScale;

    // Token: 0x040016D3 RID: 5843
    private float radToPlayer;

    // Token: 0x040016D4 RID: 5844
    private bool IsAttackState;

    // Token: 0x040016D5 RID: 5845
    [FormerlySerializedAs("ClearSpeed")]
    public bool ClearSpeedWhenExit;

    // Token: 0x040016D6 RID: 5846
    public bool AutoTurnAround = true;

    // Token: 0x040016D7 RID: 5847
    public bool AutoFlipAround;

    // Token: 0x040016D8 RID: 5848
    public bool ExitIfPlayerBehind;

    // Token: 0x040016D9 RID: 5849
    [Header("For Shielding")]
    public MonsterBase.States ShieldingAttack;

    // Token: 0x040016DA RID: 5850
    [Header("Damage")]
    public List<Collider2D> disableDamageBundles;

    // Token: 0x040016DB RID: 5851
    public bool OverideAnimationSpeed;

    // Token: 0x040016DC RID: 5852
    public float AnimationSpeed = 1f;

    // Token: 0x040016DD RID: 5853
    [Header("Outsourcing")]
    public ICutScene animatorController;

    // Token: 0x040016DE RID: 5854
    private List<IAnimationEventReceiver> animationEventReceivers;

    // Token: 0x040016DF RID: 5855
    [Header("TooFarInterrupt")]
    public float TooFarValue = 256f;

    // Token: 0x040016E0 RID: 5856
    [Header("AttackSensor Settings")]
    [Range(-3f, 3f)]
    public float DelayOffset;

    // Token: 0x040016E1 RID: 5857
    [Range(-10f, 10f)]
    public float ShareTokenCooldownOffset;

    // Token: 0x040016E2 RID: 5858
    [Header("Interrupt Turnaround")]
    public bool HasInterruptTurnaround;

    // Token: 0x040016E3 RID: 5859
    private bool _interruptTurnAround;

    // Token: 0x040016E4 RID: 5860
    public float CanInterruptStartTime = 1f;

    // Token: 0x040016E5 RID: 5861
    public float InterruptJumpToTime = 7f;

    // Token: 0x040016E6 RID: 5862
    public float AnimationLength = 7.71f;

    // Token: 0x040016E7 RID: 5863
    [Header("MoveDirectionRemap")]
    public BossGeneralState.MoveDirectionRemap directionRemap;

    // Token: 0x040016E8 RID: 5864
    public List<float> damageOnTimeList;

    // Token: 0x02000ECC RID: 3788
    public enum BridgeState {
        // Token: 0x04004F79 RID: 20345
        None,
        // Token: 0x04004F7A RID: 20346
        ToPlayer,
        // Token: 0x04004F7B RID: 20347
        ResetRightOrLeft,
        // Token: 0x04004F7C RID: 20348
        Center
    }

    // Token: 0x02000ECD RID: 3789
    public enum MoveDirectionRemap {
        // Token: 0x04004F7E RID: 20350
        Normal,
        // Token: 0x04004F7F RID: 20351
        DirectToPlayer,
        // Token: 0x04004F80 RID: 20352
        DirectToPlayerAtStart,
        // Token: 0x04004F81 RID: 20353
        DirectToPlayerAtStartAbovePlayerGroundY
    }
}
