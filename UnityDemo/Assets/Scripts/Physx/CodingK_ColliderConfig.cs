using CodingKMath;

namespace CodingKPhysx
{
    public class CodingK_ColliderConfig
    {
        public string mName;
        public ColliderType mType;
        public CodingKVector3 mPos;
        
        // box 长宽高、轴向
        public CodingKVector3 mSize;
        public CodingKVector3[] mAxis;

        // Cylinder 半径
        public CodingKInt mRadius;
    }

    public enum ColliderType
    {
        Box,
        Cylinder,
    }
}