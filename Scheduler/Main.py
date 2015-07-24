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
import time

class Algorithm:
    
    def __init__(self):
        
        self.starting_number = 10000
        self.num_to_generate_on_randomization = 10
        self.max_pq_size = 5000
        
        self.pq = []
        
    def run(self):
        
        testProf1 = Professor("Boutell")
        testProf2 = Professor("Aidoo")
        testProf3 = Professor("Song")
        testProf4 = Professor("DeVasher")
        testProf5 = Professor("Rupakheti")
        testProf6 = Professor("Coleman")
        testProf7 = Professor("Steve")
        testProf8 = Professor("Copinger")
        testProf9 = Professor("Jane Doe")
        
        testProfs = [testProf1, testProf2, testProf3, testProf4, testProf5, testProf6, testProf7, testProf8, testProf9]
        
        
        testGroup1 = Group(13, "Boutell", testProfs)
        testGroup2 = Group(18, "Coleman", testProfs)
        testGroup3 = Group(16, "Aidoo", testProfs)
        testGroup4 = Group(15, "Rupakheti", testProfs)
        testGroup5 = Group(17, "DeVasher", testProfs)
        testGroup6 = Group(16, "Song", testProfs)
        
        testGroups = [testGroup1, testGroup2, testGroup3, testGroup4, testGroup5, testGroup6]
        
        start = time.time()
        num_to_generate = self.starting_number
        for i in range(num_to_generate):
            schedule = Schedule(testProfs, testGroups)
            schedule.generate_random_schedule()
            heappush(self.pq, schedule)
        print( "Time to add {:d} to pq: {:.2f} seconds".format(num_to_generate, time.time() - start) )
            
            
            
        start = time.time()
        temp = []
        while len(self.pq) > self.max_pq_size:
            temp.append(heappop(self.pq))
        heapify(temp)
        self.pq = temp
        print( "Time to remove from pq: {:.2f} seconds".format( time.time() - start) )

            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
if __name__ == "__main__":
    alg = Algorithm()
    alg.run()
        