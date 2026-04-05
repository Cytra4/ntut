#!/bin/sh

myvar="Hi there"

echo $myvar
echo "$myvar"
echo '$myvar'
echo \$myvar

echo $0
echo "$0"
echo '$0'
echo \$0

echo date
echo "date"
echo 'date'
echo \date
echo `date`

echo Enter some text
read myvar

echo $myvar
exit 0
