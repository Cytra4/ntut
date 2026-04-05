#!/bin/bash

echo show a for loop for bar fud 43 list
for foo in bar fud 43
do 
   echo $foo
done

echo -e "\nnow we will use shell expansion to put all files with the name sc"
echo  "      into my variable and then go through the list grepping for the = sign"

for myvar in $(ls *sc*)
# note the $( ) executes the subcommand and uses the result in place of that command

# it works the same way ` ` works below. 
do
  grep = $myvar /dev/null  
# note the /dev/null which makes grep think it is dealing with many files
# so it puts the filename at the beginning of its output
# I had wondered why it displayed my grep = line in the output
#     but that was only because the = is a hit on the search 
done

echo -e "\nNow we will use the shell expansion and a file test of -x" 
echo "  to check whether the file is executable" 
for myvar in `ls *sc*`
do
  if [[ -x $myvar ]] 
   then 
     echo $myvar
# if I only wanted to show the non executables, I could put
     #then 
     #        :
     # else
     # echo $myvar
  fi 
done

exit 0
