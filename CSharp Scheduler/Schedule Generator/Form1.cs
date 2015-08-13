using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Schedule_Generator
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Starting stopwatch...");
            var watch = new Stopwatch();
            watch.Start();

            int numToGenerate = 2000;
            int numThreads = Environment.ProcessorCount;

            System.Diagnostics.Debug.WriteLine(String.Format("Number of threads: {0}", numThreads));

            ConcurrentQueue<Schedule> baseList = new ConcurrentQueue<Schedule>();

            List<Thread> threads = new List<Thread>();


            for (int i = 0; i < numThreads; i++)
                threads.Add(new Thread(() => run(numToGenerate, numThreads, baseList)));
                

            foreach (Thread t in threads)
                t.Start();
            

            foreach (Thread t in threads)
                t.Join();
            


            List<Schedule> sortedList = baseList.ToList<Schedule>();
            foreach (Schedule sched in sortedList)
                sched.scoreSchedule();
            sortedList.Sort();

            for (int i = 0; i < 10; i++)
                System.Diagnostics.Debug.WriteLine(sortedList[i].getScore());


            watch.Stop();

            System.Diagnostics.Debug.WriteLine(String.Format("All complete. Run time: {0}", watch.ElapsedMilliseconds / 1000.0));
            System.Diagnostics.Debug.WriteLine(String.Format("Length of base list: {0} elements\n", baseList.Count));
            System.Diagnostics.Debug.WriteLine(String.Format("Best Schedule:\n{0}", sortedList[0].ToString()));
        }

        static void run(int numToGenerate, int numThreads, ConcurrentQueue<Schedule> toAddTo)
        {
            // One thread will finish adding the last one and the rest will still be running, yet to add their schedule.
            while (toAddTo.Count < numToGenerate - Environment.ProcessorCount + 1)
                toAddTo.Enqueue(createSchedule());

        }

        static Schedule createSchedule()
        {
            int numberOfDays = 9;
            int numberOfRooms = 5;


            Professor prof1 = new Professor("Coleman", new int[] { 1, 2, 3 });
            Professor prof2 = new Professor("Boutell", new int[] { 0, 4, 5 });
            Professor prof3 = new Professor("Rupakheti", new int[] { 0, 7 });
            Professor prof4 = new Professor("Wilkin", new int[] { 0 });
            Professor prof5 = new Professor("Broughten", new int[] { 3 });
            Professor prof6 = new Professor("Aidoo", new int[] { 7, 8 });
            Professor prof7 = new Professor("DeVasher", new int[] { 7, 8 });
            Professor prof8 = new Professor("Doctor", new int[] { 6, 8 });
            Professor prof9 = new Professor("Name", new int[] { 4, 6 , 8});

            Professor[] professors = new Professor[] { prof1, prof2, prof3, prof4, prof5, prof6, prof7, prof8, prof9 };
            //Professor[] professors = new Professor[] { prof2, prof3, prof4, prof5, prof6, prof7, prof8, prof9 };

            Group group1 = new Group("Boutell", professors, new int[] { 0 });
            Group group2 = new Group("Rupakheti", professors, new int[] {  0 });
            Group group3 = new Group("Aidoo", professors, new int[] { 0, 2 });
            Group group4 = new Group("DeVasher", professors, new int[] { 3 });
            Group group5 = new Group("Doctor", professors, new int[] { 3, 7 });
            Group group6 = new Group("Name", professors, new int[] { 6, 8 });
            Group group7 = new Group("Wilkin", professors, new int[] { 0, 2, 3, 4, 5, 6, 7, 8 });
            Group group8 = new Group("Broughten", professors, new int[] { 8 });

            Group[] groups = new Group[] { group1, group2, group3, group4, group5, group6, group7, group8 };
            //Group[] groups = new Group[] { group4, group5, group6, group7, group8 };


            Schedule test = new Schedule(professors, groups, numberOfDays, numberOfRooms);
            Random rand = new Random();

            bool success = false;

            //var watch = new Stopwatch();
            //watch.Start();

            while (!success)
            {
                success = test.generateRandomSchedule(rand);
            }

            //watch.Stop();

            //System.Diagnostics.Debug.WriteLine("Result:");
            //System.Diagnostics.Debug.WriteLine(test.ToString());
            //System.Diagnostics.Debug.WriteLine(String.Format("Complete. Time: {0}", watch.ElapsedMilliseconds/1000.0));

            return test;
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }
    }
}
