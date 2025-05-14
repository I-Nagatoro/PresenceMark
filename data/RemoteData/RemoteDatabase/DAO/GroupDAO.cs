using System;
using System.Collections.Generic;

namespace data.RemoteData.RemoteDatabase.DAO;

public class GroupDAO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual List<UserDAO> Users { get; set; }
}