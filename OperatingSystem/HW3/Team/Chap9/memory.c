#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct block{
    int start;
    int size;
    int free;
    struct block *next;
} block;

block *head=NULL;
int MAX_MEM;

void init_memory(int size){
    head=malloc(sizeof(block));
    head->start=0;
    head->size=size;
    head->free=1;
    head->next=NULL;
}

void request(int size){
    block *cur=head;
    while(cur){
        if(cur->free && cur->size>=size){
            block *new = malloc(sizeof(block));
            new->start=cur->start+size;
            new->size=cur->size-size;
            new->free=1;
            new->next=cur->next;

            cur->size=size;
            cur->free=0;
            cur->next=new;
            printf("Allocated at %d\n",cur->start);
            return;
        }
        cur=cur->next;
    }
    printf("Allocation failed\n");
}

void release(int addr){
    block *cur=head;
    while(cur){
        if(cur->start==addr){
            cur->free=1;
            printf("Released %d\n",addr);
            return;
        }
        cur=cur->next;
    }
}

void report(){
    block *cur=head;
    while(cur){
        printf("[%d-%d] %s\n",
        cur->start,cur->start+cur->size-1,
        cur->free?"Free":"Allocated");
        cur=cur->next;
    }
}

int main(){
    init_memory(1024);
    request(100);
    request(200);
    release(0);
    report();
}