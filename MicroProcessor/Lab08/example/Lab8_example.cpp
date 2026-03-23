#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>
#include <unistd.h>
#include <fcntl.h>
#include <iostream>
#include <map>
#include <string>

using namespace std;

void setGPIO(unsigned int gpio, string status){
    int io;
    io = open("/dev/demo", O_WRONLY); //對/dev/demo 進行唯寫
    if (io < 0){
        perror("gpio error"); //開檔失敗
        return;
    }
    char buf[10] = {0};
    if (status == "on"){
        strcpy(buf, (to_string(gpio) + "1").c_str());
    }
    else{
        strcpy(buf, (to_string(gpio) + "0").c_str());
    }
    cout << buf << endl;
    write(io, buf, 5); //寫入
    close(io); //關檔
    return;
}

