using System;
using System.Collections.Generic;
using System.Linq;
using BSB.Microservices.NServiceBus.Common;
using NServiceBus;

namespace BSB.Microservices.NServiceBus.Serialization
{
    /// <summary>
    /// Given a partial type string, attempts to resolve the type by inspecting a collection of known assemblies.
    /// </summary>
    public class PartialTypeResolver
    {
        private readonly List<string> _names = new List<string>();

        private const int _headerSize = 256;

        /// <summary>
        /// Registers the assembly names
        /// </summary>
        /// <param name="names">The names.</param>
        public void RegisterAssemblyNames(params string[] names)
        {
            _names.AddRange(names);
        }

        /// <summary>
        /// Resolves for message.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public string ResolveForMessage(Dictionary<string, string> headers)
        {
            var messageTypeKey = "NServiceBus.EnclosedMessageTypes";
            if (!headers.TryGetValue(messageTypeKey, out var messageType))
            {
                return null;
            }
            EnclosedMessageTypes enclosedMessageTypes = messageType;
            if (!enclosedMessageTypes.AreAnyTypesLoadable() && _names.Any())
            {
                //take the first type and try to resolve it.
                var first = enclosedMessageTypes.MessageTypes.First();
                foreach (var name in _names)
                {
                    var typeString = $"{first.TypeName}, {name}";
                    var type = Type.GetType(typeString, false);
                    var typeStringBuilder = new List<string>();
                    var imessageType = typeof(IMessage);
                    if (type != null)
                    {
                        var temp = type;

                        while (temp != null && imessageType.IsAssignableFrom(temp))
                        {
                            typeStringBuilder.Add(type.AssemblyQualifiedName);
                            temp = temp.BaseType;
                        }
                    }

                    if (typeStringBuilder.Any())
                    {
                        headers[messageTypeKey] = new LengthConstrainedTypeString(_headerSize, typeStringBuilder, ";").TypeString;// string.Join(";", typeStringBuilder);
                    }
                }
            }
            return null;
        }
    }

    public class LengthConstrainedTypeString
    {
        public string TypeString { get; }

        public LengthConstrainedTypeString(int length, List<string> typeStrings, string delimiter)
        {
            //given a list of strings, build a string of length at most {length}
            //most specific type will be at the head of the list.
            var serializedString = string.Join(delimiter, typeStrings);
            if (serializedString.Length < length)
            {
                TypeString = serializedString;
            }
            else
            {
                int counter = 0;
                int index = 0;
                List<string> temp = new List<string>();
                foreach (var item in typeStrings)
                {
                    if (counter >= length)
                    {
                        break;
                    }
                    else
                    {
                        counter += item.Length;
                        if (index != typeStrings.Count - 1)
                        {
                            //last element has no delimeter
                            counter = counter + delimiter.Length;
                        }
                        if (counter >= length)
                        {
                            break;
                        }
                        temp.Add(item);
                    }
                    index++;
                }
                TypeString = string.Join(delimiter, temp);
            }
        }
    }
}