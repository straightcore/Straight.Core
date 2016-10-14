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
using System.Linq;
using System.Text.RegularExpressions;

namespace Straight.Core.Extensions.Guard
{
    public static class Guard
    {
        public static void CheckIfArgumentIsNull<T>(this T source, string name)
        {
            if (source == null)
                throw new ArgumentNullException(name);
        }

        public static void CheckIfArgumentIsNullOrEmpty(this string source, string name)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(name);
        }

        public static void CheckIfArgumentIsNullOrEmpty(this IEnumerable<object> source, string name)
        {
            var enumerable = source as object[] ?? source as IList<object> ?? source.ToList();
            enumerable.CheckIfArgumentIsNull(name);
            if (!enumerable.Any())
                throw new ArgumentNullException(name);
        }

        public static void CheckRegexValidity(this Regex regex, string value, string name)
        {
            if (!regex.IsMatch(value))
                throw new ArgumentException($"{name} format is not valid");
        }
    }
}