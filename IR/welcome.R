# install and load packages -----------------------------------------------
# install.packages("tidyverse")
# install.packages("psych")
# install.packages("haven")

library(tidyverse)
library(psych)
library(haven)

# import data -------------------------------------------------------------
data<-read_sav("welcome.sav")

#check our data
data

# visualization -----------------------------------------------------------
data %>% 
  ggplot(aes(group, te_2, color = group)) +
  geom_boxplot()

# model -------------------------------------------------------------------
#independent samples t-test
t.test(te_2 ~ group, data = data) 
