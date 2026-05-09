import SwiftUI

struct ContentView: View {
    
    @State private var cookies = 0
    
    @State private var clickPower = 1
    @State private var upgradeCost = 10
    
    @State private var isPressed = false
    
    @State private var granny = 0
    @State private var grannyCost = 100
    
    @State private var farms = 0
    @State private var farmCost = 500
    
    @State private var mine = 0
    @State private var mineCost = 6000
    
    @State private var clicksThisSecond = 0
    @State private var manualCPS = 0.0
    
    var autoCPS: Double {
        Double(granny) * 1.0
        + Double(farms) * 8.0
        + Double(mine) * 67.0
    }
    
    var totalCPS: Double {
        autoCPS + manualCPS
    }
    
    struct FloatingText: Identifiable {
        let id = UUID()
        var value: Int
        var xOffset: CGFloat
        var yOffset: CGFloat = 0
        var opacity: Double = 1.0
    }
    
    @State private var floatingTexts: [FloatingText] = []
    
    func formatNumber(_ number: Double) -> String {
        let absNumber = abs(number)
        if (absNumber >= 1000000000) {
            return String(format: "%.2fB", number / 1_000_000_000)
        }
        else if (absNumber >= 1000000){
            return String(format: "%.2fM", number / 1_000_000)
        }
        else if (absNumber >= 1000){
            return String(format: "%.2fK", number / 1_000)
        }
        else{
            return String(Int(number))
        }
    }
    
    func spawnFloatingText(value: Int) {
        
        var newText = FloatingText(
            value: value,
            xOffset: CGFloat.random(in: -30...30)
        )
        
        floatingTexts.append(newText)
        
        let index = floatingTexts.count - 1
        
        withAnimation(.easeOut(duration: 1.0)) {
            floatingTexts[index].yOffset = -100
            floatingTexts[index].opacity = 0
        }
        
        DispatchQueue.main.asyncAfter(deadline: .now() + 1.0) {
            floatingTexts.removeAll { $0.id == newText.id }
        }
    }
    
    var body: some View {
        
        ZStack {
            
            Color(
                red: 66/255,
                green: 122/255,
                blue: 181/255
            )
            .ignoresSafeArea()
        
            VStack(spacing: 25) {
                
                Text("Cookies: \(formatNumber(Double(cookies)))")
                    .font(.system(size: 40, weight: .bold))
                    .foregroundColor(.white)
                    .padding(10)
                    .background(Color.black.opacity(0.5))
                    .cornerRadius(10)
                
                Text("CPS: \(formatNumber(totalCPS))")
                    .font(.title2)
                    .foregroundColor(.white)
                    .padding(10)
                    .background(Color.black.opacity(0.5))
                    .cornerRadius(10)
                
                Button {
                    cookies += clickPower
                    clicksThisSecond += 1
                    spawnFloatingText(value: clickPower)
                    
                    isPressed = true
                    withAnimation(.easeOut(duration: 0.1)) {
                        isPressed = false
                    }
                    
                } label: {
                    Image("cookie")
                        .resizable()
                        .frame(width: 300, height: 300)
                        .scaleEffect(isPressed ? 0.85 : 1.0)
                        .animation(.easeOut(duration: 0.1), value: isPressed)
                }
                .buttonStyle(.plain)
                
                ScrollView{
                    VStack(spacing: 15){
                        Button {
                            cookies -= upgradeCost
                            clickPower += 1
                            
                            upgradeCost = Int(
                                Double(upgradeCost) * 1.4
                            )
                            
                        } label: {
                            Text(
                                "Upgrade Lv.\(clickPower) (Cost: \(upgradeCost))"
                            )
                            .font(.title2)
                            .padding()
                            .frame(maxWidth: .infinity)
                            .background(
                                cookies >= upgradeCost
                                ? Color.white
                                : Color.gray.opacity(0.5)
                            )
                            .cornerRadius(10)
                        }
                        .disabled(cookies < upgradeCost)
                        
                        Button {
                            cookies -= grannyCost
                            granny += 1
                            
                            grannyCost = Int(
                                Double(grannyCost) * 1.15
                            )
                            
                        } label: {
                            Text(
                                "Granny (\(granny))  +1 CPS  (Cost: \(formatNumber(Double(grannyCost))))"
                            )
                            .font(.title2)
                            .padding()
                            .frame(maxWidth: .infinity)
                            .background(
                                cookies >= grannyCost
                                ? Color.white
                                : Color.gray.opacity(0.5)
                            )
                            .cornerRadius(10)
                        }
                        .disabled(cookies < grannyCost)
                        
                        Button {
                            
                            cookies -= farmCost
                            farms += 1
                            
                            farmCost = Int(
                                Double(farmCost) * 1.15
                            )
                            
                        } label: {
                            Text(
                                "Farm (\(farms))  +8 CPS  (Cost: \(formatNumber(Double(farmCost))))"
                            )
                            .font(.title2)
                            .padding()
                            .frame(maxWidth: .infinity)
                            .background(
                                cookies >= farmCost
                                ? Color.white
                                : Color.gray.opacity(0.5)
                            )
                            .cornerRadius(10)
                        }
                        .disabled(cookies < farmCost)
                        
                        Button {
                            cookies -= mineCost
                            mine += 1
                            
                            mineCost = Int(
                                Double(mineCost) * 1.15
                            )
                            
                        } label: {
                            Text(
                                "Mine (\(mine))  +67 CPS  (Cost: \(formatNumber(Double(mineCost))))"
                            )
                            .font(.title2)
                            .padding()
                            .frame(maxWidth: .infinity)
                            .background(
                                cookies >= mineCost
                                ? Color.white
                                : Color.gray.opacity(0.5)
                            )
                            .cornerRadius(10)
                        }
                        .disabled(cookies < mineCost)
                    }
                }
            }
            .padding()
            
            ForEach(floatingTexts) { text in
                Text("+\(text.value)")
                    .font(.title)
                    .fontWeight(.bold)
                    .foregroundColor(.white)
                    .offset(x: text.xOffset, y: text.yOffset)
                    .opacity(text.opacity)
            }
        }
        .onAppear {
            
            Timer.scheduledTimer(
                withTimeInterval: 1.0,
                repeats: true
            ) { _ in
                
                cookies += Int(autoCPS)
                
                manualCPS = Double(clicksThisSecond)
                    * Double(clickPower)
                
                clicksThisSecond = 0
            }
        }
    }
}

#Preview {
    ContentView()
}
