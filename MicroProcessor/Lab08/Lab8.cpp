// Lab8.cpp
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

//LED PIN要改
static map<string, unsigned int> led_map = {
    {"LED1", 17},
    {"LED2", 18},
    {"LED3", 19},
};

void setGPIO_by_number(unsigned int gpio, int on_off) {
    int io = open("/dev/demo", O_WRONLY);
    if (io < 0){
        perror("open /dev/demo");
        return;
    }
    char buf[64];
    int len = snprintf(buf, sizeof(buf), "%u:%d", gpio, on_off);
    if (len <= 0) { close(io); return; }
    if (write(io, buf, len) != len) {
        perror("write to /dev/demo");
    }
    close(io);
}

int getGPIO_by_number(unsigned int gpio) {
    /* send GET command, then read response */
    int io = open("/dev/demo", O_RDWR);
    if (io < 0) {
        perror("open /dev/demo");
        return -1;
    }
    char cmd[32];
    int cmdlen = snprintf(cmd, sizeof(cmd), "GET %u\n", gpio);
    if (write(io, cmd, cmdlen) != cmdlen) {
        perror("write GET");
        close(io);
        return -1;
    }

    /* read response */
    char rbuf[64];
    ssize_t r = read(io, rbuf, sizeof(rbuf)-1);
    if (r < 0) {
        perror("read");
        close(io);
        return -1;
    }
    rbuf[r] = '\0';
    close(io);

    /* parse "GPIO<gpio>:<0|1>\n" */
    unsigned int g;
    int val;
    if (sscanf(rbuf, "GPIO%u:%d", &g, &val) == 2) {
        return val;
    } else {
        fprintf(stderr, "unexpected response: %s\n", rbuf);
        return -1;
    }
}

int main(int argc, char** argv) {
    // "Usage examples:"
    // "  LED1 on      -> turn LED1 on"
    // "  LED1 off     -> turn LED1 off"
    // "  LED1         -> print LED1 status"

    string cmd, arg;
    while (true) {
        if (! (cin >> cmd) ) break;
        if (cmd == "exit" || cmd == "quit") break;
        /* cmd is like "LED1" or "LED1" then maybe next token "on"/"off" */
        if (!(cin.peek() == '\n')) {
            cin >> arg; // maybe "on"/"off"
        } else arg = "";

        auto it = led_map.find(cmd);
        if (it == led_map.end()) {
            cout << "Unknown LED name: " << cmd << "\n";
            continue;
        }
        unsigned int gpio = it->second;

        if (arg == "on") {
            setGPIO_by_number(gpio, 1);
            cout << cmd << " -> set to ON (GPIO " << gpio << ")\n";
        } else if (arg == "off") {
            setGPIO_by_number(gpio, 0);
            cout << cmd << " -> set to OFF (GPIO " << gpio << ")\n";
        } else if (arg == "") {
            int val = getGPIO_by_number(gpio);
            if (val >= 0)
                cout << cmd << " Status: " << val << "\n";
            else
                cout << "Failed to read status for " << cmd << "\n";
        } else {
            cout << "Unknown command argument: " << arg << "\n";
        }
    }
    return 0;
}
