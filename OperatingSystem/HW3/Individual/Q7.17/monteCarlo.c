#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <time.h>

#define NUM_THREADS 4
#define POINTS_PER_THREAD 1000000

long long circle_count = 0;
pthread_mutex_t mutex;

void *worker(void *arg) {
    long long local_count = 0;
    unsigned int seed = time(NULL) ^ pthread_self();

    for(long long i=0;i<POINTS_PER_THREAD;i++){
        double x = rand_r(&seed) / (double)RAND_MAX;
        double y = rand_r(&seed) / (double)RAND_MAX;

        if(x*x + y*y <= 1.0)
            local_count++;
    }

    // critical section
    pthread_mutex_lock(&mutex);
    circle_count += local_count;
    pthread_mutex_unlock(&mutex);

    pthread_exit(NULL);
}

int main(){
    pthread_t tid[NUM_THREADS];
    pthread_mutex_init(&mutex,NULL);

    for(int i=0;i<NUM_THREADS;i++)
        pthread_create(&tid[i],NULL,worker,NULL);

    for(int i=0;i<NUM_THREADS;i++)
        pthread_join(tid[i],NULL);

    long long total_points = NUM_THREADS * POINTS_PER_THREAD;
    double pi = 4.0 * circle_count / total_points;

    printf("Estimated PI = %f\n",pi);

    pthread_mutex_destroy(&mutex);
    return 0;
}