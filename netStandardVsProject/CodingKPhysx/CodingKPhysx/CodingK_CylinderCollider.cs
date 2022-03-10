using System.Collections.Generic;
using CodingKMath;

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
            
            var collisionInfoList = new List<CollisionInfo>();
            CodingKVector3 normal = CodingKVector3.zero;
            CodingKVector3 adj = CodingKVector3.zero;
            for (int i = 0; i < colliders.Count; i++)
            {
                if (DetectContact(colliders[i], ref normal, ref adj))
                {
                    var info = new CollisionInfo()
                    {
                        collider = colliders[i],
                        normal = normal,
                        borderAdjust = adj,
                    };
                    collisionInfoList.Add(info);

                    //Debug.Log("Contacted.");
                }
            }

            if (collisionInfoList.Count == 1)
            {
                // 单个碰撞体，修正速度
                CollisionInfo info = collisionInfoList[0];
                velocity = CorrectVelocity(velocity, info.normal);
                borderAdjust = info.borderAdjust;
                
            }
            else if (collisionInfoList.Count > 1)
            {
                // 求中间法线
                CodingKVector3 centerNormal = CodingKVector3.zero;
                CollisionInfo info = null;
                CodingKArgs borderNormalAngle = CalcMaxNormalAngle(collisionInfoList, velocity, ref centerNormal, ref info);
                // 比较2个夹角，从而确定是否能动
                CodingKArgs angle = CodingKVector3.Angle(-velocity, centerNormal);
                if (angle > borderNormalAngle)
                {
                    velocity = CorrectVelocity(velocity, info.normal);
                    
                    CodingKVector3 adjS = CodingKVector3.zero;
                    for (int i = 0; i < collisionInfoList.Count; i++)
                    {
                        adjS += collisionInfoList[i].borderAdjust;
                    }

                    borderAdjust = adjS;
                }
                else
                {
                    // 方向在夹角内，不能动
                    velocity = CodingKVector3.zero;
                }
            }
            else
            {
                //Debug.Log("no contact objs");
            }
            
        }

        /// <summary>
        /// 返回各方向反作用力与中间法线构成的角度中，最大的角度
        /// </summary>
        private CodingKArgs CalcMaxNormalAngle(List<CollisionInfo> infoList, CodingKVector3 velocity, ref CodingKVector3 centerNormal, ref CollisionInfo info)
        {
            // 计算出中间法线
            for (int i = 0; i < infoList.Count; i++)
            {
                centerNormal += infoList[i].normal;
            }
            centerNormal /= infoList.Count;
            
            CodingKArgs normalAngle = CodingKArgs.zero;
            CodingKArgs velocityAngle = CodingKArgs.zero;
            for (int i = 0; i < infoList.Count; i++)
            {
                // 求出各方向反作用力与中间法线构成的角度中，最大的角度
                CodingKArgs tmpNormalAngle = CodingKVector3.Angle(centerNormal, infoList[i].normal);
                if (normalAngle < tmpNormalAngle)
                {
                    normalAngle = tmpNormalAngle;
                }
                
                // 找出速度方向与法线方向夹角最大的碰撞法线，速度矫正由这个法线来决定
                CodingKArgs tmpVelAngle = CodingKVector3.Angle(velocity, infoList[i].normal);
                if (velocityAngle < tmpVelAngle)
                {
                    velocityAngle = tmpVelAngle;
                    info = infoList[i];
                }

            }

            return normalAngle;
        }

        // normal 法线单位向量
        private CodingKVector3 CorrectVelocity(CodingKVector3 velocity, CodingKVector3 normal)
        {
            if (normal == CodingKVector3.zero)
            {
                return velocity;
            }
            
            // 确保角色当前方向是往墙体里走，而不是远离墙体。如果方向是在远离墙体，不需要修正。
            // 与法线（修正方向）呈90度以上，就是在远离墙体。
            if (CodingKVector3.Angle(normal, velocity) > CodingKArgs.halfPi)
            {
                // 投影值应该为负
                CodingKInt prjLen = CodingKVector3.Dot(velocity, normal);
                if (prjLen != 0)
                {
                    velocity -= prjLen * normal;
                }
            }

            return velocity;
        }

        public override bool DetectSphereContact(CodingK_CylinderCollider col, ref CodingKVector3 normal, ref CodingKVector3 borderAdjust)
        {
            CodingKVector3 disOffset = mPos - col.mPos;
            if (disOffset.sqrMagnitude > (mRadius + col.mRadius) * (mRadius + col.mRadius))
            {
                return false;
            }
            else
            {
                normal = disOffset.normalized;
                borderAdjust = normal * (mRadius + col.mRadius - disOffset.magnitude);
                return true;
            }
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
                return true;
            }
        }
    }
}