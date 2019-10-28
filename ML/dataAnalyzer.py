from dataCollector import collectAll
from dataReader import readPath

def analyze(data,race):
  plausible = len(data[race]["plausible"])
  notPlausible = len(data[race]["notPlausible"])
  total = plausible + notPlausible
  print(race)
  print("Plausible: " +str(plausible))
  print("Not Plausible: " +str(notPlausible))
  print("Total: " + str(total))
  print("Plausible rate: " + str(plausible/total))
  print()

def collectAndAnalyze():
  collectAll("dna")
  data = readPath("inputData")
  justAnalyze(data)

def justAnalyze(data):
  analyze(data, "HumanMale")
  analyze(data, "HumanFemale")