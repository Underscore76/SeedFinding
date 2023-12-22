using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
    public class Weather
    {
        public enum WeatherType
        {
            Sun,
            Rain,
            Storm,
            Snow,
            GreenRain

        }
        public static WeatherType getWeather(int day, int gameId)
        {
            if (day == 1 || day == 2 || day == 4 || day % 28 == 1) { 
                return WeatherType.Sun;
            }

            if (day == 3)
            {
                return WeatherType.Rain;
            }

            if (Utility.isGreenRainDay(day, gameId))
            {
                return WeatherType.GreenRain;
            }

            Season season = Utility.getSeasonFromDay(day);
            Random random;
            switch (season)
            {
                case Season.Spring:
                case Season.Fall:
                    random = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("location_weather"), gameId, day-1);
                    if (random.NextDouble() < 0.183)
                    {
                        return WeatherType.Rain;
                    }
                    break;
                case Season.Summer:
                    if (Utility.getDayOfMonthFromDay(day) % 13 == 0)
                    {
                        return WeatherType.Storm;
                    }
                     
                    random = Utility.CreateDaySaveRandom(day-1, gameId, Game1.hash.GetDeterministicHashCode("summer_rain_chance"));
                    float chanceToRain = 0.12f + (float)Utility.getDayOfMonthFromDay(day-1) * 0.003f;
                    if (random.NextBool(chanceToRain))
                    {
                        return WeatherType.Rain;
                    }
                    break;
                default:
                    break;
            }

            return WeatherType.Sun;
        }
    }
}
