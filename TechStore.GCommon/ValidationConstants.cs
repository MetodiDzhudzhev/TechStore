namespace TechStore.GCommon
{
    public static class ValidationConstants
    {
        public static class User
        {
            public const int FullNameMinLength = 10;
            public const int FullNameMaxLength = 100;

            public const int AddressMinLength = 20;
            public const int AddressMaxLength = 200;
        }

        public static class Review
        {
            public const int CommentMinLength = 10;
            public const int CommentMaxLength = 200;

            public const int CreatedAtLength = 10;
            public const string CreatedAtFormat = "dd-MM-yyyy";
        }

        public static class Product
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 100;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 200;

            public const int QuantityInStockMinValie = 0;

            public const decimal PriceMinValue = 0.01m;
            public const decimal PriceMaxValue = 100000.00m;
            public const string PriceSqlType = "decimal(18, 2)";
        }

        public static class Order
        {
            public const int RecipientNameMaxLength = 100;

            public const int OrderDateLength = 10;
            public const string OrderDateFormat = "dd-MM-yyyy";

            public const int ShippingAddressMinLength = 20;
            public const int ShippingAddressMaxLength = 200;

            public const int PhoneNumberMaxLength = 13; // for example +359 XXX XXX XXX
            public const int EmailMaxLength = 254;

            public const int StripeSessionIdMaxLength = 255;
        }

        public static class Category
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
        }

        public static class Brand
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 30;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 500;
        }

        public static class Shared
        {
            public const int IntIdMinValue = 1;
            public const int IntIdMaxValue = int.MaxValue;
            public const long LongIdMaxValue = long.MaxValue;
        }
    }
}
