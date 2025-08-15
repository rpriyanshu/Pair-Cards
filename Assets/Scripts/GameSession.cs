using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PairCards {
  [System.Serializable]
  public class GameSession {
    public GameplaySettings settings;
    public List<CardState> cardStates;

    public event System.Action<GameState> StateChanged;
    public event System.Action<int> MatchCountChanged;
    public event System.Action<int> FlipCountChanged;
    public event System.Action<float> RemainingTimeChanged;
    public event System.Action<int> ScoreChanged;
    public event System.Action<int> ComboChanged;

    public GameState State {
      get => _state;
      set {
        _state = value;
        StateChanged?.Invoke(value);
      }
    }

    public int MatchCount {
      get => _matchCount;
      set {
        _matchCount = value;
        MatchCountChanged?.Invoke(value);
      }
    }

    public int FlipCount {
      get => _flipCount;
      set {
        _flipCount = value;
        FlipCountChanged?.Invoke(value);
      }
    }

    public float RemainingTime {
      get => _remainingTime;
      set {
        _remainingTime = value;
        RemainingTimeChanged?.Invoke(value);
      }
    }

    public int Score {
      get => _score;
      set {
        _score = value;
        ScoreChanged?.Invoke(value);
      }
    }

    public int Combo {
      get => _combo;
      set {
        _combo = value;
        ComboChanged?.Invoke(value);
      }
    }

    [SerializeField]
    GameState _state;
    [SerializeField]
    int _matchCount;
    [SerializeField]
    int _flipCount;
    [SerializeField]
    float _remainingTime;
    [SerializeField]
    int _score;
    [SerializeField]
    int _combo;

    public GameSession(GameplaySettings settings, List<Card> cards) {
      var cardCount = settings.gridSize.x * settings.gridSize.y;
      var oddParity = cardCount % 2;
      cardCount -= oddParity;

      var chosenCards = new List<Card>(cards);

      for (var i = cardCount / 2; i < cards.Count; i++) {
        chosenCards.RemoveAtSwapBack(Random.Range(0, chosenCards.Count));
      }

      chosenCards.AddRange(chosenCards);

      for (var i = 0; i < cardCount; i++) {
        var j = Random.Range(0, cardCount);
        (chosenCards[i], chosenCards[j]) = (chosenCards[j], chosenCards[i]);
      }

      this.settings = settings;
      cardStates = new(cardCount);
      RemainingTime = settings.memorizationTime;

      for (var i = 0; i < cardCount; i++) {
        cardStates.Add(new() {
          card = chosenCards[i],
          IsFlipped = true,
        });
      }

      if (oddParity == 1) {
        cardStates.Insert(cardCount / 2, new() { IsMatched = true });
      }
    }

    public static void Persist(GameSession session) {
      PlayerPrefs.SetString(nameof(GameSession), JsonUtility.ToJson(session));
    }

    public static bool TryRestore(out GameSession session) {
      if (PlayerPrefs.HasKey(nameof(GameSession))) {
        session = JsonUtility.FromJson<GameSession>(
          PlayerPrefs.GetString(nameof(GameSession))
        );
        return true;
      }

      session = default;
      return false;
    }

    public static void Clear() {
      PlayerPrefs.DeleteKey(nameof(GameSession));
    }
  }
}
