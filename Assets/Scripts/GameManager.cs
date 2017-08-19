﻿using UnityEngine;
using System.Collections.Generic;
using GlobalSettings;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        Loading,
        Playing,
        GameOver
    }

    GameState gameState;

    public static List<Ball> balls { get; private set; }
    public static List<Food> foods { get; private set; }

    List<Ball> deadBallList;
    List<Food> deadFoodList;

    Plane plane;
    Ball player;

    float spawnEnemiesTimer = 0;
    float produceFoodsTimer = 0;


    void Start()
    {
        gameState = GameState.Loading;

        // 初始化每个队列
        balls = new List<Ball>();
        foods = new List<Food>();
        deadBallList = new List<Ball>();
        deadFoodList = new List<Food>();

        // 生成一个平面
        plane = new Plane(Color.gray, PlaneData.Width, PlaneData.Height);

        // 生成一个player对象并加入balls队列
        player = new Ball(Color.cyan, BallData.DefaultRadius, plane, Vector3.zero, true);
        balls.Add(player);

        // 动态挂载CameraFollow脚本
        GameObject mainCamera = GameObject.Find("Main Camera");
        mainCamera.AddComponent<CameraFollow>();
        Camera cameraComponent = mainCamera.GetComponent<Camera>();
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = BallData.DefaultRadius * 10;
        mainCamera.transform.localPosition = new Vector3(0, PlaneData.Height, 0);
        mainCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);

        // 开始游戏
        gameState = GameState.Playing;
    }

    void Update()
    {
        // 只有在Playing状态下才刷新界面
        if (gameState != GameState.Playing)
            return;

        if (!player.isAlive)
        {
            gameState = GameState.GameOver;
            return;
        }

        // 更新所有球的状态
        UpdateBalls();

        // 每隔一定时间随机生成一个enemy和一些food
        Vector3 planeScale = new Vector3(PlaneData.Width, 0, PlaneData.Height);
        SpawnRandomEnemies(EnemyData.SpawnInterval, planeScale);
        ProduceRandomFoods(FoodData.ProduceInterval, planeScale, FoodData.DefaultProduceQuantity);
    }


    void UpdateBalls()
    {
        // 1. 先移除临时队列中的所有元素
        deadBallList.Clear();
        deadFoodList.Clear();

        // 2. 刷新每个球
        for (int i = 0; i < balls.Count; i++)
        {
            Ball ball = balls[i];
            // 如果球已经死亡，则不再对其判断
            if (!ball.isAlive)
                continue;

            for (int j = i + 1; j < balls.Count; j++)
            {
                Ball otherBall = balls[j];
                // 如果球已经死亡，则不再对其判断
                if (!otherBall.isAlive)
                    continue;

                float radiusCompare = ball.radius - otherBall.radius;
                // 如果两个球大小相等，则不做判断
                if (System.Math.Abs(radiusCompare) < BallData.RadiusTolerance)
                    continue;

                Ball bigBall = (radiusCompare > 0) ? ball : otherBall;
                Ball smallBall = (bigBall == ball) ? otherBall : ball;

                float distance = Vector3.Distance(ball.position, otherBall.position);
                // 当小球的球心进入大球时，则将小球吃掉
                if (distance < bigBall.radius)
                {
                    bigBall.Eat(smallBall);
                    smallBall.BeEaten();
                    deadBallList.Add(smallBall);    // 先将被吃掉的球加入临时队列
                }
            }

            for(int k = 0; k < foods.Count; k++)
            {
                Food food = foods[k];
                if (!food.isAlive)
                    continue;

                float distance = Vector3.Distance(ball.position, food.position);
                if (distance < ball.radius)
                {
                    ball.Eat(food);
                    food.BeEaten();
                    deadFoodList.Add(food);    // 先将被吃掉的食物加入临时队列
                }
            }

            // 每次外循环最后，如果球还存活，则刷新该球
            if (ball.isAlive)
                ball.Update();
        }

        // 3. 移除所有被吃掉的球和食物，并将该对象置空
        for (int i = 0; i < deadBallList.Count; i++)
        {
            balls.Remove(deadBallList[i]);
            deadBallList[i] = null;
        }
        for (int i = 0; i < deadFoodList.Count; i++)
        {
            foods.Remove(deadFoodList[i]);
            deadFoodList[i] = null;
        }
    }


    /// <summary>
    /// 每隔一定时间在平面范围内随机生成一个Enemy对象。
    /// 如果在最大重试次数内未生成，则本次不生成。
    /// </summary>
    /// <param name="spawnInterval">生成间隔时间</param>
    /// <param name="planeScale">平面的大小</param>
    void SpawnRandomEnemies(float spawnInterval, Vector3 planeScale)
    {
        spawnEnemiesTimer += Time.deltaTime;
        if (spawnEnemiesTimer > spawnInterval)
        {
            // 随机取一个颜色
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // 随机生成大小
            float randomRadius = Random.Range(BallData.MinRadius, BallData.MaxRadius);

            // 随机生成地点
            Vector3 randomPosition;

            int i;
            int retryTime = 0;
            do {
                retryTime++;

                // 不能超过平面范围
                float randomPositionX = Random.Range(-planeScale.x / 2 + randomRadius, planeScale.x /2 - randomRadius);
                float randomPositionZ = Random.Range(-planeScale.z / 2 + randomRadius, planeScale.z /2 - randomRadius);
                randomPosition = new Vector3(randomPositionX, 0, randomPositionZ);

                // 与其他球保持一定距离
                for (i = 0; i < balls.Count; i++)
                {
                    Ball ball = balls[i];

                    float distance = Mathf.Abs(Vector3.Distance(ball.position, randomPosition));
                    // 如果生成的球与某个球之间的间距小于GameSettings中定义的最小间距，则不再遍历，并重新生成随机地址
                    if (distance - randomRadius - ball.radius < EnemyData.MinSpawnSpace)
                        break;
                }

            } while (i != balls.Count && retryTime <= EnemyData.MaxRetrySpawnTime);

            // 如果在最大重试次数内成功找到一个合法距离
            if (retryTime <= EnemyData.MaxRetrySpawnTime)
            {
                // 随机定义感知半径
                float randomSenseDistance = Random.Range(EnemyData.MinSenseDistance, EnemyData.MaxSenseDistance);

                // 随机定义感知间隔
                float randomSenseInterval = Random.Range(EnemyData.MinSenseInterval, EnemyData.MaxSenseInterval);

                // 生成一个随机的Enemy并将其加入balls队列中
                Ball newEnemy = new Ball(randomColor, randomRadius, plane, randomPosition, false, randomSenseDistance, randomSenseInterval);
                balls.Add(newEnemy);
            }

            // 将计时器归零
            spawnEnemiesTimer = 0;
        }
    }


    /// <summary>
    /// 每隔一定时间在一定范围内随机产生若干个食物。
    /// </summary>
    /// <param name="produceInterval">间隔</param>
    /// <param name="planeScale">生成范围</param>
    /// <param name="quantity">一次生成食物的数量，若取非法值则置为默认值</param>
    void ProduceRandomFoods(float produceInterval, Vector3 planeScale, float quantity)
    {
        produceFoodsTimer += Time.deltaTime;
        if (produceFoodsTimer > produceInterval)
        {
            if (quantity < FoodData.MinProduceQuantity || quantity > FoodData.MaxProduceQuantity)
                quantity = FoodData.DefaultProduceQuantity;

            for (int i = 0; i < quantity; i++)
            {
                // 随机取一个颜色
                Color randomColor = new Color(Random.value, Random.value, Random.value);

                // 随机生成大小
                float randomSideLength = Random.Range(FoodData.MinSideLength, FoodData.MaxSideLength);

                // 随机生成地点(不能超过平面范围)
                float randomPositionX = Random.Range(-planeScale.x / 2 + randomSideLength / 2, planeScale.x / 2 - randomSideLength / 2);
                float randomPositionZ = Random.Range(-planeScale.z / 2 + randomSideLength / 2, planeScale.z / 2 - randomSideLength / 2);
                Vector3 randomPosition = new Vector3(randomPositionX, 0, randomPositionZ);

                // 生成一个随机的Food并将其加入foods队列中
                Food newFood = new Food(randomColor, randomSideLength, randomPosition);
                foods.Add(newFood);
            }

            // 将计时器归零
            produceFoodsTimer = 0;
        }
    }
}
