namespace CardFool
{
    internal class MPlayer2
    {
        private const string Name = "Masha";
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
        }

        // Сделать ход (первый)
        public List<SCard> LayCards()
        {
            List<SCard> cards = [_hand[0]];
            _hand.Remove(_hand[0]);
            return cards;
        }

        // Отбиться.
        // На вход подается набор карт на столе, часть из них могут быть уже покрыты
        public bool Defend(List<SCardPair> table)
        {
            SCard trump = MTable.GetTrump();
            bool answer = false;

            for (int i = 0; i < table.Count; i++)
            {
                SCardPair pair = table[i];
                if (pair.Beaten) continue;

                foreach (SCard card in _hand)
                {
                    answer = pair.SetUp(card, trump.Suit);
                    if (answer)
                    {
                        table[i] = pair;
                        _hand.Remove(card);
                        return answer;
                    }
                }
            }

            return answer;
        }

        // Подбросить карты
        // На вход подаются карты на столе
        public bool AddCards(List<SCardPair> table)
        {
            HashSet<int> possibleRangs = [];

            if (_hand.Count == 0 || (table.Count >= 8 && _hand.Count >= 5))
                return false;

            foreach (SCardPair pair in table)
            {
                possibleRangs.Add(pair.Down.Rank);
                if (pair.Beaten) possibleRangs.Add(pair.Up.Rank);
            }

            foreach (SCard card in _hand)
            {
                if (possibleRangs.Contains(card.Rank))
                {
                    table.Add(new SCardPair(card));
                    _hand.Remove(card);
                    return true;
                }
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
    }
}