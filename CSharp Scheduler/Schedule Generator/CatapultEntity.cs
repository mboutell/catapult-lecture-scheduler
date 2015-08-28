using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Generator
{
    public abstract class CatapultEntity
    {
        // make this a dictionary of dates and bools. Demand a date when adding or removing a day
        public Dictionary<DateTime, bool> DailyAvailability
        {
            get;
            protected set;
        }

        public string Name
        {
            get;
            private set;
        }

        public CatapultEntity(string name)
        {
            this.Name = name;
            this.DailyAvailability = new Dictionary<DateTime, bool>();
        }
        
        public void setDate(DateTime date, bool value)
        {
            this.DailyAvailability[date] = value;
        }

        public bool isAvailableOnDate(DateTime date)
        {
            if (!this.DailyAvailability[date])
            {
                return false;
            }
            return true;
        }

        public Dictionary<DateTime, bool> getAvailablility()
        {
            return this.DailyAvailability;
        }

        public void removeDay(DateTime date)
        {
            if (!this.DailyAvailability.Remove(date))
                throw new ArgumentException("Date does not exist in entity");
        }

        public override string ToString()
        {
            return this.Name;
        }
        
    }
}
