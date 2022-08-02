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
        REGISTER, DECK, NEXT_DECK, TOSS, SHOWDOWN, LEAVE, DUMMY, READY_CHECK
    }
    public MessageType type;

    public string sender;

    public GameTable table;

    public List<bool> cardShowDown = new List<bool>(); // [isOpenFirstCard, isOpenSecondCard]

    public List<Card> deck = new List<Card>();

    public GameMessage() {}

    public GameMessage(MessageType type)
    {
        this.type = type;
    }

    public GameMessage(Guid id, MessageType type)
    {
        this.id = id;
        this.type = type;
    }

    public GameMessage(Guid id, MessageType type, string sender, GameTable table)
    {
        this.id = id;
        this.type = type;
        this.sender = sender;
        this.table = table;
    }

    public GameMessage(Guid id, MessageType type, string sender, List<bool> cardShowDown)
    {
        this.id = id;
        this.type = type;
        this.sender = sender;
        this.cardShowDown = cardShowDown;
    }

/*     public GameMessage(Guid id, MessageType type, List<Card> deck)
    {
        this.id = id;
        this.type = type;
        this.deck = deck;
    } */

}
