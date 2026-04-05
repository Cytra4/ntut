#!/bin/bash
#Scriptname: nosy

 
# demonstrate reading entire line into a variable name
answer="no answer because no question asked"
echo "\$answer is currently $answer"
echo  "Are you happy?"
read answer
echo "$answer is the right response."

# demonstrate reading up to space and then reading the rest of the line 
# and show how -e enable interpretation of the following backslash escapes 
#    \c will suppress going to a new line to keep the cursor on the same line
echo -e "What is your full name?(include middle initial) \c"
read first middle last
echo "Hello, $first is a  good first name and $last is a good last name"

# demonstrate how reply is the variable name used when none is given
# also -n to not keep the cursor on the same line
echo -n "Where do you work? "
read
echo I guess $REPLY keeps you busy!

#demonstrate read -p parm which prints a title
read -p "Enter your job title: "
echo "I though you might be an $REPLY."
#echo - "Who are your best friends? "
read -p "Who are your 3 best friends: " -a friends
echo "Say hi to ${friends[2]}."
answer="not happy anymore"
echo "OH NO! \$answer now changed to $answer"

#demonstrate using a here file to display lots of text:
cat  << HERE 
    This is a lot of talk about happy or not
    But we still have no if statement
    or loop. 
HERE
