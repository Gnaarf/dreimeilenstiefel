using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    Animator anim;
    int moveHash = Animator.StringToHash("Move");
    int idleHash = Animator.StringToHash("Idle");

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Play Walk Animation
        if (BoardManager.instance.IsPlayerMoving)
            anim.SetTrigger(moveHash);
        else
            anim.SetTrigger(idleHash);
    }
}
