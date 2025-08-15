using UnityEngine;

namespace PairCards {
  [System.Serializable]
  public class CardState {
    public static event System.Action<CardState> Selected;

    public Card card;
    public Coroutine revealCoroutine;

    public event System.Action<bool> IsMatchedChanged;
    public event System.Action<bool> IsFlippedChanged;

    public bool IsMatched {
      get => _isMatched;
      set {
        _isMatched = value;
        IsMatchedChanged?.Invoke(value);
      }
    }

    public bool IsFlipped {
      get => _isFlipped;
      set {
        _isFlipped = value;
        IsFlippedChanged?.Invoke(value);
      }
    }

    [SerializeField]
    bool _isMatched;
    bool _isFlipped;

    public void Select() => Selected?.Invoke(this);
  }
}
