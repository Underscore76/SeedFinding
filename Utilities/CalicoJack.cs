using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
    internal class CalicoJack { 

        private static int Next(List<double> results, ref int index, int min, int max)
        {
            return (int)((max - min) * NextDouble(results, ref index)) + min;
        }

        private static int Next(List<double> results, ref int index, int max)
        {
            return Next(results, ref index, 0, max);
        }

        private static double NextDouble(List<double> results, ref int index)
        {
            double result = results[index];
            index++;
            return result;
        }
        public static int CalculateHand(int timesPlayed, int daysPlayed, int seed)
        {
            List<int> playerCards = new();
            List<int> dealerCards = new();
            Random r = Utility.CreateRandom(timesPlayed, daysPlayed, seed);

            List<double> randomResults = new();
            for (int i = 0; i < 120; i++)
            {
                randomResults.Add(r.NextDouble());
            }
            int index = 0;

            dealerCards.Add(Next(randomResults, ref index, 1, 12));
            dealerCards.Add(Next(randomResults, ref index, 1, 10));

            playerCards.Add(Next(randomResults, ref index, 1, 12));
            playerCards.Add(Next(randomResults, ref index, 1, 10));

            if (WinOnStand(randomResults, index, playerCards, dealerCards))
                return 0;

            int hits = 0;
            while (playerCards.Sum() < 21)
            {
                hits++;

                SimulateHit(randomResults, ref index, playerCards);

                if (WinOnStand(randomResults, index, playerCards, dealerCards))
                {
                    if (playerCards.Sum() < 21)
                    {
                        List<int> twentyOneCheck = playerCards.ToList();
                        int twentyOneHits = hits;
                        while (twentyOneCheck.Sum() < 21)
                        {
                            twentyOneHits++;
                            SimulateHit(randomResults, ref index, twentyOneCheck);
                        }
                        if (twentyOneCheck.Sum() == 21)
                        {
                            hits = twentyOneHits;
                            playerCards.Clear();
                            playerCards.AddRange(twentyOneCheck);
                        }
                    }
                    return hits;
                }
            }

            return -1;
        }

        private static void SimulateHit(List<double> results, ref int index, List<int> playerCards)
        {
            int nextCard = Next(results, ref index, 1, 10);
            int distance = 21 - playerCards.Sum();

            if (distance > 1 && distance < 6 && NextDouble(results, ref index) < (double)(1f / (float)distance))
            {
                nextCard = ((NextDouble(results, ref index) < 0.5) ? distance : (distance - 1));
            }

            playerCards.Add(nextCard);
        }

        private static bool WinOnStand(List<double> results, int index, List<int> playerCards, List<int> dealerCards)
        {
            int playerTotal = playerCards.Sum();
            if (playerTotal == 21)
                return true;
            else if (playerTotal > 21)
                return false;

            int dealerTotal = dealerCards.Sum();

            List<int> newDealerCards = new List<int>();

            // Clone the random
            int indexCopy = index;

            while (dealerTotal < 18 || (dealerTotal < playerTotal && playerTotal <= 21))
            {
                int nextCard = Next(results, ref indexCopy, 1, 10);
                int dealerDistance = 21 - dealerTotal;

                if (playerTotal == 20 && NextDouble(results, ref indexCopy) < 0.5)
                {
                    nextCard = dealerDistance + Next(results, ref indexCopy, 1, 4);

                }
                else if (playerTotal == 19 && NextDouble(results, ref indexCopy) < 0.25)
                {
                    nextCard = dealerDistance + Next(results, ref indexCopy, 1, 4);
                }
                else if (playerTotal == 18 && NextDouble(results, ref indexCopy) < 0.1)
                {
                    nextCard = dealerDistance + Next(results, ref indexCopy, 1, 4);
                }

                newDealerCards.Add(nextCard);
                dealerTotal += nextCard;

                if (dealerTotal > 21)
                {
                    dealerCards.AddRange(newDealerCards);
                    return true;
                }

            }

            return false;
        }
    }
}
