#!/bin/bash
# Scriptname: runit

#  PS3 is the environment variable connected only to the shell 
#     select statement
#     Select makes a menu
#      PS3 holds the prompt for the menu option
#      Select header holds a list
#      do / done are like the curly braces of the select
#      The choice made with the number returns the corresponding
#          word to the program
#
echo \$PS3 = start$PS3 End
PS3="Select a progam to execute: "
select program in 'ls -F' pwd date last exit
do
        if [[ $program = exit ]]
        then 
           break
        else
           $program
# note that the $program executes a program because the contents of the $ program variable get translated and run. The contents are a program name
#   so it is nothing special having to do with select. 
      fi
done

echo "done with select now"
