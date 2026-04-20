//
//  MatchingGame.swift
//  week5
//
//  Created by student on 2026/3/24.
//  Copyright © 2026年 student. All rights reserved.
//

import Foundation

class MatchingGame{
    var cards: Array<Card>
    var score = 0;
    var indexOfOneAndOnlyFaceUpCard: Int?{
        get{
            var foundIndex:Int?
            for index in cards.indices{
                if cards[index].isFaceUp && !cards[index].isMatched{
                    if foundIndex == nil{
                        foundIndex = index
                    }
                    else{
                        return nil
                    }
                }
            }
            return foundIndex
        }
        set(newValue){
            for i in cards.indices{
                if (!cards[i].isMatched){
                    cards[i].isFaceUp = (i == newValue)
                }
            }
        }
    }
    
    func chooseCard(at index: Int){
        if !cards[index].isMatched{
            if let matchIndex = indexOfOneAndOnlyFaceUpCard, matchIndex != index{
                if cards[matchIndex].identifier == cards[index].identifier{
                    cards[matchIndex].isMatched = true
                    cards[index].isMatched = true
                    score += 2;
                }
                else{
                    score -= 1;
                }
                cards[index].isFaceUp = true
            }
            else if index == indexOfOneAndOnlyFaceUpCard && cards[index].isFaceUp{
                indexOfOneAndOnlyFaceUpCard = nil
                score -= 1;
            }
            else{
                if cards[index].isFaceUp{
                    indexOfOneAndOnlyFaceUpCard = nil
                }
                else{
                    indexOfOneAndOnlyFaceUpCard = index
                }
            }
        }
    }
    
    init(pairs: Int){
        cards = Array<Card>()
        for _ in 1...pairs{
            let card = Card()
            cards.append(card)
            cards.append(card)
        }
    }
}
