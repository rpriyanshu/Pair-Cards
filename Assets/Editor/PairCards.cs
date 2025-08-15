using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PairCards.Editor {
  public static class PairCards {
    [MenuItem("Pair Cards/Log gameplay settings")]
    static void LogGameplaySettings() {
      Debug.Log(PlayerPrefs.HasKey(nameof(GameplaySettings)) ? PlayerPrefs.GetString(nameof(GameplaySettings)) : $"{nameof(GameplaySettings)} is not set!");
    }

    [MenuItem("Pair Cards/Clear gameplay settings")]
    static void ClearGameplaySettings() {
      GameplaySettings.Clear();
      Debug.Log($"{nameof(GameplaySettings)} cleared!");
    }

    [MenuItem("Pair Cards/Log game session")]
    static void LogGameSession() {
      Debug.Log(PlayerPrefs.HasKey(nameof(GameSession)) ? PlayerPrefs.GetString(nameof(GameSession)) : $"{nameof(GameSession)} is not set!");
    }

    [MenuItem("Pair Cards/Clear game session")]
    static void ClearGameSession() {
      GameSession.Clear();
      Debug.Log($"{nameof(GameSession)} cleared!");
    }

    [MenuItem("Pair Cards/Convert selected Sprites to Cards")]
    static void ConvertSelectedSpritesToCard() {
      var folder = EditorUtility.SaveFolderPanel("Choose folder to save the cards", "Assets", null);

      if (string.IsNullOrEmpty(folder)) return;

      var sprites = GetSpritesFromSelection().ToList();

      try {
        for (var i = 0; i < sprites.Count; i++) {
          var sprite = sprites[i];
          EditorUtility.DisplayProgressBar($"Converting... ({i + 1}/{sprites.Count})", sprite.name, (i + 1f) / sprites.Count);

          var card = ScriptableObject.CreateInstance<Card>();
          card.frontFace = sprite;

          var relativePath = Path.GetRelativePath(Application.dataPath, folder);
          AssetDatabase.CreateAsset(card, Path.Combine("Assets", relativePath, $"{sprite.name}.asset"));
        }
      } finally {
        EditorUtility.ClearProgressBar();
      }
    }

    [MenuItem("Pair Cards/Convert selected Sprites to Cards", true)]
    static bool ValidateConvertSelectedSpritesToCard() => GetSpritesFromSelection().Any();

    static IEnumerable<Sprite> GetSpritesFromSelection() => Selection.objects.OfType<Sprite>();
  }
}
