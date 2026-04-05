#!/bin/bash

if (( $# != 2 ))
then
    echo "Usage: $0 source_directory destination_directory"
    exit 1
fi

src=$1
dest=$2

if [[ ! -d "$src" ]]
then
    echo "Error: Source directory does not exist"
    exit 1
fi

if [[ ! -d "$dest" ]]
then
    echo "Destination does not exist. Creating directory..."
    mkdir -p "$dest"
fi

TIMESTAMP=$(date +%Y%m%d_%H%M%S)

backup_file="$dest/backup_${TIMESTAMP}.tar.gz"

cleanup() {
    echo
    echo "Backup interrupted. Removing partial backup file..."
    rm -f "$backup_file"
    exit 1
}

trap cleanup INT

echo "Starting backup..."

tar -czf "$backup_file" "$src"

trap - INT

echo "Backup completed successfully!"
echo

echo "Backup files in $dest:"
ls -l "$dest
