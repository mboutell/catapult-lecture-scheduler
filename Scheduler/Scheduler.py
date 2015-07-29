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



import random, sys
from math import ceil, floor

class Schedule:
    
    def __init__(self, profs, groups, num_days, num_rooms, students_per_room):
        # list of days containing a group professor pair
        # can switch days, groups, and professors around
        # a score and comparisons necessary to sort them
        
        self.score = 0
        self.num_days = num_days
        self.num_rooms = num_rooms
        self.max_num_students_in_room = students_per_room
        
        self.professors = profs
        self.groups = groups
        
        self.get_prof_by_name = {}
        
        for prof in self.professors:
            self.get_prof_by_name[ prof.get_name() ] = prof
        
        self.days = [ {} for i in range(self.num_days) ]
        
        self.max_groups_per_room = ceil(len(self.groups)/self.num_rooms)
        
        self.avg_lectures_given = self.num_rooms * self.num_days / len(self.professors)
        self.ideal_lectures_given = floor( self.avg_lectures_given ), ceil( self.avg_lectures_given )
        
        
    # scoring methods
    
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
        
        
        
        self.score_repeat_lectures()
                
        self.score_num_of_prof_lectures()
        
        self.score_crowded_classroom()
        
                    
        if self.repeat_lecture_score > 1000:
            print("Prof repeat")
            
        if self.num_lectures_score > 1000:
            print("num lectures")
            
        if self.crowded_class_score > 1000:
            print("Crowded class")
                    
        self.score = self.repeat_lecture_score + self.num_lectures_score + self.crowded_class_score
               
               
    def score_crowded_classroom(self):
        crowded_class_score = 0
        multiplier = 0.2
        
        for day in self.days:
            for room in day.values():
                
                num_in_room = 0
                
                for group in room:
                    num_in_room += group.get_num_students()
                    if num_in_room > 35:
                        print("Far too many in a room")
                        sys.exit()
                        
                    
                if num_in_room > self.max_num_students_in_room:
                    crowded_class_score += int( (( num_in_room - self.max_num_students_in_room ) ** 2) * multiplier )
                    
        self.crowded_class_score = crowded_class_score
        return self.crowded_class_score
        
    def score_repeat_lectures(self):
        
        
        for group in self.groups:
            group.reset_lectures()
        
        for day in self.days:
            for prof_name, group_list in day.items():
                for group in group_list:
                    group.watch_lecture(prof_name)
                    
        prof_repeat_score = 0
        multiplier = 2
        
        for group in self.groups:
            for prof in self.professors:
                num = group.times_seen_professor( prof.get_name() )
                prof_repeat_score += int( ((num - 1) ** 2) * multiplier )
                
        self.repeat_lecture_score = prof_repeat_score
        
        return self.repeat_lecture_score
    
    def score_num_of_prof_lectures(self):
        
        for prof in self.professors:
            prof.reset_lectures()
        
        for day in self.days:
            for prof_name in day.keys():
                self.get_prof_by_name[ prof_name ].give_lecture()
                
                
        multiplier = 1
        num_lectures_score = 0
                
        for prof in self.professors:
            
            num = prof.get_days_lectured()
            
            if not num == self.ideal_lectures_given[0] and not num == self.ideal_lectures_given[1]:
                num_lectures_score += int( (( num - self.avg_lectures_given ) ** 2) * multiplier )
                
        self.num_lectures_score = num_lectures_score
        return self.num_lectures_score
        
    
                    
    # end scoring methods
    
    # randomization methods
    
    def swap_days(self, day1_index, day2_index):
        cp = self.days[day1_index]
        self.days[day1_index] = self.days[day2_index]
        self.days[day2_index] = cp
        
    def swap_professors(self, prof1, prof2, day):
        cp = day[prof1]
        day[prof1] = day[prof2]
        day[prof2] = cp
    
    def switch_in_professor(self, day, old_prof, new_prof):
        cp = day.pop(old_prof)
        day[new_prof] = cp
        
    def randomize_professors_day(self, day_number):
        
        day = self.days[day_number]
        
        groups_for_day = []
        profs_for_day = []
        iter_ls = list(day.keys())
        
        for prof_name in iter_ls:
            groups_for_day.append( day.pop(prof_name) )
            profs_for_day.append( prof_name )
            
        scramble_list( profs_for_day )
        
        for i in range(len(profs_for_day)):
            
            day[ profs_for_day[i] ] = groups_for_day[i]
    
    def randomize_groups_day(self, day_number):
        
        day = self.days[day_number]
        
        groups_for_day = []
        profs_for_day = []
        iter_ls = list(day.keys())
        
        for prof_name in iter_ls:
            
            groups = day.pop( prof_name )
            for group in groups:
                groups_for_day.append(group)
            
            profs_for_day.append( prof_name )
            
            day[prof_name] = []
            
        
        scramble_list( groups_for_day )
        
        done = False
        counter = 0
        for unused_var in range(self.max_groups_per_room):
            
            if done:
                break
            
            for room in day.values():
                
                room.append( groups_for_day[counter] )
                counter += 1
                
                if counter >= len(groups_for_day):
                    done = True
                    break
        
    def randomize_all_professors(self):
        """
        changes all days
        groups that were together will stay together
        """
        for day in self.days:
            
            prof_groups = []
            iter_ls = list(day.keys())
            
            for prof in iter_ls:
                prof_groups.append(day.pop(prof))
            
            scramble_list(self.professors)
            
            for i in range(self.num_rooms):
                day[ self.professors[i].get_name() ] = prof_groups[i]
            
    def randomize_all_groups(self):
        """
        changes all days
        the professors wont change 
        """
        
        for day in self.days:
            
            # resetting the professor's
            for prof in day.keys():  
                day[prof] = []
            
            # rescrambling
            scramble_list(self.groups)
            
            # re adding the groups
            done = False
            counter = 0            
            for unused_var in range(self.max_groups_per_room):
                
                if done:
                    break
                
                for room in day.values():  
                    
                    room.append( self.groups[counter] )
                    counter += 1
    
                    if counter >= len(self.groups):
                        done = True
                        break 
    
    def generate_random_schedule(self):
        
        
        # resetting the days
        self.days = [ {} for i in range(self.num_days) ]
        
        # we want to scramble all the days
        for day in self.days:
            
            scramble_list(self.professors)
            scramble_list(self.groups)
            
            # putting a professor in each room
            for room_num in range(self.num_rooms):
                prof = self.professors[room_num]
                day[ prof.get_name() ] = []
                prof.give_lecture()
                
            
            # Outer loop and counter account for multiple groups in one room
            exit_loop = False    
            counter = 0
            
            for unused_var in range( self.max_groups_per_room ):
                
                if exit_loop:
                    break
                
                for prof_name, room in day.items():
                    room.append(self.groups[counter])
                    self.groups[counter].watch_lecture( prof_name )
                    
                    counter += 1
                    
                    if counter >= len(self.groups):
                        exit_loop = True
                        break
                    
#         self.score_schedule()
            
    # end randomization methods
                
    # getters and setters            
    
    def get_score(self):
        return self.score
    
    # end getters and setters
    
    # magic methods
    
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
            result += "Day {:d}: {}\n".format(counter, day)
            counter += 1
            
        return result
    
    # end magic methods
    
    def copy(self):
        """
        Returns an unscored copy. Copies down to the groups and professors
        """
        
        # copying profs and groups to avoid pointer troubles
        profs_copy = []
        groups_copy = []
        
        # to make it easy to use the same copy
        # a bit of a convoluted solution but for now it works
        getting_group_by_name = {}
        
        for prof in self.professors:
            profs_copy.append( prof.copy() )
            
        for group in self.groups:
            cp = group.copy()
            groups_copy.append( cp )
            getting_group_by_name[ group.get_name() ] = cp
        
        cp = Schedule( profs_copy, groups_copy, self.num_days, self.num_rooms, self.max_num_students_in_room )
        
        
        for day_number in range(len(self.days)):
            
            for prof_name, group_list in self.days[day_number].items():
                
                if not prof_name in cp.days[day_number]:
                    cp.days[day_number][prof_name] = []
                    
                for group in group_list:
                    cp.days[day_number][prof_name].append( getting_group_by_name[ group.get_name() ] )
            
            
        
        return cp
                
                
    
class Group:
    def __init__( self, num_students, prof_name , all_profs):
        # has a number of students
        # constraints on days
        # what professors they have seen
        self.num_students = num_students
        self.prof_name = prof_name
        self.times_seen_profs = {}
        
        self.all_profs = all_profs # right now just so it is easy to copy
        
        self.reset_lectures()
    
    def times_seen_professor(self, prof_name):
        return self.times_seen_profs[prof_name]
    
    def get_num_students(self):
        return self.num_students
    
    def get_name(self):
        return self.prof_name
    
    def watch_lecture(self, prof_name):
        self.times_seen_profs[prof_name] += 1
        
    def unwatch_lecture(self, prof_name ):
        self.times_seen_profs[prof_name] -= 1
        
    def reset_lectures(self):
        for prof in self.all_profs:
            self.times_seen_profs[ prof.get_name() ] = 0
    
    def copy(self):
        return Group( self.num_students, self.prof_name, self.all_profs )
    
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
        
    def get_name(self):
        return self.name
        
    def give_lecture(self):
        self.num_days_lectured += 1
        
    def ungive_lecture(self):
        self.num_days_lectured -= 1
        
    def reset_lectures(self):
        self.num_days_lectured = 0
        
    def get_days_lectured(self):
        return self.num_days_lectured
        
    def copy(self):
        return Professor(self.name)
        
    def __str__(self):
        return "Professor: " + self.name
    
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
        
        
if __name__ == "__main__":
    
    testProf1 = Professor("Boutell")
    testProf2 = Professor("Aidoo")
    testProf3 = Professor("Song")
    testProf4 = Professor("DeVasher")
    testProf5 = Professor("Rupakheti")
    testProf6 = Professor("Coleman")
    testProf7 = Professor("Steve")
    testProf8 = Professor("John Doe")
    testProf9 = Professor("Jane Doe")
    
    testProfs = [testProf1, testProf2, testProf3, testProf4, testProf5, testProf6, testProf7, testProf8, testProf9]
    
    
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
    
    test.randomize_professors_day(2)
    
    print(test)
    


        