#!/bin/bash

read -p "Enter your name: " name
read -p "Enter your age: " age

if (( age > 0 && age < 150)); then
	echo "Hello, $name! You are $age years old."
	echo "You will retire in $((65 - age)) years!"
else
	echo "Error: You have entered an invalid age, please try again."
	exit 1
fi
