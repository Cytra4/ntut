/* The Hex game
   https://en.wikipedia.org/wiki/Hex_(board_game)
   desigened by Jean-Christophe Filliâtre

   grid size : n*n

   playable cells : (i,j) with 1 <= i, j <= n

   blue edges (left and right) : i=0 or i=n+1, 1 <= j <= n
    red edges (top and bottom) : 1 <= i <= n, j=0 or j=n+1

      note: the four corners have no color

   adjacence :      i,j-1   i+1,j-1

                 i-1,j    i,j   i+1,j

                    i-1,j+1    i,j+1

*/

class UnionFind {
  private int[] parent;

  UnionFind(int size) {
    parent = new int[size];

    for (int i = 0; i < size; i++) {
      parent[i] = i;
    }
  }

  int find(int x) {
    if (parent[x] != x) {
      parent[x] = find(parent[x]);
    }

    return parent[x];
  }

  void union(int a, int b) {
    int rootA = find(a);
    int rootB = find(b);

    if (rootA != rootB) {
      parent[rootB] = rootA;
    }
  }

  boolean connected(int a, int b) {
    return find(a) == find(b);
  }
}

public class Hex {

  enum Player {
    NOONE, BLUE, RED
  }

  private Player[][] grid;
  private UnionFind uni;
  private int size;
  private Player playerTurn = Player.RED;

  int id(int i, int j) {
    return i + size * j;
  }

  // create an empty board of size n*n
  Hex(int n) {
    size = n + 2;
    uni = new UnionFind(size * size);
    grid = new Player[size][size];
    for (int i = 0; i < size; i++) {
      if (i == 0 || i == size - 1) {
        grid[i][0] = Player.NOONE;
        grid[i][size - 1] = Player.NOONE;
        for (int j = 1; j < size - 1; j++) {
          grid[i][j] = Player.BLUE;
          if (j < size - 2) {
            uni.union(id(i, j), id(i, j + 1));
          }
        }
      } else {
        grid[i][0] = Player.RED;
        grid[i][size - 1] = Player.RED;
        for (int j = 1; j < size - 1; j++) {
          grid[i][j] = Player.NOONE;
        }
        if (i < size - 2) {
          uni.union(id(i, 0), id(i + 1, 0));
          uni.union(id(i, size - 1), id(i + 1, size - 1));
        }
      }
    }
  }

  // return the color of cell i,j
  Player get(int i, int j) {
    if (i < 0 || i > size - 1) {
      return Player.NOONE;
    }
    if (j < 0 || j > size - 1) {
      return Player.NOONE;
    }

    return grid[i][j];
  }

  // update the board after the player with the trait plays the cell (i, j).
  // Does nothing if the move is illegal.
  // Returns true if and only if the move is legal.
  boolean click(int i, int j) {
    if (winner() != Player.NOONE) {
      return false;
    }

    if (grid[i][j] != Player.NOONE) {
      return false;
    }

    Player p = currentPlayer();

    grid[i][j] = p;

    int[][] neighbors = {
        { i, j - 1 }, { i + 1, j - 1 }, { i + 1, j },
        { i, j + 1 }, { i - 1, j + 1 }, { i - 1, j }
    };

    for (int[] nb : neighbors) {

      int ni = nb[0];
      int nj = nb[1];

      if (get(ni, nj) == p) {
        uni.union(id(i, j), id(ni, nj));
      }
    }

    playerTurn = (p == Player.RED)
        ? Player.BLUE
        : Player.RED;

    return true;
  }

  // return the player with the trait or Player.NOONE if the game is over
  // because of a player's victory.
  Player currentPlayer() {
    if (winner() != Player.NOONE) {
      return Player.NOONE;
    }
    return playerTurn;
  }

  // return the winning player, or Player.NOONE if no player has won yet
  Player winner() {
    if (uni.connected(id(1, 0), id(1, size - 1))) {
      return Player.RED;
    }
    if (uni.connected(id(0, 1), id(size - 1, 1))) {
      return Player.BLUE;
    }
    return Player.NOONE;
  }

  int label(int i, int j) {
    return uni.find(i + size * j);
  }

  public static void main(String[] args) {
    HexGUI.createAndShowGUI();
  }
}
