using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
    internal class Resort
    {
        static Dictionary<int, String[]> hospitalDates = new Dictionary<int, string[]>
        {
            { 4, new String[] { "Abigail" } },
            { 9, new String[] { "Willy" } },
            { 11, new String[] { "Jodi", "Vincent" } },
            { 16, new String[] { "Leah" } },
            { 18, new String[] { "Jodi" } },
            { 25, new String[] { "Pam" } },
            { 32, new String[] { "Sebastian" } },
            { 37, new String[] { "Elliot" } },
            { 44, new String[] { "Alex" } },
            { 46, new String[] { "Robin" } },
            { 53, new String[] { "Demetrius" } },
            { 60, new String[] { "Gus" } },
            { 65, new String[] { "Lewis" } },
            { 67, new String[] { "Sam" } },
            { 74, new String[] { "Marnie" } },
            { 81, new String[] { "Caroline" } },
            { 88, new String[] { "Penny" } },
            { 93, new String[] { "Haley" } },
            { 95, new String[] { "Emily" } },
            { 99, new String[] { "Harvey" } },
            { 100, new String[] { "Clint" } },
            { 102, new String[] { "Jas", "Marnie" } },


        };

        static List<String> GetAvailableNPCs(int daysPlayed)
        {
            List<String> list = new List<String>();

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

        static void AddToList(List<String> list, int daysPlayed, string person)
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

        public static List<String> CalculateResortVisitors(int seed, int daysPlayed)
        {

            List<String> valid_visitors = GetAvailableNPCs(daysPlayed);
            List<String> visitors = new List<String>();


            Random seeded_random;
            float seedf = (float)(ulong)seed;
            float one = ((float)seedf * 1.21f);
            float two = ((float)daysPlayed * 2.5f);
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
