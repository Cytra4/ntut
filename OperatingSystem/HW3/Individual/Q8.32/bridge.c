#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <semaphore.h>
#include <unistd.h>

sem_t bridge;
pthread_mutex_t mutex;

int north_count=0;
int south_count=0;

void cross_bridge(char *dir){
    printf("%s farmer entering bridge\n",dir);
    sleep(rand()%3+1);
    printf("%s farmer leaving bridge\n",dir);
}

void *north_farmer(void *arg){
    pthread_mutex_lock(&mutex);
    north_count++;
    int first = (north_count == 1);
    pthread_mutex_unlock(&mutex);

    if(first) sem_wait(&bridge);

    cross_bridge("North");

    pthread_mutex_lock(&mutex);
    north_count--;
    int last = (north_count == 0);
    pthread_mutex_unlock(&mutex);

    if(last) sem_post(&bridge);
    pthread_exit(NULL);
}

void *south_farmer(void *arg){
    pthread_mutex_lock(&mutex);
    south_count++;
    int first = (south_count == 1);
    pthread_mutex_unlock(&mutex);

    if(first) sem_wait(&bridge);

    cross_bridge("South");

    pthread_mutex_lock(&mutex);
    south_count--;
    int last = (south_count == 0);
    pthread_mutex_unlock(&mutex);

    if(last) sem_post(&bridge);
    pthread_exit(NULL);
}

int main(){
    pthread_t t[10];
    sem_init(&bridge,0,1);
    pthread_mutex_init(&mutex,NULL);

    for(int i=0;i<5;i++){
        pthread_create(&t[i],NULL,north_farmer,NULL);
        pthread_create(&t[i+5],NULL,south_farmer,NULL);
    }

    for(int i=0;i<10;i++)
        pthread_join(t[i],NULL);

    return 0;
}