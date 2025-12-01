using System;
using System.Collections.Generic;
using BezierSolution;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000660 RID: 1632
[ExecuteInEditMode]
public class PositionOnBezierSpline : MonoBehaviour {
    // Token: 0x1700055C RID: 1372
    // (get) Token: 0x0600267F RID: 9855 RVA: 0x0009DD8B File Offset: 0x0009BF8B
    public Transform DockerTransform {
        get {
            if (this._DockerTransform == null) {
                return base.transform;
            }
            return this._DockerTransform;
        }
    }

    // Token: 0x1700055D RID: 1373
    // (get) Token: 0x06002680 RID: 9856 RVA: 0x0009DDA8 File Offset: 0x0009BFA8
    private float currentDockPoint {
        get {
            return this.targetSpeeds[Mathf.RoundToInt(this.Ratio * (float)(this.targetSpeeds.Count - 1))];
        }
    }

    // Token: 0x06002681 RID: 9857 RVA: 0x0009DDCF File Offset: 0x0009BFCF
    private void Awake() {
    }

    // Token: 0x06002682 RID: 9858 RVA: 0x0009DDD1 File Offset: 0x0009BFD1
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.DockerTransform.position, 10f);
    }

    // Token: 0x06002683 RID: 9859 RVA: 0x0009DDF4 File Offset: 0x0009BFF4
    private void Update() {
        if (this.refSpline == null) {
            return;
        }
        Vector3 position = this.DockerTransform.position;
        if (this.targetSpeeds != null && this.targetSpeeds.Count > 0) {
            if (this.currentDockPoint > this.speed) {
                this.speed += RCGTime.deltaTime * this.acc;
                if (this.speed > this.currentDockPoint) {
                    this.speed = this.currentDockPoint;
                }
            } else {
                this.speed -= RCGTime.deltaTime * this.acc;
                if (this.speed < this.currentDockPoint) {
                    this.speed = this.currentDockPoint;
                }
            }
        } else {
            this.speed += RCGTime.deltaTime * this.acc;
            if (this.speed > this.maxSpeed) {
                this.speed = this.maxSpeed;
            }
            if (this.speed < this.minSpeed) {
                this.speed = this.minSpeed;
            }
        }
        if (this.Simulate && Application.isPlaying) {
            this.DockerTransform.position = this.refSpline.MoveAlongSpline(ref this.Ratio, this.speed * RCGTime.deltaTime, 1);
        } else {
            this.DockerTransform.position = this.refSpline.MoveAlongSpline(ref this.Ratio, 0f, 1);
        }
        if (position != this.DockerTransform.position) {
            Vector3 vector = this.DockerTransform.position - position;
            vector.z = 0f;
            this.dir = vector.normalized;
            if (this.OnPositionUpdate != null) {
                this.OnPositionUpdate.Invoke();
            }
            if (this.AutoRotateAlignVector) {
                this.RotateAlignVector(this.refSpline.GetTangent(this.Ratio));
            }
        }
        if (Application.isPlaying) {
            this.CheckDettachTime();
            this.CheckDetachAngle();
        }
        this.beforeRatio = this.Ratio;
    }

    // Token: 0x06002684 RID: 9860 RVA: 0x0009DFF0 File Offset: 0x0009C1F0
    private void CheckDetachAngle() {
        if (!this.AutoDettachByAngle) {
            return;
        }
        Vector2 lhs = this.refSpline.GetTangent(this.Ratio).normalized;
        Vector2 rhs = (Player.i.transform.position - this.DockerTransform.position).normalized;
        if (Vector2.Dot(lhs, rhs) > Mathf.Cos(0.017453292f * this.DettachAngle)) {
            this.DetachAllProjectile(this.speed, this.DockerTransform.right);
        }
    }

    // Token: 0x06002685 RID: 9861 RVA: 0x0009E081 File Offset: 0x0009C281
    private void CheckDettachTime() {
        if (!this.AutoDettachProjectile) {
            return;
        }
        if (this.beforeRatio < this.DettachRatio && this.Ratio >= this.DettachRatio) {
            this.DetachAllProjectile(this.speed, this.DockerTransform.right);
        }
    }

    // Token: 0x06002686 RID: 9862 RVA: 0x0009E0C0 File Offset: 0x0009C2C0
    public void DetachAllProjectile(float speed, Vector3 direction) {
        if (this.DockerTransform.childCount == 0) {
            return;
        }
        Vector3 vector = speed * direction;
        if (this.DockerTransform.lossyScale.x < 0f) {
            vector = -vector;
        }
        if (this.DockerTransform.childCount == 0) {
            return;
        }
        foreach (Projectile projectile in this.DockerTransform.GetComponentsInChildren<Projectile>()) {
            if (projectile.gameObject.activeInHierarchy) {
                projectile.DetachFromPath(vector);
                if (this.AutoDestroyOnDettach) {
                    projectile.InvokePathDestroy();
                }
            }
        }
    }

    // Token: 0x06002687 RID: 9863 RVA: 0x0009E14F File Offset: 0x0009C34F
    public void Reset() {
        this.Ratio = 0f;
        this.speed = this.initSpeed;
        this.DockerTransform.position = this.refSpline.MoveAlongSpline(ref this.Ratio, 0f, 1);
    }

    // Token: 0x06002688 RID: 9864 RVA: 0x0009E18C File Offset: 0x0009C38C
    private void RotateAlignVector(Vector2 dir) {
        if (dir.magnitude == 0f) {
            return;
        }
        if (this.DockerTransform.lossyScale.x <= 0f) {
            if (this.DockerTransform.lossyScale.x < 0f) {
                float num = Vector2.SignedAngle(Vector2.right, dir);
                if (float.IsNaN(num)) {
                    return;
                }
                this.DockerTransform.localEulerAngles = new Vector3(0f, 0f, -num + 180f);
            }
            return;
        }
        float num2 = Vector2.SignedAngle(Vector2.right, dir);
        if (float.IsNaN(num2)) {
            return;
        }
        this.DockerTransform.localEulerAngles = new Vector3(0f, 0f, num2);
    }

    // Token: 0x04001F95 RID: 8085
    public Transform _DockerTransform;

    // Token: 0x04001F96 RID: 8086
    public BezierSpline refSpline;

    // Token: 0x04001F97 RID: 8087
    public Vector3 dir;

    // Token: 0x04001F98 RID: 8088
    public bool AutoRotateAlignVector;

    // Token: 0x04001F99 RID: 8089
    public AnimationCurve targetSpeedCurve;

    // Token: 0x04001F9A RID: 8090
    public List<float> targetSpeeds;

    // Token: 0x04001F9B RID: 8091
    [Header("Control by Animation")]
    [Range(0f, 1f)]
    public float Ratio;

    // Token: 0x04001F9C RID: 8092
    [Header("Control by V")]
    public float initSpeed = 100f;

    // Token: 0x04001F9D RID: 8093
    public float minSpeed = 100f;

    // Token: 0x04001F9E RID: 8094
    public float maxSpeed = 400f;

    // Token: 0x04001F9F RID: 8095
    public float acc = 600f;

    // Token: 0x04001FA0 RID: 8096
    public float speed = 100f;

    // Token: 0x04001FA1 RID: 8097
    public bool Simulate;

    // Token: 0x04001FA2 RID: 8098
    [Header("AutoDetach")]
    public bool AutoDettachProjectile;

    // Token: 0x04001FA3 RID: 8099
    public bool AutoDettachByAngle;

    // Token: 0x04001FA4 RID: 8100
    public float DettachAngle = 15f;

    // Token: 0x04001FA5 RID: 8101
    [Range(0f, 1f)]
    public float DettachRatio = 0.5f;

    // Token: 0x04001FA6 RID: 8102
    private float beforeRatio;

    // Token: 0x04001FA7 RID: 8103
    [Header("AutoDestroy")]
    public bool OverrideDestroyTime = true;

    // Token: 0x04001FA8 RID: 8104
    public float DestroyTime = 10f;

    // Token: 0x04001FA9 RID: 8105
    public bool AutoDestroyOnDettach;

    // Token: 0x04001FAA RID: 8106
    public UnityEvent OnPositionUpdate;
}

