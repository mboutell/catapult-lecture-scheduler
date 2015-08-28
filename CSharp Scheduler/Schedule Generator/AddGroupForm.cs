using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Schedule_Generator
{
    public partial class AddGroupForm : Schedule_Generator.AddEntityForm
    {
        public AddGroupForm(ScheduleGenerator generator) : base(generator)
        {
            InitializeComponent();
        }

        private void addEntityButton_Click(object sender, EventArgs e)
        {
            if (this.isEditting)
            {
                this.editExistingGroup();
            }
            else
            {
                this.addNewGroup();
            }
        }

        private void addNewGroup()
        {
            string name = this.nameTextBox.Text;

            List<DateTime> dates = new List<DateTime>();
            foreach (DateTime date in this.datesUnavailableListBox.Items)
                dates.Add(date.Date);

            try
            {
                this.scheduleGenerator.addGroup(name, dates);
            }
            catch (ArgumentException repeatGroupException)
            {
                //Tell the user what horrid thing they have attempted
            }
        }

        private void editExistingGroup()
        {
            this.scheduleGenerator.removeGroup(this.nameToEdit);
            this.addNewGroup();
        }
    }
}
