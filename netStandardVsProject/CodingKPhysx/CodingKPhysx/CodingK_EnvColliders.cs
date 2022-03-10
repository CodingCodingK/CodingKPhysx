using System.Collections.Generic;

namespace CodingKPhysx
{
    public class CodingK_EnvColliders
    {
        public List<CodingK_ColliderConfig> envConfigList;
        private List<CodingK_ColliderBase> envColliderList;

        public void Init()
        {
            envColliderList = new List<CodingK_ColliderBase>();
            for (int i = 0; i < envConfigList.Count; i++)
            {
                var cfg = envConfigList[i];
                if (cfg.mType == ColliderType.Box)
                {
                    envColliderList.Add(new CodingK_BoxCollider(cfg));
                }
                else if (cfg.mType == ColliderType.Cylinder)
                {
                    envColliderList.Add(new CodingK_CylinderCollider(cfg));
                }
            }
        }

        public List<CodingK_ColliderBase> GetEnvColliders()
        {
            return envColliderList;
        }
    }
}