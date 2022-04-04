using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

public class GameMessage
{
    public Guid id;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageType{
        REGISTER
    }
    public MessageType type;

    public string sender;
    public Player.State action;
    public int betchips;

    public GameTable table;

    public GameMessage() {}

    public GameMessage(Guid id, string sender, GameTable table)
    {
        this.id = id;
        this.sender = sender;
        this.table = table;
    }

}
