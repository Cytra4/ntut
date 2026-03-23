#ifndef DEMO_H
#define DEMO_H

/* tutorial-style simple defs expected by the slides */
#define SUCCESS 0
#define DEVICE_NAME "demo"
#define BUF_LEN 100

/* IOCTL commands used in tutorial (simple numeric) */
#define IOCTL_GET_MSG 1

/* simple open-lock flags used by example */
//#define CDEV_NOT_USED 0
//#define CDEV_EXCLUSIVE_OPEN 1
//static atomic_t already_open = ATOMIC_INIT(CDEV_NOT_USED);

/* state variables the tutorial example expects */
static int device_opened = 0;

/* message buffer used in the example */
static char message[BUF_LEN];
static char *message_ptr = message;

#endif /* DEMO_H */
