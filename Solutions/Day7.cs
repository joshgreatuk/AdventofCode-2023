using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day7 : Solution
    {
        public Day7(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day7";

        public Dictionary<char, int> specialScores = new()
        {
            { 'T', 10 },
            { 'J', 11 },
            { 'Q', 12 },
            { 'K', 13 },
            { 'A', 14 }
        };

        public Dictionary<char, int> jokerScores = new()
        {
            { 'T', 10 },
            { 'J', 1 },
            { 'Q', 12 },
            { 'K', 13 },
            { 'A', 14 }
        };

        public class Hand
        {
            public HandType handType;
            public int[] cardScores;
            public int bet;

            public Hand(HandType handType, int[] scores, int bet)
            {
                this.handType = handType;
                this.cardScores = scores;
                this.bet = bet;
            }
        }

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, $"Preparing to G A M B L E");
            //Poker Rules, 5 <- 4 <- 2+3 <- 3 <- 2+2 <- 2 <- High
            string[] handStrings = problemContents.Split("\n");

            int[] normalRankedBets = RankBets(handStrings, specialScores);
            int[] jokerRankedBets = RankBets(handStrings, jokerScores, true);

            _logger.LogAsync(LogSeverity.Info, this, $"The real winner is the cards we dealt along the way :)");
            return new(normalRankedBets.Sum().ToString(), jokerRankedBets.Sum().ToString());
        }

        public HandType GetHandType(char[] cards, bool jokerRule)
        {
            Dictionary<char, int> cardCounts = new();
            int jokerCount = 0;
            for (int i=0; i < cards.Length; i++)
            {
                if (jokerRule && cards[i] == 'J')
                {
                    jokerCount++; 
                    continue;
                }

                if (cardCounts.ContainsKey(cards[i])) cardCounts[cards[i]]++;
                else cardCounts.Add(cards[i], 1);
            }

            if (jokerRule && jokerCount > 0)
            {
                for (int i=0; i < jokerCount; i++)
                {
                    if (cardCounts.Count < 1)
                    {
                        cardCounts.Add('J', jokerCount);
                        break;
                    }

                    cardCounts[cardCounts.Keys.Aggregate((x, n) => x = cardCounts[n] > cardCounts[x] ? n : x)] += 1;
                }
            }

            //Strip out card counts lower than 2
            cardCounts = cardCounts.Where(x => x.Value > 1).ToDictionary(x => x.Key, x => x.Value);

            if (cardCounts.Any(x => x.Value == 5)) return HandType.FiveKind;
            else if (cardCounts.Any(x => x.Value == 4)) return HandType.FourKind;
            else if (cardCounts.Any(x => x.Value == 3)) return cardCounts.Any(x => x.Value == 2) ? HandType.FullHouse : HandType.ThreeKind;
            int pairCount = cardCounts.Count(x => x.Value == 2);
            
            if (pairCount == 2) return HandType.TwoPair;
            else if (pairCount == 1) return HandType.OnePair;

            return HandType.HighCard;
        }

        public int[] RankBets(string[] handStrings, Dictionary<char, int> scores, bool jokerRule=false)
        {
            List<Hand> rankedHands = new();
            _logger.LogAsync(LogSeverity.Info, this, $"When the fun stops. Stop!");
            for (int i = 0; i < handStrings.Length; i++)
            {
                string[] splitHand = handStrings[i].Split(" "); //0 is cards, 1 is bet
                char[] cards = splitHand[0].ToCharArray();
                HandType handType = GetHandType(cards, jokerRule);

                Hand hand = new Hand(handType, cards.Select(x => char.IsDigit(x) ? int.Parse(x.ToString()) : scores[x]).ToArray(), int.Parse(splitHand[1]));

                for (int j = 0; j < rankedHands.Count + 1; j++)
                {
                    bool handHigher = rankedHands.Count <= j;
                    for (int k = 0; k < 5; k++)
                    {
                        if (handHigher) break;
                        if (rankedHands[j].handType < hand.handType)
                        {
                            handHigher = true;
                            break;
                        }
                        if (rankedHands[j].handType > hand.handType) continue;
                        if (rankedHands[j].cardScores[k] == hand.cardScores[k]) continue;
                        if (rankedHands[j].cardScores[k] < hand.cardScores[k])
                        {
                            handHigher = true;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (!handHigher) continue;

                    rankedHands.Insert(j, hand);
                    break;
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, $"Counting up the winnings");
            int[] totalRankedBets = new int[rankedHands.Count];
            for (int i = rankedHands.Count; i > 0; i--)
            {
                int index = rankedHands.Count - i;
                totalRankedBets[index] = rankedHands[index].bet * i;
            }

            return totalRankedBets;
        }

        public enum HandType
        {
            HighCard = 0,
            OnePair = 1,
            TwoPair = 2,
            ThreeKind = 3,
            FullHouse = 4,
            FourKind = 5,
            FiveKind = 6
        }
    }
}
