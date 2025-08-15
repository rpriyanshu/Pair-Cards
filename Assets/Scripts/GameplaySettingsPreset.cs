using UnityEngine;

namespace PairCards {
  [CreateAssetMenu]
  public class GameplaySettingsPreset : ScriptableObject {
    public string displayName;
    public GameplaySettings settings;
  }
}
