using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace PairCards.UI {
  public class MainUI : VisualElement {
    public new class UxmlFactory : UxmlFactory<MainUI> { }

    Label GameState => this.Q<Label>("game-state");
    Label RemainingTime => this.Q<Label>("remaining-time");
    Label Combo => this.Q<Label>("combo");
    Label Score => this.Q<Label>("score");
    Label MatchCount => this.Q<Label>("match-count");
    Label FlipCount => this.Q<Label>("flip-count");
    VisualElement Body => this.Q("body");
    VisualElement GameCompleteDialog => this.Q("game-complete-dialog");
    VisualElement GameOverDialog => this.Q("game-over-dialog");

    Referee _referee;
    GameSession _session;

    public MainUI() {
      _referee = Object.FindAnyObjectByType<Referee>();

      if (_referee != null) {
        RegisterCallback<AttachToPanelEvent>(_ => {
          _referee.SessionChanged += OnSessionChanged;
          OnSessionChanged(_referee.Session);
        });
        RegisterCallback<DetachFromPanelEvent>(_ => {
          _referee.SessionChanged -= OnSessionChanged;
          OnSessionChanged(null);
        });
      }

      RegisterCallback<AttachToPanelEvent>(_ => {
        foreach (var button in this.Query<Button>("main-menu").Build()) {
          button.clicked += () => {
            GameSession.Clear();
            SceneManager.LoadScene("Menu");
          };
        }
      });
    }

    void OnSessionChanged(GameSession session) {
      if (_session != null) {
        _session.StateChanged -= OnGameStateChanged;
        _session.RemainingTimeChanged -= OnRemainingTimeChanged;
        _session.ComboChanged -= OnComboChanged;
        _session.ScoreChanged -= OnScoreChanged;
        _session.MatchCountChanged -= OnMatchCountChanged;
        _session.FlipCountChanged -= OnFlipCountChanged;
      }

      _session = session;

      if (_session != null) {
        _session.StateChanged += OnGameStateChanged;
        _session.RemainingTimeChanged += OnRemainingTimeChanged;
        _session.ComboChanged += OnComboChanged;
        _session.ScoreChanged += OnScoreChanged;
        _session.MatchCountChanged += OnMatchCountChanged;
        _session.FlipCountChanged += OnFlipCountChanged;

        OnGameStateChanged(_session.State);
        OnRemainingTimeChanged(_session.RemainingTime);
        OnComboChanged(_session.Combo);
        OnScoreChanged(_session.Score);
        OnMatchCountChanged(_session.MatchCount);
        OnFlipCountChanged(_session.FlipCount);

        foreach (var cardState in _session.cardStates) {
          Body.Add(new CardElement(cardState, _referee));
        }
      }
    }

    void OnGameStateChanged(GameState state) {
      GameState.text = state switch {
        PairCards.GameState.Memorization => "Get Ready",
        PairCards.GameState.Playing => "Playing",
        PairCards.GameState.Completed => "Game Finished",
        PairCards.GameState.Failed => "Game Over",
        _ => null
      };

      switch (state) {
        case PairCards.GameState.Completed:
          GameCompleteDialog.AddToClassList("opened");
          break;
        case PairCards.GameState.Failed:
          GameOverDialog.AddToClassList("opened");
          break;
      }
    }

    void OnRemainingTimeChanged(float time) => RemainingTime.text = float.IsFinite(time) && time > 0f ? $": {time:F1}s" : null;

    void OnComboChanged(int count) => Combo.text = count > 0 ? $"Combo: +{count}" : "No Combo";

    void OnScoreChanged(int score) => Score.text = score.ToString();

    void OnMatchCountChanged(int count) => MatchCount.text = count.ToString();

    void OnFlipCountChanged(int count) => FlipCount.text = count.ToString();
  }
}
