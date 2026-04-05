#!/bin/bash
# Progra name: numberit
# Put line numbers on all lines of memo
if (( $# < 1 ))
then
        echo "Usage: $0 filename" ?&2
        exit 1
fi
count=1 #Initialize count
cat $1 | while read line
# Input is cming from file provded at command line
do
	(( count == 1 )) && echo "Processing $1..." > /dev/tty
         echo  -e "$count\t$line"
        let count+=1
done > tmp$$
mv tmp$$ $1

