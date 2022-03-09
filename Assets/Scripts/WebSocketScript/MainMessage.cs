
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using System;

[Serializable]
public class MainMessage

{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MessageType{
		[EnumMember(Value = "SIGNUP")]
		SIGNUP,

		[EnumMember(Value = "DENIED")]
		DENIED,

		[EnumMember(Value = "CHAT")] 
		CHAT, 

		[EnumMember(Value = "REMOVE")]
		REMOVE

	}

	public MessageType type;
	
	public string name;
	public string msg;

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
}
