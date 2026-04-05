#!/bin/bash

# this function returns a value

yes_or_no() {
   echo "The variables this yes or no function can access are $* ?"
   return 0
   echo "we never get here"
   }

echo "original parameters are $*"

if yes_or_no $2 word
then 
    echo "true path and The parameter variables are now $*"
else
    echo "false path and the parameter variables are now $*"
fi
exit 0
