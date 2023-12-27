using System;

namespace ObjectPrinting.Serialization
{
    public interface IInnerPrintingConfig<TOwner, TPropType>
    {
        IInnerPrintingConfig<TOwner, TPropType> Using(Func<TPropType, string> serializer);
        PrintingConfig<TOwner> And();

        IInnerPrintingConfig<TOwner, TPropType> Wrap(Func<string, string> wrapper);
    }
}