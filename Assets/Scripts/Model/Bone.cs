using UnityEngine;

namespace Model
{
    
    public enum BonePosition
    {
        RIGHT,
        LEFT,
        CENTER
    }

    public enum BonePart
    {
        ARM,
        FOREARM,
        LEG,
        CALF,
        BODY,
        HEAD,
        FOOT,
        HAND
    }
    
    public class Bone
    {
        public Transform part;
        public GameObject physicalBone;

        public Point extrem1;
        public Point extrem2;

        public string name;

        public Bone(Point extrem1, Point extrem2, Transform part)
        {
            this.extrem1 = extrem1;
            this.extrem2 = extrem2;
            this.part = part;
            this.name = part.name;
            
            this.extrem1.bonesLinked.Add(this);
            this.extrem2.bonesLinked.Add(this);
            
            CreateBone();
        }

        public Bone(Point extrem1, Point extrem2, string name)
        {
            this.extrem1 = extrem1;
            this.extrem2 = extrem2;
            this.name = name;
            
            this.extrem1.bonesLinked.Add(this);
            this.extrem2.bonesLinked.Add(this);
            
            CreateBone();
        }

        public BonePosition GetBonePosition()
        {
            string nameLower = part.name.ToLower();

            return nameLower.Contains("left") ? BonePosition.LEFT : nameLower.Contains("right") ? BonePosition.RIGHT : BonePosition.CENTER;
        }

        public BonePart GetBonePart()
        {
            string nameLower = part.name.ToLower();

            return nameLower.Contains("leg") ? BonePart.LEG : nameLower.Contains("calf") ? BonePart.CALF : nameLower.Contains("body") ? BonePart.BODY : nameLower.Contains("head") ? BonePart.HEAD : nameLower.Contains("foot") ? BonePart.FOOT : nameLower.Contains("hand") ? BonePart.HAND : nameLower.Contains("forearm") ? BonePart.FOREARM : BonePart.ARM;
        }

        public void ReplaceBone(Vector3 p1, Vector3 p2)
        {
            if (p1 != extrem1.position && p1 != extrem2.position)
            {
                if (p2 == extrem1.position)
                {
//                    extrem2.position = p1;
                    extrem2.MovePoint(p1);
                }
                else if (p2 == extrem2.position)
                {
//                    extrem1.position = p1;
                    extrem1.MovePoint(p1);
                }
            }
            
            if (p2 != extrem1.position && p2 != extrem2.position)
            {
                if (p1 == extrem1.position)
                {
//                    extrem2.position = p2;
                    extrem2.MovePoint(p2);
                }
                else if (p1 == extrem2.position)
                {
//                    extrem1.position = p2;
                    extrem1.MovePoint(p2);
                }
            }
        }

        public void CreateBone()
        {
            if (physicalBone != null)
            {
                Controller.DeleteEdge(physicalBone);
            }

            physicalBone = Controller.DrawOneEdge(extrem1.position, extrem2.position);
            physicalBone.name = name;
        }

        public Bone JoinsBoneRelatedTo(Bone otherBone)
        {
            Point nearestOriginalPoint = extrem1;
            Point farestOriginalPoint = extrem2;

            Point nearestOtherBonePoint = otherBone.extrem1;
            Point farestOtherBonePoint = otherBone.extrem2;

            float minDistance = Vector3.Distance(extrem1.position, otherBone.extrem1.position);
            float testedValue;
            
            if ((testedValue = Vector3.Distance(extrem1.position, otherBone.extrem2.position)) < minDistance)
            {
                minDistance = testedValue;
                nearestOriginalPoint = extrem1;
                farestOriginalPoint = extrem2;

                nearestOtherBonePoint = otherBone.extrem2;
                farestOtherBonePoint = otherBone.extrem1;
            }

            if ((testedValue = Vector3.Distance(extrem2.position, otherBone.extrem1.position)) < minDistance)
            {
                minDistance = testedValue;
                nearestOriginalPoint = extrem2;
                farestOriginalPoint = extrem1;

                nearestOtherBonePoint = otherBone.extrem1;
                farestOtherBonePoint = otherBone.extrem2;
            }

            if (Vector3.Distance(extrem2.position, otherBone.extrem2.position) < minDistance)
            {
                nearestOriginalPoint = extrem2;
                farestOriginalPoint = extrem1;

                nearestOtherBonePoint = otherBone.extrem2;
                farestOtherBonePoint = otherBone.extrem1;
            }

            Vector3 position1Found = nearestOriginalPoint.position;
            Vector3 position2Found = nearestOtherBonePoint.position;
            
            if (Vector3.Dot(nearestOriginalPoint.position - farestOriginalPoint.position, nearestOtherBonePoint.position - nearestOriginalPoint.position) < 0)
            {
                Vector3 newBoneCenter = nearestOtherBonePoint.position - nearestOriginalPoint.position;
                float dist = newBoneCenter.magnitude / 2;

                position1Found = nearestOriginalPoint.position + (farestOriginalPoint.position - nearestOriginalPoint.position).normalized * dist;
                position2Found = nearestOtherBonePoint.position + (farestOtherBonePoint.position - nearestOtherBonePoint.position).normalized * dist;

                ReplaceBone(position1Found, farestOriginalPoint.position);
                otherBone.ReplaceBone(position2Found, farestOtherBonePoint.position);
            }

            return new Bone(nearestOriginalPoint, nearestOtherBonePoint, "Raccordement-" + part.name + "-" + otherBone.part.name);
        }
    }
}