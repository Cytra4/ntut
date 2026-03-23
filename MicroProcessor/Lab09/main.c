#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>
#include <fcntl.h>

pthread_mutex_t lock = PTHREAD_MUTEX_INITIALIZER;
int workers_done = 0;

//這邊的pin可以改，這邊用pin15
void gpio_write(int value) {
    int fd = open("/sys/class/gpio/gpio255/value", O_WRONLY);
    if (fd < 0) return;
    write(fd, value ? "1" : "0", 1);
    close(fd);
}

void* WorkerT1(void* arg) {
    printf("T1 Started\n");

    int *result = malloc(sizeof(int));
    *result = 0;

    for (int i = 1; i <= 25000000; i++){
        (*result)++;
        if (i % 100000 == 0)
            usleep(2000);
    }

    printf("T1 Finished, res:%d\n", *result);

    pthread_mutex_lock(&lock);
    workers_done++;
    pthread_mutex_unlock(&lock);

    pthread_exit(result);
}

void* WorkerT2(void* arg) {
    printf("T2 Started\n");

    int *result = malloc(sizeof(int));
    *result = 0;

    for (int i = 25000001; i <= 50000000; i++){
        (*result)++;
        if (i % 100000 == 0)
            usleep(2000);
    }

    printf("T2 Finished, res:%d\n", *result);

    pthread_mutex_lock(&lock);
    workers_done++;
    pthread_mutex_unlock(&lock);

    pthread_exit(result);
}

void* MonitorT3(void* arg) {
    int led = 0;

    printf("T3 Started\n");

    while (1) {
        pthread_mutex_lock(&lock);
        if (workers_done == 2) {
            pthread_mutex_unlock(&lock);
            break;
        }
        pthread_mutex_unlock(&lock);

        led = !led;
        gpio_write(led);
        usleep(500000);
    }

    gpio_write(1);
    printf("T3 Finished\n");
    pthread_exit(NULL);
}

int main() {
    pthread_t t1, t2, t3;
    int *retvalue;
    int total = 0;

    pthread_create(&t3, NULL, MonitorT3, NULL);
    pthread_create(&t1, NULL, WorkerT1, NULL);
    pthread_create(&t2, NULL, WorkerT2, NULL);

    pthread_join(t1, (void**)&retvalue);
    total += *retvalue;
    free(retvalue);

    pthread_join(t2, (void**)&retvalue);
    total += *retvalue;
    free(retvalue);

    pthread_join(t3, NULL);

    printf("Total = %d\n", total);
    return 0;
}
