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
   
   

"""



import random
from heapq import *

class Schedule:
    def __init__(self, profs, groups):
        # list of days containing a group professor pair
        # can switch days, groups, and professors around
        # a score and comparisons necessary to sort them
        self.score = 0
        self.num_days = 8
        self.num_rooms = 5
        self.days = [ {} for i in range(self.num_days) ]
        
        self.profs = profs
        self.groups = groups
    
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
            
            for room in self.num_rooms:
                prof = self.profs[random.randint(0, len(self.profs)-1)]
                room[]
    
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
    
def scramble_list(self, ls):
        
    for x in range(len(ls)):
        num = random.randint(x, len(ls)-1)
        swap_elements(x, num, ls)
        
        
# maybe have a room
        
        
        
        
        
        
        
        