using UnityEngine;

namespace PairCards {
  public class ScoreSystem : MonoBehaviour {
    Referee _referee;

    void Awake() {
      _referee = FindAnyObjectByType<Referee>();
    }

    void OnEnable() {
      _referee.Matched += OnMatched;
      _referee.Mismatched += OnMismatched;
    }

    void OnDisable() {
      _referee.Matched -= OnMatched;
      _referee.Mismatched -= OnMismatched;
    }

    void OnMatched() {
      _referee.Session.Combo++;
      _referee.Session.Score += _referee.Session.Combo;
    }

    void OnMismatched() {
      _referee.Session.Combo = 0;
    }
  }
}
