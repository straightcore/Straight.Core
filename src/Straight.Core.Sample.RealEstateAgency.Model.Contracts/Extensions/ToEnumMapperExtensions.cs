﻿using System;

namespace Straight.Core.Sample.RealEstateAgency.Model.Contracts.Extensions
{
    public static class ToEnumMapperExtensions
    {
        public static T ToMappedEnum<T>(this Enum dtoGender) where T : struct
        {
            T value;
            return Enum.TryParse(dtoGender.ToString(), out value) ? value : default(T);
        }
    }
}