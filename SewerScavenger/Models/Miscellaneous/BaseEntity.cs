using SQLite;

namespace SewerScavenger.Models
{
    // This is the base class, that everything comes from that gets saved in the DB.
    // The fields requried by the DB for all records are here.

    public class BaseEntity<T>
    {
        // Datbase record key for the entry into the database, used to fetch the data
        [PrimaryKey]
        public string Id { get; set; }

        // Used for List Identification, not used for game interaction or exposed to users
        public string Guid { get; set; }
    }
}
