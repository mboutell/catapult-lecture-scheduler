using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Generator
{
    public class Professor : CatapultEntity
    {
        private int numLecturesGiven;

        public Professor(string name) : base(name)
        {
            this.numLecturesGiven = 0;
        }

        public int getNumLecturesGiven()
        {
            return this.numLecturesGiven;
        }

        public void resetLectures()
        {
            this.numLecturesGiven = 0;
        }

        public bool giveLecture(DateTime date)
        {
            if (base.isAvailableOnDate(date))
            {
                this.numLecturesGiven++;
                return true;
            }

            return false;
        }

        public void unGiveLecture()
        {
            this.numLecturesGiven--;
            if (this.numLecturesGiven < 0)
                throw new ArgumentOutOfRangeException("Number of lectures cannot be negative.");
;        }

        public Professor copy()
        {
            Professor profCopy = new Professor(this.Name);
            profCopy.numLecturesGiven = this.numLecturesGiven;
            profCopy.dailyAvailability = new Dictionary<DateTime, bool>(base.dailyAvailability);
            return profCopy;
        }
    }
}
