using UnityEngine;

public class LoopAnimation : MonoBehaviour
{
    public string animationName = "LOOP_IDOL_EVILHAND";

    void Start()
    {
        // Assicurati che l'animazione sia impostata in loop
        Animation anim = GetComponent<Animation>();
        anim[animationName].wrapMode = WrapMode.Loop;
        anim.Play(animationName);
    }
}
