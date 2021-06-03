using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField] private Animator animator;


    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        Invoke("Hide", 5f);
    }

    private void Hide()
    {
        animator.SetTrigger("Hide");
    }

    private void SelfKill()
    {
        gameObject.SetActive(false);
    }
}
 