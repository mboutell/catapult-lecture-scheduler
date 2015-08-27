using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Generator
{
    public class Schedule : IComparable<Schedule>
    {
        private int numberOfRooms;
        private List<DateTime> activeDates;
        private List<Day> days;
        private List<Professor> professors;
        private Dictionary<string, Professor> profsByName;
        private List<Group> groups;
        private double score;

        public Schedule(List<Professor> profs, List<Group> groups, List<DateTime> activeDates, int numRooms)
        {
            this.professors = new List<Professor>();
            foreach (Professor prof in profs)
                this.professors.Add(prof.copy());

            this.groups = new List<Group>();
            foreach (Group group in groups)
                this.groups.Add(group.copy());


            this.profsByName = new Dictionary<string, Professor>();

            foreach (Professor prof in this.professors)
                this.profsByName[prof.Name] = prof;

            this.activeDates = activeDates;
            this.numberOfRooms = numRooms;

            this.days = new List<Day>();
            foreach (DateTime date in this.activeDates)
            {
                this.days.Add(new Day(this.numberOfRooms, date));
            }

        }

        // This could give a much prettier string
        public override string ToString()
        {
            string returnVal = "";

            foreach (Day day in this.days)
            {
                returnVal += String.Format("{0}\n", day.ToString());
            }

            return returnVal;
        }

        public bool generateRandomSchedule(Random randomGenerator)
        {
            int counter;
            bool admitDefeat = false;

            for (int dayNum = 0; dayNum < this.activeDates.Count; dayNum++)
            {
                if (admitDefeat)
                    break;

                counter = 0;
                while (!this.days[dayNum].generateRandomDay(this.professors, this.groups, randomGenerator))
                {
                    counter++;

                    // at some point we should admit defeat for the day.
                    // with some testing this around 1000 yields the best run times
                    if (counter > 1000)
                    {
                        admitDefeat = true;
                        break;
                    }

                }
            }

            if (admitDefeat)
            {
                foreach (Day day in this.days)
                {
                    day.resetDay();
                }

                return false;
            }

            return true;
        }

        private static void scrambleArray<T>(List<T> arr, Random randGenerator)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                int randNum = randGenerator.Next(arr.Count);
                T backup = arr[randNum];
                arr[randNum] = arr[i];
                arr[i] = backup;
            }
        }

        public double getScore()
        {
            return this.score;
        }

        public double scoreSchedule()
        {
            double score = 0;


            score += this.scoreNumProfLectures();


            this.score = score;
            return this.score;
        }

        private double scoreNumProfLectures()
        {
            double score = 0;

            Dictionary<string, Professor> profByName = new Dictionary<string, Professor>();
            double goalNumLectures = (double)this.professors.Count / (double)this.activeDates.Count;
            int goalNumLecturesLow = (int)goalNumLectures;
            int goalNumLecturesHigh = (int)Math.Ceiling(goalNumLectures);


            foreach (Professor prof in this.professors)
                profByName[prof.Name] = prof;

            int numLectures;

            foreach (Professor prof in this.professors)
            {
                numLectures = prof.getNumLecturesGiven();

                if (numLectures == goalNumLecturesHigh || numLectures == goalNumLecturesLow)
                    continue;

                score += (numLectures - goalNumLectures) * (numLectures - goalNumLectures);
            }

            return score;
        }

        int IComparable<Schedule>.CompareTo(Schedule other)
        {
            if (this.score < other.getScore())
                return -1;
            if (this.score > other.getScore())
                return 1;
            return 0;
        }

        // adding an equality method would be good to remove repeats after schedule generation.









        // wont be needed outside of the schedules
        private class Day
        {
            private Dictionary<string, List<Group>> rooms;
            private Dictionary<string, Professor> profsInRooms;
            private DateTime date;
            private int numberOfRooms;


            public Day(int numberOfRooms, DateTime date)
            {
                this.numberOfRooms = numberOfRooms;
                this.date = date;
                this.rooms = new Dictionary<string, List<Group>>();
                this.profsInRooms = new Dictionary<string, Professor>();

            }
            
            private bool addProfessorToDay(Professor prof)
            {

                if (prof.giveLecture(this.date))
                {
                    this.profsInRooms[prof.Name] = prof;
                    this.rooms.Add(prof.Name, new List<Group>());
                    return true;
                }

                return false;
                
                
            }

            private void removeProfessorFromDay(Professor prof)
            {
                if (!this.rooms.Remove(prof.Name) || !this.profsInRooms.Remove(prof.Name))
                    throw new ArgumentException("Professor not in this day.");
                
                prof.unGiveLecture();
            }
            

            private void clearDay()
            {
                List<Professor> toRemove = new List<Professor>(this.profsInRooms.Values);
                foreach (Professor prof in toRemove)
                {
                    this.removeProfessorFromDay(prof);
                }
            }

            private bool addGroupToProfessor(string profName, Group group)
            {
                if (!group.isAvailableOnDate(this.date))
                    return false;

                //System.Diagnostics.Debug.WriteLine("Group available.");

                if (!group.watchLecture(profName, this.date))
                    return false;

                this.rooms[profName].Add(group);
                return true;
            }

            private List<Group> getProfessorGroups(string profName)
            {
                return this.rooms[profName];
            }

            public List<string> getActiveProfessorNames()
            {
                return new List<string>(this.rooms.Keys);
            }


            private void addProfsToDay(List<Professor> profs, Random randGenerator)
            {
                // note that it is possible that there could be less profs than rooms. Should alert the user or anything?
                scrambleArray(profs, randGenerator);
                
                int numProfsAdded = 0;

                for (int i = 0; i < profs.Count; i++)
                {
                    if (this.addProfessorToDay(profs[i]))
                    {
                        numProfsAdded++;
                    }

                    if (numProfsAdded == this.numberOfRooms)
                    {
                        return;
                    }

                }
            
            }

            
            private bool addGroupsToDay(List<Group> groupsToAdd)
            {
                int maxNumLectures = (int)Math.Ceiling((double)groupsToAdd.Count / (double)this.getActiveProfessorNames().Count);

                // will advance professors by level and examine the lowest levels first
                List<string>[] profLevels = new List<string>[maxNumLectures + 1];
                profLevels[0] = this.getActiveProfessorNames();
                for (int i = 1; i < profLevels.Length; i++)
                    profLevels[i] = new List<string>();


                bool groupAdded = true;
                string profName;

                for (int groupIndex = 0; groupIndex < groupsToAdd.Count; groupIndex++)
                {

                    groupAdded = false;

                    for (int levelNumber = 0; levelNumber < profLevels.Length - 1; levelNumber++)
                    {
                        if (groupAdded)
                            break;

                        for (int groupNum = 0; groupNum < profLevels[levelNumber].Count; groupNum++)
                        {
                            profName = profLevels[levelNumber][groupNum];

                            if (this.addGroupToProfessor(profName, groupsToAdd[groupIndex]))
                            {
                                groupAdded = true;
                                profLevels[levelNumber].Remove(profName);
                                profLevels[levelNumber + 1].Add(profName);

                                break;
                            }
                        }

                    }

                    if (!groupAdded)
                        return false;                    

                }

                return true;
            }

            private bool removeGrouplessProfs()
            {
                // this method assumes there should be at most one group per room
                // it will return false if there is more than one

                foreach (string profName in this.getActiveProfessorNames())
                {
                    if (this.rooms[profName].Count == 0)
                        this.rooms.Remove(profName);
                    else if (this.rooms[profName].Count > 1)
                        return false;
                    
                }

                return true;
            }
            
            public bool generateRandomDay(List<Professor> profs, List<Group> groups, Random randGenerator)
            {
                this.clearDay();

                // adds whatever profs it can. Doesn't fail.
                this.addProfsToDay(profs, randGenerator);

                scrambleArray(groups, randGenerator);
                List<Group> groupsToAdd = new List<Group>();

                foreach (Group group in groups)
                {
                    if (group.isAvailableOnDate(this.date))
                        groupsToAdd.Add(group);
                }
                
                if (!this.addGroupsToDay(groupsToAdd))
                {
                    this.resetDay();
                    return false;
                }

                // less groups than rooms is the only time a room should be empty
                if (this.numberOfRooms > groupsToAdd.Count)
                {
                    if (!this.removeGrouplessProfs())
                    {
                        this.resetDay();
                        return false;
                    }
                }
                else
                {
                    // If after all this there are still empty rooms there is a problem
                    foreach (List<Group> groupList in this.rooms.Values)
                    {
                        if (groupList.Count == 0)
                        {
                            this.resetDay();
                            return false;
                        }
                    }
                }
            

                return true;

                
                
            }

            public void resetDay()
            {
                foreach (string profName in this.rooms.Keys)
                {
                    foreach (Group group in this.rooms[profName])
                    {
                        group.unwatchLecture(profName);
                    }
                }

                this.clearDay();
            }

            public override string ToString()
            {
                string returnVal = "";

                foreach (string profName in this.rooms.Keys)
                {
                    returnVal += String.Format("  {0}: ", profName);

                    foreach (Group group in this.rooms[profName])
                    {
                        returnVal += String.Format("{0}, ", group.ToString());
                    }
                }

                return returnVal;
            }
        }


    }
}
