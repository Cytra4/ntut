// demo.c
#include <linux/init.h>
#include <linux/kernel.h>
#include <linux/module.h>
#include <linux/fs.h>
#include <linux/gpio.h>
#include <linux/uaccess.h>
#include <linux/mutex.h>
#include <linux/slab.h>

#define MAJOR_NUM 60
#define DEVICE_NAME "demo"
#define MAX_BUF 128

static char resp_buf[MAX_BUF];
static size_t resp_len = 0;
static DEFINE_MUTEX(demo_mutex);

static ssize_t drv_read(struct file *flip, char __user *buf, size_t count, loff_t *ppos);
static ssize_t drv_write(struct file *flip, const char __user *buf, size_t count, loff_t *ppos);
static int drv_open(struct inode *inode, struct file *file);
static int drv_release(struct inode *inode, struct file *file);

static struct file_operations drv_fops = {
    .owner = THIS_MODULE,
    .read = drv_read,
    .write = drv_write,
    .open = drv_open,
    .release = drv_release,
};

static int demo_init(void){
    int rc;
    rc = register_chrdev(MAJOR_NUM, DEVICE_NAME, &drv_fops);
    if (rc < 0){
        printk(KERN_ERR "%s: can't get major %d\n", DEVICE_NAME, MAJOR_NUM);
        return -EBUSY;
    }
    mutex_init(&demo_mutex);
    pr_info("%s: module started (major %d)\n", DEVICE_NAME, MAJOR_NUM);
    return 0;
}

static void demo_exit(void){
    unregister_chrdev(MAJOR_NUM, DEVICE_NAME);
    mutex_destroy(&demo_mutex);
    pr_info("%s: module removed\n", DEVICE_NAME);
}

module_init(demo_init);
module_exit(demo_exit);

static int drv_open(struct inode *inode, struct file *file){
    pr_info("%s: device opened\n", DEVICE_NAME);
    return 0;
}

static int drv_release(struct inode *inode, struct file *file){
    pr_info("%s: device closed\n", DEVICE_NAME);
    return 0;
}

/*
 * read: returns whatever is currently stored in resp_buf (set by previous write GET)
 */
static ssize_t drv_read(struct file *flip, char __user *buf, size_t count, loff_t *ppos){
    ssize_t to_copy;

    if (mutex_lock_interruptible(&demo_mutex))
        return -EINTR;

    if (resp_len == 0) {
        /* nothing available */
        mutex_unlock(&demo_mutex);
        return 0;
    }

    to_copy = min((size_t)resp_len, count);
    if (copy_to_user(buf, resp_buf, to_copy)) {
        mutex_unlock(&demo_mutex);
        return -EFAULT;
    }
    /* clear response after read */
    resp_len = 0;
    resp_buf[0] = '\0';
    mutex_unlock(&demo_mutex);
    return to_copy;
}

/*
 * write: accepts commands:
 *   "<gpio>:<0|1>"    -> set gpio to 0 or 1
 *   "GET <gpio>\n"    -> prepare response "GPIO<gpio>:<0|1>\n" for next read()
 */
static ssize_t drv_write(struct file *flip, const char __user *buf, size_t count, loff_t *ppos){
    char *kbuf;
    ssize_t ret = count;
    unsigned int gpio;
    int val;
    int rc;

    if (count == 0 || count > MAX_BUF - 1)
        return -EINVAL;

    kbuf = kmalloc(count + 1, GFP_KERNEL);
    if (!kbuf)
        return -ENOMEM;

    if (copy_from_user(kbuf, buf, count)) {
        kfree(kbuf);
        return -EFAULT;
    }
    kbuf[count] = '\0';

    pr_info("%s: write received: %s\n", DEVICE_NAME, kbuf);

    if (mutex_lock_interruptible(&demo_mutex)) {
        kfree(kbuf);
        return -EINTR;
    }

    /* handle GET command */
    if (sscanf(kbuf, "GET %u", &gpio) == 1) {
        /* query gpio value */
        if (!gpio_is_valid(gpio)) {
            resp_len = snprintf(resp_buf, MAX_BUF, "GPIO%u:INVALID\n", gpio);
        } else {
            /* request if not requested yet: safe to call request; ignore error if already requested */
            rc = gpio_request_one(gpio, GPIOF_IN, "demo_gpio_in");
            if (rc && rc != -EBUSY) {
                resp_len = snprintf(resp_buf, MAX_BUF, "GPIO%u:REQUEST_ERR\n", gpio);
            } else {
                val = gpio_get_value(gpio);
                resp_len = snprintf(resp_buf, MAX_BUF, "GPIO%u:%d\n", gpio, val);
                /* if we requested it here, free it to avoid holding resources (safe for demo) */
                gpio_free(gpio);
            }
        }
        mutex_unlock(&demo_mutex);
        kfree(kbuf);
        return ret;
    }

    /* handle set: "<gpio>:<0|1>" */
    if (sscanf(kbuf, "%u:%d", &gpio, &val) == 2) {
        if (!gpio_is_valid(gpio)) {
            pr_err("%s: invalid gpio %u\n", DEVICE_NAME, gpio);
            ret = -EINVAL;
            goto out_unlock;
        }
        if (val != 0 && val != 1) {
            pr_err("%s: invalid value %d\n", DEVICE_NAME, val);
            ret = -EINVAL;
            goto out_unlock;
        }

        /* request and set as output */
        rc = gpio_request(gpio, "demo_gpio_out");
        if (rc && rc != -EBUSY) {
            pr_err("%s: gpio_request %u failed %d\n", DEVICE_NAME, gpio, rc);
            ret = -EBUSY;
            goto out_unlock;
        }

        rc = gpio_direction_output(gpio, val);
        if (rc) {
            pr_err("%s: gpio_direction_output %u failed %d\n", DEVICE_NAME, gpio, rc);
            gpio_free(gpio);
            ret = -EIO;
            goto out_unlock;
        }

        gpio_set_value(gpio, val);
        pr_info("%s: set GPIO%u -> %d\n", DEVICE_NAME, gpio, val);

        /* free the gpio so other users can use it; if you prefer persistent ownership, skip gpio_free */
        gpio_free(gpio);

        mutex_unlock(&demo_mutex);
        kfree(kbuf);
        return ret;
    }

    pr_warn("%s: unrecognized cmd: %s\n", DEVICE_NAME, kbuf);
    ret = -EINVAL;

out_unlock:
    mutex_unlock(&demo_mutex);
    kfree(kbuf);
    return ret;
}

MODULE_LICENSE("GPL");
MODULE_AUTHOR("student");
MODULE_DESCRIPTION("Simple GPIO control char driver demo");
