namespace CardFool
{
    internal class MPlayer1
    {
        private const string Name = "Vasya";
        private int _counter = -6;
        private readonly List<SCard> _hand = []; // карты на руке

        // Возвращает имя игрока
        public string GetName()
        {
            return Name;
        }

        // количество карт на руке
        public int GetCount()
        {
            return _hand.Count;
        }

        // Добавляет новую карту в руку
        public void AddToHand(SCard card)
        {
            _hand.Add(card);
            _counter++;
        }

        // Сделать ход (первый)
        public List<SCard> LayCards()
        {
            SCard sCard = MinCurrentCard(_hand);
            _hand.Remove(sCard);
            return [sCard];
        }

        // Отбиться.
        // На вход подается набор карт на столе, часть из них могут быть уже покрыты
        public bool Defend(List<SCardPair> table)
        {
            Suits trumpSuit = MTable.GetTrump().Suit;

            for (int i = 0; i < table.Count; i++)
            {
                SCardPair pair = table[i];
                if (pair.Beaten) continue;

                Suits suitDown = pair.Down.Suit;
                int rankDown = pair.Down.Rank;

                if (suitDown == trumpSuit && _counter <= 10 && _hand.Count() <= 7) return false;

                List<SCard> validCards = new List<SCard>();
                foreach (SCard card in _hand)
                {
                    if ((card.Suit == suitDown && card.Rank > rankDown)
                        || (card.Suit == trumpSuit && suitDown != trumpSuit)
                        || (card.Suit == trumpSuit && card.Rank > rankDown))
                    {
                        validCards.Add(card);
                    }
                }

                if (validCards.Count == 0) return false;

                SCard myCardUp = MinCurrentCard(validCards);

                if (myCardUp.Suit == trumpSuit && _counter <= 5) return false;

                SCardPair newPair = pair;
                if (newPair.SetUp(myCardUp, trumpSuit))
                {
                    table[i] = newPair;
                    _hand.Remove(myCardUp);
                }
            }

            return true;
        }

        // Подбросить карты
        // На вход подаются карты на столе
        public bool AddCards(List<SCardPair> table)
        {
            HashSet<int> possibleRangs = new HashSet<int>();
            List<SCard> validCards = new List<SCard>();
            SCard selectedCard = new SCard(0, 0);
            bool key = _counter >= 5 && _hand.Count <= 5 || _counter > 19;

            if (_hand.Count == 0 || _counter <= 4 && _hand.Count > 5) return false;

            foreach (SCardPair pair in table)
            {
                possibleRangs.Add(pair.Down.Rank);
                if (pair.Beaten) possibleRangs.Add(pair.Up.Rank);
            }

            if (_counter < 19 || _hand.Count <= 10)
            {
                foreach (SCard card in _hand)
                {
                    if (card.Suit != MTable.GetTrump().Suit && possibleRangs.Contains(card.Rank))
                        validCards.Add(card);
                }

                if (validCards.Count() == 0 && _hand.Count > 5) return false;
            }

            if (!key && selectedCard.Rank != 0)
            {
                selectedCard = MinCurrentCard(validCards);
                table.Add(new SCardPair(selectedCard));
                _hand.Remove(selectedCard);
                return true;
            }

            if (_counter < 5) return false;

            foreach (SCard card in _hand)
            {
                if (possibleRangs.Contains(card.Rank)) validCards.Add(card);
            }

            selectedCard = MinCurrentCard(validCards);
            if (selectedCard.Rank != 0)
            {
                table.Add(new SCardPair(selectedCard));
                _hand.Remove(selectedCard);
                return true;
            }

            return false;
        }

        // Вывести в консоль карты на руке
        public void ShowHand()
        {
            Console.WriteLine("Hand " + Name);
            foreach (SCard card in _hand)
            {
                MTable.ShowCard(card);
                Console.Write(MTable.Separator);
            }

            Console.WriteLine();
        }

        private SCard MinCurrentCard(List<SCard> cards)
        {
            List<SCard> selectedList = cards.ToList();
            int minRank = 15;
            SCard cardForPlay = new SCard(0, 0);

            foreach (SCard card in selectedList)
            {
                if (card.Suit == MTable.GetTrump().Suit) continue;

                if (card.Rank < minRank)
                {
                    minRank = card.Rank;
                    cardForPlay = card;
                }
            }

            if (cardForPlay.Rank == 0)
            {
                foreach (SCard card in selectedList)
                {
                    if (card.Suit != MTable.GetTrump().Suit) continue;

                    if (card.Rank < minRank)
                    {
                        minRank = card.Rank;
                        cardForPlay = card;
                    }
                }
            }

            return cardForPlay;
        }
    }
}