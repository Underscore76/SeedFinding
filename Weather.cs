﻿using System;
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
            GreenRain,
			Debris

        }
		public static bool isRain(int day, long gameId)
		{
			WeatherType type = getWeather(day, gameId);

			return type == WeatherType.Rain || type == WeatherType.Storm || type == WeatherType.GreenRain;
		}
        public static WeatherType getWeather(int day, long gameId)
        {
            if (day == 1 || day == 2 || day == 4 || day % 28 == 1) { 
                return WeatherType.Sun;
            }

            if (day == 3)
            {
                return WeatherType.Rain;
            }

            // Off by one error forcing day 5 to be sun
            if (day == 5)
            {
                return WeatherType.Sun;
            }

            if (Utility.isGreenRainDay(day, gameId))
            {
                return WeatherType.GreenRain;
            }

            int dayOfMonth = Utility.getDayOfMonthFromDay(day);

            Season season = Utility.getSeasonFromDay(day);
            Random random;
            switch (season)
            {
                case Season.Spring:
                    if (dayOfMonth == 13 || dayOfMonth == 24)
                    {
                        return WeatherType.Sun;
                    }
                    if (dayOfMonth == 2 || dayOfMonth == 3 || dayOfMonth == 5 )
                    {
                        return WeatherType.Sun;
                    }
                    if (dayOfMonth == 4)
                    {
                        return WeatherType.Rain;
                    }
                    random = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("location_weather"), gameId, day - 1);
                    if (random.NextDouble() < 0.183)
                    {
                        return WeatherType.Rain;
                    }
                    break;
                case Season.Fall:
                    if (dayOfMonth == 16 || dayOfMonth == 27)
                    {
                        return WeatherType.Sun;
                    }
                    random = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("location_weather"), gameId, day-1);
                    if (random.NextDouble() < 0.183)
                    {
                        return WeatherType.Rain;
                    }
                    break;
                case Season.Summer:
                    if (dayOfMonth % 13 == 0)
                    {
                        return WeatherType.Storm;
                    }
                    if (dayOfMonth == 11 || dayOfMonth == 28){
                        return WeatherType.Sun;
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

		public static bool isRainType(WeatherType type)
		{
			return new HashSet<Weather.WeatherType> { Weather.WeatherType.Rain, Weather.WeatherType.Storm, Weather.WeatherType.GreenRain }.Contains(type);
		}

		public static WeatherType getWeatherTypeFromName(string name)
		{
			switch(name)
			{
				case "Rain":
					return WeatherType.Rain;
				case "GreenRain":
					return WeatherType.GreenRain;
				case "Storm":
					return WeatherType.Storm;
				case "Wind":
					return WeatherType.Debris;
				case "Snow":
					return WeatherType.Snow;
				default:
					return WeatherType.Sun;
			}
		}
    }
}
