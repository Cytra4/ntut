#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <time.h>

#define NUM_THREADS 4
#define TOTAL_POINTS 1000000

long points_in_circle = 0;
pthread_mutex_t mutex;

void* monte_carlo(void* arg) {
    long local_count = 0;
    int points_per_thread = TOTAL_POINTS / NUM_THREADS;

    unsigned int seed = time(NULL) ^ pthread_self();

    for (int i = 0; i < points_per_thread; i++) {
        double x = ((double)rand_r(&seed) / RAND_MAX) * 2.0 - 1.0;
        double y = ((double)rand_r(&seed) / RAND_MAX) * 2.0 - 1.0;

        if (x*x + y*y <= 1.0)
            local_count++;
    }

    // critical section
    pthread_mutex_lock(&mutex);
    points_in_circle += local_count;
    pthread_mutex_unlock(&mutex);

    pthread_exit(NULL);
}

int main() {
    pthread_t threads[NUM_THREADS];
    pthread_mutex_init(&mutex, NULL);

    for (int i = 0; i < NUM_THREADS; i++)
        pthread_create(&threads[i], NULL, monte_carlo, NULL);

    for (int i = 0; i < NUM_THREADS; i++)
        pthread_join(threads[i], NULL);

    double pi = 4.0 * points_in_circle / TOTAL_POINTS;
    printf("Estimated PI = %f\n", pi);

    pthread_mutex_destroy(&mutex);
    return 0;
}