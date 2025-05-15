using UnityEngine;
using System.Collections;

public class CardVisual : MonoBehaviour
{
    private Card hiddenCard;
    private Animator animator;

    public Transform parentAfterFlip;

    void Awake()
    {
        animator = GetComponent<Animator>();
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

        StartCoroutine(ReplaceWithFrontPrefab(1.2f));
    }

    private IEnumerator ReplaceWithFrontPrefab(float delay)
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

        string prefabName = $"Card_{face}_{hiddenCard.suit}";
        GameObject cardPrefab = Resources.Load<GameObject>("Cards/Sprites/" + prefabName);

        if (cardPrefab != null)
        {
            GameObject newCard = Instantiate(cardPrefab, transform.parent);
            newCard.transform.SetSiblingIndex(transform.GetSiblingIndex());
            newCard.transform.localScale = transform.localScale;
        }
        else
        {
            Debug.LogWarning("Missing card prefab: " + prefabName);
        }

        Destroy(gameObject);
    }
}
