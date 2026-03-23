#include "stdio.h"
#include "stdlib.h"
#include "string.h"
#include "errno.h"
#include "unistd.h"
#include "fcntl.h"
#include "iostream"

using namespace std;

int gpio_export(unsigned int gpio)
{
	int  fd, len;
	char buf[64];
	fd = open("/sys/class/gpio/export", O_WRONLY);
	if (fd < 0){
		perror("gpio/export");
		return fd;
	}
	
	len = snprintf(buf, sizeof(buf), "%d", gpio);
	write(fd, buf, len);
	close(fd);
	return 0;
}
