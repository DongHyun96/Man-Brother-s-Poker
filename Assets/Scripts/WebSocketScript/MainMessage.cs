
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using System;

[Serializable]
public class MainMessage

{
	/**Summary
	 * SIGNUP : When player request singup. / or / By server broadcasting, some player made valid signUp.
	 * DENIED : When player's signup request denied.
	 * CHAT : Chat in Lobby panel.
	 * REMOVE : When some player turns off the game completely.
	 * UPDATE : Update player's isInvitable etc.
	 */
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MessageType{
		[EnumMember(Value = "SIGNUP")]
		SIGNUP,

		[EnumMember(Value = "DENIED")]
		DENIED,

		[EnumMember(Value = "CHAT")] 
		CHAT, 

		[EnumMember(Value = "REMOVE")]
		REMOVE,

		[EnumMember(Value = "UPDATE")]
		UPDATE,

		[EnumMember(Value = "GET")]
		GET

	}

	public MessageType type;
	
	public string name;
	public string msg;

	public bool invitable;

	public Dictionary<string, Player> playerMap; 
	public MainMessage() {}

	public MainMessage(MessageType type)
	{
		this.type = type;
	}
	
	public MainMessage(MessageType type, string name)
	{
		this.type = type;
		this.name = name;
	}
	public MainMessage(MessageType type, string name, string msg)
	{
		this.type = type;
		this.name = name;
		this.msg = msg;
	}
	public MainMessage(MessageType type, string name, bool invitable)
	{
		
	}
}
