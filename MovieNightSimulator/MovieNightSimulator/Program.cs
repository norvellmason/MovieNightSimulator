using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieNightSimulator
{
    internal class Program
    {
        public enum People
        {
            Corey,
            Jon,
            Kasey,
            Mason,
            Zak
        }

        static Dictionary<People, float> AttendanceRates2024 = new Dictionary<People, float>
        {
            { People.Corey, 0.9f },
            { People.Jon, 0.725f },
            { People.Kasey, 0.85f },
            { People.Mason, 0.9f },
            { People.Zak, 0.375f },
        };

        static Random random = new Random();

        static void Main(string[] args)
        {
            List<int> totalMovieNightTotals = new List<int>();
            List<int> totalSequelPickTotals = new List<int>();
            List<int> totalSequelConflictTotals = new List<int>();

            for (int i = 0; i < 5000; i++)
            {
                SimulateAYearOfMovieNight(out int totalMovieNights, out int totalSequelPicks, out int totalSequelConflicts);
                totalMovieNightTotals.Add(totalMovieNights);
                totalSequelPickTotals.Add(totalSequelPicks);
                totalSequelConflictTotals.Add(totalSequelConflicts);
            }

            Console.WriteLine("Total Movie Nights Average: " + totalMovieNightTotals.Average());
            Console.WriteLine("Times Corey Picked a Sequel Average: " + totalSequelPickTotals.Average());
            Console.WriteLine("Times Someone Attended a Sequel But Missed Part 1 Average: " + totalSequelConflictTotals.Average());

            Console.WriteLine("Percentage Of Sequels That Were Problematic: " + (totalSequelConflictTotals.Average() / totalSequelPickTotals.Average()));
        }

        static void SimulateAYearOfMovieNight(out int totalMovieNights, out int totalSequelPicks, out int totalSequelConflicts)
        {
            Console.WriteLine("Simulating One Year Of Movie Night...");

            HashSet<People> peopleEligibleToBePicked = GetNewSetOfAllPeople();
            totalMovieNights = 0;
            totalSequelPicks = 0;
            totalSequelConflicts = 0;
            bool waitingForCoreysSequelPick = false;
            HashSet<People> peopleThatSawCoreysFirstPick = new HashSet<People>();

            for (int currentWeek = 0; currentWeek < 52; currentWeek++)
            {
                HashSet<People> attendees = GetAttendees();
                if (attendees.Count >= 2)
                {
                    IEnumerable<People> eligibleAndAttending = peopleEligibleToBePicked.Intersect(attendees);
                    if (eligibleAndAttending.Count() == 0)
                    {
                        // Reset week
                        peopleEligibleToBePicked = GetNewSetOfAllPeople();
                        eligibleAndAttending = peopleEligibleToBePicked.Intersect(attendees);
                    }

                    People personPicked = eligibleAndAttending.ElementAt(random.Next(eligibleAndAttending.Count()));
                    totalMovieNights++;
                    if (personPicked == People.Corey)
                    {
                        if (waitingForCoreysSequelPick)
                        {
                            totalSequelPicks++;
                            foreach (People person in attendees)
                            {
                                if (!peopleThatSawCoreysFirstPick.Contains(person))
                                {
                                    totalSequelConflicts++;
                                    break;
                                }
                            }
                            waitingForCoreysSequelPick = false;
                            peopleThatSawCoreysFirstPick.Clear();
                        }
                        else
                        {
                            waitingForCoreysSequelPick = true;
                            foreach (People person in attendees)
                            {
                                peopleThatSawCoreysFirstPick.Add(person);
                            }
                        }
                    }
                    peopleEligibleToBePicked.Remove(personPicked);
                }
            }

            Console.WriteLine("Total Movie Nights: " + totalMovieNights);
            Console.WriteLine("Times Corey Picked a Sequel: " + totalSequelPicks);
            Console.WriteLine("Times Someone Attended a Sequel But Missed Part 1: " + totalSequelConflicts);
            Console.WriteLine("");
        }

        static HashSet<People> GetAttendees()
        {
            HashSet<People> attendees = new HashSet<People>();

            bool PersonWillAttend(People person)
            {
                return random.NextDouble() <= AttendanceRates2024[person];
            }   

            foreach (People person in Enum.GetValues(typeof(People)))
            {
                if (PersonWillAttend(person))
                {
                    attendees.Add(person);
                }
            }

            return attendees;
        }

        static HashSet<People> GetNewSetOfAllPeople()
        {
            HashSet<People> allPeople = new HashSet<People>();

            foreach (People person in Enum.GetValues(typeof(People)))
            {
                allPeople.Add(person);
            }

            return allPeople;
        }
    }
}
