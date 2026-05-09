library(tidyverse)
library(psych)
library(haven)

data<-read_sav("./datasets/sdt_cv.sav")

glimpse(data)

#中介變數分析
process(data=data, 
        y="performance", 
        x="leader", 
        m=c("auto", "control"), 
        total=1,
        seed=12345,
        bmatrix=c(1, 1, 0, 1, 1, 1)
        )

#如果今天也想要知道Tenure和Salary對auto, control和performance的影響呢?
data %>% describe()

#先做Pearson分析
#Pearson分析: 知道x變數和y變數的相關性
#範圍: -1 ~ 1

# install.packages("Hmisc")

library(Hmisc)

data %>% 
  as.matrix() %>% 
  Hmisc::rcorr(type = "pearson")

#在原本的中介分析中加入Covariates (控制變數)的變數
#所以現在的分析是在原本的中介迴歸分析上加上Tenure和Salary的控制
process(data = data,
        y = "performance",
        x = "leader",
        m = c("auto", "control"),
        cov = c("tenure", "salary"),
        total = 1,
        seed = 12345,
        bmatrix = c(1, 1, 0, 1, 1, 1)
        )	

#從分析的結果會發現任何變數中salary的coeff都是0、而且都不顯著
#不過從Pearson的結果可以看到salary對其他變數都有正相關的影響，為什麼?

#因為Pearson是一對一的關係，而中介分析是整體模型
#所以今天以salary和auto來看:
#在leader(X)相同、tenure相同的情況下，salary不會對auto有影響
#所以不是salary對auto不重要，而是leader和tenure對auto的影響更大
#因為從Pearson的結果來看salary確實對auto有正相關的影響

#========================================================================

data<-read_sav("./datasets/nsch.sav")

library(labelled)
val_labels(data)<-NULL

glimpse(data)

data %>% 
  as.matrix() %>% 
  Hmisc::rcorr(type = "pearson")

process(data = data,
        y = "TeethCond_21",
        x = "SC_AGE_YEARS",
        m = c("SugarDrink_21", "fruit_21", "vegetables_21"),
        cov = c("povlev4_21"),
        total = 1,
        seed = 12345,
        bmatrix = c(1,1,0,1,0,0,0,1,1,1)
)	

# a1 = 0.1575, c1 = -0.2229
# a2 = -0.1098, c2 = 0.1981
# a3 = -0.0821, c3 = 0.0968
# b1 = 0.0424, b2 = -0.0075, b3 = -0.0163, c4 = -0.0481
# sugardrink R^2 = 0.0808
# fruit21 R^2 = 0.0362
# vegetable21 R^2 = 0.0116
# Y R^2 = 0.0396
# a1xb1 = [0.0054 ~ 0.0080]
# a2xb2 = [0.0001 ~ 0.0016]
# a3xb3 = [0.0008 ~ 0.0019]