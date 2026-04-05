#!/bin/bash
# Scriptname: numm
num=0
while (( $num < $1 ))
do
        echo -n "$num"
        let num+=1
done
echo -e "\nAfter loop exits, continue running here"

