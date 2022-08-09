using System.Collections.Generic;
using System.Linq;

namespace BSB.Microservices.NServiceBus.Common
{
    public class EnclosedMessageTypes
    {
        public List<MessageType> MessageTypes { get; } = new List<MessageType>();

        internal EnclosedMessageTypes(string typeString)
        {
            if (!string.IsNullOrEmpty(typeString))
            {
                var typeStrings = typeString.Split(";".ToCharArray());
                foreach (var s in typeStrings)
                {
                    MessageType mt = s;
                    MessageTypes.Add(mt);
                }
            }
        }

        /// <summary>
        /// Returns a delimited string excluding any assembly information
        /// </summary>
        /// <returns></returns>
        public string SerializeWithPartialTypeNames()
        {
            return string.Join(";", MessageTypes.Select(x => x.TypeName));
        }

        /// <summary>
        /// Iterates over the collection attempting to load each type until one is successful.
        /// </summary>
        /// <returns></returns>
        public bool AreAnyTypesLoadable()
        {
            foreach (var item in MessageTypes)
            {
                if (item.IsLoadable())
                {
                    return true;
                }
            }
            return false;
        }

        public bool AnySystemTypes()
        {
            foreach (var item in MessageTypes)
            {
                if (item.IsSystemType())
                {
                    return true;
                }
            }
            return false;
        }

        public static implicit operator EnclosedMessageTypes(string value)
        {
            return new EnclosedMessageTypes(value);
        }
    }

    public class MessageType
    {
        public string TypeName { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyVersion { get; set; }

        private string _original;

        internal MessageType(string value)
        {
            _original = value;
            var tokens = value.Split(",".ToCharArray());
            if (tokens.Length >= 1)
            {
                TypeName = tokens[0].Trim();
            }
            if (tokens.Length >= 2)
            {
                AssemblyName = tokens[1].Trim();
            }
            if (tokens.Length >= 3)
            {
                //contains commas, so we need to join it
                AssemblyVersion = string.Join(",", tokens.Skip(2)).Trim();
            }
        }

        /// <summary>
        /// Determines whether [is system type].  whether the type comes from nservice bus or this lib
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is system type]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSystemType()
        {
            if (AssemblyName != null)
            {
                if (AssemblyName.Equals("BSB.Microservices.NServiceBus", System.StringComparison.OrdinalIgnoreCase) ||
                        AssemblyName.StartsWith("NServiceBus", System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLoadable()
        {
            try
            {
                var type = System.Type.GetType(_original, false);
                return type != null ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public static implicit operator MessageType(string value)
        {
            return new MessageType(value);
        }
    }
}