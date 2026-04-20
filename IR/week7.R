library(tidyverse)
library(psych)
library(haven)
data <- read_sav("./datasets/Album Sales.sav")
glimpse(data)

#Linear Regression
#lm(y ~ x, data = *The data you want to use*)
#*lm = linear model
fit<-lm(Sales ~ Adverts, data = data)
summary(fit)

#Summary of fit:
#What's important in Linear Regression is the quality of the model
#1. The closer R-squared's value to 1, the higher quality it has
#2. The more stars the X has next to Pr(>|t|), the better the X variable it is

#What does the (Intercept) means in Linear regression summary?
#A: When x = 0, what would the y's value be

#y = ax + b, b would be the Intercept, a would be the Estimate
#以Adverts為例: Estimate = 0.096 -> 每投入一單位的Adverts，Sales增加0.096

# Airplay -> Sales
fit<-lm(Sales ~ Airplay, data = data)
summary(fit)

#b = 84.8725, a = 3.9392
#假如投入1000單位的Airplay -> Sales = 3.9392 * 1000 + 84.8785 = 4024.0785

#When we only have 1 x variable, it is called Simple(簡單) Linear Regression
#When we have more than 1 x variables, it is called Multiple(多元) Linear Regression
fit<-lm(Sales ~ Adverts + Airplay, data = data)
summary(fit)

#What would the formula be when we have two x variables?
#A: y = 0.086 * x1(Adverts) + 3.58 * x2(Airplay) + 41.12

#Because Airplay has higher Estimate value than Adverts so it is better?
#Not always the case, cause different x variables might have different scaling 
#So we should look at the Z score to be accurate
fit<-lm(Sales ~ scale(Adverts) + scale(Airplay), data = data)
summary(fit)

#===================================================================

#Another example

happy <- read_sav("./datasets/happy.sav")
glimpse(happy)

fit_happy<-lm(happiness ~ income + health, data = happy)
summary(fit_happy)

fit_happy<-lm(happiness ~ scale(income) + scale(health), data = happy)
summary(fit_happy)

#===================================================================

#Now we try to use health.sav, where income and happy is X, health is Y

health <- read_sav("./datasets/health.sav")
glimpse(health)

fit_health<-lm(health ~ income + happy, data = health)
summary(fit_health)

fit_health<-lm(health ~ scale(income) + scale(happy), data = health)
summary(fit_health)

#Based on summary, seems like income doesn't have to do with health
#But that doesn't seem to make sense, so why is the result like that?
#That's because in Linear Regression, all the X variables are parallel relationship
#It is possible that income affects happy first, then happy affects health
#But in Linear Regression, it would assume income has nothing to do with happy

#So what can we do in this situation?
#We can use the Process Model
# *m = 中介變數
# In this situation, we use model = 4
process(data=health, y="health", x="income", m="happy", total=1, model=4, seed=12345)

#Based on the section of Outcome Variable: happy,
#we can see that income really affects happy

#In the Total, Direct, And Indirect Effects Of X On Y Section
#We can see that in "Indirect effect(s) of X on Y",
#Look at happy's BootLLCI value and BootULCI value
#*if 0 is not in the range of BootLLCI and BootULCI, it means that it is effective

#===================================================================
