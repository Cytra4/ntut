#include <stdio.h>
#include <unistd.h>
#include <fcntl.h>
#include <string.h>
#include <stdio.h>

int main()
{
	char buf[1024] = "Data Input 123456 hello world";
	// FILE *fp = fopen("/dev/demo", "w+");
	//  if(fp == NULL){
	//  	printf("can't open device!\n");
	//  	return 0;
	//  }
	//  fwrite(buf, sizeof(buf), 1, fp);
	//  fread(buf, sizeof(buf), 1, fp);
	//  fclose(fp);

	int fd = open("/dev/demo", O_RDWR);
	if (fd < 0)
	{
		perror("open");
		return 0;
	}

	write(fd, buf, strlen(buf));
	read(fd, buf, sizeof(buf));
	close(fd);
	return 0;
}