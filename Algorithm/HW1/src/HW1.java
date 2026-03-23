/* HW1. Battle
 * This file contains two classes :
 * 		- Deck represents a pack of cards,
 * 		- Battle represents a battle game.
 */

import java.util.LinkedList;

class Deck { // represents a pack of cards

	LinkedList<Integer> cards;
	// The methods toString, hashCode, equals, and copy are used for
	// display and testing, you should not modify them.

	@Override
	public String toString() {
		return cards.toString();
	}

	@Override
	public int hashCode() {
		return 0;
	}

	@Override
	public boolean equals(Object o) {
		Deck d = (Deck) o;
		return cards.equals(d.cards);
	}

	Deck copy() {
		Deck d = new Deck();
		for (Integer card : this.cards)
			d.cards.addLast(card);
		return d;
	}

	// constructor of an empty deck
	Deck() {
		cards = new LinkedList<Integer>();
	}

	// constructor from field
	Deck(LinkedList<Integer> cards) {
		this.cards = cards;
	}

	// constructor of a complete sorted deck of cards with nbVals values
	Deck(int nbVals) {
		cards = new LinkedList<Integer>();
		for (int j = 1; j <= nbVals; j++)
			for (int i = 0; i < 4; i++)
				cards.add(j);
	}

	// Question 1

	// takes a card from deck d to put it at the end of the current packet
	int pick(Deck d) {
		// throw new Error("Method pick(Deck d) to complete (Question 1)");
		if (!d.cards.isEmpty()) {
			int x = d.cards.removeFirst();
			cards.addLast(x);
			return x;
		} else {
			return -1;
		}
	}

	// takes all the cards from deck d to put them at the end of the current deck
	void pickAll(Deck d) {
		// throw new Error("Method pickAll(Deck d) to complete (Question 1)");
		while (!d.cards.isEmpty()) {
			pick(d);
		}
	}

	// checks if the current packet is valid
	boolean isValid(int nbVals) {
		// throw new Error("Method isValid(int nbVals) to complete (Question 1)");
		int[] numbers = new int[nbVals];
		for (Integer x : cards) {
			if (x < 1 || x > nbVals || numbers[x - 1] > 3)
				return false;
			numbers[x - 1]++;
		}
		return true;
	}

	// Question 2.1

	// chooses a position for the cut
	int cut() {
		int heads = 0;
		int n = cards.size();
		double rand;
		for (int i = 0; i < n; i++) {
			rand = Math.random();
			if (rand > 0.5) {
				heads++;
			}
		}
		return heads;
	}

	// cuts the current packet in two at the position given by cut()
	Deck split() {
		int c = cut();
		Deck newDeck = new Deck();
		for (int i = 0; i < c; i++) {
			Integer card = cards.removeFirst();
			newDeck.cards.add(card);
		}
		return newDeck;
	}

	// Question 2.2

	// mixes the current deck and the deck d
	void riffleWith(Deck d) {
		Deck finalDeck = new Deck();
		int a, b;
		while (!cards.isEmpty() || !d.cards.isEmpty()) {
			a = cards.size();
			b = d.cards.size();
			if (Math.random() < (double) a / (a + b)) {
				finalDeck.pick(this);
			} else {
				finalDeck.pick(d);
			}
		}

		this.pickAll(finalDeck);
		// throw new Error("Method riffleWith(Deck d) to complete (Question 2.2)");
	}

	// Question 2.3

	// shuffles the current deck using the riffle shuffle
	void riffleShuffle(int m) {
		for (int i = 0; i < m; i++) {
			Deck secondDeck = this.split();
			riffleWith(secondDeck);
		}
		// throw new Error("Method riffleShuffle(int m) to complete (Question 2.3)");
	}
}

class Battle { // represents a battle game

	Deck player1;
	Deck player2;
	Deck trick;
	Boolean turn;

	// constructor of a battle without cards
	Battle() {
		player1 = new Deck();
		player2 = new Deck();
		trick = new Deck();
		turn = true;
	}

	// constructor from fields
	Battle(Deck player1, Deck player2, Deck trick) {
		this.player1 = player1;
		this.player2 = player2;
		this.trick = trick;
		turn = true;
	}

	// copy the battle
	Battle copy() {
		Battle r = new Battle();
		r.player1 = this.player1.copy();
		r.player2 = this.player2.copy();
		r.trick = this.trick.copy();
		return r;
	}

	// string representing the battle
	@Override
	public String toString() {
		return "Player 1 : " + player1.toString() + "\n" + "Player 2 : " + player2.toString() + "\nPli "
				+ trick.toString();
	}

	// Question 3.1

	// constructor of a battle with a deck of cards of nbVals values
	Battle(int nbVals) {
		player1 = new Deck();
		player2 = new Deck();
		trick = new Deck();
		turn = true;
		Deck fullDeck = new Deck(nbVals);
		fullDeck.riffleShuffle(7);
		while (!fullDeck.cards.isEmpty()) {
			player1.pick(fullDeck);
			player2.pick(fullDeck);
		}
		// throw new Error("Constructor Battle() to complete (Question 3.1)");
	}

	// Question 3.2

	// test if the game is over
	boolean isOver() {
		return player1.cards.isEmpty() || player2.cards.isEmpty();
		// throw new Error("Method isOver() to complete (Question 3.2)");
	}

	// effectue un tour de jeu
	boolean oneRound() {
		int p1Card, p2Card;
		while (true) {
			if (turn) {
				p1Card = trick.pick(player1);
				if (p1Card == -1)
					return false;

				p2Card = trick.pick(player2);
				if (p2Card == -1) {
					player1.pickAll(trick);
					return false;
				}

			} else {
				p2Card = trick.pick(player2);
				if (p2Card == -1)
					return false;

				p1Card = trick.pick(player1);
				if (p1Card == -1) {
					player2.pickAll(trick);
					return false;
				}
			}

			if (p1Card > p2Card) {
				player1.pickAll(trick);
				break;
			} else if (p1Card < p2Card) {
				player2.pickAll(trick);
				break;
			}

			if (turn) {
				if (trick.pick(player1) == -1)
					return false;
				if (trick.pick(player2) == -1) {
					player1.pickAll(trick);
					return false;
				}
			} else {
				if (trick.pick(player2) == -1)
					return false;
				if (trick.pick(player1) == -1) {
					player2.pickAll(trick);
					return false;
				}
			}
		}
		turn = !turn;
		return true;
		// throw new Error("Method oneRound() to complete (Question 3.2)");
	}

	// Question 3.3

	// returns the winner
	int winner() {
		if (player1.cards.size() > player2.cards.size()) {
			return 1;
		} else if (player1.cards.size() < player2.cards.size()) {
			return 2;
		} else {
			return 0;
		}
		// throw new Error("Method winner() to complete (Question 3.3)");
	}

	// plays a game with a fixed maximum number of moves
	int game(int turns) {
		for (int i = 0; i < turns; i++) {
			oneRound();
		}
		return winner();
		// throw new Error("Method game(int turns) to complete (Question 3.3)");
	}

	// Question 4.1

	// plays a game without limit of moves, but with detection of infinite games
	int game() {
		Battle hare = this.copy();
		while (!this.isOver() && !hare.isOver()) {
			this.oneRound();
			if (this.isOver()) {
				return this.winner();
			}
			hare.oneRound();
			hare.oneRound();
			if (hare.isOver()) {
				return hare.winner();
			}

			if (this.toString().equals(hare.toString())) {
				return 3;
			}
		}

		if (this.isOver()) {
			return this.winner();
		} else {
			return hare.winner();
		}
		// throw new Error("Method game() to complete (Question 4.1)");
	}

	// Question 4.2

	// performs statistics on the number of infinite games
	static void stats(int nbVals, int nbGames) {
		int w1, w2, infinte, draw;
		w1 = w2 = infinte = draw = 0;
		int result;
		Battle new_game;
		for (int i = 0; i < nbGames; i++) {
			new_game = new Battle(nbVals);
			result = new_game.game(nbVals);
			if (result == 0) {
				draw++;
			} else if (result == 1) {
				w1++;
			} else if (result == 2) {
				w2++;
			} else {
				infinte++;
			}
		}

		System.out.println("-------------------------");
		System.out.printf("In total of %d games: \n Player1 wins %d games \n Player2 wins %d games \n", nbGames, w1,
				w2);
		System.out.printf("%d games end in draw, %d games are infinte \n", draw, infinte);
		System.out.println("-------------------------");
		// throw new Error("Method stats(int bvVals, int nb_of_games) to complete
		// (Question 4.2)");
	}
}
