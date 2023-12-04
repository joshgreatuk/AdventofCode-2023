using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day4 : Solution
    {
        public Day4(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day4";

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, $"Scratching cards");
            string[] cards = problemContents.Split('\n');
            

            _logger.LogAsync(LogSeverity.Info, this, $"Crunching the numbers");
            int cardsTotal = 0;
            int cardCount = 0;
            Dictionary<int, int> multipliers = new();
            for (int i=0; i < cards.Length; i++)
            {
                string card = cards[i];
                int winCount = 0;
                string actualCard = card.Split(":")[1].Trim();
                string[] cardHalves = actualCard.Split("|");

                int cardMultiplier = multipliers.ContainsKey(i) ? multipliers[i] : 1;

                int[] winners = ParseNumbers(cardHalves[0]);
                int[] numbers = ParseNumbers(cardHalves[1]);

                for (int j=0; j < numbers.Length; j++)
                {
                    if (winners.Contains(numbers[j])) winCount += 1;
                }

                cardsTotal += 2 ^ winCount;
                cardCount += 1 * cardMultiplier;

                for (int j=0; j < winCount; j++)
                {
                    int index = i + j + 1;
                    if (multipliers.TryGetValue(index, out int existing))
                    {
                        existing += 1 * cardMultiplier;
                        multipliers[index] = existing;
                        continue;
                    }

                    multipliers.Add(index, 1+(1*cardMultiplier));
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, $"When the fun stops, stop!");
            return new(cardsTotal.ToString(), cardCount.ToString());
        }

        public int[] ParseNumbers(string input) => input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(x => int.Parse(x.Trim())).ToArray();
    }
}
