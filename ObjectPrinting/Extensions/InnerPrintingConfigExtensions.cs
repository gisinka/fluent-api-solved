using System;
using System.Globalization;
using ObjectPrinting.Serialization;

namespace ObjectPrinting.Extensions;

public static class InnerPrintingConfigExtensions
{
    public static IInnerPrintingConfig<TOwner, TPropType> Using<TOwner, TPropType>(this IInnerPrintingConfig<TOwner, TPropType> config,
        CultureInfo culture)
        where TPropType : IFormattable
    {
        return config.Using(value => value.ToString(null, culture));
    }

    public static IInnerPrintingConfig<TOwner, TPropType> Trim<TOwner, TPropType>(
        this IInnerPrintingConfig<TOwner, TPropType> config,
        int length)
    {
        return config.Wrap(value => value.Length <= length ? value : value[..length]);
    }
}