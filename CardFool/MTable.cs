﻿using System.Diagnostics.CodeAnalysis;

namespace CardFool
{
    public struct SCard // карта
    {
        private Suits _suit; // масть, от 0 до 3
        private int _rank; // величина, от 6 до 14

        public SCard(Suits suit, int rank)
        {
            _suit = suit;
            _rank = rank;
        }

        public Suits Suit
        {
            get { return _suit; }
        }

        public int Rank
        {
            get { return _rank; }
        }
    }

    // Пара карт на столе
    public struct SCardPair
    {
        private SCard _down; // карта снизу
        private SCard _up; // карта сверху
        private bool _beaten; // признак бита карта или нет

        public SCard Down
        {
            get { return _down; }
            set
            {
                _down = value;
                _beaten = false;
            }
        }

        public bool Beaten
        {
            get { return _beaten; }
        }

        public SCard Up
        {
            get { return _up; }
        }

        public bool SetUp(SCard up, Suits trump)
        {
            if (_down.Suit == up.Suit)
            {
                if (_down.Rank < up.Rank)
                {
                    _up = up;
                    _beaten = true;
                    return true;
                }
            }
            else if (up.Suit == trump)
            {
                _up = up;
                _beaten = true;
                return true;
            }

            return false;
        }

        public SCardPair(SCard down)
        {
            _down = down;
            _up = new SCard(0, 0);
            _beaten = false;
        }
    }

    // Результат игры
    public enum EndGame
    {
        First,
        Second,
        Draw
    };

    public enum Suits
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }; // черви, бубны, крести, пики

    internal class MTable
    {
        // Количество карт на руке при раздаче
        public const int TotalCards = 6;

        public const string Separator = " | ";

        // Колода карт в прикупе
        private static List<SCard> deck = new List<SCard>();
        private static MPlayer1 player1; // игрок 1
        private static MPlayer2 player2; // игрок 2
        private static List<SCard> plHand1 = new List<SCard>();
        private static List<SCard> plHand2 = new List<SCard>();
        private static SCard trump; // козырь
        private static List<SCardPair> table; // карты на столе

        static void Main(string[] args)
        {
            EndGame res;

            Initialize();

            res = Play(true);
            Console.WriteLine("    =====================");
            Console.WriteLine("          Результат");
            Console.WriteLine("\u2665\uFE0F".PadLeft(10) + "\u2663\uFE0F".PadLeft(5)
                                                         + "\u2666\uFE0F".PadLeft(5) + "\u2660\uFE0F".PadLeft(5));
            if (res == EndGame.First)
            {
                Console.WriteLine("      Победитель: " + player1.GetName());
            }
            else if (res == EndGame.Second)
            {
                Console.WriteLine("      Победитель: " + player2.GetName());
            }
            else
            {
                Console.WriteLine("Игра завершилась вничью!");
            }

            Console.WriteLine("    =====================");
        }

        // Настройка игры
        private static void Initialize()
        {
            List<SCard> temp = new List<SCard>();
            Random rnd = new Random();

            // создаем полную колоду карт
            for (int c = 0; c <= 3; c++)
            for (int d = 6; d <= 14; d++)
            {
                SCard card = new SCard((Suits)c, d);
                temp.Add(card);
            }

            // формирование прикупа - перемешиваем карты
            for (int c = 0; c < 4 * 9; c++)
            {
                int num = rnd.Next(temp.Count);
                deck.Add(temp[num]);
                temp.RemoveAt(num);
            }

            // Создаем игроков
            player1 = new MPlayer1();
            player2 = new MPlayer2();

            // раздача карт первому и второму игроку
            for (int c = 0; c < TotalCards; c++)
            {
                player1.AddToHand(deck[0]);
                plHand1.Add(deck[0]);
                deck.RemoveAt(0);
                player2.AddToHand(deck[0]);
                plHand2.Add(deck[0]);
                deck.RemoveAt(0);
            }

            // формирование козыря
            trump = deck[deck.Count - 1];
            Console.Write("\n         Козырь ");
            ShowCard(trump);

            //***********************************
            Console.WriteLine();
            Console.Write(" ".PadLeft(9));
            player1.ShowHand();
            Console.Write(" ".PadLeft(9));
            player2.ShowHand();
            Console.WriteLine();
        }

        // Козырь
        public static SCard GetTrump()
        {
            return trump;
        }

        // Процесс игры
        private static EndGame Play(bool first)
        {
            bool playerFirst = true;
            bool defend, added = true;

            // процесс игры
            while (true)
            {
                List<SCard> cards;
                // Начало хода - стол пустой
                table = new List<SCardPair>();

                // игрок делает ход
                if (playerFirst)
                    cards = player1.LayCards();
                else
                    cards = player2.LayCards();
                // добавляем карты на стол
                while (cards.Count > 0)
                {
                    table.Add(new SCardPair(cards[0]));
                    cards.RemoveAt(0);
                }

                //************************
                if (playerFirst)
                {
                    Console.WriteLine(" Первым ходит " + player1.GetName());
                    ShowTable(table);
                }
                else
                {
                    Console.WriteLine(" Первым ходит " + player2.GetName());
                    ShowTable(table);
                }

                // ==== Начало хода ====
                // процесс защиты и подкидывания карт
                while (true)
                {
                    if (playerFirst)
                    {
                        // второй игрок отбивается
                        defend = player2.Defend(table);
                        //************************
                        if (defend)
                        {
                            Console.WriteLine(" Отбивается " + player2.GetName());
                            ShowTable(table);
                        }
                        else
                        {
                            Console.WriteLine(player2.GetName() + " не может отбиться");
                            Console.WriteLine();
                        }

                        // игрок подкидывает
                        added = player1.AddCards(table);
                        //************************
                        if (added)
                        {
                            Console.WriteLine(" Подкидывает " + player1.GetName());
                            ShowTable(table);
                        }
                        else
                        {
                            Console.WriteLine(" " + player1.GetName() + " не подкидывает");
                            Console.WriteLine("     *    *    *");
                        }

                        // если не отбился, то принимает
                        if (!defend)
                        {
                            while (table.Count > 0)
                            {
                                player2.AddToHand(table[0].Down);
                                plHand2.Add(table[0].Down);
                                if (table[0].Beaten)
                                {
                                    player2.AddToHand(table[0].Up);
                                    plHand2.Add(table[0].Up);
                                }

                                table.RemoveAt(0);
                            }

                            break; // окончание хода
                        }

                        // если не подкинули, то окончание хода
                        if (!added) break;
                    }
                    else
                    {
                        // первый игрок отбивается
                        defend = player1.Defend(table);
                        //************************
                        if (defend)
                        {
                            Console.WriteLine(" Отбивается " + player1.GetName());
                            ShowTable(table);
                        }
                        else
                        {
                            Console.WriteLine(player1.GetName() + " не может отбиться");
                            Console.WriteLine();
                        }

                        // игрок подкидывает
                        added = player2.AddCards(table);
                        //************************
                        if (added)
                        {
                            Console.WriteLine(" Подкидывает " + player2.GetName());
                            ShowTable(table);
                        }
                        else
                        {
                            Console.WriteLine(" " + player2.GetName() + " не подкидывает");
                            Console.WriteLine("     *    *    *");
                        }

                        // если не отбился, то принимает
                        if (!defend)
                        {
                            while (table.Count > 0)
                            {
                                player1.AddToHand(table[0].Down);
                                plHand1.Add(table[0].Down);
                                if (table[0].Beaten)
                                {
                                    player1.AddToHand(table[0].Up);
                                    plHand1.Add(table[0].Up);
                                }

                                table.RemoveAt(0);
                            }

                            break; // окончание хода
                        }

                        // если не подкинули, то окончание хода
                        if (!added) break;
                    }
                }
                // ==== Конец хода ====

                // Проверка корректности
                if (playerFirst)
                {
                    CheckHand(table, plHand1, true);
                    if (defend) CheckHand(table, plHand2, false);
                }
                else
                {
                    CheckHand(table, plHand2, true);
                    if (defend) CheckHand(table, plHand1, false);
                }

                // Добавляем игрокам карты из колоды
                AddCards(playerFirst);
                if (defend) playerFirst = !playerFirst;

                //***********************************
                Console.WriteLine();
                Console.Write(" ".PadLeft(9));
                player1.ShowHand();
                Console.Write(" ".PadLeft(9));
                player2.ShowHand();
                Console.WriteLine();

                // Если конец игры, то выходим
                if (player1.GetCount() == 0 && player2.GetCount() == 0) return EndGame.Draw;
                else if (player1.GetCount() == 0) return EndGame.First;
                else if (player2.GetCount() == 0) return EndGame.Second;
            }
        }

        // Добавляем карты из колоды первому и второму игроку
        private static void AddCards(bool first)
        {
            // добавляем карты из колоды
            if (first)
            {
                // добавляем первому игроку
                while (player1.GetCount() < TotalCards && deck.Count > 0)
                {
                    player1.AddToHand(deck[0]);
                    plHand1.Add(deck[0]);
                    deck.RemoveAt(0);
                }

                // добавляем второму игроку
                while (player2.GetCount() < TotalCards && deck.Count > 0)
                {
                    player2.AddToHand(deck[0]);
                    plHand2.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }
            else
            {
                // добавляем второму игроку
                while (player2.GetCount() < TotalCards && deck.Count > 0)
                {
                    player2.AddToHand(deck[0]);
                    plHand2.Add(deck[0]);
                    deck.RemoveAt(0);
                }

                // добавляем первому игроку
                while (player1.GetCount() < TotalCards && deck.Count > 0)
                {
                    player1.AddToHand(deck[0]);
                    plHand1.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }
        }

        private static void CheckHand(List<SCardPair> table, List<SCard> plHand, bool down)
        {
            if (down)
            {
                foreach (SCardPair cp in table)
                {
                    if (plHand.Contains(cp.Down))
                        plHand.Remove(cp.Down);
                    else
                        throw new Exception();
                }
            }
            else
            {
                foreach (SCardPair cp in table)
                {
                    if (cp.Beaten)
                    {
                        if (plHand.Contains(cp.Up))
                            plHand.Remove(cp.Up);
                        else
                            throw new Exception();
                    }
                    else
                        throw new Exception();
                }
            }
        }

        public static void ShowCard(SCard card)
        {
            string msg = "";
            if ((int)card.Suit < 2) Console.ForegroundColor = ConsoleColor.Red;
            if (card.Suit == GetTrump().Suit) Console.ForegroundColor = ConsoleColor.Yellow;
            switch (card.Suit)
            {
                case Suits.Hearts:
                    msg = "\u2665\uFE0F" + "ч";
                    break;
                case Suits.Diamonds:
                    msg = "\u2666\uFE0F" + "б";
                    break;
                case Suits.Clubs:
                    msg = "\u2663\uFE0F" + "к";
                    break;
                case Suits.Spades:
                    msg = "\u2660\uFE0F" + "п";
                    break;
            }

            switch (card.Rank)
            {
                case 6:
                    msg += "6";
                    break;
                case 7:
                    msg += "7";
                    break;
                case 8:
                    msg += "8";
                    break;
                case 9:
                    msg += "9";
                    break;
                case 10:
                    msg += "10";
                    break;
                case 11:
                    msg += "В";
                    break;
                case 12:
                    msg += "Д";
                    break;
                case 13:
                    msg += "К";
                    break;
                case 14:
                    msg += "Т";
                    break;
            }

            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ShowTable(List<SCardPair> table)
        {
            foreach (SCardPair pair in table)
            {
                if (pair.Beaten) ShowCard(pair.Up);
                else Console.Write("    ");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(pair.Up.Rank == 10 ? "| " : Separator);
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine();
            foreach (SCardPair pair in table)
            {
                ShowCard(pair.Down);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(pair.Down.Rank == 10 ? "| " : Separator);
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine();
        }
    }
}