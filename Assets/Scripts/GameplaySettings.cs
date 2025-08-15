using UnityEngine;

namespace PairCards {
  [System.Serializable]
  public struct GameplaySettings {
    public Vector2Int gridSize;
    public float memorizationTime;
    public float timeLimitPerPair;

    public static void Persist(GameplaySettings settings) {
      PlayerPrefs.SetString(nameof(GameplaySettings), JsonUtility.ToJson(settings));
    }

    public static bool TryRestore(out GameplaySettings settings) {
      if (PlayerPrefs.HasKey(nameof(GameplaySettings))) {
        settings = JsonUtility.FromJson<GameplaySettings>(
          PlayerPrefs.GetString(nameof(GameplaySettings))
        );
        return true;
      }

      settings = default;
      return false;
    }

    public static void Clear() {
      PlayerPrefs.DeleteKey(nameof(GameplaySettings));
    }
  }
}
