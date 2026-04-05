#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/wait.h>

void collatz_conjecture(int n) {
    while (n != 1) {
        printf("%d ", n);
        if (n % 2 == 0) {
            n /= 2;
        } else {
            n = 3*n+1;
        }
    }
    printf("1\n");
}

int main(int argc, char *argv[]) {
    if (argc != 2) {
        fprintf(stderr, "Error: Please enter a integer \n");
        return 1;
    }

    int n = atoi(argv[1]);
    if (n <= 0) {
        fprintf(stderr, "Error: Please enter a positive integer.\n");
        return 1;
    }

    pid_t pid = fork();

    if (pid < 0) {
        // Fork failed
        perror("Fork");
        return 1;
    } else if (pid == 0) {
        // Child process
        printf("Collatz sequence: ");
        collatz_conjecture(n);
        exit(0);
    } else {
        // Parent process
        wait(NULL); // Wait for the child process to complete
        printf("Child process completed.\n");
    }

    return 0;
}