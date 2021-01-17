using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class MathRigging
    {
        public const float EPSILON = 0.0001f;

        public static Vector3 CalculBarycenter(List<Vector3> points)
        {
            Vector3 barycenter = Vector3.zero;

            foreach (Vector3 point in points)
            {
                barycenter.x += point.x;
                barycenter.y += point.y;
                barycenter.z += point.z;
            }

            barycenter /= points.Count;

            return barycenter;
        }

        public static float GetMoy(List<float> data)
        {
            return data.Sum() / data.Count;
        }

        public static float Variance(List<float> data, float moy = 0)
        {
            if (Mathf.Abs(moy) <= EPSILON)
            {
                moy = GetMoy(data);
            }

            float variance = 0.0f;
            foreach (float f in data)
            {
                variance += Mathf.Pow(f - moy, 2);
            }

            return variance / (data.Count - 1);
        }
        
        public static float Covariance(List<float> data1, List<float> data2)
        {
            if (data1.Count != data2.Count)
            {
                Debug.LogWarning("Covariance : Data 1 size != Data 2 size");
                return 0.0f;
            }

            float moyData1 = GetMoy(data1);
            float moyData2 = GetMoy(data2);

            float covariance = 0;
            for (int i = 0; i < data1.Count; ++i)
            {
                float a = data1[i] - moyData1;
                float b = data2[i] - moyData2;

                covariance += (a * b) / data1.Count;
            }

            return covariance;
        }
    }
}