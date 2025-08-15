using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PairCards {
  public class Referee : MonoBehaviour {
    public event System.Action Selected;
    public event System.Action Matched;
    public event System.Action Mismatched;
    public event System.Action<GameSession> SessionChanged;

    public GameplaySettingsPreset settingsPreset;
    public List<Card> cards;

    public GameSession Session {
      get => _session;
      private set {
        _session = value;
        SessionChanged?.Invoke(value);
      }
    }

    GameSession _session;
    CardState _selectedCardState;

    void Start() {
      Matched += CheckGameCompletion;
      CardState.Selected += OnCardSelected;
      InitializeSession();
    }

    void OnDestroy() {
      CardState.Selected -= OnCardSelected;
    }

    void OnApplicationFocus(bool hasFocus) {
      if (!hasFocus && Session.State == GameState.Playing) {
        GameSession.Persist(Session);
      }
    }

    void OnApplicationPause(bool isPaused) {
      if (isPaused && Session.State == GameState.Playing) {
        GameSession.Persist(Session);
      }
    }

    void InitializeSession() {
      if (GameSession.TryRestore(out var session)) {
        Session = session;

        foreach (var cardState in Session.cardStates) {
          cardState.IsFlipped = cardState.IsMatched;
        }
      } else {
        if (!GameplaySettings.TryRestore(out var settings)) {
          settings = settingsPreset.settings;
        }

        Session = new(settings, cards);
      }

      Session.StateChanged += OnGameStateChanged;
      OnGameStateChanged(Session.State);
    }

    void CheckGameCompletion() {
      if (Session.cardStates.TrueForAll(s => s.IsMatched)) {
        Session.State = GameState.Completed;
      }
    }

    void OnGameStateChanged(GameState state) {
      switch (state) {
        case GameState.Memorization:
          StartCoroutine(MemorizationPhase());
          break;
        case GameState.Playing:
          StartCoroutine(PlayingPhase());
          break;
        case GameState.Completed:
        case GameState.Failed:
          GameSession.Clear();
          StopAllCoroutines();
          break;
      }
    }

    void OnCardSelected(CardState cardState) {
      if (cardState.revealCoroutine != null) {
        StopCoroutine(cardState.revealCoroutine);
        cardState.revealCoroutine = null;
      }

      if (_selectedCardState == null) {
        cardState.IsFlipped = true;
        _selectedCardState = cardState;
        Selected?.Invoke();
      } else if (_selectedCardState == cardState) {
        cardState.IsFlipped = false;
        _selectedCardState = null;
        Selected?.Invoke();
      } else if (_selectedCardState.card == cardState.card) {
        cardState.IsFlipped = true;
        _selectedCardState.IsMatched = cardState.IsMatched = true;
        _selectedCardState = null;
        Session.MatchCount++;
        Session.FlipCount++;
        Matched?.Invoke();
      } else {
        RevealMismatch(cardState);
        RevealMismatch(_selectedCardState);
        _selectedCardState = null;
        Session.FlipCount++;
        Mismatched?.Invoke();
      }
    }

    void RevealMismatch(CardState cardState) {
      cardState.revealCoroutine = StartCoroutine(RevealMemorizeHide(cardState));
    }

    IEnumerator RevealMemorizeHide(CardState cardState) {
      cardState.IsFlipped = true;
      yield return new WaitForSeconds(.25f);
      cardState.IsFlipped = false;
      cardState.revealCoroutine = null;
    }

    IEnumerator MemorizationPhase() {
      while (Session.RemainingTime > 0f) {
        yield return null;
        Session.RemainingTime -= Time.deltaTime;
      }

      foreach (var cardState in Session.cardStates) {
        cardState.IsFlipped = false;
      }

      var settings = Session.settings;
      var gridSize = settings.gridSize;
      Session.RemainingTime = settings.timeLimitPerPair * gridSize.x * gridSize.y / 2;
      Session.State = GameState.Playing;
    }

    IEnumerator PlayingPhase() {
      if (float.IsInfinity(Session.RemainingTime)) yield break;

      while (Session.RemainingTime > 0f) {
        yield return null;
        Session.RemainingTime -= Time.deltaTime;
      }

      Session.State = GameState.Failed;
    }
  }
}
