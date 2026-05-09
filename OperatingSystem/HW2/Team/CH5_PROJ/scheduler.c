#include <stdio.h>
#include <stdlib.h>

#define N 5
#define QUANTUM 2

typedef struct {
    int pid;
    int burst;
    int priority;
    int waiting;
    int turnaround;
    int remaining;
} Process;

void printResult(Process p[]){
    double avgW=0, avgT=0;
    printf("PID\tWT\tTAT\n");
    for(int i=0;i<N;i++){
        printf("P%d\t%d\t%d\n",p[i].pid,p[i].waiting,p[i].turnaround);
        avgW+=p[i].waiting;
        avgT+=p[i].turnaround;
    }
    printf("Avg WT=%.2f  Avg TAT=%.2f\n\n",avgW/N,avgT/N);
}

//FCFS
void FCFS(Process p[]){
    int time=0;
    for(int i=0;i<N;i++){
        p[i].waiting=time;
        time+=p[i].burst;
        p[i].turnaround=time;
    }
    printf("=== FCFS ===\n");
    printResult(p);
}

//SJF
int cmpBurst(const void* a,const void* b){
    return ((Process*)a)->burst - ((Process*)b)->burst;
}

void SJF(Process p[]){
    qsort(p,N,sizeof(Process),cmpBurst);
    int time=0;
    for(int i=0;i<N;i++){
        p[i].waiting=time;
        time+=p[i].burst;
        p[i].turnaround=time;
    }
    printf("=== SJF ===\n");
    printResult(p);
}

//Priority-based
int cmpPriority(const void* a,const void* b){
    return ((Process*)b)->priority - ((Process*)a)->priority;
}

void PrioritySched(Process p[]){
    qsort(p,N,sizeof(Process),cmpPriority);
    int time=0;
    for(int i=0;i<N;i++){
        p[i].waiting=time;
        time+=p[i].burst;
        p[i].turnaround=time;
    }
    printf("=== Priority ===\n");
    printResult(p);
}

//Round-Robin
void RoundRobin(Process p[]){
    int time=0, done;
    for(int i=0;i<N;i++) p[i].remaining=p[i].burst;

    do{
        done=1;
        for(int i=0;i<N;i++){
            if(p[i].remaining>0){
                done=0;
                if(p[i].remaining>QUANTUM){
                    time+=QUANTUM;
                    p[i].remaining-=QUANTUM;
                }else{
                    time+=p[i].remaining;
                    p[i].waiting=time-p[i].burst;
                    p[i].remaining=0;
                    p[i].turnaround=time;
                }
            }
        }
    }while(!done);

    printf("=== Round Robin ===\n");
    printResult(p);
}

//Priority with Round-Robin
void PriorityRR(Process p[]){
    qsort(p,N,sizeof(Process),cmpPriority);
    RoundRobin(p);
}

int main(){
    Process original[N]={
        {1,10,3,0,0,0},
        {2,1,1,0,0,0},
        {3,2,4,0,0,0},
        {4,1,5,0,0,0},
        {5,5,2,0,0,0}
    };

    Process temp[N];

    temp[0]=original[0]; temp[1]=original[1]; temp[2]=original[2]; temp[3]=original[3]; temp[4]=original[4];
    FCFS(temp);

    temp[0]=original[0]; temp[1]=original[1]; temp[2]=original[2]; temp[3]=original[3]; temp[4]=original[4];
    SJF(temp);

    temp[0]=original[0]; temp[1]=original[1]; temp[2]=original[2]; temp[3]=original[3]; temp[4]=original[4];
    PrioritySched(temp);

    temp[0]=original[0]; temp[1]=original[1]; temp[2]=original[2]; temp[3]=original[3]; temp[4]=original[4];
    RoundRobin(temp);

    temp[0]=original[0]; temp[1]=original[1]; temp[2]=original[2]; temp[3]=original[3]; temp[4]=original[4];
    PriorityRR(temp);
}