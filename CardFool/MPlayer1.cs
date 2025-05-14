using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardFool
{
    internal class MPlayer1
    {
        private string name = "Vasya";
        private int counter = 0;
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
            List<SCard> cards = [hand[0]];
            hand.Remove(hand[0]);
            return cards;
        }

        // Отбиться.
        // На вход подается набор карт на столе, часть из них могут быть уже покрыты
        public bool Defend(List<SCardPair> table)
        {
            Suits suit;
            int rank;
            SCard trump = MTable.GetTrump();
            bool answer = false;

            for (int i = 0; i < table.Count; i++)
            {
                SCardPair pair = table[i];
                if (pair.Beaten) continue;

                suit = pair.Down.Suit;
                rank = pair.Down.Rank;

                foreach (SCard card in hand)
                {
                    if ((card.Suit == suit && card.Rank > rank)
                        || (card.Suit == trump.Suit && card.Rank > rank))
                    {
                        answer = pair.SetUp(card, trump.Suit);
                        if (answer)
                        {
                            table[i] = pair;
                            hand.Remove(card);
                            return answer;
                        }
                    }
                }
            }
            return answer;
        }

        // Подбросить карты
        // На вход подаются карты на столе
        public bool AddCards(List<SCardPair> table)
        {
            if (counter <= 15 || hand.Count <= 10) return false;

            foreach (SCard card in hand) 
            {
                if (card.Suit != MTable.GetTrump().Suit && card.Rank < 9)
                {
                    table.Add(new SCardPair(card));
                    hand.Remove(card);
                    return true;
                }
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
    }
}
