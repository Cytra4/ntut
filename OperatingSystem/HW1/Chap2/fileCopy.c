#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <unistd.h>
#include <string.h>
#include <errno.h>

#define BUFFER_SIZE 1024

int main(){
    char srcName[256];
    char dstName[256];
    
    int srcFD;
    int dstFD;

    ssize_t bytesRead;
    ssize_t bytesWritten;

    char buf[BUFFER_SIZE];

    //讀要複製內容的檔案
    printf("Enter file name that you want to copy from: ");
    if (scanf("%255s", srcName) != 1){
        fprintf(stderr, "Error reading the source file \n");
        exit(EXIT_FAILURE);
    }

    //讀要貼上內容的檔案
    printf("Enter file name that you want to copy to: ");
    if (scanf("%255s", dstName) != 1){
        fprintf(stderr, "Error reading the destination file \n");
        exit(EXIT_FAILURE);
    }

    srcFD = open(srcName, O_RDONLY);
    if (srcFD == -1){
        fprintf(
            stderr, "Cannot open source file: '%s' \n",
            srcName
        );
        exit(EXIT_FAILURE);
    }

    dstFD = open(
        dstName,
        O_WRONLY | O_CREAT | O_TRUNC,
        0644
    );

    if (dstFD == -1){
        fprintf(stderr, "Cannot open destination file: '%s' \n",
            dstName
        );
    }

    //讀資料
    while ((bytesRead = read(srcFD, buf, BUFFER_SIZE)) > 0){
        bytesWritten = write(dstFD, buf, bytesRead);

        if (bytesWritten == -1){
            fprintf(stderr, "Error writing to destination file: %s \n",
                strerror(errno)
            );
            close(srcFD);
            close(dstFD);
            exit(EXIT_FAILURE);
        }
    }

    if (bytesRead == -1){
        fprintf(stderr, "Error reading from source file: %s \n",
            strerror(errno)
        );
        close(srcFD);
        close(dstFD);
        exit(EXIT_FAILURE);
    }

    if (close(srcFD) == -1) {
        fprintf(stderr, "Error closing source file: %s\n", strerror(errno));
        exit(EXIT_FAILURE);
    }

    if (close(dstFD) == -1) {
        fprintf(stderr, "Error closing destination file: %s\n", strerror(errno));
        exit(EXIT_FAILURE);
    }

    printf("File copied successfully \n");
    
    return 0;
}