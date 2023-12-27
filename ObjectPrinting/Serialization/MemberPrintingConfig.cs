using System;
using System.Reflection;

namespace ObjectPrinting.Serialization
{
    public class MemberPrintingConfig<TOwner, TPropType> : IInnerPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly MemberInfo selectedMember;

        internal MemberPrintingConfig(PrintingConfig<TOwner> printingConfig, MemberInfo selectedMember)
        {
            this.printingConfig = printingConfig;
            this.selectedMember = selectedMember;
        }

        public PrintingConfig<TOwner> And()
        {
            return printingConfig;
        }

        public IInnerPrintingConfig<TOwner, TPropType> Using(Func<TPropType, string> serializer)
        {
            if (serializer is null)
                throw new ArgumentNullException(nameof(serializer));
            
            printingConfig.AddCustomMemberSerializer(selectedMember, serializer);
            return this;
        }

        public IInnerPrintingConfig<TOwner, TPropType> Wrap(Func<string, string> wrapper)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            if (printingConfig.TryGetCustomMemberSerializer(selectedMember, out var serializerDelegate) && serializerDelegate is Func<TPropType, string> serializer)
                printingConfig.AddCustomMemberSerializer<TPropType>(selectedMember, value => wrapper(serializer(value)));
            else
                printingConfig.AddCustomMemberSerializer<TPropType>(selectedMember,value => wrapper(value.ToString()));
            
            return this;
        }
    }
}