using CodingKMath;

namespace CodingKPhysx
{
    public abstract class CodingK_ColliderBase
    {
        public string name;
        public CodingKVector3 mPos;

        /// <summary>
        /// 碰撞检测
        /// </summary>
        public virtual bool DetectContact(CodingK_ColliderBase collider, ref CodingKVector3 normal,
            ref CodingKVector3 borderAdjust)
        {
            if (collider is CodingK_BoxCollider box)
            {
                return DetectBoxContact(box, ref normal, ref borderAdjust);
            }
            else if (collider is CodingK_CylinderCollider cylinder)
            {
                return DetectSphereContact(cylinder, ref normal, ref borderAdjust);
            }
            else
            {
                // TODO add
                return false;
            }
        }

        /// <summary>
        /// 球体碰撞检测
        /// </summary>
        public abstract bool DetectSphereContact(CodingK_CylinderCollider col, ref CodingKVector3 normal,
            ref CodingKVector3 borderAdjust);
        
        /// <summary>
        /// 长方体碰撞检测
        /// </summary>
        public abstract bool DetectBoxContact(CodingK_BoxCollider col, ref CodingKVector3 normal,
            ref CodingKVector3 borderAdjust);
    }

    public class CollisionInfo
    {
        public CodingK_ColliderBase collider;
        public CodingKVector3 normal;
        public CodingKVector3 borderAdjust;
        
    }
}