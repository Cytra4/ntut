library(tidyverse)
library(psych)
library(haven)
library(labelled)

#試著用期中的資料來做分析

data<-read_csv("./datasets/StudentPerformanceFactors.csv")
val_labels(data)<-NULL

#因為資料集中動機等級和家長介入程度都是類別變數，所以沒辦法放在Process分析
#解決: 建立傻瓜變項
#install.packages("fastDummies")
library(fastDummies)

data<-data %>% dummy_cols(select_columns = "Motivation_Level")
data<-data %>% dummy_cols(select_columns = "Parental_Involvement")

glimpse(data)

#其中動機等級和家長介入程度都分為低、中、高
#這邊兩個變數的低都當對照組

process(data = data,
        y = "Exam_Score",
        x = "Motivation_Level_Medium",
        m = c("Hours_Studied", "Attendance"),
        cov = c("Parental_Involvement_High",
                "Parental_Involvement_Medium",
                "Motivation_Level_High"),
        total = 1,
        seed = 12345,
        bmatrix = c(1, 1, 0, 0, 1, 1)
)

#a1 = 0.1237, c11 = -0.2837, c21 = -0.1535, c31 = -0.2324
#a2 = 0.1654, c12 = -0.3606, c22 = -0.4400, C32 = -0.2290
#b1 = 0.2959*, b2 = 0.1981*, c13 = 1.9033*, c23 = 0.8769*, c33 = 0.7723*
#a1 x b1 = [-0.0612 ~ 0.1366]
#a2 x b2 = [-0.0957 ~ 0.1632]
