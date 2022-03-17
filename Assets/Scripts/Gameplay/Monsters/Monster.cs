using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterInfo info;
    private TopDownController topDownController;
    private  Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        topDownController = GetComponent<TopDownController>();

        animator.runtimeAnimatorController = info.animatorController;
        topDownController.setMovement(GetComponent<Rigidbody2D>().position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
