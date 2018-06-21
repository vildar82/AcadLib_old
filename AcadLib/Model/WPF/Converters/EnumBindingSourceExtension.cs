namespace AcadLib.WPF.Converters
{
    using System;
    using System.Windows.Markup;
    using JetBrains.Annotations;

    [Obsolete]
    public static class EnumDescriptionExt
    {
        public static string Description(this object enumValue)
        {
            return EnumDescriptionTypeConverter.GetEnumDescription(enumValue);
        }
    }

    /// <summary>
    /// ComboBox ItemsSource="{Binding Source={local:EnumBindingSource {x:Type local:MyEnum}}}"    ///
    /// </summary>
    [Obsolete]
    [PublicAPI]
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public EnumBindingSourceExtension()
        {
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public Type EnumType
        {
            get => _enumType;
            set
            {
                if (value != _enumType)
                {
                    if (null != value)
                    {
                        var enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }

                    _enumType = value;
                }
            }
        }

        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == _enumType)
                throw new InvalidOperationException("The EnumType must be specified.");

            var actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            var enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == _enumType)
                return enumValues;

            var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}