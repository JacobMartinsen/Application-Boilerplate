using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationManagement;

namespace Notes.Models
{
    [Table("Client", "ID", "NotesDB")]
    public class Client : iMigratable<Client>
    {
        [Field("ID", DataType.UUID, false)]
        public Guid ID;

        [Field("ClientName", DataType.VARCHAR, false)]
        public string ClientName;

        [Field("ProductName", DataType.VARCHAR, false)]
        public string ProductName;

        [Field("Title", DataType.VARCHAR, false)]
        public string Title;


        public Client()
        {

        }
    }
}
