# this script is used to send data to mongodb
import pymongo
import json

def send(data,database,collection):

    url = ""

    with open("key/key.json") as json_file:
        keyRead = json.load(json_file)
        url = keyRead["connURL"]

    # conecta to client
    client = pymongo.MongoClient(url)
    db = client.get_database(database)
    collection = db.get_collection(collection)
    collection.insert_one(data)
