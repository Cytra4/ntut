#!/bin/bash
# Scriptname: dater
# Purpose: set positional parameters with the set
# command and shift through the parameters
set $(date)
echo $*
echo done showing the parms
while (( $# > 0 ))
do
        echo $1
        shift
done

