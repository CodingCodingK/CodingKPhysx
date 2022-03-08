using System.Collections.Generic;
using CodingKMath;
using UnityEngine;

namespace CodingKPhysx
{
    /// <summary>
    /// 圆柱体
    /// </summary>
    public class CodingK_CylinderCollider : CodingK_ColliderBase
    {
        public CodingKInt mRadius;

        public CodingK_CylinderCollider(CodingK_ColliderConfig cfg)
        {
            mPos = cfg.mPos;
            mRadius = cfg.mRadius;
            name = cfg.mName;
        }

        /// <summary>
        /// 检测是否碰撞
        /// </summary>
        public void CalcCollidersInteraction(List<CodingK_ColliderBase> colliders,ref CodingKVector3 velocity, ref CodingKVector3 borderAdjust)
        {
            if (velocity == CodingKVector3.zero)
            {
                return;
            }

            CodingKVector3 normal = CodingKVector3.zero;
            CodingKVector3 adj = CodingKVector3.zero;
            if (DetectContact(colliders[0], ref normal, ref adj))
            {
                Debug.Log("Contacted.");
            }
        }

        public override bool DetectSphereContact(CodingK_CylinderCollider col, ref CodingKVector3 normal, ref CodingKVector3 borderAdjust)
        {
            throw new System.NotImplementedException();
        }

        public override bool DetectBoxContact(CodingK_BoxCollider col, ref CodingKVector3 normal, ref CodingKVector3 borderAdjust)
        {
            CodingKVector3 disOffset = mPos - col.mPos;
            CodingKInt dot_disX = CodingKVector3.Dot(disOffset, col.mDir[0]);
            CodingKInt dot_disZ = CodingKVector3.Dot(disOffset, col.mDir[2]);
            
            // 投影钳制在x轴向量
            CodingKInt clamp_x = CodingKCalc.Clamp(dot_disX, -col.mSize.x, col.mSize.x);
            CodingKInt clamp_z = CodingKCalc.Clamp(dot_disZ, -col.mSize.z, col.mSize.z);
            // 计算轴向上的投影向量
            CodingKVector3 s_x = clamp_x * col.mDir[0];
            CodingKVector3 s_z = clamp_z * col.mDir[2];
            // 计算表面最近的接触点：碰撞体中心位置 + 轴向偏移
            CodingKVector3 point = col.mPos;
            point += s_x;
            point += s_z;

            CodingKVector3 po = mPos - point;
            po.y = 0;
            if (po.sqrMagnitude > mRadius * mRadius)
            {
                return false;
            }
            else
            {
                normal = po.normalized; // 法线向量
                CodingKInt len = po.magnitude;
                borderAdjust = normal * (mRadius - len);
            }
        }
    }
}