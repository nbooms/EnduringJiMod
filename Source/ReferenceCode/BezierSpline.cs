using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution {
    // Token: 0x0200000D RID: 13
    [DefaultExecutionOrder(-1000)]
    [ExecuteInEditMode]
    public class BezierSpline : MonoBehaviour {
        // Token: 0x17000039 RID: 57
        // (get) Token: 0x060000CF RID: 207 RVA: 0x000048DE File Offset: 0x00002ADE
        public List<BezierPoint> EndPoints {
            get {
                return this.endPoints;
            }
        }

        // Token: 0x1700003A RID: 58
        // (get) Token: 0x060000D0 RID: 208 RVA: 0x000048E6 File Offset: 0x00002AE6
        public int Count {
            get {
                return this.endPoints.Count;
            }
        }

        // Token: 0x1700003B RID: 59
        // (get) Token: 0x060000D1 RID: 209 RVA: 0x000048F3 File Offset: 0x00002AF3
        public float Length {
            get {
                this._length = this.GetLengthApproximately(0f, 1f, 1000f);
                return this._length;
            }
        }

        // Token: 0x1700003C RID: 60
        public BezierPoint this[int index] {
            get {
                if (index < this.Count) {
                    return this.endPoints[index];
                }
                Debug.LogError("Bezier index " + index.ToString() + " is out of range: " + this.Count.ToString());
                return null;
            }
        }

        // Token: 0x060000D3 RID: 211 RVA: 0x00004965 File Offset: 0x00002B65
        private void OnEnable() {
            this.Refresh();
        }

        // Token: 0x060000D4 RID: 212 RVA: 0x0000496D File Offset: 0x00002B6D
        private void LateUpdate() {
            this.CheckDirty();
        }

        // Token: 0x060000D5 RID: 213 RVA: 0x00004978 File Offset: 0x00002B78
        internal void CheckDirty() {
            for (int i = 0; i < this.endPoints.Count; i++) {
                this.endPoints[i].RefreshIfChanged();
            }
            if (this.isDirty) {
                UnityEvent<BezierSpline> unityEvent = this.onSplineChanged;
                if (unityEvent != null) {
                    unityEvent.Invoke(this);
                }
            }
            this.isDirty = false;
        }

        // Token: 0x060000D6 RID: 214 RVA: 0x000049CD File Offset: 0x00002BCD
        private void OnTransformChildrenChanged() {
            this.Refresh();
        }

        // Token: 0x060000D7 RID: 215 RVA: 0x000049D8 File Offset: 0x00002BD8
        public void Initialize(int endPointsCount) {
            if (endPointsCount < 2) {
                Debug.LogError("Can't initialize spline with " + endPointsCount.ToString() + " point(s). At least 2 points are needed");
                return;
            }
            for (int i = this.endPoints.Count - 1; i >= 0; i--) {
                UnityEngine.Object.DestroyImmediate(this.endPoints[i].gameObject);
            }
            this.endPoints.Clear();
            for (int j = 0; j < endPointsCount; j++) {
                this.InsertNewPointAt(j);
            }
            this.Refresh();
        }

        // Token: 0x060000D8 RID: 216 RVA: 0x00004A58 File Offset: 0x00002C58
        public void Refresh() {
            this.splineRenderer = base.GetComponentInParent<ISplineRenderer>(true);
            this.endPoints.Clear();
            base.GetComponentsInChildren<BezierPoint>(this.endPoints);
            foreach (BezierPoint bezierPoint in this.endPoints) {
                bezierPoint.parentPath = this;
                bezierPoint.Refresh();
            }
        }

        // Token: 0x060000D9 RID: 217 RVA: 0x00004AD4 File Offset: 0x00002CD4
        public BezierPoint InsertNewPointAt(int index) {
            if (index < 0 || index > this.endPoints.Count) {
                Debug.LogError(string.Concat(new string[]
                {
                    "Index ",
                    index.ToString(),
                    " is out of range: [0,",
                    this.endPoints.Count.ToString(),
                    "]"
                }));
                return null;
            }
            int count = this.endPoints.Count;
            BezierPoint bezierPoint = new GameObject("Point").AddComponent<BezierPoint>();
            bezierPoint.parentPath = this;
            bezierPoint.transform.SetParent((this.endPoints.Count == 0) ? base.transform : ((index == 0) ? this.endPoints[0].transform.parent : this.endPoints[index - 1].transform.parent), false);
            bezierPoint.transform.SetSiblingIndex((index == 0) ? 0 : (this.endPoints[index - 1].transform.GetSiblingIndex() + 1));
            bezierPoint.precedingControlPointLocalPosition = new Vector3(100f, 0f, 0f);
            bezierPoint.followingControlPointLocalPosition = new Vector3(100f, 0f, 0f);
            if (this.endPoints.Count == count) {
                this.endPoints.Insert(index, bezierPoint);
            }
            return bezierPoint;
        }

        // Token: 0x060000DA RID: 218 RVA: 0x00004C30 File Offset: 0x00002E30
        public BezierPoint DuplicatePointAt(int index) {
            if (index < 0 || index >= this.endPoints.Count) {
                Debug.LogError(string.Concat(new string[]
                {
                    "Index ",
                    index.ToString(),
                    " is out of range: [0,",
                    (this.endPoints.Count - 1).ToString(),
                    "]"
                }));
                return null;
            }
            BezierPoint bezierPoint = this.InsertNewPointAt(index + 1);
            this.endPoints[index].CopyTo(bezierPoint);
            return bezierPoint;
        }

        // Token: 0x060000DB RID: 219 RVA: 0x00004CB8 File Offset: 0x00002EB8
        public void RemovePointAt(int index) {
            if (this.endPoints.Count <= 2) {
                Debug.LogError("Can't remove point: spline must consist of at least two points!");
                return;
            }
            if (index < 0 || index >= this.endPoints.Count) {
                Debug.LogError(string.Concat(new string[]
                {
                    "Index ",
                    index.ToString(),
                    " is out of range: [0,",
                    this.endPoints.Count.ToString(),
                    ")"
                }));
                return;
            }
            Component component = this.endPoints[index];
            this.endPoints.RemoveAt(index);
            UnityEngine.Object.DestroyImmediate(component.gameObject);
        }

        // Token: 0x060000DC RID: 220 RVA: 0x00004D5C File Offset: 0x00002F5C
        public void Reverse() {
            for (int i = 0; i < this.EndPoints.Count / 2; i++) {
                Transform transform = this.EndPoints[i].transform;
                Transform transform2 = this.EndPoints[this.EndPoints.Count - i - 1].transform;
                Vector3 position = this.EndPoints[this.EndPoints.Count - i - 1].transform.position;
                Vector3 position2 = this.EndPoints[i].transform.position;
                transform.position = position;
                transform2.position = position2;
                Vector3 precedingControlPointPosition = this.EndPoints[i].precedingControlPointPosition;
                Vector3 followingControlPointPosition = this.EndPoints[i].followingControlPointPosition;
                this.EndPoints[i].precedingControlPointPosition = this.EndPoints[this.EndPoints.Count - i - 1].followingControlPointPosition;
                this.EndPoints[i].followingControlPointPosition = this.EndPoints[this.EndPoints.Count - i - 1].precedingControlPointPosition;
                this.EndPoints[this.EndPoints.Count - i - 1].followingControlPointPosition = precedingControlPointPosition;
                this.EndPoints[this.EndPoints.Count - i - 1].precedingControlPointPosition = followingControlPointPosition;
            }
            if (this.isConstructLinear) {
                this.ConstructLinearPath();
            }
        }

        // Token: 0x060000DD RID: 221 RVA: 0x00004EE0 File Offset: 0x000030E0
        public void SwapPointsAt(int index1, int index2) {
            if (index1 == index2) {
                Debug.LogError("Indices can't be equal to each other");
                return;
            }
            if (index1 < 0 || index1 >= this.endPoints.Count || index2 < 0 || index2 >= this.endPoints.Count) {
                Debug.LogError("Indices must be in range [0," + (this.endPoints.Count - 1).ToString() + "]");
                return;
            }
            BezierPoint bezierPoint = this.endPoints[index1];
            int siblingIndex = bezierPoint.transform.GetSiblingIndex();
            this.endPoints[index1] = this.endPoints[index2];
            this.endPoints[index2] = bezierPoint;
            bezierPoint.transform.SetSiblingIndex(this.endPoints[index1].transform.GetSiblingIndex());
            this.endPoints[index1].transform.SetSiblingIndex(siblingIndex);
        }

        // Token: 0x060000DE RID: 222 RVA: 0x00004FC0 File Offset: 0x000031C0
        public int IndexOf(BezierPoint point) {
            return this.endPoints.IndexOf(point);
        }

        // Token: 0x060000DF RID: 223 RVA: 0x00004FCE File Offset: 0x000031CE
        public void DrawGizmos(Color color, int smoothness = 4) {
            this.drawGizmos = true;
            this.gizmoColor = color;
            this.gizmoStep = 1f / (float)(this.endPoints.Count * Mathf.Clamp(smoothness, 1, 30));
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x00005000 File Offset: 0x00003200
        public void HideGizmos() {
            this.drawGizmos = false;
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x00005009 File Offset: 0x00003209
        public void RefreshCheck() {
            if (!this._isRefreshed) {
                this.Refresh();
                this._isRefreshed = true;
            }
        }

        // Token: 0x060000E2 RID: 226 RVA: 0x00005020 File Offset: 0x00003220
        public Vector3 GetPoint(float normalizedT) {
            if (!this.loop) {
                if (normalizedT <= 0f) {
                    return this.endPoints[0].position;
                }
                if (normalizedT >= 1f) {
                    List<BezierPoint> list = this.endPoints;
                    return list[list.Count - 1].position;
                }
            } else if (normalizedT < 0f) {
                normalizedT += 1f;
            } else if (normalizedT >= 1f) {
                normalizedT -= 1f;
            }
            float num = normalizedT * (float)(this.loop ? this.endPoints.Count : (this.endPoints.Count - 1));
            int num2 = (int)num;
            int num3 = num2 + 1;
            if (num3 == this.endPoints.Count) {
                num3 = 0;
            }
            BezierPoint bezierPoint = this.endPoints[num2];
            BezierPoint bezierPoint2 = this.endPoints[num3];
            float num4 = num - (float)num2;
            float num5 = 1f - num4;
            return num5 * num5 * num5 * bezierPoint.position + 3f * num5 * num5 * num4 * bezierPoint.followingControlPointPosition + 3f * num5 * num4 * num4 * bezierPoint2.precedingControlPointPosition + num4 * num4 * num4 * bezierPoint2.position;
        }

        // Token: 0x060000E3 RID: 227 RVA: 0x00005160 File Offset: 0x00003360
        public float GetSize(float normalizedT) {
            if (!this.loop) {
                if (normalizedT <= 0f) {
                    return this.endPoints[0].Size;
                }
                if (normalizedT >= 1f) {
                    List<BezierPoint> list = this.endPoints;
                    return list[list.Count - 1].Size;
                }
            } else if (normalizedT < 0f) {
                normalizedT += 1f;
            } else if (normalizedT >= 1f) {
                normalizedT -= 1f;
            }
            float num = normalizedT * (float)(this.loop ? this.endPoints.Count : (this.endPoints.Count - 1));
            int num2 = (int)num;
            int num3 = num2 + 1;
            if (num3 == this.endPoints.Count) {
                num3 = 0;
            }
            BezierPoint bezierPoint = this.endPoints[num2];
            BezierPoint bezierPoint2 = this.endPoints[num3];
            float t = num - (float)num2;
            return Mathf.Lerp(bezierPoint.Size, bezierPoint2.Size, t);
        }

        // Token: 0x060000E4 RID: 228 RVA: 0x00005244 File Offset: 0x00003444
        public Vector3 GetPointAtLength(float targetLength, ref float normalizedT, float accuracy = 100f) {
            Vector3 vector = Vector3.zero;
            normalizedT = -1f;
            float num = this.AccuracyToStepSize(accuracy);
            float num2 = 0f;
            Vector3 vector2 = this.GetPoint(0f);
            Vector3 point = this.GetPoint(0f);
            float num3 = 0f;
            if (targetLength == 0f) {
                vector = this.GetPoint(0f);
                normalizedT = 0f;
                return vector;
            }
            float num4 = 0f;
            while (num4 < 1f) {
                point = this.GetPoint(num4);
                num2 += Vector3.Distance(point, vector2);
                if (targetLength <= num2) {
                    if (targetLength - num3 < num2 - targetLength) {
                        vector = vector2;
                        normalizedT = num4 - num;
                        break;
                    }
                    vector = point;
                    normalizedT = num4;
                    break;
                } else {
                    vector2 = point;
                    num3 = num2;
                    num4 += num;
                }
            }
            if (vector == Vector3.zero) {
                vector = this.GetPoint(1f);
                normalizedT = 1f;
                return vector;
            }
            return vector;
        }

        // Token: 0x060000E5 RID: 229 RVA: 0x00005320 File Offset: 0x00003520
        [return: TupleElementNames(new string[]
        {
            "point",
            "tangent"
        })]
        public ValueTuple<Vector3, Vector3> GetPointAndTangent(float normalizedT) {
            if (!this.loop) {
                if (normalizedT <= 0f) {
                    return new ValueTuple<Vector3, Vector3>(this.endPoints[0].Position, 3f * (this.endPoints[0].m_followingControlPointPosition - this.endPoints[0].Position));
                }
                if (normalizedT >= 1f) {
                    int index = this.endPoints.Count - 1;
                    return new ValueTuple<Vector3, Vector3>(this.endPoints[index].Position, 3f * (this.endPoints[index].Position - this.endPoints[index].m_precedingControlPointPosition));
                }
            } else if (normalizedT < 0f) {
                normalizedT += 1f;
            } else if (normalizedT >= 1f) {
                normalizedT -= 1f;
            }
            float num = normalizedT * (float)(this.loop ? this.endPoints.Count : (this.endPoints.Count - 1));
            int num2 = (int)num;
            int num3 = num2 + 1;
            if (num3 == this.endPoints.Count) {
                num3 = 0;
            }
            BezierPoint bezierPoint = this.endPoints[num2];
            BezierPoint bezierPoint2 = this.endPoints[num3];
            float num4 = num - (float)num2;
            float num5 = 1f - num4;
            float num6 = num5 * num5 * 3f;
            float d = num5 * num5 * num5;
            float num7 = num4 * num4;
            float num8 = 3f * num7;
            Vector3 item = d * bezierPoint.Position + num6 * num4 * bezierPoint.m_followingControlPointPosition + num8 * num5 * bezierPoint2.m_precedingControlPointPosition + num7 * num4 * bezierPoint2.Position;
            Vector3 item2 = num6 * (bezierPoint.m_followingControlPointPosition - bezierPoint.Position) + 6f * num5 * num4 * (bezierPoint2.m_precedingControlPointPosition - bezierPoint.m_followingControlPointPosition) + num8 * (bezierPoint2.Position - bezierPoint2.m_precedingControlPointPosition);
            return new ValueTuple<Vector3, Vector3>(item, item2);
        }

        // Token: 0x060000E6 RID: 230 RVA: 0x00005548 File Offset: 0x00003748
        public Vector3 GetPointCatmull(float t) {
            int num = this.endPoints.Count - 3;
            int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
            float num3 = t * (float)num - (float)num2;
            Vector3 position = this.endPoints[num2].position;
            Vector3 position2 = this.endPoints[num2 + 1].position;
            Vector3 position3 = this.endPoints[num2 + 2].position;
            Vector3 position4 = this.endPoints[num2 + 3].position;
            return 0.5f * ((-position + 3f * position2 - 3f * position3 + position4) * (num3 * num3 * num3) + (2f * position - 5f * position2 + 4f * position3 - position4) * (num3 * num3) + (-position + position3) * num3 + 2f * position2);
        }

        // Token: 0x060000E7 RID: 231 RVA: 0x0000567C File Offset: 0x0000387C
        public Vector3 GetTangentCatmull(float t) {
            int num = this.endPoints.Count - 3;
            int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
            float num3 = t * (float)num - (float)num2;
            Vector3 position = this.endPoints[num2].position;
            Vector3 position2 = this.endPoints[num2 + 1].position;
            Vector3 position3 = this.endPoints[num2 + 2].position;
            Vector3 position4 = this.endPoints[num2 + 3].position;
            return 0.5f * ((-3f * position + 9f * position2 - 9f * position3 + 3f * position4) * (num3 * num3) + (4f * position - 10f * position2 + 8f * position3 - 2f * position4) * num3 + (-position + position3)).normalized;
        }

        // Token: 0x060000E8 RID: 232 RVA: 0x000057B8 File Offset: 0x000039B8
        public Vector3 GetTangent(float normalizedT) {
            if (!this.loop) {
                if (normalizedT <= 0f) {
                    return 3f * (this.endPoints[0].followingControlPointPosition - this.endPoints[0].position);
                }
                if (normalizedT >= 1f) {
                    int index = this.endPoints.Count - 1;
                    return 3f * (this.endPoints[index].position - this.endPoints[index].precedingControlPointPosition);
                }
            } else if (normalizedT < 0f) {
                normalizedT += 1f;
            } else if (normalizedT >= 1f) {
                normalizedT -= 1f;
            }
            float num = normalizedT * (float)(this.loop ? this.endPoints.Count : (this.endPoints.Count - 1));
            int num2 = (int)num;
            int num3 = num2 + 1;
            if (num3 == this.endPoints.Count) {
                num3 = 0;
            }
            BezierPoint bezierPoint = this.endPoints[num2];
            BezierPoint bezierPoint2 = this.endPoints[num3];
            float num4 = num - (float)num2;
            float num5 = 1f - num4;
            return 3f * num5 * num5 * (bezierPoint.followingControlPointPosition - bezierPoint.position) + 6f * num5 * num4 * (bezierPoint2.precedingControlPointPosition - bezierPoint.followingControlPointPosition) + 3f * num4 * num4 * (bezierPoint2.position - bezierPoint2.precedingControlPointPosition);
        }

        // Token: 0x060000E9 RID: 233 RVA: 0x0000594C File Offset: 0x00003B4C
        public float GetLengthApproximately(float startNormalizedT, float endNormalizedT, float accuracy = 50f) {
            if (this.isConstructLinear) {
                float num = 0f;
                for (int i = 0; i < this.Count - 1; i++) {
                    num += Vector3.Distance(this[i].transform.position, this[i + 1].transform.position);
                }
                if (this.loop) {
                    num += Vector3.Distance(this[0].transform.position, this[this.Count - 1].transform.position);
                }
                return num;
            }
            if (endNormalizedT < startNormalizedT) {
                float num2 = startNormalizedT;
                startNormalizedT = endNormalizedT;
                endNormalizedT = num2;
            }
            if (startNormalizedT < 0f) {
                startNormalizedT = 0f;
            }
            if (endNormalizedT > 1f) {
                endNormalizedT = 1f;
            }
            float num3 = this.AccuracyToStepSize(accuracy) * (endNormalizedT - startNormalizedT);
            int num4 = 1;
            float num5 = 0f;
            this[0].percentage = 0f;
            Vector3 vector = this.GetPoint(startNormalizedT);
            for (float num6 = startNormalizedT + num3; num6 <= endNormalizedT; num6 += num3) {
                Vector3 point = this.GetPoint(num6);
                num5 += Vector3.Distance(point, vector);
                if (!this.isCalculated && num4 < this.Count && (this[num4].position - point).magnitude <= Vector3.Distance(point, vector)) {
                    this[num4].percentage = num6;
                    num4++;
                }
                vector = point;
            }
            this.isCalculated = true;
            return num5 + Vector3.Distance(vector, this.GetPoint(endNormalizedT));
        }

        // Token: 0x060000EA RID: 234 RVA: 0x00005AD0 File Offset: 0x00003CD0
        public Vector3 FindNearestPointTo(Vector3 worldPos, out float normalizedT, float accuracy = 100f) {
            Vector3 result = Vector3.zero;
            normalizedT = -1f;
            float num = this.AccuracyToStepSize(accuracy);
            float num2 = float.PositiveInfinity;
            for (float num3 = 0f; num3 < 1f; num3 += num) {
                Vector3 point = this.GetPoint(num3);
                float sqrMagnitude = (worldPos - point).sqrMagnitude;
                if (sqrMagnitude < num2) {
                    num2 = sqrMagnitude;
                    result = point;
                    normalizedT = num3;
                }
            }
            return result;
        }

        // Token: 0x060000EB RID: 235 RVA: 0x00005B38 File Offset: 0x00003D38
        public Vector3 MoveAlongSpline(ref float normalizedT, float deltaMovement, int accuracy = 1) {
            float num = deltaMovement / (float)((this.loop ? this.endPoints.Count : (this.endPoints.Count - 1)) * accuracy);
            for (int i = 0; i < accuracy; i++) {
                normalizedT += num / this.GetTangent(normalizedT).magnitude;
            }
            if (!this.loop) {
                if (normalizedT > 1f) {
                    normalizedT = 1f;
                } else if (normalizedT < 0f) {
                    normalizedT = 0f;
                }
            }
            return this.GetPoint(normalizedT);
        }

        // Token: 0x060000EC RID: 236 RVA: 0x00005BC4 File Offset: 0x00003DC4
        public void ConstructLinearPath() {
            this.isConstructLinear = true;
            for (int i = 0; i < this.endPoints.Count; i++) {
                this.endPoints[i].handleMode = BezierPoint.HandleMode.Free;
                if (i < this.endPoints.Count - 1) {
                    Vector3 vector = (this.endPoints[i].position + this.endPoints[i + 1].position) * 0.5f;
                    this.endPoints[i + 1].precedingControlPointPosition = vector;
                    this.endPoints[i].followingControlPointPosition = vector;
                } else {
                    Vector3 vector2 = (this.endPoints[i].position + this.endPoints[0].position) * 0.5f;
                    this.endPoints[i].followingControlPointPosition = vector2;
                    this.endPoints[0].precedingControlPointPosition = vector2;
                }
            }
        }

        // Token: 0x060000ED RID: 237 RVA: 0x00005CC8 File Offset: 0x00003EC8
        public void AutoConstructSpline() {
            this.isConstructLinear = false;
            for (int i = 0; i < this.endPoints.Count; i++) {
                this.endPoints[i].handleMode = BezierPoint.HandleMode.Mirrored;
            }
            int num = this.endPoints.Count - 1;
            if (num == 1) {
                this.endPoints[0].followingControlPointPosition = (2f * this.endPoints[0].position + this.endPoints[1].position) / 3f;
                this.endPoints[1].precedingControlPointPosition = 2f * this.endPoints[0].followingControlPointPosition - this.endPoints[0].position;
                return;
            }
            Vector3[] array;
            if (this.loop) {
                array = new Vector3[num + 1];
            } else {
                array = new Vector3[num];
            }
            for (int j = 1; j < num - 1; j++) {
                array[j] = 4f * this.endPoints[j].position + 2f * this.endPoints[j + 1].position;
            }
            array[0] = this.endPoints[0].position + 2f * this.endPoints[1].position;
            if (!this.loop) {
                array[num - 1] = (8f * this.endPoints[num - 1].position + this.endPoints[num].position) * 0.5f;
            } else {
                array[num - 1] = 4f * this.endPoints[num - 1].position + 2f * this.endPoints[num].position;
                array[num] = (8f * this.endPoints[num].position + this.endPoints[0].position) * 0.5f;
            }
            Vector3[] firstControlPoints = BezierSpline.GetFirstControlPoints(array);
            for (int k = 0; k < num; k++) {
                this.endPoints[k].followingControlPointPosition = firstControlPoints[k];
                if (this.loop) {
                    this.endPoints[k + 1].precedingControlPointPosition = 2f * this.endPoints[k + 1].position - firstControlPoints[k + 1];
                } else if (k < num - 1) {
                    this.endPoints[k + 1].precedingControlPointPosition = 2f * this.endPoints[k + 1].position - firstControlPoints[k + 1];
                } else {
                    this.endPoints[k + 1].precedingControlPointPosition = (this.endPoints[num].position + firstControlPoints[num - 1]) * 0.5f;
                }
            }
            if (this.loop) {
                float d = Vector3.Distance(this.endPoints[0].followingControlPointPosition, this.endPoints[0].position);
                Vector3 a = Vector3.Normalize(this.endPoints[num].position - this.endPoints[1].position);
                this.endPoints[0].precedingControlPointPosition = this.endPoints[0].position + a * d;
                this.endPoints[0].followingControlPointLocalPosition = -this.endPoints[0].precedingControlPointLocalPosition;
            }
        }

        // Token: 0x060000EE RID: 238 RVA: 0x000060F4 File Offset: 0x000042F4
        private static Vector3[] GetFirstControlPoints(Vector3[] rhs) {
            int num = rhs.Length;
            Vector3[] array = new Vector3[num];
            float[] array2 = new float[num];
            float num2 = 2f;
            array[0] = rhs[0] / num2;
            for (int i = 1; i < num; i++) {
                float num3 = 1f / num2;
                array2[i] = num3;
                num2 = ((i < num - 1) ? 4f : 3.5f) - num3;
                array[i] = (rhs[i] - array[i - 1]) / num2;
            }
            for (int j = 1; j < num; j++) {
                array[num - j - 1] -= array2[num - j] * array[num - j];
            }
            return array;
        }

        // Token: 0x060000EF RID: 239 RVA: 0x000061CC File Offset: 0x000043CC
        public void AutoConstructSpline2() {
            this.isConstructLinear = false;
            for (int i = 0; i < this.endPoints.Count; i++) {
                Vector3 position;
                if (i == 0) {
                    if (this.loop) {
                        position = this.endPoints[this.endPoints.Count - 1].position;
                    } else {
                        position = this.endPoints[0].position;
                    }
                } else {
                    position = this.endPoints[i - 1].position;
                }
                Vector3 position2;
                Vector3 position3;
                if (this.loop) {
                    position2 = this.endPoints[(i + 1) % this.endPoints.Count].position;
                    position3 = this.endPoints[(i + 2) % this.endPoints.Count].position;
                } else if (i < this.endPoints.Count - 2) {
                    position2 = this.endPoints[i + 1].position;
                    position3 = this.endPoints[i + 2].position;
                } else if (i == this.endPoints.Count - 2) {
                    position2 = this.endPoints[i + 1].position;
                    position3 = this.endPoints[i + 1].position;
                } else {
                    position2 = this.endPoints[i].position;
                    position3 = this.endPoints[i].position;
                }
                this.endPoints[i].followingControlPointPosition = this.endPoints[i].position + (position2 - position) / 6f;
                this.endPoints[i].handleMode = BezierPoint.HandleMode.Mirrored;
                if (i < this.endPoints.Count - 1) {
                    this.endPoints[i + 1].precedingControlPointPosition = position2 - (position3 - this.endPoints[i].position) / 6f;
                } else if (this.loop) {
                    this.endPoints[0].precedingControlPointPosition = position2 - (position3 - this.endPoints[i].position) / 6f;
                }
            }
        }

        // Token: 0x060000F0 RID: 240 RVA: 0x0000640A File Offset: 0x0000460A
        private float AccuracyToStepSize(float accuracy) {
            if (accuracy <= 0f) {
                return 0.2f;
            }
            return Mathf.Clamp(1f / accuracy, 0.001f, 0.2f);
        }

        // Token: 0x04000069 RID: 105
        private static Material gizmoMaterial;

        // Token: 0x0400006A RID: 106
        public ISplineRenderer splineRenderer;

        // Token: 0x0400006B RID: 107
        private Color gizmoColor = Color.white;

        // Token: 0x0400006C RID: 108
        private float gizmoStep = 0.05f;

        // Token: 0x0400006D RID: 109
        private List<BezierPoint> endPoints = new List<BezierPoint>();

        // Token: 0x0400006E RID: 110
        private List<Vector3> _pointData = new List<Vector3>();

        // Token: 0x0400006F RID: 111
        public bool loop;

        // Token: 0x04000070 RID: 112
        public bool drawGizmos;

        // Token: 0x04000071 RID: 113
        private float _length = -999f;

        // Token: 0x04000072 RID: 114
        public bool isConstructLinear = true;

        // Token: 0x04000073 RID: 115
        [NonSerialized]
        public UnityEvent<BezierSpline> onSplineChanged = new UnityEvent<BezierSpline>();

        // Token: 0x04000074 RID: 116
        [NonSerialized]
        public bool isDirty;

        // Token: 0x04000075 RID: 117
        private bool _isRefreshed;

        // Token: 0x04000076 RID: 118
        private bool isCalculated;
    }
}

