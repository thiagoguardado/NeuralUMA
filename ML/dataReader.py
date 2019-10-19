# used to read json files from a directory containing DNA information

import json
import yaml
import os

# read all files from directory
def readFiles(path):
    dnas = []
    for filename in os.listdir(path):
        if filename.endswith(".json"):
            with open(path + "/" + filename) as json_file:
                dans_read = json.load(json_file)
                for dna in dans_read:
                    dnas.append(dna)
    return dnas

# list all fields from files
def listFields(dnas):
    fields = []
    for dna in dnas:
        for pair in dna["fields"]:
            if not pair["field"] in fields:
                fields.append(pair["field"])
    fields.sort()
    return fields

# process data collected from files using a list of fields
def processData(dnas, fields):
    processedDna = {
        "Fields": fields,
        "HumanMale": {"plausible": [], "notPlausible": []},
        "HumanFemale": {"plausible": [], "notPlausible": []}
    }

    for dna in dnas:
        values = []
        for field in fields:
            hasField = False
            for pairs in dna["fields"]:
                if pairs["field"] == field:
                    values.append(pairs["value"])
                    hasField = True
                    break
            if not hasField:
                values.append(None)

        processedDna[dna["race"]]["plausible" if dna["plausible"]
                                     else "notPlausible"].append(values)

    return processedDna


def readPath(path):
    dnas = readFiles(path)
    fields = listFields(dnas)
    processedDna = processData(dnas, fields)
    return processedDna
