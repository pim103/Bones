using UnityEngine;

namespace Model
{
    public class Point
    {
        public Vector3 position = Vector3.zero;

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
    }
}