#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>

#define SIZE 20

int arr[SIZE] = {29,10,14,37,13,5,2,8,19,1,50,42,23,9,17,4,6,30,11,7};
int sorted[SIZE];

typedef struct {
    int start;
    int end;
} parameters;

int compare(const void* a, const void* b){
    return (*(int*)a - *(int*)b);
}

void* sort_thread(void* param){
    parameters *p = (parameters*)param;
    qsort(&arr[p->start], p->end - p->start + 1, sizeof(int), compare);
    pthread_exit(NULL);
}

void* merge_thread(void* param){
    int mid = SIZE/2;
    int i=0, j=mid, k=0;

    while(i < mid && j < SIZE){
        if(arr[i] < arr[j]) sorted[k++] = arr[i++];
        else sorted[k++] = arr[j++];
    }
    while(i < mid) sorted[k++] = arr[i++];
    while(j < SIZE) sorted[k++] = arr[j++];

    pthread_exit(NULL);
}

int main(){
    pthread_t t1, t2, t3;

    parameters left = {0, SIZE/2 -1};
    parameters right = {SIZE/2, SIZE-1};

    printf("Original array:\n");
    for(int i=0;i<SIZE;i++) printf("%d ", arr[i]);
    printf("\n");

    pthread_create(&t1,NULL,sort_thread,&left);
    pthread_create(&t2,NULL,sort_thread,&right);
    pthread_join(t1,NULL);
    pthread_join(t2,NULL);

    pthread_create(&t3,NULL,merge_thread,NULL);
    pthread_join(t3,NULL);

    printf("\nSorted array:\n");
    for(int i=0;i<SIZE;i++) printf("%d ", sorted[i]);
    printf("\n");
}