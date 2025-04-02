using UnityEngine;

public class CardModel : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private CardFlipper flipper;
    public Sprite[] faces;
    public Sprite cardBack;
    public int cardIndex; // e.g. faces[cardIndex];

    public void ToggleFace(bool showFace)
    {
        if (showFace)
        {
            if (cardIndex > 0 && cardIndex < faces.Length)
            {
                flipper.FlipCard(faces[cardIndex - 1], faces[cardIndex], cardIndex);
            }
            else
            {
                Debug.LogError($"Invalid cardIndex: {cardIndex}, faces.Length: {faces.Length}");
            }
        }
        else
        {
            flipper.FlipCard(cardBack, faces[cardIndex], cardIndex);
        }
    }

    public void ToggleFaceNoAnimation(bool showFace)
    {
        if (showFace)
        {
            _renderer.sprite = faces[cardIndex];
        }
        else
        {
            _renderer.sprite = cardBack;
        }
    }

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        flipper = GetComponent<CardFlipper>();
    }
}
