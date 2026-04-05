#!/bin/bash

# this function returns a value and does the real yes/no job

yes_or_no() {
	echo "Is your name $1 ?"
	while true
	do
		echo -n "Enter yes or no: "
		read name
		case "$name" in  
			y | yes ) return 0 ;; # 0 is good
			n | no ) return 1 ;;
			* ) echo "answer yes or no"
		esac
	done
}

echo "original parameters are $*"

if yes_or_no $1
	then 
	echo "Hi, $1, nice name"
else
	echo "Never mind"
fi
exit 0
