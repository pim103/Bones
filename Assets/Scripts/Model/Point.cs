using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Model
{
    public class Point
    {
        public Vector3 position = Vector3.zero;

        public List<Bone> bonesLinked = new List<Bone>();

        public float x
        {
            set => position.x = value;
            get => position.x;
        }
        
        public float y
        {
            set => position.y = value;
            get => position.y;
        }
        
        public float z
        {
            set => position.z = value;
            get => position.z;
        }

        public void MovePoint(Vector3 newPos)
        {
            position = newPos;

            bonesLinked.ForEach(boneLinked =>
            {
                boneLinked.CreateBone();
            });
        }
    }
}