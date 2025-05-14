using System;
using System.Collections.Generic;

namespace data.RemoteData.RemoteDatabase.DAO;

public class UserDAO
{
    public int UserId { get; set; }

    public required string FIO { get; set; }

    public int GroupId { get; set; }

    public virtual GroupDAO Group { get; set; }

    public virtual ICollection<PresenceDAO> Presences { get; set; } = new List<PresenceDAO>();
}