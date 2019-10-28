# this script is used to send data to mongodb
import pymongo


def send(data,database,collection):
    # conecta to client
    client = pymongo.MongoClient(
        "mongodb+srv://admin:admin@cluster0-vto77.gcp.mongodb.net/test?retryWrites=true&w=majority")
    db = client.get_database(database)
    collection = db.get_collection(collection)
    collection.insert_one(data)
