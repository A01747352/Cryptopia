using UnityEngine;

public class ChangeSkin : MonoBehaviour
{
    public AnimatorOverrideController Girl2Idle;
    public AnimatorOverrideController Girl2Walk;
    public AnimatorOverrideController Girl3Idle;
    public AnimatorOverrideController Girl3Walk;
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    public void IdleGirl2()
    {
        GetComponent<Animator>().runtimeAnimatorController = Girl2Idle as RuntimeAnimatorController;
    }
public void IdleGirl3()
    {
        GetComponent<Animator>().runtimeAnimatorController = Girl3Idle as RuntimeAnimatorController;
    }
    public void WalkGirl2()
    {
        GetComponent<Animator>().runtimeAnimatorController = Girl2Walk as RuntimeAnimatorController;
    }
    public void WalkGirl3()
    {
        GetComponent<Animator>().runtimeAnimatorController = Girl3Walk as RuntimeAnimatorController;
    }
}



