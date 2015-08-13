using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Schedule_Generator
{
    public class Group
    {

        private string groupName;
        private List<string> professorsSeen;
        private List<string> professorsNotSeen;
        private int[] daysCantWatchLecture;

        public Group(string groupName, string[] profNames, int[] daysCantWatchLecture)
        {
            this.groupName = groupName;
            this.professorsNotSeen = new List<string>();
            this.professorsSeen = new List<string>();
            this.daysCantWatchLecture = daysCantWatchLecture;

            foreach (string profName in profNames)
            { // there is probably a method to do this
                this.professorsNotSeen.Add(profName);
            }

            //foreach (string foo in this.professorsNotSeen)
            //    System.Diagnostics.Debug.WriteLine(foo);


        }


        public Group(string groupName, Professor[] profs, int[] daysCantWatchLecture)
            : this(groupName, profArrToStrArr(profs), daysCantWatchLecture)
        { }


        // for use by the copy method
        private Group(string groupName, List<string> profsSeen, List<string> profsNotSeen, int[] daysCantWatchLecture)
        {
            this.groupName = groupName;
            this.professorsSeen = new List<string>(profsSeen);
            this.professorsNotSeen = new List<string>(profsNotSeen);
            this.daysCantWatchLecture = daysCantWatchLecture; // this should never change
        }


        // To allow an array of professors to be used in the constructor
        private static string[] profArrToStrArr(Professor[] profs)
        {
            string[] names = new string[profs.Length];
            for (int i = 0; i < profs.Length; i++)
            {
                names[i] = profs[i].getName();
            }

            return names;
        }


        public bool hasSeen(string profName)
        {
            return this.professorsSeen.Contains(profName);
        }

        public bool canAttendOnDay(int day)
        {
            return !this.daysCantWatchLecture.Contains(day);
        }


        private bool watchLecture(string profName)
        {

            //Remove returns true if it successfully removed something
            if (this.professorsNotSeen.Remove(profName))
            {
                //System.Diagnostics.Debug.WriteLine("Got this far");
                this.professorsSeen.Add(profName);
                return true;
            }

            // Can't watch a lecture already seen/not in the not seen list
            return false;

        }

        public bool watchLecture(string profName, int dayNum)
        {
            if (this.canAttendOnDay(dayNum))
            {
                //System.Diagnostics.Debug.WriteLine("So far so good.");
                return this.watchLecture(profName);
            }

            return false;
        }

        public bool unwatchLecture(string profName)
        {
            if (!this.hasSeen(profName))
                return false;

            this.professorsSeen.Remove(profName);
            this.professorsNotSeen.Add(profName);

            return true;
            
        }

        private Group copy()
        {
            List<string> profsSeenCopy = new List<string>(this.professorsSeen);
            List<string> profsNotSeenCopy = new List<string>(this.professorsNotSeen);

            //the daysCantWatchLecture array should never change so it should not need to be copied.

            return new Group(this.groupName, profsSeenCopy, profsNotSeenCopy, this.daysCantWatchLecture);

        }

        public override string ToString()
        {
            return this.groupName;
        }
    }
}
