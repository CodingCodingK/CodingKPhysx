using System;
using System.Collections;
using System.Collections.Generic;
using CodingKMath;
using CodingKPhysx;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    // UI输入
    private CodingKVector3 InputDir;
    
    public int logicSpeed;
    public float multiplier;

    // 逻辑玩家
    public CodingKVector3 logicPos;
    public CodingKVector3 logicDir;
    private CodingK_CylinderCollider playerCollider;
    // 逻辑环境
    private CodingK_EnvColliders logicEnv;
    
    // 视图
    public Transform player;
    public Transform transEnvRoot;
    
    private void Start()
    {
        InitEnv();
        InitPlayer();
        Debug.Log("player Init Succeeded!");
    }

    private void InitPlayer()
    {
        var cfg = new CodingK_ColliderConfig()
        {
            mName = "player",
            mPos = new CodingKVector3(player.position),
            mType = ColliderType.Cylinder,
            mRadius = (CodingKInt) (player.localScale.x / 2), // 测试
        };
        
        playerCollider = new CodingK_CylinderCollider(cfg);
        logicPos = cfg.mPos;
        logicDir = CodingKVector3.zero;
    }

    private void InitEnv()
    {
        var cfg = GenerateEnvColliderCfg();
        logicEnv = new CodingK_EnvColliders()
        {
            envConfigList = cfg,
        };
        
        logicEnv.Init();
    }
    
    private List<CodingK_ColliderConfig> GenerateEnvColliderCfg()
    {
        List<CodingK_ColliderConfig> envCfgList = new List<CodingK_ColliderConfig>();
        
        // 长方体
        BoxCollider[] boxArr = transEnvRoot.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < boxArr.Length; i++)
        {
            Transform trans = boxArr[i].transform;
            if (trans.gameObject.activeSelf == false)
            {
                continue;
            }

            var cfg = new CodingK_ColliderConfig()
            {
                mPos = new CodingKVector3(trans.position),
            };

            cfg.mName = trans.gameObject.name;
            cfg.mType = ColliderType.Box;
            cfg.mSize = new CodingKVector3(trans.localScale / 2);
            cfg.mAxis = new CodingKVector3[3];
            cfg.mAxis[0] = new CodingKVector3(trans.right);
            cfg.mAxis[1] = new CodingKVector3(trans.up);
            cfg.mAxis[2] = new CodingKVector3(trans.forward);
            
            envCfgList.Add(cfg);
        }
        
        // 胶囊体
        CapsuleCollider[] cylinderArr = transEnvRoot.GetComponentsInChildren<CapsuleCollider>();
        for (int i = 0; i < cylinderArr.Length; i++)
        {
            Transform trans = cylinderArr[i].transform;
            if (trans.gameObject.activeSelf == false)
            {
                continue;
            }

            var cfg = new CodingK_ColliderConfig()
            {
                mPos = new CodingKVector3(trans.position),
            };

            cfg.mName = trans.gameObject.name;
            cfg.mType = ColliderType.Cylinder;
            cfg.mRadius = (CodingKInt) trans.localScale.x / 2;

            envCfgList.Add(cfg);
        }

        return envCfgList;
    }
    
    private void UpdateInput()
    {
        // 实现：
        // CharacterController -> cc.SimpleMove(Vector3.back * speed * Time.deltaTime);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        InputDir = new CodingKVector3(new Vector3(h, 0, v).normalized);

    }

    private int index = 0;
    private void FixedUpdate()
    {
        UpdateInput();
        var moveDir = InputDir;
        playerCollider.mPos += moveDir * logicSpeed * (CodingKInt)multiplier;
        CodingKVector3 borderAdjust = CodingKVector3.zero;
        // 碰撞检测与矫正值获取
        playerCollider.CalcCollidersInteraction(logicEnv.GetEnvColliders(), ref moveDir, ref borderAdjust);
        // 矫正
        if (logicDir != moveDir)
        {
            logicDir = moveDir;
        }
        if (logicDir != CodingKVector3.zero)
        {
            logicPos = playerCollider.mPos + borderAdjust;
        }
        
        playerCollider.mPos = logicPos;
        player.position = logicPos.ConvertViewVector3();
    }

    
}
