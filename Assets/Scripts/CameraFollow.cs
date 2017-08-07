﻿using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject target;      // 玩家角色位置
    float smoothing;        // 镜头移动速度
    Vector3 offset;         // 偏移量


    void Start()
    {
        target = GameObject.Find("Player");
        smoothing = 5;
        offset = transform.position - target.transform.position;  // 镜头到玩家角色之间的向量
    }


    void Update()
    {  
        if (target != null)
        {
            // 玩家新移动位置向量+之前两者之差的向量
            Vector3 targetCamPos = target.transform.position + offset;
            // 镜头从原位置向新位置平滑移动
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}
