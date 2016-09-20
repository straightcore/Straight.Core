using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Straight.Core.Extensions.Helper
{
    public static class MappingTypeToMethodHelper
    {        
        public static IReadOnlyDictionary<Type, MethodInfo> ToMappingTypeMethod(
            Type aggregatorType,
            Type genericParameterType, 
            Type typeOfInterfaceBase, 
            string methodName)
        {
            return new ReadOnlyDictionary<Type, MethodInfo>(
                aggregatorType.GetInterfaces()
                    .Where(t => IsGenericMethod(t, genericParameterType, typeOfInterfaceBase))
                    .ToDictionary(
                        interfaceType => interfaceType.GetGenericArguments().FirstOrDefault(), 
                        interfaceType => interfaceType.GetMethod(methodName)));
        }

        private static bool IsGenericMethod(
            Type interfaceType,
            Type genericParameterType, 
            Type typeOfInterfaceBase)
        {
            var alreadyFound = false;
            return interfaceType.IsGenericType
                && interfaceType.GetGenericTypeDefinition() == typeOfInterfaceBase
                && (alreadyFound |= interfaceType.GetGenericArguments().FirstOrDefault() != genericParameterType);
        }

    }
}
