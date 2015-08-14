using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Generator
{
    public abstract class CatapultEntity
    {
        protected List<bool> dailyAvailability;

        public string Name
        {
            get;
            private set;
        }

        public CatapultEntity(string name)
        {
            this.Name = name;
            this.dailyAvailability = new List<bool>();
        }
        
        public void addDay(int dayNum, bool value)
        {
            this.dailyAvailability.Insert(dayNum, value);
        }

        public void addDay()
        {
            this.addDay(this.dailyAvailability.Count, true);
        }

        public bool isAvailableOnDay(int dayNum)
        {
            return this.dailyAvailability[dayNum];
        }

        public void setAvailabilityOnDay(int dayNum, bool value)
        {
            if (dayNum < this.dailyAvailability.Count)
                this.dailyAvailability[dayNum] = value;

            throw new IndexOutOfRangeException(
                String.Format("Tried to edit day index {0} but indices only go up to {1}"
                , dayNum, this.dailyAvailability.Count - 1));
        }

        public void removeDay(int dayNum)
        {
            this.dailyAvailability.RemoveAt(dayNum);
        }

        public override string ToString()
        {
            return this.Name;
        }
        
    }
}
