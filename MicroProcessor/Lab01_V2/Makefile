CXX=g++
CXXFLAGS=-std=c++11
OBJ=circular_sector.o ellipse.o sort.o triangle.o main.o
SRC=circular_sector.cpp ellipse.cpp main.cpp sort.cpp triangle.cpp
EXE=tx2_exe
all:$(EXE)
$(EXE): $(OBJ)
	$(CXX) -o $@ $?
$(OBJ):$(SRC)
	$(CXX) $(CXXFLAGS) -c $(SRC)
clean:
	rm -f $(EXE)
	rm -f $(OBJ)
