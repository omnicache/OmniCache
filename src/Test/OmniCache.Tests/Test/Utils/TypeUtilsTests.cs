using System;
using System.ComponentModel;
using OmniCache.Utils;
using Shouldly;
using Xunit;

namespace OmniCache.IntegrationTests.Test.Utils
{
    [Collection("Tests")]
    public class TypeUtilsTests
    {
        public TypeUtilsTests()
        {
        }

        [Fact]
        public async Task CanSystemConvertTypes()
        {
            CanConvert(typeof(int), typeof(int)).ShouldBe(true);
            CanConvert(typeof(int?), typeof(int)).ShouldBe(true);
            //CanConvert(typeof(int), typeof(int?)).ShouldBe(true);
            //CanConvert(typeof(int?), typeof(int?)).ShouldBe(true);

            //CanConvert(typeof(List<int>), typeof(List<int>)).ShouldBe(true);
            //CanConvert(typeof(List<int?>), typeof(List<int>)).ShouldBe(true);
            //CanConvert(typeof(List<int>), typeof(List<int?>)).ShouldBe(true);
            //CanConvert(typeof(List<int?>), typeof(List<int?>)).ShouldBe(true);

        }

        private bool CanConvert(Type fromType, Type toType)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(fromType);
            return converter.CanConvertTo(toType);
        }


        [Fact]
        public async Task TestSystemConvert()
        {
            System.Convert.ChangeType((int)7, typeof(int));
            //System.Convert.ChangeType((int)7, typeof(int?));        //fail
            System.Convert.ChangeType((int?)7, typeof(int));
            //System.Convert.ChangeType((int?)7, typeof(int?));         //fail

            System.Convert.ChangeType((int)7, typeof(long));
            System.Convert.ChangeType((long)7, typeof(int));

            System.Convert.ChangeType(true, typeof(int));
            System.Convert.ChangeType((int)7, typeof(bool));
        }


        [Fact]
        public async Task CanConvertNullableTypes()
        {
            TypeUtils.CanConvert(typeof(int), typeof(int)).ShouldBe(true);
            TypeUtils.CanConvert(typeof(int?), typeof(int)).ShouldBe(true);
            TypeUtils.CanConvert(typeof(int), typeof(int?)).ShouldBe(true);
            TypeUtils.CanConvert(typeof(int?), typeof(int?)).ShouldBe(true);

            TypeUtils.CanConvert(typeof(List<int>), typeof(List<int>)).ShouldBe(true);
            TypeUtils.CanConvert(typeof(List<int?>), typeof(List<int>)).ShouldBe(true);
            TypeUtils.CanConvert(typeof(List<int>), typeof(List<int?>)).ShouldBe(true);
            TypeUtils.CanConvert(typeof(List<int?>), typeof(List<int?>)).ShouldBe(true);

        }


        [Fact]
        public async Task ConvertNullableTypes()
        {
            TypeUtils.Convert((int)7, typeof(int)).ShouldBe(7);
            TypeUtils.Convert((int)7, typeof(int?)).ShouldBe(7);
            TypeUtils.Convert((int?)7, typeof(int)).ShouldBe(7);
            TypeUtils.Convert((int?)7, typeof(int?)).ShouldBe(7);

            TypeUtils.Convert((int?)null, typeof(int?)).ShouldBe(null);
            TypeUtils.Convert((int?)null, typeof(int)).ShouldBe(null);

            List<int> list = new List<int> { 2, 3, 4, 5};
            TypeUtils.Convert(list, typeof(List<int>));
            TypeUtils.Convert(list, typeof(List<int?>));
            TypeUtils.Convert(list, typeof(List<long>));
            TypeUtils.Convert(list, typeof(List<long?>));
        }

    }
}

