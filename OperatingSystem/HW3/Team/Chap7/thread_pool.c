#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>

#define THREADS 4
#define QUEUE_SIZE 10

typedef struct {
    void (*function)(void *);
    void *arg;
} task_t;

task_t queue[QUEUE_SIZE];
int front=0,rear=0,count=0;

pthread_mutex_t mutex;
pthread_cond_t not_empty;
pthread_cond_t not_full;

pthread_t workers[THREADS];
int shutdown_pool = 0;

void enqueue(task_t task){
    pthread_mutex_lock(&mutex);
    while(count == QUEUE_SIZE)
        pthread_cond_wait(&not_full,&mutex);

    queue[rear]=task;
    rear=(rear+1)%QUEUE_SIZE;
    count++;

    pthread_cond_signal(&not_empty);
    pthread_mutex_unlock(&mutex);
}

task_t dequeue(){
    pthread_mutex_lock(&mutex);
    while(count==0 && !shutdown_pool)
        pthread_cond_wait(&not_empty,&mutex);

    task_t task = queue[front];
    front=(front+1)%QUEUE_SIZE;
    count--;

    pthread_cond_signal(&not_full);
    pthread_mutex_unlock(&mutex);
    return task;
}

void* worker(void* arg){
    while(!shutdown_pool){
        task_t task = dequeue();
        task.function(task.arg);
    }
    return NULL;
}

void pool_submit(void (*function)(void*), void* arg){
    task_t task;
    task.function=function;
    task.arg=arg;
    enqueue(task);
}

// 測試任務
void job(void* arg){
    int num = *(int*)arg;
    printf("Thread %lu working on task %d\n",pthread_self(),num);
    sleep(1);
}

int main(){
    pthread_mutex_init(&mutex,NULL);
    pthread_cond_init(&not_empty,NULL);
    pthread_cond_init(&not_full,NULL);

    for(int i=0;i<THREADS;i++)
        pthread_create(&workers[i],NULL,worker,NULL);

    for(int i=0;i<20;i++){
        int *x = malloc(sizeof(int));
        *x=i;
        pool_submit(job,x);
    }

    sleep(5);
    shutdown_pool=1;
    pthread_cond_broadcast(&not_empty);

    for(int i=0;i<THREADS;i++)
        pthread_join(workers[i],NULL);
}