namespace CardFool
{
    internal class MPlayer1
    {
        private string name = "Vasya";
        private int counter = -6;
        private List<SCard> hand = new List<SCard>();       // карты на руке

        // Возвращает имя игрока
        public string GetName()
        {
            return name;
        }

        // количество карт на руке
        public int GetCount()
        {
            return hand.Count;
        }
        // Добавляет новую карту в руку
        public void AddToHand(SCard card)
        {
            hand.Add(card);
            counter++;
        }

        // Сделать ход (первый)
        public List<SCard> LayCards()
        {
            SCard sCard = MinCurrentCard(hand);
            hand.Remove(sCard);
            return new List<SCard> { sCard };
        }

        // Отбиться.
        // На вход подается набор карт на столе, часть из них могут быть уже покрыты
        public bool Defend(List<SCardPair> table)
        {
            Suits trumpSuit = MTable.GetTrump().Suit;
            bool answer = false;
            SCardPair newPair;


            for (int i = 0; i < table.Count; i++)
            {
                SCardPair pair = table[i];
                if (pair.Beaten) continue;

                Suits suitDown = pair.Down.Suit;
                int rankDown = pair.Down.Rank;

                if (suitDown == trumpSuit && counter <= 10 && hand.Count() <= 7) return answer;

                List<SCard> validCards = new List<SCard>();
                foreach (SCard card in hand)
                {
                    if ((card.Suit == suitDown && card.Rank > rankDown)
                        || (card.Suit == trumpSuit && suitDown != trumpSuit)
                        || (card.Suit == trumpSuit && card.Rank > rankDown))
                    {
                        validCards.Add(card);
                        Console.WriteLine($"Кладем карту: {card.Suit} {card.Rank} в список выборки");
                    }
                }
                if (validCards.Count == 0) continue;
                Console.WriteLine($"В carts: {validCards.Count}");

                SCard myCardUp = MinCurrentCard(validCards);
                Console.WriteLine($"Минимальная выбранная карта: {myCardUp.Suit} {myCardUp.Rank}");

                if (myCardUp.Suit == trumpSuit && counter <= 5)
                {
                    Console.WriteLine($"Наша козырная и counter = <= 5. Пропускаем");
                    continue;
                }

                newPair = pair;
                if (newPair.SetUp(myCardUp, trumpSuit))
                {
                    answer = true;
                    table[i] = newPair;

                    hand.Remove(myCardUp);
                    Console.WriteLine($"Кладем карту: {myCardUp.Suit} {myCardUp.Rank}. Pair: {table[i].Up.Suit} {table[i].Up.Rank}");
                    return answer;
                }
            }
            Console.WriteLine($"Выходим");
            return answer;
        }

        // Подбросить карты
        // На вход подаются карты на столе
        public bool AddCards(List<SCardPair> table)
        {
            bool key = false;
            List<SCard> validCards = new List<SCard>();
            SCard selectedCard;

            if (counter >= 19) key = true;

            if (!key && counter <= 8)
            {
                Console.WriteLine($"False. Counter: {counter} <= 8");
                return false;
            }

            if (!key && (counter <= 20 || hand.Count <= 10))
            {
                foreach (SCard card in hand)
                {
                    if (card.Suit != MTable.GetTrump().Suit)
                    {
                        validCards.Add(card);
                    }
                }
                selectedCard = MinCurrentCard(validCards);
                table.Add(new SCardPair(selectedCard));
                hand.Remove(selectedCard);
                return true;
            }

            selectedCard = MinCurrentCard(hand);
            if (selectedCard.Rank != 0)
            {
                table.Add(new SCardPair(selectedCard));
                hand.Remove(selectedCard);
                return true;
            }
            
            return false;
        }

        // Вывести в консоль карты на руке
        public void ShowHand()
        {
            Console.WriteLine("Hand " + name);
            foreach (SCard card in hand)
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
                minRank = 15;
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
