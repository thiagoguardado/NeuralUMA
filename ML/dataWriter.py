# used to write DNA information to files

import json
import os
import shutil


def writeFile(data, name, path):
    file = open(path + "/" + name + ".json", "w")
    file.write(json.dumps(data))


def writeDNAs(dnaFields, dnasValues, race, path):
    # check if path exists
    checkDir(path)

    i = 0
    for dnaValue in dnasValues:
        writeDNA(dnaFields, dnaValue, race, "dna_"+str(i), path)
        i += 1


# write structured data o directory
def writeDNA(dnaFields, dnaValues, race, name, path):
    #  construct dict
    data = {}
    data["race"] = race
    data["fields"] = []
    for i in range(len(dnaFields)):
        data["fields"].append({"field": dnaFields[i], "value": dnaValues[i]})

    # write to file
    file = open(path + "/" + name + ".json", "w")
    file.write(json.dumps(data))

# clear a directory if exists or create it
def checkDir(dir):
    if os.path.exists(dir):
        shutil.rmtree(dir, ignore_errors=True)
    os.makedirs(dir)
