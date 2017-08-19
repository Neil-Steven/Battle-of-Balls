using UnityEngine;

namespace GlobalSettings
{
    public struct PlaneData
    {
        public static float Width = 320.0f;
        public static float Height = 180.0f;
        public static Color Color = Color.grey;
    }


    public struct FoodData
    {
        // Side Length
        public static float MinSideLength = 1.0f;
        public static float DefaultSideLength = MaxSideLength / 2;
        public static float MaxSideLength = BallData.MinRadius;   //  最大的食物也要比所有球都要小

        // Produce
        public static float ProduceInterval = 5.0f;

        // Produce Quantity
        public static int MinProduceQuantity = 1;
        public static int DefaultProduceQuantity = 20;
        public static int MaxProduceQuantity = 50;
    }


    public struct BallData
    {
        // Accelerate
        public static float AccelerateTime = 1.0f;  // 固定加速时间

        // Radius
        public static float MinRadius = 3.0f;
        public static float DefaultRadius = MinRadius + MaxRadius / 2;
        public static float MaxRadius = 4.0f;
        public static float RadiusTolerance = 0.2f;   // 两球比较大小时的误差
    }


    public struct EnemyData
    {
        // Spawn
        public static float SpawnInterval = 5.0f;
        public static float MinSpawnSpace = 3.0f;
        public static int MaxRetrySpawnTime = 20;   // 最大重试(尝试生成Enemy)次数

        // Sense Distance
        public static float MinSenseDistance = 1.0f;
        public static float DefaultSenseDistance = MaxSenseDistance / 2;
        public static float MaxSenseDistance = 30.0f;

        // Sense Interval
        public static float MinSenseInterval = 1.0f;
        public static float DefaultSenseInterval = MaxSenseInterval / 2;
        public static float MaxSenseInterval = 5.0f;
    }
}
