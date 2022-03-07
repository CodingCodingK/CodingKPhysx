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
            throw new System.NotImplementedException();
        }
    }
}