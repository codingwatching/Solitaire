using Solitaire.Models;
using System.Collections.Generic;

namespace Solitaire.Services
{
    public class HintService
    {
        Game _game;

        public HintService(Game game)
        {
            _game = game;
        }

        public Pile FindValidMove(Card card)
        {
            if (card == null)
            {
                return null;
            }

            // Check foundations
            Pile pileTarget = CheckPilesForMove(_game.PileFoundations, card);

            // Check tableaus
            if (pileTarget == null)
            {
                pileTarget = CheckPilesForMove(_game.PileTableaus, card);
            }

            return pileTarget;
        }

        public Pile CheckPilesForMove(IList<Pile> piles, Card card)
        {
            // Return the first available pile the card can move to
            for (int i = 0; i < piles.Count; i++)
            {
                Pile pile = piles[i];

                if (pile.CanAddCard(card))
                {
                    return pile;
                }
            }

            return null;
        }

        public Hint GetHint()
        {
            Hint hint = null;

            // Check cards in tableau piles for move hint
            for (int i = 0; i < _game.PileTableaus.Count; i++)
            {
                Pile pileTableau = _game.PileTableaus[i];

                for (int j = 0; j < pileTableau.Cards.Count; j++)
                {
                    if (TryGenerateMoveHint(pileTableau.Cards[j], out hint))
                    {
                        return hint;
                    }
                }
            }

            // Check top card of the waste pile for move hint
            if (_game.PileWaste.HasCards && TryGenerateMoveHint(_game.PileWaste.TopCard(), out hint))
            {
                return hint;
            }

            // Draw hint
            if (_game.PileStock.HasCards && _game.PileStock.TopCard().IsDrawable)
            {
                hint = new Hint()
                {
                    Card = _game.PileStock.TopCard(),
                    Pile = _game.PileWaste,
                };
            }

            return hint;
        }

        private bool TryGenerateMoveHint(Card card, out Hint hint)
        {
            hint = null;

            if (card == null || !card.IsMoveable)
            {
                return false;
            }

            Pile pile = FindValidMove(card);

            if (pile == null)
            {
                return false;
            }

            hint = new Hint()
            {
                Card = card,
                Pile = pile,
            };

            return true;
        }
    }
}
