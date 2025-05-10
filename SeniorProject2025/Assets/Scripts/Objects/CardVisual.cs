using UnityEngine;

public class CardVisual : MonoBehaviour
{
    private Card hiddenCard;

    public void SetHiddenCard(Card card)
    {
        hiddenCard = card;
    }

    public void FlipToFront(Card card)
    {
        Debug.Log("Flipping card to reveal: " + card.ToString());

        
    }
}
