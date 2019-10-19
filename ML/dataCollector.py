# this script is used to collect data from MongoDb database
# data collected with no race attribute is considered to be from "HumanMale" race (previous data versions)

import pymongo
import bson.json_util as json_util
import json

# conecta to client
client = pymongo.MongoClient(
    "mongodb+srv://admin:admin@cluster0-vto77.gcp.mongodb.net/test?retryWrites=true&w=majority")
db = client.get_database("dna")

# iterate all collections
i = 0
for colName in db.list_collection_names():
    collection = db.get_collection(colName)

    # find all documents in collection
    cursor = collection.find({})

    # write all documents
    file = open("userData/" + str(i) + "_" + collection.name + ".json", "w")
    file.write('[')
    j = 0
    for document in cursor:
        if not "race" in document:
            document["race"] = "HumanMale"
        if (j > 0):
            file.write(",")
        file.write(json_util.dumps(document))
        j += 1
    file.write(']')
    i += 1
