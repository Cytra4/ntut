CXX=g++
CXXFLAGS=-std=c++11
OBJ=export.o unexport.o set_dir.o set_value.o command_handle.o main.o
SRC=export.cpp unexport.cpp set_dir.cpp set_value.cpp command_handle.cpp main.cpp
EXE=L2Program
all:$(EXE)

$(EXE): $(OBJ)
	$(CXX) -o $@

%.o: %.cpp pin_lookup.hpp
	$(CXX) $(CXXFLAGS) -c $< -o $@

clean:
	rm -f $(OBJS) $(TARGET)