#!/bin/bash

echo "please enter a number "
read n1
echo "please enter a number "
read n2
echo $((n2))
if (( $n1 < $n2 ))
then
  echo "$n1 is less than $n2"
elif (( $n2 < $n1)) ; then
  echo "$n2 is less than $n1"
elif (( $n2 == $n1 )) ; then 
  echo "you entered equal numbers "
else
  echo "the numbers you entered were not recognized"
  exit 3
fi

exit 4
