#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>
#include <fcntl.h>

pthread_mutex_t lock = PTHREAD_MUTEX_INITIALIZER;

int Counter = 0;

// 這邊的pin可以改，這邊用pin7
void gpio_write(int value)
{
    int fd = open("/sys/class/gpio/gpio396/value", O_WRONLY);
    if (fd < 0)
        return;
    write(fd, value ? "1" : "0", 1);
    close(fd);
}

typedef struct{
    const char *name;
}ThreadArg;

void *AddCount(void *arg)
{
    ThreadArg *targ = (ThreadArg*)arg;
    for (int i = 0; i < 10000; i++)
    {
        pthread_mutex_lock(&lock);

        printf("%s Enter Critical Section: %d \n",targ->name, i);
        gpio_write(1);
        Counter++;
        gpio_write(0);
        //usleep(50);
        usleep(500000);

        pthread_mutex_unlock(&lock);
    }
    return NULL;
}

int main()
{
    pthread_t t1, t2;

    ThreadArg arg1 = { .name = "T1" };
    ThreadArg arg2 = { .name = "T2" };

    pthread_create(&t1, NULL, AddCount, &arg1);
    pthread_create(&t2, NULL, AddCount, &arg2);
    
    pthread_join(t1, NULL);
    pthread_join(t2, NULL);
    
    printf("Final Counter: %d", Counter);
    return 0;
}
