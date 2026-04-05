#!/bin/bash

# demonstrate using numbers

echo  See it do math into a non-number variable
newvar=1
num=3+$newvar
echo "   3+\$newvar = $num"

echo -e "\nNow try with parentheses"
num=$((3+$newvar))
echo "   3+\$newvar = $num"


echo -e "\nNow see it do the same math when the result variable"
echo   "    is declared as a number"
declare -i num
num=3+$newvar
echo "   3+\$newvar = $num"

echo -e "\nAnd if I try to put letters into my int"
num=hello
echo "     When I set an integer to a letter, it made \$num $num"

echo -e "\nSee how declare -i shows you all variables of i type"
declare -i

declare -i n
echo -e "\nNow see how to set a binary number into your int"
echo "        setting using n=2#101"
n=2#101
echo "n is now $n"

echo -e "\nNow see how to set a hex number into your int"
echo "         setting using n=16#1F"
n=16#1F
echo "n is now $n"

echo -e "\nNow see how to set an octal number into your int"
echo "      setting using n=16#1F"
n=16#1F
echo "n is now $n"

echo -e "\nNow see how to set an octal number into your int"
echo "     setting using n=017 which is 17 with a leading zero"
n=017
echo "n is now $n"

echo -e "\nNow see how to set a decimal number into your int"
echo "    just removed the leading zero"
echo "    setting using n=17"
n=17
echo "n is now $n"
