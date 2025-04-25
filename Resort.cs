using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeedFinding.Utilities;

namespace SeedFinding
{
    internal class Resort
    {
        static Dictionary<int, string[]> hospitalDates = new Dictionary<int, string[]>
        {
            { 4, new string[] { "Abigail" } },
            { 9, new string[] { "Willy" } },
            { 11, new string[] { "Jodi", "Vincent" } },
            { 16, new string[] { "Leah" } },
            { 18, new string[] { "Jodi" } },
            { 25, new string[] { "Pam" } },
            { 32, new string[] { "Sebastian" } },
            { 37, new string[] { "Elliot" } },
            { 44, new string[] { "Alex" } },
            { 46, new string[] { "Robin" } },
            { 53, new string[] { "Demetrius" } },
            { 60, new string[] { "Gus" } },
            { 65, new string[] { "Lewis" } },
            { 67, new string[] { "Sam" } },
            { 74, new string[] { "Marnie" } },
            { 81, new string[] { "Caroline" } },
            { 88, new string[] { "Penny" } },
            { 93, new string[] { "Haley" } },
            { 95, new string[] { "Emily" } },
            { 99, new string[] { "Harvey" } },
            { 100, new string[] { "Clint" } },
            { 102, new string[] { "Jas", "Marnie" } },


        };

        static List<string> GetAvailableNPCs(int daysPlayed)
        {
            List<string> list = new();

            //AddToList(list, daysPlayed, "George");
            AddToList(list, daysPlayed, "Alex");
            //AddToList(list, daysPlayed, "Evelyn");

            AddToList(list, daysPlayed, "Emily");
            AddToList(list, daysPlayed, "Haley");

            AddToList(list, daysPlayed, "Vincent");
            AddToList(list, daysPlayed, "Jodi");
            AddToList(list, daysPlayed, "Sam");
            AddToList(list, daysPlayed, "Kent");

            AddToList(list, daysPlayed, "Clint");

            AddToList(list, daysPlayed, "Lewis");

            AddToList(list, daysPlayed, "Caroline");
            AddToList(list, daysPlayed, "Abigail");
            AddToList(list, daysPlayed, "Pierre");

            AddToList(list, daysPlayed, "Gus");

            AddToList(list, daysPlayed, "Penny");
            AddToList(list, daysPlayed, "Pam");

            AddToList(list, daysPlayed, "Harvey");

            AddToList(list, daysPlayed, "Elliott");

            AddToList(list, daysPlayed, "Robin");
            AddToList(list, daysPlayed, "Maru");
            AddToList(list, daysPlayed, "Demetrius");
            AddToList(list, daysPlayed, "Sebastian");

            //AddToList(list, daysPlayed, "Linus");

            AddToList(list, daysPlayed, "Jas");
            AddToList(list, daysPlayed, "Marnie");
            AddToList(list, daysPlayed, "Shane");

            AddToList(list, daysPlayed, "Leah");

            AddToList(list, daysPlayed, "Leo");

            return list;
        }

        static void AddToList(List<string> list, int daysPlayed, string person)
        {

            if (daysPlayed <= 112 && person == "Kent")
            {
                return;
            }

            int dayInYear = (daysPlayed - 1) % 112 + 1;
            // Fall 15, any year
            if (dayInYear == 71)
            {
                if (person == "Pam" || person == "Emily")
                {
                    return;
                }
            }

            int day = (daysPlayed - 1) % 7 + 1;
            if (day == 2 || day == 3 || day == 5)
            {
                if (person == "Vincent" || person == "Jas" || person == "Penny")
                {
                    return;
                }
            }

            if (day == 2 || day == 4)
            {
                if (person == "Harvey" || person == "Maru")
                {
                    return;
                }
            }

            if (day != 5 && person == "Clint")
            {
                return;
            }

            if (day != 2 && person == "Robin")
            {
                return;
            }

            if (day != 2 && day != 1 && person == "Marnie")
            {
                return;
            }

            if (hospitalDates.ContainsKey(dayInYear))
            {
                string[] persons = hospitalDates[dayInYear];
                if (persons.Contains(person))
                {
                    return;
                }
            }

            list.Add(person);
        }

        static bool IsChild(string person)
        {
            return person == "Vincent" || person == "Jas" || person == "Leo";
        }

        public static List<string> CalculateResortVisitors(int seed, int daysPlayed)
        {

            List<string> valid_visitors = GetAvailableNPCs(daysPlayed);
            List<string> visitors = new();


            Random seeded_random;
            float seedf = (ulong)seed;
            float one = (float)seedf * 1.21f;
            float two = daysPlayed * 2.5f;
            int number = (int)one + (int)two;
            //Random seeded_random = new Random((int)((float)seed * 1.21f) + (int)((float)daysPlayed * 2.5f));
            seeded_random = new Random(number);

            if (seeded_random.NextDouble() < 0.4)
            {
                for (int k = 0; k < 5; k++)
                {
                    string visitor4 = Utility.GetRandom(valid_visitors, seeded_random);
                    if (visitor4 != null && !IsChild(visitor4))
                    {
                        valid_visitors.Remove(visitor4);
                        visitors.Add(visitor4);
                    }
                }
            }
            else
            {
                List<List<string>> potentialGroups = new List<List<string>>();
                potentialGroups.Add(new List<string> { "Sebastian", "Sam", "Abigail" });
                potentialGroups.Add(new List<string> { "Jodi", "Kent", "Vincent", "Sam" });
                potentialGroups.Add(new List<string> { "Jodi", "Vincent", "Sam" });
                potentialGroups.Add(new List<string> { "Pierre", "Caroline", "Abigail" });
                potentialGroups.Add(new List<string> { "Robin", "Demetrius", "Maru", "Sebastian" });
                potentialGroups.Add(new List<string> { "Lewis", "Marnie" });
                potentialGroups.Add(new List<string> { "Marnie", "Shane", "Jas" });
                potentialGroups.Add(new List<string> { "Penny", "Jas", "Vincent" });
                potentialGroups.Add(new List<string> { "Pam", "Penny" });
                potentialGroups.Add(new List<string> { "Caroline", "Marnie", "Robin", "Jodi" });
                potentialGroups.Add(new List<string> { "Haley", "Penny", "Leah", "Emily", "Maru", "Abigail" });
                potentialGroups.Add(new List<string> { "Alex", "Sam", "Sebastian", "Elliott", "Shane", "Harvey" });
                List<string> group = potentialGroups[seeded_random.Next(potentialGroups.Count)];
                bool failed = false;
                foreach (string s in group)
                {
                    if (!valid_visitors.Contains(s))
                    {
                        failed = true;
                        break;
                    }
                }
                if (!failed)
                {
                    foreach (string visitor3 in group)
                    {
                        valid_visitors.Remove(visitor3);
                        visitors.Add(visitor3);
                    }
                }
                for (int i = 0; i < 5 - visitors.Count; i++)
                {
                    string visitor = Utility.GetRandom(valid_visitors, seeded_random);
                    if (visitor != null && !IsChild(visitor))
                    {
                        valid_visitors.Remove(visitor);
                        visitors.Add(visitor);
                    }
                }
            }

            return visitors;
        }
    }
}
