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
# *categorical type: <fct> and <chr>
# *continuous type: <dbl> and <int>
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

#==================================================

#List more detailed info of the data
summary(summer)

#Another way to do it
describe(summer)

#*Tidyverse way of doing it
summer %>% summary()
summer %>% describe()

#Draw a bar chart
summer %>% 
  ggplot(mapping = aes (x = gender)) +
  geom_bar()

summer %>% 
  ggplot(mapping = aes (x = Goal)) +
  geom_bar()

#Draw a histograms chart
#*Difference usage of bar and histograms chart:
#Bar: Used for categorical, Histograms: Used for continuous
summer %>% 
  ggplot(mapping = aes (x = Temp)) +
  geom_histogram()

summer %>% 
  ggplot(mapping = aes (x = Money)) +
  geom_histogram()

#Draw a density chart
summer %>% 
  ggplot(mapping = aes (x = Temp)) +
  geom_density()

summer %>% 
  ggplot(mapping = aes (x = Money)) +
  geom_density()

#Draw a boxplot
# x = categorical, y = continuous
summer %>% 
  ggplot(mapping = aes (x = gender, y = Temp)) +
  geom_boxplot()

summer %>% 
  ggplot(mapping = aes (x = gender, y = Money)) +
  geom_boxplot()

#==================================================

#*fill can be used to color the bar
lab1 %>% 
  ggplot(mapping = aes (x = race, fill = race)) +
  geom_bar()

lab1 %>% 
  ggplot(mapping = aes (x = greq)) +
  geom_histogram()

lab1 %>% 
  ggplot(mapping = aes (x = race, y = greq)) +
  geom_boxplot()

#==================================================

#For Normal distribution, skew and kurtosis values should be in -3 to 3
#For Standard(Z) normal distribution, mean should be 0, sd(標準差) should be 1
summer %>% summary()

#In Z normal distribution, sd in -1 ~ 1 range means 68%, -2 ~ 2 range means 95%