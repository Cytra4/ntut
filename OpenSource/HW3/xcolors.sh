#!/bin/bash
# Scriptname: xcolors

echo -n "Choose a foreground color for your xterm window:"
read color

case "$color" in
[Bb]l??)
        xterm -fg blue -fn terminal &
        echo trying to change to blue
        ;;
[Gg]ree*)
        xterm -fg darkgreen -fn terminal &
        echo trying to change to green
        ;;
red | orange)   # | means "or"
        xterm -fg "$color" -fn terminal &
        echo trying to change to red or orange;;
*)
        xterm -fn terminal
        echo no color change
        ;;
esac
echo "Out of case command"

