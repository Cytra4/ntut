library(tidyverse)
library(psych)
library(haven)

data<-read.csv("StudentPerformanceFactors.csv")

glimpse(data)

#題目: 分析不同類型因素對學生表現的影響

#1.家庭與學校環境對學生表現影響
fit<-lm(Exam_Score ~
          Parental_Involvement +
          Parental_Education_Level +
          Family_Income +
          Teacher_Quality +
          Access_to_Resources +
          Peer_Influence
        , data=data)
summary(fit)

#2.學生個人因素對自身表現影響
fit2<-lm(Exam_Score ~
          Hours_Studied +
          Attendance +
          Extracurricular_Activities +
          Tutoring_Sessions +
          Sleep_Hours +
          Internet_Access +
          Physical_Activity +
          Motivation_Level +
          Previous_Scores
        , data=data)
summary(fit2)