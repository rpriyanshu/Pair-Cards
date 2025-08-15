using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace PairCards.UI {
  public class MenuUI : VisualElement {
    public new class UxmlFactory : UxmlFactory<MenuUI> { }

    DropdownField Difficulty => this.Q<DropdownField>("difficulty");
    SliderInt Row => this.Q<SliderInt>("row");
    SliderInt Column => this.Q<SliderInt>("column");
    Slider MemorizationTime => this.Q<Slider>("memorization-time");
    Slider TimeLimitPerPair => this.Q<Slider>("time-limit-per-pair");
    Button StartGame => this.Q<Button>("start-game");
    Button Quit => this.Q<Button>("quit");

    GameplaySettings _settings;

    public MenuUI() {
      if (GameSession.TryRestore(out _)) {
        SceneManager.LoadScene("Main");
      } else {
        RegisterCallback<AttachToPanelEvent>(OnPanelAttached);
      }
    }

    void OnPanelAttached(AttachToPanelEvent _) {
      var presets = Resources.LoadAll<GameplaySettingsPreset>("Gameplay Settings Preset");

      if (!GameplaySettings.TryRestore(out _settings)) {
        _settings = presets[0].settings;
      }

      Difficulty.choices = presets.Select(p => p.displayName).Append("Custom").ToList();
      Difficulty.value = PlayerPrefs.GetString(nameof(GameplaySettingsPreset), presets[0].displayName);
      Difficulty.RegisterValueChangedCallback(e => {
        if (e.newValue != "Custom") {
          _settings = presets.First(p => p.displayName == e.newValue).settings;
          UpdateControlValues();
          UpdateControlLabels();
          GameplaySettings.Persist(_settings);
        }

        PlayerPrefs.SetString(nameof(GameplaySettingsPreset), e.newValue);
      });
      Row.RegisterValueChangedCallback(e => {
        _settings.gridSize.y = e.newValue;
        OnAnyValueChange();
      });
      Column.RegisterValueChangedCallback(e => {
        _settings.gridSize.x = e.newValue;
        OnAnyValueChange();
      });
      MemorizationTime.RegisterValueChangedCallback(e => {
        _settings.memorizationTime = e.newValue;
        OnAnyValueChange();
      });
      TimeLimitPerPair.RegisterValueChangedCallback(e => {
        _settings.timeLimitPerPair = e.newValue;
        OnAnyValueChange();
      });
      StartGame.clicked += () => SceneManager.LoadScene("Main");
      Quit.clicked += () => {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
      };

      UpdateControlValues();
      UpdateControlLabels();
    }

    void OnAnyValueChange() {
      UpdateControlLabels();
      Difficulty.value = "Custom";
      GameplaySettings.Persist(_settings);
    }

    void UpdateControlLabels() {
      Row.label = $"Row: {Row.value}";
      Column.label = $"Column: {Column.value}";
      MemorizationTime.label = $"Memorization Time: {MemorizationTime.value:F2}s";
      TimeLimitPerPair.label = $"Time Limit per Pair: {TimeLimitPerPair.value:F2}s";
    }

    void UpdateControlValues() {
      Row.SetValueWithoutNotify(_settings.gridSize.y);
      Column.SetValueWithoutNotify(_settings.gridSize.x);
      MemorizationTime.SetValueWithoutNotify(_settings.memorizationTime);
      TimeLimitPerPair.SetValueWithoutNotify(_settings.timeLimitPerPair);
    }
  }
}
