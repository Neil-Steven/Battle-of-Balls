﻿using UnityEngine;
using GlobalSettings;

public class AI : IController
{
    enum Intention
    {
        Stay,
        Chase,
        Escape,
        FindFood
    }

    Ball owner;
    float senseDistance;
    float senseInterval;

    Intention intention = Intention.Stay;

    float timer = 0;

    SensibleBall nearestBigBall;
    SensibleBall nearestSmallBall;

    Ball lastChasingBall;
    Vector3 lastPosition;



    public AI(Ball owner, float senseDistance, float senseInterval)
    {
        this.owner = owner;

        if (senseDistance < EnemyData.MinSenseDistance || senseDistance > EnemyData.MaxSenseDistance)
            senseDistance = EnemyData.DefaultSenseDistance;
        if (senseInterval < EnemyData.MinSenseInterval || senseInterval > EnemyData.MaxSenseInterval)
            senseInterval = EnemyData.DefaultSenseInterval;

        this.senseDistance = senseDistance;
        this.senseInterval = senseInterval;

        nearestBigBall = new SensibleBall(0, Vector3.zero, float.MaxValue);
        nearestSmallBall = new SensibleBall(0, Vector3.zero, float.MaxValue);
    }


    public void Control()
    {
        timer += Time.deltaTime;
        if (timer > senseInterval)
        {
            Sense();
            Think();
            Action();
            timer = 0;
        }
    }


    void Sense()
    {
        // 1. 重置最近的大球和小球的数据
        nearestBigBall.Reset();
        nearestSmallBall.Reset();

        // 2. 遍历所有的ball
        for (int i = 0; i < GameManager.balls.Count; i++)
        {
            Ball ball = GameManager.balls[i];
            // 如果球已经死亡/两球大小相等(包含球是自身的情况)，则不对其判断
            if (!ball.isAlive || Mathf.Abs(ball.radius - owner.radius) < BallData.RadiusTolerance)
                continue;

            float distance = owner.AwayFrom(ball);
            // 如果进入了感知范围
            if (distance <= senseDistance)
            {
                // 如果自己比对方体积大，且和它的距离近于最近的小球
                if (owner.radius > ball.radius && distance < nearestSmallBall.distance)
                {
                    // 如果上次也是追的这个球，且自身位置并没有变化的话，则放弃该球
                    if (ReferenceEquals(ball, lastChasingBall) && lastPosition == owner.position)
                        continue;

                    nearestSmallBall.Set(ball.radius, ball.position, distance);
                    lastChasingBall = ball;
                    lastPosition = owner.position;
                }
                // 如果自己比对方体积小，且和它的距离近于最近的大球
                else if (owner.radius < ball.radius && distance < nearestBigBall.distance)
                {
                    nearestBigBall.Set(ball.radius, ball.position, distance);
                }
            }
        }
    }


    void Think()
    {
        // 如果范围内既有大球也有小球
        if (nearestBigBall.exists && nearestSmallBall.exists)
        {
            if (nearestSmallBall.distance < nearestBigBall.distance)
                intention = Intention.Chase;
            else
                intention = Intention.Escape;
        }

        // 如果范围内只有大球
        else if (nearestBigBall.exists)
            intention = Intention.Escape;

        // 如果范围内只有小球
        else if (nearestSmallBall.exists)
            intention = Intention.Chase;

        // 如果范围内没有球，且自身是空闲状态
        else if (!nearestBigBall.exists && !nearestSmallBall.exists)
        {
            // 随机选一个食物去吃(如果有食物的话)
            if (GameManager.foods.Count != 0 && intention == Intention.Stay)
                intention = Intention.FindFood;
        }
    }


    void Action()
    {
        switch (intention)
        {
            case Intention.Stay:
                owner.StopMoving();
                break;

            case Intention.Chase:
                owner.MoveTowards(nearestSmallBall.position - owner.position);
                break;

            case Intention.Escape:
                owner.MoveTowards(owner.position - nearestBigBall.position);
                break;

            case Intention.FindFood:    /// Attention index out of range
                int i = Random.Range(0, GameManager.foods.Count - 1);
                Vector3 foodPosition = GameManager.foods[i].position;
                owner.MoveTowards(foodPosition);
                break;
        }
    }
}
