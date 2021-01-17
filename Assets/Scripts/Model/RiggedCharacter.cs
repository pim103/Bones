using System.Collections.Generic;

namespace Model
{
    public class RiggedCharacter
    {
        private List<Bone> bones = new List<Bone>();
        private List<Bone> relatedBones = new List<Bone>();

        public void AddBone(Bone bone)
        {
            bones.Add(bone);
        }

        private List<Bone> GetSpecificBonesFromPart(BonePart bonePart)
        {
            return bones.FindAll(bone => bone.GetBonePart() == bonePart);
        }

        public void JoinsRelatedBones()
        {
            List<Bone> bodys = GetSpecificBonesFromPart(BonePart.BODY);
            List<Bone> heads = GetSpecificBonesFromPart(BonePart.HEAD);

            Bone body;
            if (bodys.Count > 0)
            {
                body = bodys[0];
            }
            else
            {
                return;
            }

            Bone head = null;
            if (heads.Count > 0)
            {
                head = heads[0];
            }

            List<Bone> forearms = GetSpecificBonesFromPart(BonePart.FOREARM);
            List<Bone> arms = GetSpecificBonesFromPart(BonePart.ARM);
            List<Bone> legs = GetSpecificBonesFromPart(BonePart.LEG);
            List<Bone> calfs = GetSpecificBonesFromPart(BonePart.CALF);
            List<Bone> feet = GetSpecificBonesFromPart(BonePart.FOOT);
            List<Bone> hands = GetSpecificBonesFromPart(BonePart.HAND);

            if (forearms.Count > 0 && arms.Count > 0)
            {
                forearms.ForEach(forearm => relatedBones.Add(forearm.JoinsBoneRelatedTo(arms.Find(arm => arm.GetBonePosition() == forearm.GetBonePosition()))));
            }
            
            if (forearms.Count > 0 && hands.Count > 0)
            {
                forearms.ForEach(forearm => relatedBones.Add(forearm.JoinsBoneRelatedTo(hands.Find(hand => hand.GetBonePosition() == forearm.GetBonePosition()))));
            }

            if (arms.Count > 0 && body != null)
            {
                arms.ForEach(arm => relatedBones.Add(arm.JoinsBoneRelatedTo(body)));
            }

            if (head != null && body != null)
            {
                relatedBones.Add(head.JoinsBoneRelatedTo(body));
            }

            if (legs.Count > 0 && body != null)
            {
                legs.ForEach(leg => relatedBones.Add(leg.JoinsBoneRelatedTo(body)));
            }

            if (legs.Count > 0 && calfs.Count > 0)
            {
                calfs.ForEach(calf => relatedBones.Add(calf.JoinsBoneRelatedTo(legs.Find(leg => leg.GetBonePosition() == calf.GetBonePosition()))));
            }

            if (calfs.Count > 0 && feet.Count > 0)
            {
                calfs.ForEach(calf => relatedBones.Add(calf.JoinsBoneRelatedTo(feet.Find(foot => foot.GetBonePosition() == calf.GetBonePosition()))));
            }
        }
    }

}