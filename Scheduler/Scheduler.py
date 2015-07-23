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



import random
from heapq import *

class Schedule:
    def __init__(self, profs, groups):
        # list of days containing a group professor pair
        # can switch days, groups, and professors around
        # a score and comparisons necessary to sort them
        self.score = 0
        self.num_days = 2
        self.num_rooms = 4
        self.days = [ {} for i in range(self.num_days) ]
        
        self.profs = profs
        self.groups = groups
        
        self.max_groups_per_room = 2
    
    def score_schedule(self):
        # weighted probably
        # same group seeing same prof
        # number of prof lectures
        # low score good
        # 1. 70% 
        # (each group: num times seen prof - 1) ** 2
        # 2. 20%
        # average = rooms * days / profs
        # further away from average (floor or ceil = no prob)
        # ( num_lecture_given - avg ) ** 2
        # 3. 10%
        # if over a max num of students add square of difference (num_in_class - avg) ** 2
        
        pass
    
    
            
    
    def generate_random_schedule(self):
        
        for day in self.days:
            
            scramble_list(self.profs)
            scramble_list(self.groups)
            
            # putting a professor in each room
            for room in range(self.num_rooms):
                prof = self.profs[room]
                day[prof] = []
                
            
            exit_loop = False    
            
            
            # DOESNT WORK
            for room in list(day.values()):
                
                if exit_loop:
                    break
                
                counter = 0
                
                # will flip between 0 and 1 (in the case that max_groups... == 2)
                # this means it will add 2 groups in each of the above 'room' iterations
                for i in range(self.max_groups_per_room):
                    room.append(self.groups[counter])
                    counter += 1
                    if counter >= len(self.groups):
                        exit_loop = True
                        break
            # DOESNT WORK
            
                
                
                
    
    def get_score(self):
        return self.score
    
    def __gt__(self, other):
        return self.score > other.get_score()
    
    def __lt__(self, other):
        return self.score < other.get_score()
    
    def __ge__(self, other):
        return self.score >= other.get_score()
    
    def __le__(self, other):
        return self.score <= other.get_score()
    
    def __eq__(self, other):
        return self.score == other.get_score()
    
    def __str__(self):
        result = ""
        
        for day in self.days:
            result += str(day) + "\n"
            
        return result
    
    def copy(self):
        pass
    
class Group:
    def __init__( self, num_students, prof_name ):
        # has a number of students
        # constraints on days
        # what professors they have seen
        self.num_students = num_students
        self.prof_name = prof_name
        self.has_seen = {}
        
    def watch_lecture(self, prof_name):
        self.has_seen[prof_name] = True
    
    def has_seen(self, prof_name):
        return prof_name in self.has_seen
    
    
class Professor:
    def __init__(self):
        # has constraints on days
        # number of days they have lectured
        self.num_days_lectured = 0
        
        
        
        
def swap_elements(el1, el2, ls):
    cp = ls[el1]
    ls[el1] = ls[el2]
    ls[el2] = cp
    
def scramble_list(ls):
    for x in range(len(ls)):
        num = random.randint(x, len(ls)-1)
        swap_elements(x, num, ls)
        

        
        
# maybe have a room
        
        
testProf1 = Professor()
testProf2 = Professor()
testProf3 = Professor()
testProf4 = Professor()


testGroup1 = Group(5, "Boutell")
testGroup2 = Group(9, "Coleman")
testGroup3 = Group(18, "Aidoo")
testGroup4 = Group(10, "Rupakheti")
testGroup5 = Group(1, "DeVasher")
testGroup6 = Group(73, "Song")

testProfs = [testProf1, testProf2, testProf3, testProf4]
testGroups = [testGroup1, testGroup2, testGroup3, testGroup4, testGroup5, testGroup6]

test = Schedule(testProfs, testGroups)
test.generate_random_schedule()
print(test)
        
        
        
        
        
        