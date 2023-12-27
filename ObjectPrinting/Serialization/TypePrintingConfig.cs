using System;

namespace ObjectPrinting.Serialization
{
    public class TypePrintingConfig<TOwner, TPropType> : IInnerPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;

        internal TypePrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }

        public PrintingConfig<TOwner> And()
        {
            return printingConfig;
        }

        public IInnerPrintingConfig<TOwner, TPropType> Using(Func<TPropType, string> serializer)
        {
            if (serializer is null)
                throw new ArgumentNullException(nameof(serializer));
            
            printingConfig.AddCustomTypeSerializer(serializer);
            
            return this;
        }

        public IInnerPrintingConfig<TOwner, TPropType> Wrap(Func<string, string> wrapper)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            if (printingConfig.TryGetCustomTypeSerializer<TPropType>(out var serializerDelegate) && serializerDelegate is Func<TPropType, string> serializer)
                printingConfig.AddCustomTypeSerializer<TPropType>(value => wrapper(serializer(value)));
            else
                printingConfig.AddCustomTypeSerializer<TPropType>(value => wrapper(value.ToString()));
            
            return this;
        }
    }
}