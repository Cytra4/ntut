#!/bin/bash

echo "Is it morning? Please answer yes or no "
read timeofday

if [  "$timeofday" = "yes"  -o  "$timeofday" = "yeah" ]
then
  echo "good morning"
elif [[ "$timeofday" = [Nn]* ]] ; then
  echo "good afternoon"
else 
  echo "sorry \$timeofday not recognized. Enter yes or no "
  exit 1
fi
exit 0
