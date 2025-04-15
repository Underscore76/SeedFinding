using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
    public class StepPredictions
    {
        private static List<string> List = new();

        public struct StepResult
        {
            public double DailyLuck;
            public int Dish;
            public int DishAmount;
            public string Gifter;
            public int HeartsRequired;
            public int Day;
            public int Steps;

            public StepResult(double luck, int dish, int amount, string gifter, int hearts, int day, int steps)
            {
                DailyLuck = luck;
                Dish = dish;
                DishAmount = amount;
                Gifter = gifter;
                HeartsRequired = hearts;
                Day = day;
                Steps = steps;
            }
            public override string ToString()
            {
                return string.Format("DailyLuck: {0}, Dish: {1} {2}, Gifter:{3}, Hearts:{4}", DailyLuck, Item.Get(Dish.ToString()).Name,DishAmount, Gifter, HeartsRequired);
            }
        }
        public static bool IsForbiddenDishOfTheDay(string id)
        {
            switch (id)
            {
                case "346":
                case "196":
                case "216":
                case "224":
                case "206":
                case "395":
                    return true;
                default:
                    return false;
            }
        }
		 
		public static StepResult Predict(long gameid, int day, int steps, List<string> friends, int numberMachinesProcessing = 0)
		{
			Random random;
			return Predict(gameid, day, steps, friends, out random, numberMachinesProcessing);
		}

		public static StepResult Predict(long gameid, int day, int steps, List<string> friends, out Random random, int numberMachinesProcessing = 0)
        {
            int calcDay = day + 1;
            Season season = Utility.getSeasonFromDay(calcDay);

            random = Utility.CreateRandom( gameid / 100, (calcDay * 10) + 1, steps );

            for (int k = 0; k < Utility.getDayOfMonthFromDay(calcDay); k++)
            {
                random.Next();
            }
            int itemId;
            do
            {
                itemId = random.Next(194, 240);
            }
            while (IsForbiddenDishOfTheDay(itemId.ToString()));
            int count = random.Next(1, 4 + ((random.NextDouble() < 0.08) ? 10 : 0));

            // Object constructor
            random.NextDouble();

            // Processing machines
            for (int index = 0; index < numberMachinesProcessing; index++)
                random.Next();

            // Friend
            string friend = "";
            int amountRequired = 0;
            if (friends != null && friends.Count > 0)
            {
                friend = friends[random.Next(friends.Count)];
                amountRequired = random.Next(10) + 1;
            }

            // Rarecrow
            random.Next();

            // Mannequin

            // DailyLuck
            double dailyLuck = Math.Min(0.10000000149011612, (double)random.Next(-100, 101) / 1000.0);

            return new StepResult(dailyLuck, itemId, count, friend, amountRequired,day,steps);
        }
    }
}
