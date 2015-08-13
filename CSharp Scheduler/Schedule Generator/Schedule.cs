using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Generator
{
    public class Schedule : IComparable<Schedule>
    {
        private int numberOfDays;
        private int numberOfRooms;
        private List<Day> days;
        private Professor[] professors;
        private Group[] groups;
        private double score;


        // wont be needed outside of the scheduleS
        private class Day
        {
            private Dictionary<string, List<Group>> rooms;
            private int dayNumber;
            private int numberOfRooms;

            public Day(int numberOfRooms, int dayNum)
            {
                this.numberOfRooms = numberOfRooms;
                this.dayNumber = dayNum;
                this.rooms = new Dictionary<string, List<Group>>();
            }

            private bool addProfessor(Professor prof)
            {

                if (!prof.canLectureOnDay(this.dayNumber))
                    return false;

                this.rooms.Add(prof.getName(), new List<Group>());
                return true;
                
            }

            private bool removeProfessor(Professor prof)
            {
                return this.removeProfessor(prof.getName());
            }

            private bool removeProfessor(string profName)
            {
                return this.rooms.Remove(profName);
            }

            private bool clearProfessorGroups(Professor prof)
            {
                return this.clearProfessorGroups(prof.getName());
            }

            private bool clearProfessorGroups(string profName)
            {
                List<Group> placeholder;
                return this.rooms.TryGetValue(profName, out placeholder);
            }

            private void clearDay()
            {
                List<string> toRemove = new List<string>(this.rooms.Keys);
                foreach (string profName in toRemove)
                {
                    this.removeProfessor(profName);
                }
            }

            private void setProfessorGroups(Professor prof, List<Group> groups)
            {
                this.setProfessorGroups(prof.getName(), groups);
            }

            private void setProfessorGroups(string profName, List<Group> groups)
            {
                this.rooms[profName] = groups;
            }

            private bool addGroupsToProfessor(Professor prof, List<Group> groups)
            {
                return this.addGroupsToProfessor(prof.getName(), groups);
            }

            private bool addGroupsToProfessor(string profName, List<Group> groups)
            {

                foreach (Group group in groups)
                {
                    if (!this.addGroupToProfessor(profName, group))
                        return false;
                }

                return true;
            }

            private bool addGroupToProfessor(string profName, Group group)
            {
                if (!group.canAttendOnDay(this.dayNumber))
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


            private void addProfs(Professor[] profs, Random randGenerator)
            {
                // note that it is possible that there could be less profs than rooms. Should alert the user or anything?
                scrambleArray(profs, randGenerator);
                
                int numProfsAdded = 0;

                for (int i = 0; i < profs.Length; i++)
                {
                    if (this.addProfessor(profs[i]))
                        numProfsAdded++;

                    if (numProfsAdded == this.numberOfRooms)
                    {
                        return;
                    }

                }
            
            }

            
            private bool addGroups(List<Group> groupsToAdd)
            {
                int maxNumLectures = (int)Math.Ceiling((double)groupsToAdd.Count / (double)this.getActiveProfessorNames().Count);

                //will advance professors by level and examine the lowest levels first
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

                            if (groupsToAdd[groupIndex].watchLecture(profName, this.dayNumber))
                            {
                                groupAdded = true;

                                this.addGroupToProfessor(profName, groupsToAdd[groupIndex]);
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

                // if there are less groups the extra rooms arent needed
                //if (this.numberOfRooms > groupsToAdd.Count)
                //    foreach (string tempProfName in numGroupsLecturedTo.Keys)
                //        if (numGroupsLecturedTo[tempProfName] == 0)
                //            this.rooms.Remove(tempProfName);
            }
            
            public bool generateRandomDay(Professor[] profs, Group[] groups, Random randGenerator)
            {
                this.clearDay();

                this.addProfs(profs, randGenerator);

                scrambleArray(groups, randGenerator);
                List<Group> groupsToAdd = new List<Group>();

                foreach (Group group in groups)
                {
                    if (group.canAttendOnDay(this.dayNumber))
                        groupsToAdd.Add(group);
                }

                if (!this.addGroups(groupsToAdd))
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
                            //System.Diagnostics.Debug.WriteLine("Prof with no groups");
                            //System.Diagnostics.Debug.WriteLine(this.ToString());
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

        
        public Schedule(Professor[] profs, Group[] groups, int numDays, int numRooms)
        {
            this.professors = profs;
            this.groups = groups;
            this.numberOfDays = numDays;
            this.numberOfRooms = numRooms;

            this.days = new List<Day>();

            for (int dayNum = 0; dayNum < this.numberOfDays; dayNum++)
            {
                this.days.Add(new Day(this.numberOfRooms, dayNum));
            }

        }

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
            bool success = true;
            for (int dayNum = 0; dayNum < this.numberOfDays; dayNum++)
            {
                //System.Diagnostics.Debug.WriteLine(dayNum);

                if (!success)
                    break;

                counter = 0;
                while (!this.days[dayNum].generateRandomDay(this.professors, this.groups, randomGenerator))
                {
                    counter++;
                    //System.Diagnostics.Debug.WriteLine(String.Format("Current day: {0}", dayNum));
                    //System.Diagnostics.Debug.WriteLine(this.ToString());

                    // at some point we should admit defeat for the day.
                    // with some testing this around 1000 yields the best run times
                    if (counter > 1000)
                    {
                        success = false;
                        break;
                    }

                }
            }

            if (!success)
            {
                foreach (Day day in this.days)
                {
                    day.resetDay();
                }
            }

            return success;
        }

        private static void scrambleArray<T>(T[] arr, Random randGenerator)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                int randNum = randGenerator.Next(arr.Length);
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
            double goalNumLectures = (double)this.professors.Length / (double)this.numberOfDays;
            int goalNumLecturesLow = (int)goalNumLectures;
            int goalNumLecturesHigh = (int)Math.Ceiling(goalNumLectures);


            foreach (Professor prof in this.professors)
                profByName[prof.getName()] = prof;

            foreach (Day day in this.days)
            {
                foreach (string profName in day.getActiveProfessorNames())
                {
                    profByName[profName].giveLecture();
                }
            }

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
    }
}
