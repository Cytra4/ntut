# install.packages("writexl")
library(tidyverse)
library(haven)
library(psych)
library(writexl)

summer <- read_sav("summersurvey.sav")

summer

glimpse(summer)

#change variable type
# %>% -> then (shortcut: ctrl + shift + m)
# factor -> <fct> -> categorical variable
summer<-summer %>% 
  mutate(gender = as.factor(gender),
         Degree = as.factor(Degree),
         Goal = as.factor(Goal),
         Attitude = as.factor(Attitude),
         Excited = as.factor(Excited),
         Anxious = as.factor(Anxious))

glimpse(summer)

#Show the certain column in data
table(summer$gender)
#In percentages
prop.table(table(summer$gender)) * 100

table(summer$Goal)
prop.table(table(summer$Goal)) * 100

#Using Tidyverse's pipe to do the same thing
summer %>% select(gender) %>% table()

#==================================================

lab1 <- read_sav("lab 1 example data.sav")
glimpse(lab1)

lab1 <- lab1 %>% 
  mutate(gender = as.factor(gender),
         race = as.factor(race),
         scitype = as.factor(scitype),
         stutype = as.factor(stutype))

glimpse(lab1)

#List all values of CGPA and their count, then sort in descending order
lab1 %>% count(cgpa) %>% arrange(desc(cgpa))

table(lab1$greq)
prop.table(table(lab1$greq)) * 100
