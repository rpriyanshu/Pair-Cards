using UnityEngine;
using UnityEngine.UIElements;

namespace PairCards.UI {
  public class CardElement : VisualElement {
    VisualElement Container => this.Q("container");
    VisualElement FrontFace => this.Q("front-face");
    VisualElement BackFace => this.Q("back-face");

    CardState _state;

    public CardElement(CardState cardState, Referee referee) {
      _state = cardState;

      var template = Resources.Load<VisualTreeAsset>("UI/CardElement/CardElement");
      template.CloneTree(this);

      var gridSize = referee.Session.settings.gridSize;
      style.flexBasis = new Length(100f / gridSize.x, LengthUnit.Percent);
      style.height = new Length(100f / gridSize.y, LengthUnit.Percent);
      FrontFace.style.backgroundImage = new(cardState.card?.frontFace);
      BackFace.style.backgroundImage = new(cardState.card?.backFace);

      RegisterCallback<AttachToPanelEvent>(e => {
        referee.Session.StateChanged += OnGameStateChanged;
        cardState.IsMatchedChanged += OnIsMatchedChanged;
        cardState.IsFlippedChanged += OnIsFlippedChanged;
        OnGameStateChanged(referee.Session.State);
        OnIsMatchedChanged(cardState.IsMatched);
        OnIsFlippedChanged(cardState.IsFlipped);
      });
      RegisterCallback<DetachFromPanelEvent>(e => {
        referee.Session.StateChanged -= OnGameStateChanged;
        cardState.IsMatchedChanged -= OnIsMatchedChanged;
        cardState.IsFlippedChanged -= OnIsFlippedChanged;
      });
    }

    void OnGameStateChanged(GameState state) {
      switch (state) {
        case GameState.Playing:
          RegisterCallback<PointerDownEvent>(OnPointerDown);
          break;
        default:
          UnregisterCallback<PointerDownEvent>(OnPointerDown);
          break;
      }
    }

    void OnIsMatchedChanged(bool isMatched) {
      SetEnabled(!isMatched);
      Container.EnableInClassList("is-matched", isMatched);
    }

    void OnIsFlippedChanged(bool isFlipped) {
      Container.EnableInClassList("flipped", isFlipped);
    }

    void OnPointerDown(PointerDownEvent _) => _state.Select();
  }
}
