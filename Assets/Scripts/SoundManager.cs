using UnityEngine;

namespace PairCards {
  [RequireComponent(typeof(AudioSource))]
  public class SoundManager : MonoBehaviour {
    public AudioClip Select;
    public AudioClip Match;
    public AudioClip Mismatch;
    public AudioClip GameComplete;
    public AudioClip GameOver;
    public AudioSource BackgroundMusic;

    AudioSource _audioSource;
    Referee _referee;
    GameSession _session;

    void Awake() {
      _audioSource = GetComponent<AudioSource>();
      _referee = FindAnyObjectByType<Referee>();
    }

    void OnEnable() {
      _referee.Selected += OnSelected;
      _referee.Matched += OnMatched;
      _referee.Mismatched += OnMismatched;
      _referee.SessionChanged += OnSessionChanged;
      OnSessionChanged(_referee.Session);
    }

    void OnDisable() {
      _referee.Selected -= OnSelected;
      _referee.Matched -= OnMatched;
      _referee.Mismatched -= OnMismatched;
      _referee.SessionChanged -= OnSessionChanged;
    }

    void OnSelected() => _audioSource.PlayOneShot(Select);

    void OnMatched() => _audioSource.PlayOneShot(Match);

    void OnMismatched() => _audioSource.PlayOneShot(Mismatch);

    void OnSessionChanged(GameSession session) {
      if (_session != null) {
        _session.StateChanged -= OnGameStateChanged;
      }

      _session = session;

      if (_session != null) {
        _session.StateChanged += OnGameStateChanged;
      }
    }

    void OnGameStateChanged(GameState state) {
      switch (state) {
        case GameState.Completed:
          _audioSource.PlayOneShot(GameComplete);
          BackgroundMusic.Stop();
          break;
        case GameState.Failed:
          _audioSource.PlayOneShot(GameOver);
          BackgroundMusic.Stop();
          break;
      }
    }
  }
}
