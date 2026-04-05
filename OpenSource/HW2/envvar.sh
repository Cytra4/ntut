#!/bin/bash
echo \$home is $HOME
echo \$PATH is $PATH
echo \$PS1 which is my prompt is $PS1
echo \$# is number of parms: $#
echo \$\$ is PID of this shell $$
echo I could create a file called /tmp/tmpfile_$$
echo \$0 is $0
echo \$1 is $1
echo \$2 is $2
echo \$3 is $3
echo \$4 is $4
echo "\$IFS is "
echo "START $IFS END"
echo '$@' is "$@"
echo '"$*"' is $*
echo '$*' is "$*"
IFS=':'
echo "\$IFS is "
echo "START $IFS END"
echo '$@' is $@
echo '"$@"' is "$@"
echo '$*' is $*
echo '"$*"' is "$*"
echo loop through '$@' no quotes
for i in $@
do
echo $i
done
echo loop through '"@"' with quotes
for i in "$@"
do
echo $i
done

echo loop through '$*' no quotes
for i in $*
do
echo $i
done
echo loop through '"$*"' with quotes
for i in "$*"
do
echo $i
done

unset IFS
set $(date)
echo "I reset parms to the date, so now the parms are "
for i in $*
do 
echo $i 
done
