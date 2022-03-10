using CodingKMath;

namespace CodingKPhysx
{
    /// <summary>
    /// 长方体
    /// </summary>
    public class CodingK_BoxCollider : CodingK_ColliderBase
    {
        // box 长宽高、轴向
        public CodingKVector3 mSize;
        public CodingKVector3[] mDir;

        public CodingK_BoxCollider(CodingK_ColliderConfig cfg)
        {
            name = cfg.mName;
            mPos = cfg.mPos;
            mSize = cfg.mSize;

            mDir = new CodingKVector3[3];
            mDir[0] = cfg.mAxis[0];
            mDir[1] = cfg.mAxis[1];
            mDir[2] = cfg.mAxis[2];
        }

        public override bool DetectSphereContact(CodingK_CylinderCollider col, ref CodingKVector3 normal, ref CodingKVector3 borderAdjust)
        {
            // TODO MobaDemo用不到，没有测
            // 直接用Cylinder撞向box的反向结果
            CodingKVector3 tmpNormal = CodingKVector3.zero;
            CodingKVector3 tmpAdjust = CodingKVector3.zero; 
            var result = col.DetectBoxContact(this, ref tmpNormal, ref tmpAdjust);
            normal = -tmpNormal;
            tmpAdjust = -tmpAdjust;
            return result;
        }

        public override bool DetectBoxContact(CodingK_BoxCollider col, ref CodingKVector3 normal, ref CodingKVector3 borderAdjust)
        {
            // TODO MobaDemo用不到，没有做，可以用分离轴算法实现
            return false;
        }
    }
}