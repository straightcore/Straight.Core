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
            return interfaceType.IsGenericType
                   && (interfaceType.GetGenericTypeDefinition() == typeOfInterfaceBase)
                //&& (interfaceType.GetGenericArguments().FirstOrDefault() != genericParameterType)
                ;
        }
    }
}