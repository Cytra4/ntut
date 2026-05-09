#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>
#include <time.h>

#define CUSTOMERS 5
#define RESOURCES 3

int available[RESOURCES];
int max[CUSTOMERS][RESOURCES];
int allocation[CUSTOMERS][RESOURCES];
int need[CUSTOMERS][RESOURCES];

pthread_mutex_t mutex;

void print_state(){
    printf("\n===== SYSTEM STATE =====\n");
    printf("Available: ");
    for(int i=0;i<RESOURCES;i++) printf("%d ",available[i]);
    printf("\n");
}

int safety_check(){
    int work[RESOURCES];
    int finish[CUSTOMERS]={0};

    for(int i=0;i<RESOURCES;i++)
        work[i]=available[i];

    int progress;
    do{
        progress=0;
        for(int i=0;i<CUSTOMERS;i++){
            if(!finish[i]){
                int possible=1;
                for(int j=0;j<RESOURCES;j++)
                    if(need[i][j] > work[j]) possible=0;

                if(possible){
                    for(int j=0;j<RESOURCES;j++)
                        work[j]+=allocation[i][j];
                    finish[i]=1;
                    progress=1;
                }
            }
        }
    }while(progress);

    for(int i=0;i<CUSTOMERS;i++)
        if(!finish[i]) return 0;
    return 1;
}

int request_resources(int id,int req[]){
    pthread_mutex_lock(&mutex);

    printf("Customer %d requesting: ",id);
    for(int i=0;i<RESOURCES;i++) printf("%d ",req[i]);
    printf("\n");

    for(int i=0;i<RESOURCES;i++)
        if(req[i] > need[id][i]){
            pthread_mutex_unlock(&mutex);
            return -1;
        }

    for(int i=0;i<RESOURCES;i++)
        if(req[i] > available[i]){
            printf("→ Denied (not enough resources)\n");
            pthread_mutex_unlock(&mutex);
            return -1;
        }

    for(int i=0;i<RESOURCES;i++){
        available[i] -= req[i];
        allocation[id][i] += req[i];
        need[id][i] -= req[i];
    }

    if(!safety_check()){
        for(int i=0;i<RESOURCES;i++){
            available[i] += req[i];
            allocation[id][i] -= req[i];
            need[id][i] += req[i];
        }
        printf("→ Denied (unsafe state)\n");
        pthread_mutex_unlock(&mutex);
        return -1;
    }

    printf("→ GRANTED\n");
    print_state();
    pthread_mutex_unlock(&mutex);
    return 0;
}

void release_resources(int id,int rel[]){
    pthread_mutex_lock(&mutex);

    printf("Customer %d releasing: ",id);
    for(int i=0;i<RESOURCES;i++) printf("%d ",rel[i]);
    printf("\n");

    for(int i=0;i<RESOURCES;i++){
        available[i] += rel[i];
        allocation[id][i] -= rel[i];
        need[id][i] += rel[i];
    }

    print_state();
    pthread_mutex_unlock(&mutex);
}

void* customer(void* arg){
    int id = *(int*)arg;

    while(1){
        int req[RESOURCES];

        for(int i=0;i<RESOURCES;i++)
            req[i] = rand() % (need[id][i] + 1);

        if(request_resources(id,req) == 0){
            sleep(rand()%3 + 1);
            release_resources(id,req);
        }

        sleep(rand()%3 + 1);
    }
}

int main(int argc,char* argv[]){
    if(argc != RESOURCES+1){
        printf("Usage: ./banker r1 r2 r3\n");
        return 0;
    }

    srand(time(NULL));
    pthread_mutex_init(&mutex,NULL);

    printf("Initializing system...\n");

    for(int i=0;i<RESOURCES;i++)
        available[i] = atoi(argv[i+1]);
        
    for(int i=0;i<CUSTOMERS;i++)
        for(int j=0;j<RESOURCES;j++){
            max[i][j] = rand() % (available[j] + 1);
            allocation[i][j] = 0;
            need[i][j] = max[i][j];
        }

    print_state();

    pthread_t tid[CUSTOMERS];
    int ids[CUSTOMERS];

    for(int i=0;i<CUSTOMERS;i++){
        ids[i]=i;
        pthread_create(&tid[i],NULL,customer,&ids[i]);
    }

    for(int i=0;i<CUSTOMERS;i++)
        pthread_join(tid[i],NULL);
}