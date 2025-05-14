using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardFool
{
    internal class MPlayer2
    {
        private string name = "Masha";
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
            List<SCard> cards = [hand[0]];
            hand.Remove(hand[0]);
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

                foreach (SCard card in hand)
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
            return answer;
        }

        // Подбросить карты
        // На вход подаются карты на столе
        public bool AddCards(List<SCardPair> table)
        {
            if (hand.Count > 0 && table.Count < 7)
            {
                table.Add(new SCardPair(hand[0]));
                hand.Remove(hand[0]);
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
    }
}
