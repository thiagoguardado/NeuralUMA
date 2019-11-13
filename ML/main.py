# start training and saves files on directory

from dataCollector import collectAll
from dataAnalyzer import justAnalyze
from dataReader import readPath
from dataWriter import writeFile
from machineLearning import runML
from dataSender import send

# run machine learning and save epoches snapshot
output = {}
output["data"] = []

finalOutput = {}
finalOutput["finalData"] = []

# train model and add to output
def train(data, race):
  # run machine learning
  trainedDNAs = runML(data[race]["plausible"],race)

  # epoches
  i = 1
  raceOutput = {}
  raceOutput["key"] = race
  raceOutput["values"] = []
  for epoch, dnaList in trainedDNAs["epoches"].items():
    epochOutput = {}
    epochOutput["key"] = "{:02d}".format(i) + " - " + epoch + " epoches"
    epochOutput["values"] = []
    for trainedDNA in dnaList:
        #  construct dna
        dna = {}
        dna["race"] = race
        dna["fields"] = []
        for j in range(len(data["Fields"])):
            dna["fields"].append({"field": data["Fields"][j], "value": trainedDNA[j]})
        epochOutput["values"].append(dna)
    i += 1
    raceOutput["values"].append(epochOutput)
  
  output["data"].append(raceOutput)

  # final
  raceOutput = {}
  raceOutput["key"] = race
  raceOutput["values"] = []

  for trainedDNA in trainedDNAs["final"]:
    #  construct dna
    dna = {}
    dna["race"] = race
    dna["fields"] = []
    for j in range(len(data["Fields"])):
        dna["fields"].append({"field": data["Fields"][j], "value": trainedDNA[j]})
    raceOutput["values"].append(dna)

  finalOutput["finalData"].append(raceOutput)

#  colect data from database and read from directory
collectAll("dna")
data = readPath("inputData")
justAnalyze(data)

# run models
train(data, "HumanMale")
train(data, "HumanFemale")

# #  send to database
# send(output,"gan_output","output")
# send(finalOutput,"gan_final_output","output")

# writeFile(output,"GanOutput","C:/Users/Thiago/Documents/Unity Projects/NeuralUMA/Assets/StreamingAssets/GanData")