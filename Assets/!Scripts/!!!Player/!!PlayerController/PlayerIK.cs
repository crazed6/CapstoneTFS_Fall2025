using UnityEngine;

public class PlayerIK : MonoBehaviour
{

    Animator anim;

    public LayerMask layerMask;

    [Range(0, 1f)]
    public float DistanceToGround;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

   
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

            //Left Foot
            RaycastHit hit;
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask))
            {
                if(hit.transform.tag == "Ground")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround; // Adjust the foot position to be above the ground
                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                }
            }


            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
        }
    }
}
