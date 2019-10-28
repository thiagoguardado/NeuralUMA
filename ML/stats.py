from dataCollector import collectAll
from dataAnalyzer import justAnalyze
from dataReader import readPath

collectAll("dna")
data = readPath("inputData")
print("--- Random Generation ---")
justAnalyze(data)

print();

collectAll("trainedDNA")
data = readPath("inputData")
print("--- Trained Data ---")
justAnalyze(data)