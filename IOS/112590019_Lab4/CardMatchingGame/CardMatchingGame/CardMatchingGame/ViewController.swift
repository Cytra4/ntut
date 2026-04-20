//
//  ViewController.swift
//  week5
//
//  Created by student on 2026/3/23.
//  Copyright © 2026年 student. All rights reserved.
//

import UIKit

class ViewController: UIViewController {
    var emojiChoices = [
        ["😐","💀","🤔","🤡","🤢","😡","☹️","😤"],
        ["🖕","☝️","✋","✌️","👌","👊","🤞","🖖"],
        ["🍏","🍎","🍐","🍊","🍋","🍌","🍉","🍇"],
    ]
    var emojiDictionary = [Int: String]()
    var flipCount = 0;
    var flipAllCount = 0;
    var theme = 0;
    var lastTheme: Int?
    lazy var game: MatchingGame = MatchingGame(pairs: 8)
    
    var numberOfPairsCard: Int{
        return (cards.count+1)/2
    }
    
    @IBOutlet weak var flipText: UILabel!
    @IBOutlet weak var scoreText: UILabel!
    @IBOutlet var cards: [UIButton]!
    
    
    @IBAction func touchCard(_ sender: UIButton) {
        if let id = cards.index(of: sender){
            if (!game.cards[id].isMatched){
                flipCount += 1
                game.chooseCard(at: id)
                updateViewFromModel()
            }
        }
        else{
            print("Not in the collection")
        }
        scoreText.text = "Score: \(game.score)"
        flipText.text = "Flips: \(flipCount)"
    }
    
    func updateViewFromModel(){
        for index in cards.indices{
            let button = cards[index]
            let card = game.cards[index]
            let title = getEmoji(for: card)
            
            if !card.isFaceUp{
                button.setTitle("", for: UIControlState.normal)
                button.backgroundColor = #colorLiteral(red: 0.9686274529, green: 0.78039217, blue: 0.3450980484, alpha: 1)
                button.alpha = 1.0
            }
            else{
                button.setTitle(title, for: UIControlState.normal)
                button.backgroundColor = #colorLiteral(red: 1, green: 1, blue: 1, alpha: 1)
                
                if card.isMatched{
                    button.alpha = 0.5
                }
                else{
                    button.alpha = 1.0
                }
            }
        }
    }
    
    func getEmoji(for card: Card) -> String{
        if emojiDictionary[card.identifier] == nil, emojiChoices[theme].count > 0{
            let randomIndex = Int(arc4random_uniform(UInt32(emojiChoices[theme].count)))
            emojiDictionary[card.identifier] = emojiChoices[theme].remove(at: randomIndex)
        }
        return emojiDictionary[card.identifier] ?? "?"
    }
    
    @IBAction func touchReset(_ sender: UIButton) {
        resetFunction()
    }
    
    func resetFunction(){
        game.score = 0
        scoreText.text = "Score: \(game.score)"
        
        flipCount = 0
        flipText.text = "Flips: \(flipCount)"
        
        var newTheme: Int
        repeat {
            newTheme = Int(arc4random_uniform(UInt32(emojiChoices.count)))
        } while newTheme == lastTheme

        theme = newTheme
        lastTheme = theme
        
        emojiDictionary = [Int: String]()
        emojiChoices = [
            ["😐","💀","🤔","🤡","🤢","😡","☹️","😤"],
            ["🖕","☝️","✋","✌️","👌","👊","🤞","🖖"],
            ["🍏","🍎","🍐","🍊","🍋","🍌","🍉","🍇"],
        ]
        
        for index in game.cards.indices{
            game.cards[index].isFaceUp = false
            game.cards[index].isMatched = false
            game.indexOfOneAndOnlyFaceUpCard = nil
        }
        var n = game.cards.count
        while n > 1 {
            n -= 1
            let k = Int(arc4random_uniform(UInt32(n + 1)))
            game.cards.swapAt(n, k)
        }
        
        updateViewFromModel()
    }
    
    @IBAction func touchFlipAll(_ sender: UIButton) {
        flipCount = 0
        flipText.text = "Flips: \(flipCount)"
        game.score = 0
        scoreText.text = "Score: \(game.score)"
        
        if (flipAllCount % 2 == 0){
            for index in game.cards.indices{
                game.cards[index].isFaceUp = true
                game.cards[index].isMatched = true
            }
        }
        else{
            resetFunction()
        }
        game.indexOfOneAndOnlyFaceUpCard = nil
        flipAllCount += 1
        
        updateViewFromModel()
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        resetFunction()
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }


}

