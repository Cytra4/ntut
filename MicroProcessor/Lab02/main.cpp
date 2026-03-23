#include "command_handle.cpp"
#include "iostream"

int main(int argc, char *argv[]){
    string input;
    string command;
    std::cin>>input>>command;

    char led_num = input[3];

    if (led_num == '1'){
        command_handle(396, command);
    }
    else if (led_num == '2'){
        command_handle(392, command);
    }
    else if (led_num == '3'){
        command_handle(255, command);
    }
    else if (led_num == '4'){
        command_handle(429, command);
    }
    else{
        std::cout<<"Invalid Command";
    }

    return 0;
}