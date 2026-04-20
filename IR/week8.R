library(tidyverse)
library(psych)
library(haven)

data<-read_sav("./datasets/sdt.sav")

glimpse(data)

#In week7.R, we uses process to analysis if income affects happy
#then happy affects health, so happy is the m variable

#What if we have more than 1 m variables today?
#*c = vector
#*What does the bmatrix mean? 
#It defines the relationship between the variables,
#From the process we defined below, the relationship table would be like this:
# (1, 1, 0, 1, 1, 1):
#     x    m1    m2
# m1  1    X     X
# m2  1    0     X
# y   1    1     1
# * 1 = connected, 0 = not connected, the bmatrix is sorted in rows
process(data=data, y="performance", x="leader", m=c("auto", "control"), total=1,
        seed=12345, bmatrix=c(1, 1, 0, 1, 1, 1))

#=======================================================================

#install.packages("labelled")
library(labelled)

data<-read_sav("./datasets/nsch.sav")
val_labels(data) <- NULL

process(data=data, y="TeethCond_21", x="SC_AGE_YEARS", m=c("SugarDrink_21", "fruit_21"),
        total=1, seed=12345, bmatrix=c(1, 1, 0, 1, 1, 1))