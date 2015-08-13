using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Generator
{
    public class Professor
    {
        private string name;
        private int numLecturesGiven;
        private int[] daysCantLecture;

        public Professor(string name, int[] daysCantLecture)
        {
            this.name = name;
            this.numLecturesGiven = 0;
            this.daysCantLecture = daysCantLecture;
        }

        public string getName()
        {
            return this.name;
        }

        public int getNumLecturesGiven()
        {
            return this.numLecturesGiven;
        }

        public void resetLectures()
        {
            this.numLecturesGiven = 0;
        }
        
        public bool canLectureOnDay(int dayNum)
        {
            return !this.daysCantLecture.Contains(dayNum);
        }

        public void giveLecture()
        {
            this.numLecturesGiven++;
        }

        public bool giveLecture(int dayNum)
        {
            if (this.canLectureOnDay(dayNum))
            {
                this.giveLecture();
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
