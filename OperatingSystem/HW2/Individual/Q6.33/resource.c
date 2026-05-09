#include <stdio.h>
#include <pthread.h>
#include <semaphore.h>
#include <unistd.h>

#define MAX_RESOURCES 5

int available_resources = MAX_RESOURCES;
sem_t sem;

int decrease_count(int count) {
    for (int i = 0; i < count; i++)
        sem_wait(&sem);

    return 0;
}

int increase_count(int count) {
    for (int i = 0; i < count; i++)
        sem_post(&sem);

    return 0;
}

void* worker(void* arg) {
    int id = *(int*)arg;

    printf("Thread %d requesting 2 resources\n", id);
    decrease_count(2);

    printf("Thread %d got resources\n", id);
    sleep(2);

    printf("Thread %d releasing resources\n", id);
    increase_count(2);

    pthread_exit(NULL);
}

int main() {
    pthread_t t[3];
    int id[3] = {1,2,3};

    sem_init(&sem, 0, MAX_RESOURCES);

    for(int i=0;i<3;i++)
        pthread_create(&t[i], NULL, worker, &id[i]);

    for(int i=0;i<3;i++)
        pthread_join(t[i], NULL);

    sem_destroy(&sem);
    return 0;
}