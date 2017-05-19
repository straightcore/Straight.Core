using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Straight.Core.Sample.RealEstateAgency.Contracts;

namespace Straight.Core.Sample.RealEstateAgency.Model.Contracts.Extensions
{
    public class RealEstateAgencyModelConverter : IModelConverter
    {
        private static readonly ToModelConverter Converter = new ToModelConverter();
        private static readonly ImmutableDictionary<Type, ImmutableDictionary<KeyType, MethodInfo>> MapperDtoToModelMethodInfo;
        
        static RealEstateAgencyModelConverter()
        {
            var excludeMethodName = new HashSet<string>(new[] { "ToString", "Equals", "GetHashCode", "GetType" });
            var methodsInfo = Converter.GetType()
                                       .GetMethods()
                                       .Where(m => !excludeMethodName.Contains(m.Name))
                                       .ToList();
            MapperDtoToModelMethodInfo = methodsInfo.GroupBy(m => m.ReturnType)
                                                    .ToImmutableDictionary(k => k.Key,
                                                                           m => m.ToImmutableDictionary(sm => CreateKey(sm.GetParameters())));
        }

        private static KeyType CreateKey(IReadOnlyList<ParameterInfo> parametersInfo)
        {
            return new KeyType(parametersInfo);
        }
        
        public TDto ToDto<TDto>(object model)
            where TDto : class
        {
            return ToInputToOutput<TDto>(model);
        }

        public TModel ToModel<TModel>(object dto)
            where TModel : class
        {
            return ToInputToOutput<TModel>(dto);
        }

        private static TOuput ToInputToOutput<TOuput>(object model)
            where TOuput : class
        {
            var typeInput = model.GetType();
            ImmutableDictionary<KeyType, MethodInfo> mapArgumentToMethod;
            if (!MapperDtoToModelMethodInfo.TryGetValue(typeof(TOuput), out mapArgumentToMethod))
            {
                throw new ArgumentException(
                                            $"Cannot convert {typeof(TOuput).Name} to {typeInput.Name}",
                                            new ArgumentOutOfRangeException($"{typeof(TOuput).Name} is not key"));
            }
            MethodInfo methodInfo;
            var key = new KeyType(new[] { typeInput });
            if (!mapArgumentToMethod.TryGetValue(key, out methodInfo))
            {
                throw new ArgumentException(
                                            $"Cannot convert {typeof(TOuput).Name} to {typeInput.Name}",
                                            new ArgumentOutOfRangeException($"{key} is not key"));
            }
            return (TOuput) methodInfo.Invoke(Converter, new[] { model });
        }

        private class KeyType : IEqualityComparer<KeyType>
        {
            private readonly Type[] _types;

            public KeyType(Type[] types)
            {
                _types = new Type[types.Length];
                for (var index = 0; index < types.Length; index++)
                {
                    _types[index] = types[index];
                }
            }

            public KeyType(IReadOnlyList<ParameterInfo> parameterInfos)
            {
                _types = new Type[parameterInfos.Count];
                for (var index = 0; index < parameterInfos.Count; index++)
                {
                    _types[index] = parameterInfos[index].ParameterType;
                }
            }

            public bool Equals(KeyType x, KeyType y)
            {
                if (x == null
                    && y == null)
                {
                    return true;
                }
                if (x == null
                    || y == null)
                {
                    return false;
                }
                return x._types.SequenceEqual(y._types);
            }

            public int GetHashCode(KeyType obj)
            {
                return obj.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                KeyType left;
                return (left = obj as KeyType) != null && Equals(this, left);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return _types.Aggregate(0, (current, type) => (current * 397) ^ type.GetHashCode());
                }
                
            }

            public override string ToString()
            {
                return string.Concat("{", string.Join(",", _types.Select(t => t.Name)), "}");
            }
        }
    }
}
