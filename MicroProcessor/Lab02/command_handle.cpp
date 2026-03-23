#include "export.cpp"
#include "unexport.cpp"
#include "set_dir.cpp"
#include "set_value.cpp"
#include "pin_lookup.hpp"
#include "iostream"

void command_handle(int pin_num, string command)
{
    if (command == "on")
    {
        gpio_export(pin_num);
        gpio_set_dir(pin_num, "out");
        gpio_set_value(pin_num, 1);
    }
    else
    {
        gpio_set_value(pin_num, 0);
        gpio_unexport(pin_num);
    }
}