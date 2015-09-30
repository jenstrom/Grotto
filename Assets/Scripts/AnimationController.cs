using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour {

    public Animator AnimatorComponent { get; private set; }

    // Use this for initialization
    void Start () {
        AnimatorComponent = GetComponent<Animator>();
        AnimatorComponent.Play("HumanoidIdle");
    }

    // TESTING
    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            RunAnimation("walk");
        }
        if (Input.GetMouseButton(0))
        {
            RunAnimation("idle");
        }
    }
    // END TESTING

    public void RunAnimation(string animName) {

        switch (animName)
        {
            case "idle":
                AnimatorComponent.CrossFade("HumanoidIdle", 0.4f);
                break;

            case "walk":
                AnimatorComponent.CrossFade("GENWalk", 0.4f);
                break;

            case "sneak":
                AnimatorComponent.CrossFade("GenSneaking", 0.4f);
                break;

            case "fight":
                //if weapon == sword 
                AnimatorComponent.CrossFade("SwordSwingLowRight", 0.4f);
                //else
                // AnimatorComponent.CrossFade("NPCArmKickRight", 0.4f);
                break;

            case "die":
                AnimatorComponent.CrossFade("NPCDyingB", 0.4f);
                break;

            case "take":
                AnimatorComponent.CrossFade("NPCUseObject", 0.4f);
                break;

            case "eat":
                AnimatorComponent.CrossFade("NPCEating", 0.4f);
                break;

            case "drink":
                AnimatorComponent.CrossFade("NPCDrinking", 0.4f);
                break;

            default:
                AnimatorComponent.CrossFade("HumanoidIdle", 0.4f);
                break;
        }

    }

}
