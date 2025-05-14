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
        }

        // Сделать ход (первый)
        public List<SCard> LayCards()
        {
            List < SCard > cards = new List < SCard >();
            cards.Add(hand[0]);
            hand.Remove(hand[0]);
            return cards;
        }

        // Отбиться.
        // На вход подается набор карт на столе, часть из них могут быть уже покрыты
        public bool Defend(List<SCardPair> table)
        {
            Suits suit;
            int rank;
            bool answer = false;

            foreach (SCardPair pair in table)
            {
                if (pair.Beaten) continue;

                suit = pair.Down.Suit;
                rank = pair.Down.Rank;

                foreach (SCard card in hand)
                {
                    if ((card.Suit == suit && card.Rank > rank)
                        || (card.Suit == MTable.GetTrump().Suit && card.Rank > rank))
                    {
                        answer = pair.SetUp(pair.Up, MTable.GetTrump().Suit);
                        hand.Remove(card);
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
            if (hand.Count > 0) return true;
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
