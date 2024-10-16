using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Utils
{
    /// <summary>
    /// A class to convert classes to data dictionaries
    /// Takes any string properties from a class and adds them to a dictionary
    /// Also allows default mappings via AddMapping, and precompilation of types via Precompile
    /// </summary>
    public interface IClassToDictionaryConverter
    {
        IDictionary<string, string> Convert<T>(T obj) where T : class;
        void AddMapping(Type type, Action<object, Dictionary<string, string>> action);
        void Precompile(params Type[] types);
    }

    /// <summary>
    /// A class to convert classes to data dictionaries
    /// Takes any string properties from a class and adds them to a dictionary
    /// Also allows default mappings via AddMapping, and precompilation of types via Precompile
    /// </summary>
    public class ClassToDictionaryConverter : IClassToDictionaryConverter
    {
        private readonly Dictionary<Type, Action<object, Dictionary<string, string>>>
             mappingCache = new Dictionary<Type, Action<object, Dictionary<string, string>>>();

        public IDictionary<string, string> Convert<T>(T obj) where T : class
        {
            var type = typeof(T)!;
            var dict = new Dictionary<string, string>();
            if (mappingCache.ContainsKey(type))
            {
                mappingCache[type].Invoke(obj, dict);
                return dict;
            }

            // Create an expression which constructs a dictionary and then assigns the properties to it
            var compiledFunc = CreateMapping(type);
            mappingCache[type] = compiledFunc;
            compiledFunc(obj, dict);
            return dict;
        }

        public void AddMapping(Type type, Action<object, Dictionary<string, string>> action)
        {
            mappingCache[type] = action;
        }

        public void AddMapping<T>(Action<object, Dictionary<string, string>> action) where T : class
        {
            AddMapping(typeof(T), action);
        }

        public void Precompile(params Type[] types)
        {
            foreach(var type in types)
            {
                mappingCache[type] = CreateMapping(type);
            }
        }

        private Type[] allowedValueTypes = new[]
        {
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(byte),
            typeof(byte?),
            typeof(decimal),
            typeof(decimal?),
            typeof(double),
            typeof(double?),
            typeof(float),
            typeof(float?),
            typeof(bool),
            typeof(bool?),
        };

        private Action<object, Dictionary<string, string>> CreateMapping(Type type)
        {
            var dictionaryType = typeof(Dictionary<string, string>);
            var toString = typeof(object).GetMethod("ToString");
            var dictionarySet = dictionaryType.GetMethod("Add")!;
            var objExpr = Expression.Parameter(typeof(object), "p");
            var castExpr = Expression.Convert(objExpr, type);
            var dictExpr = Expression.Parameter(dictionaryType, "dict");
            Expression expr = objExpr;
            var properties = type.GetProperties();
            var blockExpressions = new List<Expression> { };
            foreach (var property in properties)
            {
                if(property.PropertyType == typeof(string))
                {
                    var getFromObjExpr = Expression.Property(castExpr, property.Name);
                    var keyExpression = Expression.Constant(property.Name);
                    var setOnDictExpr = Expression.Call(
                        dictExpr,
                        dictionarySet,
                        keyExpression,
                        getFromObjExpr);
                    blockExpressions.Add(setOnDictExpr);
                } else if (allowedValueTypes.Contains(property.PropertyType))
                {
                    var getFromObjExpr = Expression.Property(castExpr, property.Name);
                    var keyExpression = Expression.Constant(property.Name);
                    var stringExpr = Expression.Call(getFromObjExpr, toString);
                    var setOnDictExpr = Expression.Call(
                        dictExpr,
                        dictionarySet,
                        keyExpression,
                        stringExpr);
                    blockExpressions.Add(setOnDictExpr);
                }
            }
            var blockExpression = Expression.Block(blockExpressions);
            var fullExpression = Expression.Lambda(blockExpression, objExpr, dictExpr);
            return (Action<object, Dictionary<string, string>>)fullExpression.Compile();
        }
    }
}
