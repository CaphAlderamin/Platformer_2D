using UnityEngine;

public class ChestTrigger : MonoBehaviour
{
    public float storedHealth;
    public int storedCoins;
    private HeroKnight player;
    private Animator animator;

    private void Start()
    {
        player = PlayerManager.Instance.Player.GetComponent<HeroKnight>();
        animator = GetComponent<Animator>(); 
    }

    public void OpenChest()
    {
        animator.SetBool("chestOpen", true);

        player.GetHeal(storedHealth);
        player.AddCoins(storedCoins);

        //animator.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
    }
}
