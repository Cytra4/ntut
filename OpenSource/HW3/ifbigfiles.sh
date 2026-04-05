#!/bin/bash

# Purpose: Use the find command to find any files in
# the root partition that have not been modified
# within the past n (any number within 30 days) days
# and are larger than 20 blocks(512-byte blocks)

if (( $# != 2 ))  # [ $# -ne 2 ]
then
        echo "Usage: $0 expects 2 parms min days and then bmin size" 1>&2
        exit 1
fi
if (( $1 < 0 || $1 > 30 ))
# [ $1 -lt 0 -o$1 -gt 30 ]
then
        echo "mdays is out of range"
        exit 2
fi

if (( $2 <= 20 ))       # [ $2 -lw 20 ]
then
        echo "size is out of range"
        exit 3
fi
# -xdev = do not search other partitiions
# -mtime =  n - # of days since file was modified
# -size = size of the fle in 512 -bytes blocks.
find ~ -xdev -mtime $1 -size +$2

