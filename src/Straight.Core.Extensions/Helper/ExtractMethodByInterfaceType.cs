// ==============================================================================================================
// Straight Compagny
// Straight Core
// ==============================================================================================================
// ©2016 Straight Compagny. All rights reserved.
// Licensed under the MIT License (MIT); you may not use this file except in compliance
// with the License. You may obtain have a last condition or last licence at https://github.com/straightcore/Straight.Core/blob/master
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Straight.Core.Extensions.Helper
{
    public static class MappingTypeToMethodHelper
    {
        private const BindingFlags AcceptedBindingsFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        
        public static IReadOnlyDictionary<Type, MethodInfo> ToMappingTypeByInterfaceMethod(Type aggregatorType, Type genericParameterType, Type typeOfInterfaceBase, string methodName)
        {
            return new ReadOnlyDictionary<Type, MethodInfo>(aggregatorType.GetInterfaces()
                    .Where(t => IsGenericMethod(t.GetTypeInfo(), genericParameterType.GetTypeInfo(), typeOfInterfaceBase))
                    .ToDictionary(interfaceType => interfaceType.GetGenericArguments().FirstOrDefault(),
                                  interfaceType => interfaceType.GetMethod(methodName)));
        }

        private static bool IsGenericMethod(TypeInfo interfaceType, TypeInfo genericParameterType, Type typeOfInterfaceBase)
        {
            return interfaceType.IsGenericType
                   && (interfaceType.GetGenericTypeDefinition() == typeOfInterfaceBase);
        }

        public static IReadOnlyDictionary<Type, MethodInfo> ToMappingTypeMethod(Type sourceType, Type parameterType, Type returnType, string methodName)
        {
            var dictionary = sourceType.GetMethods(AcceptedBindingsFlags);
            var resolver = new ReadModelMethodInfoResolver(sourceType, parameterType, returnType, methodName);
            return new ReadOnlyDictionary<Type, MethodInfo>(resolver.Resolve(dictionary));
        }
                
        private class ReadModelMethodInfoResolver
        {
            private readonly Type _sourceType;
            private readonly Type _parameterType;
            private readonly Type _returnType;
            private readonly string _methodName;

            public ReadModelMethodInfoResolver(Type sourceType, Type parameterType, Type returnType, string methodName)
            {
                _sourceType = sourceType;
                _parameterType = parameterType;
                _returnType = returnType;
                _methodName = methodName;
            }

            public IDictionary<Type, MethodInfo> Resolve(IEnumerable<MethodInfo> methods)
            {
                return Filter(methods).ToDictionary(methodInfo => methodInfo.GetParameters().First().ParameterType,
                                                    methodInfo => methodInfo);
            }

            private IEnumerable<MethodInfo> Filter(IEnumerable<MethodInfo> methods)
            {
                return methods.Where(Filter);
            }

            private bool Filter(MethodInfo methodInfo)
            {
                return IfEqualToMethodName(methodInfo)
                    && IfParameterImplementInterface(methodInfo)
                    && IfEqualToReturnType(methodInfo);
            }

            private bool IfParameterImplementInterface(MethodInfo methodInfo)
            {
                return _parameterType == null 
                    || methodInfo.GetParameters().Count() == 1
                    && ExtractInterfaceTypeInFirstParameter(methodInfo) == _parameterType;
            }

            private Type ExtractInterfaceTypeInFirstParameter(MethodInfo methodInfo)
            {
                return methodInfo.GetParameters()
                                 .First()
                                 .ParameterType
                                 .GetTypeInfo()
                                 .GetInterface(_parameterType.Name);
            }

            private bool IfEqualToReturnType(MethodInfo methodInfo)
            {
                return _returnType == null || methodInfo.ReturnType == _returnType;
            }

            private bool IfEqualToMethodName(MethodInfo methodInfo)
            {
                return methodInfo.Name.Equals(_methodName, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}