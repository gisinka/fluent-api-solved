using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ObjectPrinting.Serialization;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly HashSet<MemberInfo> excludedMembers = new();
        private readonly HashSet<Type> excludedTypes = new();
        private readonly Dictionary<MemberInfo, Delegate> membersSerializers = new();
        private readonly Dictionary<Type, Delegate> typeSerializers = new();

        private Func<object, string> handleMaxRecursion;

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        public PrintingConfig<TOwner> WithRecursionHandler(Func<object,string> recursionHandler)
        {
            handleMaxRecursion = recursionHandler;

            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> exclude)
        {
            if (exclude == null)
                throw new ArgumentNullException();

            var memberInfo = GetMemberInfo(exclude);
            excludedMembers.Add(memberInfo);

            return this;
        }


        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));

            return this;
        }

        public IInnerPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new TypePrintingConfig<TOwner, TPropType>(this);
        }

        public IInnerPrintingConfig<TOwner, T> Printing<T>(Expression<Func<TOwner, T>> property)
        {
            if (property == null)
                throw new ArgumentNullException();

            return new MemberPrintingConfig<TOwner, T>(this, GetMemberInfo(property));
        }

        internal void AddCustomTypeSerializer<TPropType>(Func<TPropType, string> serializer)
        {
            typeSerializers[typeof(TPropType)] = serializer;
        }
        
        internal void AddCustomMemberSerializer<TPropType>(MemberInfo member, Func<TPropType, string> serializer)
        {
            membersSerializers[member] = serializer;
        }

        internal bool TryGetCustomTypeSerializer<TPropType>(out Delegate serializer)
        {
            serializer = null;
            return typeSerializers.TryGetValue(typeof(TPropType), out serializer);
        }
        
        internal bool TryGetCustomMemberSerializer(MemberInfo memberInfo,out Delegate serializer)
        {
            serializer = null;
            return membersSerializers.TryGetValue(memberInfo, out serializer);
        }
        
        private static MemberInfo GetMemberInfo<TPropType>(Expression<Func<TOwner, TPropType>> expression)
        {
            var memberExpression = expression.Body is UnaryExpression unaryExpression
                ? (MemberExpression)unaryExpression.Operand
                : (MemberExpression)expression.Body;

            return memberExpression.Member;
        }
        
        private string PrintToString(object obj, int nestingLevel)
        {
            var serializer = new Serializer(
                excludedMembers,
                excludedTypes,
                membersSerializers,
                typeSerializers,
                handleMaxRecursion);

            return serializer.Serialize(obj, nestingLevel);
        }
    }
}