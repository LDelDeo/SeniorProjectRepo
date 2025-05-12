using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardVisual : MonoBehaviour
{
    private Card hiddenCard;
    private Animator animator;
    private Image image;

    void Awake()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
    }

    public void SetHiddenCard(Card card)
    {
        hiddenCard = card;
    }

    public void FlipToFront(Card card)
    {
        hiddenCard = card;

        if (animator != null)
        {
            animator.SetTrigger("Flip");
        }

        StartCoroutine(DelayedReveal(0.25f));
    }

    private IEnumerator DelayedReveal(float delay)
    {
        yield return new WaitForSeconds(delay);

        string face = hiddenCard.value switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => hiddenCard.value.ToString()
        };

        string spriteName = $"Card_{face}_{hiddenCard.suit}";
        Sprite cardSprite = Resources.Load<Sprite>("CardSprites/" + spriteName);

        if (cardSprite != null)
        {
            image.sprite = cardSprite;
        }
        else
        {
            Debug.LogWarning("Missing sprite: " + spriteName);
        }
    }
}

