namespace Watchtower.Data;
using MongoDB.Driver;

/**
 * Representation of hosts, resources and requests in the database.
 * Hierarchical and easily customizable to work with different database back ends.
 */

public class WatchtowerRealm : IWatchtowerRealm {
    public List<WatchtowerHost> Hosts { get; set; }
    public IMongoClient Client;
    private IMongoDatabase Database;

    public WatchtowerRealm(string credentials, string databaseName) {
        var settings = MongoClientSettings.FromConnectionString($"mongodb+srv://{credentials}@watchtower.tbosq.mongodb.net/Watchtower?retryWrites=true&w=majority");
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        Client = new MongoClient(settings);
        Database = Client.GetDatabase(databaseName);
        Hosts = new List<WatchtowerHost>();
    }

    public void Update() {
        var hosts = Database.GetCollection<WatchtowerHost>("Hosts")
            .Find(x => true)
            .ToList();
        
        foreach (var host in hosts) {
            host.BindDatabase(Database);
            host.FetchResources();
        }

        Hosts = hosts;
    }

    public void AddHost(WatchtowerHost host) {
        // TODO
        return;
    }

    public void RemoveHost(WatchtowerHost host) {
        // TODO
        return;
    }
}

interface IWatchtowerRealm  {
    /**
     * Interface representing hosts, resources and requests in the database.
     */
    public List<WatchtowerHost> Hosts { get; set; }
    public void AddHost(WatchtowerHost host);
    public void RemoveHost(WatchtowerHost host);
    public void Update();

}
