#!/bin/sh

# demo the trap of an interrupting signal

trap 'rm -f /tmp/my_tmp_file_$$' INT
echo creating file /tmp/my_tmp_file_$$
date > /tmp/my_tmp_file_$$

echo "press control c to interrupt"
while [ -f /tmp/my_tmp_file_$$ ]; do 
    echo File exikts
    sleep 1
done
echo The file no longer exists
