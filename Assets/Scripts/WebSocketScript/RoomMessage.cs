using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

[Serializable]
public class RoomMessage
{
	public Guid id;
	
	/* Summary
     * INIT : First initialization when player sign in.
	 * REGISTER : When player registers room.
	 * INVITE : When player sends invitation to other player.
	 * REJECT : When player rejects invitation.
	 * UPDATE : When updating room's feature.
	 * LEAVE : When some player leaves the room.
	 * REMOVE : When some player turns off the game completely.
	 */
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MessageType{
		[EnumMember(Value = "INIT")]
		INIT,
		[EnumMember(Value = "REGISTER")]
		REGISTER,
		[EnumMember(Value = "ENTER")]
		ENTER,
		[EnumMember(Value = "LEAVE")]
		LEAVE,
		[EnumMember(Value = "INVITE")]
		INVITE,
		[EnumMember(Value = "UPDATE")]
		UPDATE,
		[EnumMember(Value = "REMOVE_ROOM")]
		REMOVE_ROOM,
		[EnumMember(Value = "GET")]
		GET

	}
	public MessageType type;
	
	public string sender;
	public string receiver;
	
	public Room room;
	public string msg;

	public Dictionary<Guid, Room> roomMap;
	public RoomMessage() {}

	public RoomMessage(MessageType type) 
	{
		this.type = type;
	}

    public RoomMessage(Guid id, MessageType type, string sender)
	{
		this.id = id;
		this.type = type;
		this.sender = sender;
	}
	public RoomMessage(Guid id, MessageType type, Room room)
	{
		this.id = id;
		this.type = type;
		this.room = room;
	}
	public RoomMessage(Guid id, MessageType type, string sender, Room room)
	{
		this.id = id;
		this.type = type;
		this.sender = sender;
		this.room = room;
	}
	public RoomMessage(Guid id, MessageType type, string sender, string receiver) 
	{
		this.id = id;
		this.type = type;
		this.sender = sender;
		this.receiver = receiver;
	}
	public RoomMessage(Guid id, MessageType type, string sender, string receiver, Room room) 
	{
		this.id = id;
		this.type = type;
		this.sender = sender;
		this.receiver = receiver;
		this.room = room;
	}
	public RoomMessage(Guid id, MessageType type, string sender, string receiver, Room room, string msg)
	{
		this.id = id;
		this.type = type;
		this.sender = sender;
		this.receiver = receiver;
		this.room = room;
		this.msg = msg;
	}

}