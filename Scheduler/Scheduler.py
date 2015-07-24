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
from math import ceil, floor

class Schedule:
    def __init__(self, profs, groups):
        # list of days containing a group professor pair
        # can switch days, groups, and professors around
        # a score and comparisons necessary to sort them
        
        self.score = 0
        self.num_days = 2
        self.num_rooms = 3
        
        self.days = [ {} for i in range(self.num_days) ]
        
        self.professors = profs
        self.groups = groups
        
        self.max_groups_per_room = len(self.groups)//self.num_rooms
        self.max_num_students_in_room = 30
    
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
        
        
        # A lot of this can be compressed into the same loop
        # and some calculations should be moved outside the loop
        
        
        prof_repeat_score = 0
        multiplier = 2
        
        for group in self.groups:
            for prof in self.professors:
                num = group.times_seen_professor(prof)
                prof_repeat_score += int( ((num - 1) ** 2) * multiplier )
                
                
                
        num_lectures_score = 0
        multiplier = 1
        
        avg_lectures_given = self.num_rooms * self.num_days / len(self.professors)
        ideal_lectures_given = floor(avg_lectures_given), ceil(avg_lectures_given)
               
        for prof in self.professors:
            
            num = prof.get_days_lectured()
            
            if not num == ideal_lectures_given[0] and not num == ideal_lectures_given[1]:
                num_lectures_score += int( (( num - avg_lectures_given ) ** 2) * multiplier )
        
        
        
        crowded_class_score = 0
        multiplier = 0.2
        
        for day in self.days:
            for room in list(day.values()):
                
                num_in_room = 0
                
                for group in room:
                    num_in_room += group.get_num_students()
                    
                if num_in_room > self.max_num_students_in_room:
                    crowded_class_score += int( (( num_in_room - self.max_num_students_in_room ) ** 2) * multiplier )
                    
                    
        print("Repeat score:", prof_repeat_score)
        print("Lecture number score:", num_lectures_score)
        print("Crowded class score:", crowded_class_score)
                    
        self.score = prof_repeat_score + num_lectures_score + crowded_class_score
                    
    
    
            
    
    def generate_random_schedule(self):
        
        
        for day in self.days:
            
            scramble_list(self.professors)
            scramble_list(self.groups)
            
            # putting a professor in each room
            for room in range(self.num_rooms):
                prof = self.professors[room]
                day[prof] = []
                prof.give_lecture()
                
            
            # Outer loop and counter account for multiple groups in one room
            exit_loop = False    
            counter = 0
            
            for i in range(self.max_groups_per_room):
                
                if exit_loop:
                    break
                
                for prof, room in day.items():                
                    
                    room.append(self.groups[counter])
                    self.groups[counter].watch_lecture( prof )
                    
                    counter += 1
                    
                    if counter >= len(self.groups):
                        exit_loop = True
                        break
                    
        self.score_schedule()
            
                
                
                
    
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
        result = "Schedule:\nScore: {:d}\n".format(self.score)
        
        counter = 1
        for day in self.days:
            result += "Day " + str(counter) + ": " + str(day) + "\n"
            counter += 1
            
        return result
    
    def copy(self):
        pass
    
class Group:
    def __init__( self, num_students, prof_name , all_profs):
        # has a number of students
        # constraints on days
        # what professors they have seen
        self.num_students = num_students
        self.prof_name = prof_name
        self.times_seen_profs = {}
        
        for prof in all_profs:
            self.times_seen_profs[prof] = 0
    
    def times_seen_professor(self, prof):
        return self.times_seen_profs[prof]
    
    def get_num_students(self):
        return self.num_students
    
    def watch_lecture(self, prof):
        self.times_seen_profs[prof] += 1
    
    def __str__(self):
        return self.prof_name + ": " + str(self.num_students)
    
    def __repr__(self):
        return self.__str__()
    
    
class Professor:
    def __init__(self, name):
        # has constraints on days
        # number of days they have lectured
        self.num_days_lectured = 0
        self.name = name
        
    def give_lecture(self):
        self.num_days_lectured += 1
        
    def get_days_lectured(self):
        return self.num_days_lectured
        
    def __str__(self):
        return self.name
    
    def __repr__(self):
        return self.__str__()
        
        
        
        
def swap_elements(el1, el2, ls):
    cp = ls[el1]
    ls[el1] = ls[el2]
    ls[el2] = cp
    
def scramble_list(ls):
    for x in range(len(ls)):
        num = random.randint(x, len(ls)-1)
        swap_elements(x, num, ls)
        

        
        
# maybe have a room
        
        
testProf1 = Professor("Boutell")
testProf2 = Professor("Aidoo")
testProf3 = Professor("Song")
testProf4 = Professor("DeVasher")

testProfs = [testProf1, testProf2, testProf3, testProf4]


testGroup1 = Group(13, "Boutell", testProfs)
testGroup2 = Group(18, "Coleman", testProfs)
testGroup3 = Group(16, "Aidoo", testProfs)
testGroup4 = Group(15, "Rupakheti", testProfs)
testGroup5 = Group(17, "DeVasher", testProfs)
testGroup6 = Group(16, "Song", testProfs)

testGroups = [testGroup1, testGroup2, testGroup3, testGroup4, testGroup5, testGroup6]

test = Schedule(testProfs, testGroups)
test.generate_random_schedule()
print(test)

        