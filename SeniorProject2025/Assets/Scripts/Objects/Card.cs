[System.Serializable]
public struct Card
{
    public int value;     // 1 = A, 11 = J
    public string suit;   

    public Card(int value, string suit)
    {
        this.value = value;
        this.suit = suit;
    }

    public override string ToString()
    {
        string face = value switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => value.ToString()
        };
        return $"{face} of {suit}";
    }
}
