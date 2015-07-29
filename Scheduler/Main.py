"""

Algorithm:
   starting number of schedules
   places all of these in a pq
   do batches of randomizations of schedules, rescore
   in each randomization of a schedule generate more schedules, each in a varying level of randomization
   put them all back along with the original, unchanged schedule
   randomization level based on score
   store number of conflicts, randomize based on that
   high randomization = randomizing days totally randomly
   low randomization = swapping out profs/groups with high conflicts or swapping between
   when the pq gets over some length (1000000?) take out half the elements, dump the pq, and load the elements back into the pq
   
"""

from Scheduler import *
from heapq import *
import time, threading

class Algorithm:
    
    def __init__(self):
        
        self.num_days = 8
        self.num_rooms = 5
        self.max_students_per_room = 30
        
        # creating professors
        prof1 = Professor("Boutell")
        prof2 = Professor("Aidoo")
        prof3 = Professor("Song")
        prof4 = Professor("DeVasher")
        prof5 = Professor("Rupakheti")
        prof6 = Professor("Coleman")
        prof7 = Professor("Steve")
        prof8 = Professor("Copinger")
        prof9 = Professor("Jane Doe")
        
        self.profs = [prof1, prof2, prof3, prof4, prof5, prof6, prof7, prof8, prof9]
        
        # creating groups
        group1 = Group(13, "Boutell", self.profs)
        group2 = Group(18, "Coleman", self.profs)
        group3 = Group(16, "Aidoo", self.profs)
        group4 = Group(15, "Rupakheti", self.profs)
        group5 = Group(17, "DeVasher", self.profs)
        group6 = Group(16, "Song", self.profs)
        
        self.groups = [group1, group2, group3, group4, group5, group6]
        
        # for use in generating the base
        self.num_threads = 1
        self.base_list = []
        self.base_num_to_generate = 1000
        self.base_num_lecture_score_limit = 0
        self.base_repeat_lecture_limit = 50
        
        # Processing in the second "phase"
        self.batch_size = 100
        self.iterations = 1000
        self.max_base_list_size = 10000
        
        
        
        
        
        
    def run(self):
        
        # turns out python uses something called a global interpreter lock meaning only
        # one thread can be executed at any given time. As a result trying to multithread 
        # it actually significantly slows it down.
        
        start_time = time.time()
        
        
        
        self.generate_base()
        heapify(self.base_list)
        
        print("Base created in {:.2f} seconds.".format(time.time()-start_time))
            
        # should now only switch around who is already in there
        self.shuffle_base()
        
        # just to glance at the results
        print(len(self.base_list))
        
        for i in range(10):
            print(heappop(self.base_list))
                        
            
            
            
        time_taken = time.time() - start_time
        print("Complete.\nRun time: {:.2f} seconds".format(time_taken))
        
        
    def shuffle_base(self):
        
        for unused_var in range(self.iterations):
            
            if unused_var % 100 == 0:
                print( "Iteration: {:d}".format(unused_var) )
                print( "Size of base: {:d}".format(len(self.base_list)) )
            
            if len(self.base_list) > self.max_base_list_size:
                print("Shrinking base list...")
                temp = []
                for i in range(self.max_base_list_size//2):
                    temp.append( heappop(self.base_list) )
                    
                self.base_list = []
                while len(temp) > 0:
                    heappush( self.base_list, temp.pop() )
                print("Done.")
            
            batch = []
            to_put_back = []
            
            for i in range(self.batch_size):
                batch.append( heappop(self.base_list) )
                
            for sched in batch:
                
                to_put_back.append(sched)
                
                for day_num in range(self.num_days):
                    alternate = sched.copy()
                    alternate.randomize_groups_day( day_num )
                    alternate.score_schedule()
                    
                    if alternate < sched:
                        to_put_back.append( alternate )
                        
            for sched in to_put_back:
                heappush(self.base_list, sched)
        
        
    def generate_base(self):
        
        sched = Schedule( self.profs, self.groups, self.num_days, self.num_rooms, self.max_students_per_room )
        
        for unused_var in range( self.base_num_to_generate//self.num_threads ):
            
            sched = sched.copy()
            sched.generate_random_schedule()
            
            
            while sched.score_num_of_prof_lectures() > self.base_num_lecture_score_limit:
                sched.randomize_all_professors()
                
            
            while sched.score_repeat_lectures() > self.base_repeat_lecture_limit:
                sched.randomize_all_groups()
                

            sched.score_schedule()
            self.base_list.append(sched)
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
if __name__ == "__main__":
    alg = Algorithm()
    alg.run()
        