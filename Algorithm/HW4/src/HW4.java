/**
 * HW4. Backtracking, solution and maze generation <br>
 * This file contains 2 classes: <br> designed by Jean-Christophe Filliâtre
 * 	- ExtendCell provides a cell of the maze with operations to calculate a path to the exit and generate a maze recursively <br>
 * 	- Maze models a maze.
 */

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Random;

import java.io.IOException;

import java.nio.charset.StandardCharsets;

import java.nio.file.Files;
import java.nio.file.Paths;

/**
 * This class extends and enriches the representation of a cell of the maze. <br>
 * It provides to a cell the operations to: <br>
 * -) find a path to the exit <br>
 * -) Generate a maze recursively
 */
class ExtendedCell extends Cell {
	
	public ExtendedCell(Maze maze) {
		super(maze);
	}
	
	public Cell next;  // or ExtendedCell next;

    private boolean marked;

    public boolean isMarked() {
        return marked;
    }

    public void setMarked(boolean value) {
        marked = value;
    }
	
	// Question 1

	/**
	 * Test if there is a path from the current cell to an exit
	 * 
	 * @return true if there is a path from the current cell to an exit
	 */
	boolean searchPath() {
		maze.slow(); // slow down the search animation (to help debugging)

		// throw new Error("method searchPath() to be completed (Question 1)");

		// 1. If this cell is already marked, we can't use it
		if (isMarked()) {
			return false;
		}

		// 2. Mark this cell as part of the current path
		setMarked(true);

		// 3. If this is the exit, we're done
		if (isExit()) {
			return true;
		}

		// 4. Explore neighbors (only those without walls)
		for (Cell neighbor : getNeighbors(false)) {
			ExtendedCell next = (ExtendedCell) neighbor;

			if (next.searchPath()) {
				return true; // found a valid path
			}
		}

		// 5. Backtrack: unmark this cell (since no path worked)
		setMarked(false);

    	return false;
	}

	// Question 2

	/**
	 * generate a perfect maze using recursive backtracking
	 */
	void generateRec() {
		maze.slow();

		// throw new Error("method generateRec() to be completed (Question 2)");

		// 1. Get all neighbors (ignore walls so we see all directions)
		List<Cell> neighbors = getNeighbors(true);

		// 2. Shuffle to randomize maze structure
		Collections.shuffle(neighbors);

		// 3. Try each neighbor
		for (Cell neighbor : neighbors) {
			ExtendedCell next = (ExtendedCell) neighbor;

			// 4. Only proceed if neighbor is isolated (no passages yet)
			if (next.getNeighbors(false).isEmpty()) {
				
				// 5. Break wall to create passage
				breakWall(next);

				// 6. Recurse
				next.generateRec();
			}
		}
	}
}

/**
 * this class models a maze
 */
class Maze {

	private int height, width;
	/** the grid (array of cells) representing the maze */ 
	private Cell[][] grid;


	// Question 3

	/**
	 * generate a perfect maze using iterative backtracking
	 */
	void generateIter(int selectionMethod) {
		Bag cells = new Bag(selectionMethod);
		cells.add(getFirstCell());

		while(!cells.isEmpty()) {
			slow();

			// throw new Error("method generateIter() to be completed (Question 3)");
			
			// 1. Get current cell
			ExtendedCell current = (ExtendedCell) cells.peek();

			// 2. Get neighbors and shuffle
			List<Cell> neighbors = current.getNeighbors(true);
			Collections.shuffle(neighbors);

			boolean found = false;

			// 3. Try to find an isolated neighbor
			for (Cell neighbor : neighbors) {
				ExtendedCell next = (ExtendedCell) neighbor;

				if (next.getNeighbors(false).isEmpty()) {
					// Found valid direction

					current.breakWall(next); // create passage
					cells.add(next);         // go deeper

					found = true;
					break; // IMPORTANT: only take ONE neighbor
				}
			}

			// 4. If no neighbor worked → backtrack
			if (!found) {
				cells.pop();
			}
    	}
	}


	// Question 4
	
	/**
	 * generate a maze using Wilson's algorithm
	 */
	void generateWilson() {
		// throw new Error("method generateWilson() to be completed (Question 4)");
		List<Cell> allCells = new ArrayList<>();

		// 1. Collect all cells
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				allCells.add(getCell(i, j));
			}
		}

		// 2. Shuffle for randomness
		Collections.shuffle(allCells);

		// 3. Mark first cell
		ExtendedCell first = (ExtendedCell) allCells.get(0);
		first.setMarked(true);

		// 4. Process remaining cells
		for (Cell c : allCells) {
			ExtendedCell start = (ExtendedCell) c;

			if (start.isMarked()) continue;

			// ---- Random walk ----
			ExtendedCell current = start;

			while (!current.isMarked()) {
				List<Cell> neighbors = current.getNeighbors(true);
				Collections.shuffle(neighbors);

				ExtendedCell next = (ExtendedCell) neighbors.get(0);

				current.next = next; // store direction
				current = next;
			}

			// ---- Carve path ----
			current = start;

			while (!current.isMarked()) {
				ExtendedCell next = (ExtendedCell) current.next;

				current.breakWall(next);
				current.setMarked(true);

				current = next;
			}
		}
	}

	/**
	 * return the cell with coordinates (i, j)
	 * 
	 * @return the cell with coordinates (i, j)
	 */
	Cell getCell(int i, int j) {
		if(i < 0 || i >= height || j < 0 || j >= width)
			throw new IllegalArgumentException("invalid indices");

		return grid[i][j];
	}

	/**
	 * return the cell with coordinates (0, 0)
	 * 
	 * @return the cell with coordinates (0, 0)
	 */
	Cell getFirstCell() {
		return getCell(0, 0);
	}

	// translate coordinates to cell number
	int coordToInt(int i, int j) {
		if(i < 0 || i >= height || j < 0 || j >= width)
			throw new IndexOutOfBoundsException();

		return i*width + j;
	}

	// translate cell number to coordinates
	Coordinate intToCoord(int x) {
		if(x < 0 || x >= height*width)
			throw new IndexOutOfBoundsException();

		return new Coordinate(x/width, x%width);
	}


	// slow down the display of the maze if a graphical window is open
	void slow(){
		if (frame == null) return;

		try {
			Thread.sleep(10);
			frame.repaint();
		} catch (InterruptedException e) {}
	}

	private MazeFrame frame;
	private static final int step = 20;

	Maze(int height, int width) {
		this(height, width, true);
	}

	Maze(int height, int width, boolean window) {
		if((height <= 0) || (width <= 0))
			throw new IllegalArgumentException("height and width of a Maze must be positive");

		this.height = height;
		this.width = width;

		grid = new Cell[height][width];

		for(int i = 0; i < height; ++i)
			for(int j = 0; j < width; ++j)
				grid[i][j] = new ExtendedCell(this);

		for(int i = 0; i < height; ++i) {
			for(int j = 0; j < width; ++j) {
				if(i < height - 1) {
					grid[i][j].addNeighbor(grid[i+1][j]);
					grid[i+1][j].addNeighbor(grid[i][j]);
				}

				if(j < width - 1) {
					grid[i][j].addNeighbor(grid[i][j+1]);
					grid[i][j+1].addNeighbor(grid[i][j]);
				}
			}
		}

		grid[height-1][width-1].setExit(true);

		if(window)
			frame = new MazeFrame(grid, height, width, step);
	}

	Maze(String path) throws IOException {
		this(Files.readAllLines(Paths.get(path), StandardCharsets.UTF_8));
	}

	Maze(String path, boolean window) throws IOException {
		this(Files.readAllLines(Paths.get(path), StandardCharsets.UTF_8), window);
	}

	Maze(List<String> lines) {
		this(lines, true);
	}

	Maze(List<String> lines, boolean window) {
		if(lines.size() < 2)
			throw new IllegalArgumentException("too few lines");

		this.height = Integer.parseInt(lines.get(0));
		this.width = Integer.parseInt(lines.get(1));

		this.grid = new Cell[height][width];
		for(int i = 0; i < height; ++i)
			for(int j = 0; j < width; ++j)
				grid[i][j] = new ExtendedCell(this);

		for(int i = 0; i < height; ++i) {
			for(int j = 0; j < width; ++j) {
				if(i < height - 1) {
					grid[i][j].addNeighbor(grid[i+1][j]);
					grid[i+1][j].addNeighbor(grid[i][j]);
				}

				if(j < width - 1) {
					grid[i][j].addNeighbor(grid[i][j+1]);
					grid[i][j+1].addNeighbor(grid[i][j]);
				}
			}
		}

		grid[height-1][width-1].setExit(true);

		int i = 0;
		int j = 0;

		for(String line : lines.subList(2, lines.size())) {

			for(int k = 0; k < line.length(); ++k) {
				switch(line.charAt(k)) {
					case 'N':
						grid[i][j].breakWall(grid[i-1][j]);
						break;
					case 'E':
						grid[i][j].breakWall(grid[i][j+1]);
						break;
					case 'S':
						grid[i][j].breakWall(grid[i+1][j]);
						break;
					case 'W':
						grid[i][j].breakWall(grid[i][j-1]);
						break;
					case '*':
						grid[i][j].setMarked(true);
						break;
					default:
						throw new IllegalArgumentException("illegal character");
				}
			}
			++j;
			if(j >= width) {
				j = 0;
				++i;
			}
		}

		if(window)
			frame = new MazeFrame(grid, height, width, step);
	}

	@Override
	public String toString() {
		StringBuilder sb = new StringBuilder();

		sb.append(height);
		sb.append('\n');
		sb.append(width);
		sb.append('\n');

		for(int i = 0; i < height; ++i) {
			for(int j = 0; j < width; ++j) {
				if(i > 0 && grid[i][j].hasPassageTo(grid[i-1][j]))
					sb.append('N');
				if(j < width-1 && grid[i][j].hasPassageTo(grid[i][j+1]))
					sb.append('E');
				if(i < height-1 && grid[i][j].hasPassageTo(grid[i+1][j]))
					sb.append('S');
				if(j > 0 && grid[i][j].hasPassageTo(grid[i][j-1]))
					sb.append('W');
				if(grid[i][j].isMarked())
					sb.append('*');
				sb.append('\n');
			}
		}

		return sb.toString();
	}

	@Override
	public boolean equals(Object o) {
		if(!(o instanceof Maze))
			return false;
		Maze that = (Maze)o;

		return this.toString().equals(that.toString());
	}

	@Override
	public int hashCode() {
		return this.toString().hashCode();
	}

	boolean isPerfect() {
		UnionFind uf = new UnionFind(height*width);

		// union find cycle detection
		for(int i = 0; i < height; ++i) {
			// horizontal edges
			for(int j = 0; j < width-1; ++j) {
				if(grid[i][j].hasPassageTo(grid[i][j+1])) {
					if(uf.sameClass(coordToInt(i,j), coordToInt(i,j+1)))
						return false;
					uf.union(coordToInt(i,j), coordToInt(i,j+1));
				}
			}

			// there are no vertical edges in last row, so we're done
			if(i == height-1)
				continue;

			// vertical edges
			for(int j = 0; j < width; ++j) {
				if(grid[i][j].hasPassageTo(grid[i+1][j])) {
					if(uf.sameClass(coordToInt(i,j), coordToInt(i+1,j)))
						return false;
					uf.union(coordToInt(i,j), coordToInt(i+1,j));
				}
			}
		}

		// check if connected
		return (uf.getSize(0) == height*width);
	}

	void clearMarks() {
		for (Cell[] row : grid)
			for (Cell c : row)
				c.setMarked(false);
	}
}

