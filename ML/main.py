# start training and saves files on directory

from dataReader import readPath
from dataWriter import writeDNAs
from machineLearning import runML

# run machine learning and save epoches snapshot
def train(data, race):
    trainedDNAs = runML(data[race]["plausible"])
    outputDir = 'C:/Users/Thiago/Documents/Unity Projects/NeuralUMA/Assets/StreamingAssets/GanData/' + race
    i = 0
    for key, value in trainedDNAs.items():
        writeDNAs(data["Fields"], value, race, outputDir + "/" + "{:02d}".format(i) + " - " + key + " epoches")
        i += 1

#  colect data from directory
data = readPath("inputData")

# run models
train(data, "HumanMale")
train(data, "HumanFemale")