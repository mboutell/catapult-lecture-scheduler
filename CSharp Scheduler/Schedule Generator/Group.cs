using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Schedule_Generator
{
    public class Group : CatapultEntity
    {
        
        private List<string> professorsSeen;
        private List<string> professorsNotSeen;

        public Group(string groupName) : base(groupName)
        {
            this.professorsNotSeen = new List<string>();
            this.professorsSeen = new List<string>();
        }

        public void addProfessor( Professor prof)
        {
            this.addProfessor(prof.Name);
        }

        private void addProfessor(string profName)
        {
            this.professorsNotSeen.Add(profName);
        }

        public void removeProfessor(Professor prof)
        {
            this.removeProfessor(prof.Name);
        }

        public void removeProfessor(string profName)
        {
            if (this.professorsNotSeen.Remove(profName))
                return;
            else if (this.professorsSeen.Remove(profName))
                return;
            else
                throw new ArgumentException("Professor does not exist in this group");            
        }

        public bool hasSeen(string profName)
        {
            return this.professorsSeen.Contains(profName);
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

            if (base.isAvailableOnDay(dayNum))
                return this.watchLecture(profName);

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

        public Group copy()
        {
            Group groupCopy = new Group(this.Name);

            groupCopy.dailyAvailability = new List<bool>(base.dailyAvailability);
            groupCopy.professorsSeen = new List<string>(this.professorsSeen);
            groupCopy.professorsNotSeen = new List<string>(this.professorsNotSeen);

            return groupCopy;
        }
    }
}
