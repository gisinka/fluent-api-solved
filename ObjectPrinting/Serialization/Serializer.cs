using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ObjectPrinting.Extensions;

namespace ObjectPrinting.Serialization
{
    internal class Serializer
    {
        private static readonly HashSet<Type> finalTypes =
        [
            typeof(int), typeof(double), typeof(float), typeof(string), typeof(DateTime), typeof(TimeSpan), typeof(Guid)
        ];
        
        private readonly HashSet<object> visited = [];
        private readonly HashSet<MemberInfo> excludedMembers;
        private readonly HashSet<Type> excludedTypes;
        private readonly Dictionary<MemberInfo, Delegate> memberSerializers;
        private readonly Dictionary<Type, Delegate> typeSerializers;
        private readonly Func<object, string> recursionHandler;

        public Serializer(
            HashSet<MemberInfo> excludedMembers,
            HashSet<Type> excludedTypes,
            Dictionary<MemberInfo, Delegate> memberSerializers,
            Dictionary<Type, Delegate> typeSerializers,
            Func<object, string> recursionHandler)
        {
            this.typeSerializers = typeSerializers;
            this.excludedMembers = excludedMembers;
            this.excludedTypes = excludedTypes;
            this.memberSerializers = memberSerializers;
            this.recursionHandler = recursionHandler;
        }

        public string Serialize(object obj, int nestingLevel)
        {
            if (obj is null)
                return string.Intern($"null{Environment.NewLine}");

            if (finalTypes.Contains(obj.GetType()))
                return $"{obj}{Environment.NewLine}";

            if (!visited.Add(obj))
            {
                if (recursionHandler is null)
                    throw new InvalidOperationException("Cyclic link detected");

                return $"{recursionHandler(obj)}{Environment.NewLine}";
            }
            
            var result =  obj switch
            {
                IEnumerable enumerable => SerializeEnumerable(enumerable, nestingLevel),
                _ => SerializeMember(obj, nestingLevel)
            };
            
            visited.Remove(obj);

            return result;
        }

        private string SerializeMember(object obj, int nestingLevel)
        {
            var type = obj.GetType();
            var sb = new StringBuilder($"{type.Name}{Environment.NewLine}");
            var indentation = string.Intern(new string('\t', nestingLevel + 1));

            foreach (var memberInfo in type.GetPropertiesAndFields().Where(x => !IsExcluded(x)))
            {
                string serializedMember;
                if (TryGetCustomSerializer(memberInfo, out var serializer))
                    serializedMember = $"{(string)serializer.DynamicInvoke(memberInfo.GetMemberValue(obj))}{Environment.NewLine}";
                else if (finalTypes.Contains(type))
                    serializedMember = $"{obj}{Environment.NewLine}";
                else
                    serializedMember = Serialize(memberInfo.GetMemberValue(obj), nestingLevel + 1);

                sb.Append($"{indentation}{memberInfo.Name} = {serializedMember}");
            }

            return sb.ToString();
        }

        private string SerializeEnumerable(IEnumerable enumerable, int nestingLevel)
        {
            var indentation = string.Intern(new string('\t', nestingLevel + 1));
            var sb = new StringBuilder($"{enumerable.GetType().Name}{Environment.NewLine}");

            foreach (var item in enumerable)
                sb.Append($"{indentation}{Serialize(item, nestingLevel + 1)}");

            return sb.ToString();
        }

        private bool TryGetCustomSerializer(MemberInfo memberInfo, out Delegate serializer)
        {
            if (memberSerializers.TryGetValue(memberInfo, out var memberSerializer))
            {
                serializer = memberSerializer;
                return true;
            }
            
            if (typeSerializers.TryGetValue(memberInfo.GetTypeOfMember(), out var typeSerializer))
            {
                serializer = typeSerializer;
                return true;
            }

            serializer = null;
            return false;
        }
        
        private bool IsExcluded(MemberInfo memberInfo)
        {
            return excludedMembers.Contains(memberInfo) || excludedTypes.Contains(memberInfo.GetTypeOfMember());
        }
    }
}