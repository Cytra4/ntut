#!/bin/bash

if [ $# -lt 1 ]; then
    echo "Usage: $0 {add|list|remove}"
    exit 1
fi

action=$1

case $action in

add)
    echo "Current scheduled tasks:"
    atq

    echo
    read -p "Enter task description: " description
    read -p "Enter execution time (e.g., now + 1 minute): " time

    echo "Scheduling task..."

    echo "$description" | at "$time"

    if [ $? -eq 0 ]; then
        echo "Task successfully added."
    else
        echo "Failed to add task."
    fi
    ;;

list)
    echo "Listing all scheduled tasks:"
    atq
    ;;

remove)
    echo "Current scheduled tasks:"
    atq

    echo
    read -p "Enter task number to remove: " job_id

    atrm "$job_id"

    if [ $? -eq 0 ]; then
        echo "Task $job_id removed successfully."
    else
        echo "Failed to remove task."
    fi
    ;;

*)
    echo "Invalid option."
    echo "Usage: $0 {add|list|remove}"
    exit 1
    ;;

esac
