using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Foundry.Services
{
    public class PartsQryGenerator<TEntity> : IPartsQryGenerator<TEntity> where TEntity : class
    {
        private PropertyInfo[] properties;
        private string[] propertiesNames;
        private string typeName;

        private string characterParameter;

        public PartsQryGenerator(char characterParameter = '@')
        {
            var type = typeof(TEntity);

            this.characterParameter = characterParameter.ToString();

            properties = type.GetProperties();
            propertiesNames = properties.Where(a => !IsComplexType(a)).Select(a => a.Name).ToArray();
            typeName = type.Name;
        }

        public string GeneratePartInsert(string identityField = null)
        {
            var result = string.Empty;

            var sb = new StringBuilder($"INSERT INTO [{typeName}] (");

            var list = new List<string> { identityField.ToLower(CultureInfo.InvariantCulture), "isactive", "isdeleted", "createddate", "modifieddate", "updateddate" };
            var propertiesNamesDef = propertiesNames.Where(a => !list.Contains(a.ToLower().Trim())).ToArray();

            string camps = string.Join(",", propertiesNamesDef);

            sb.Append($"{camps}) VALUES (");

            string[] parametersCampsCol = propertiesNamesDef.Select(a => $"{characterParameter}{a}").ToArray();

            string campsParameter = string.Join(",", parametersCampsCol);

            sb.Append($"{campsParameter})");

            result = sb + "; SELECT CAST(SCOPE_IDENTITY() as int)";

            return result;
        }

        public string GenerateSelect()
        {
            var result = string.Empty;

            var sb = new StringBuilder("SELECT ");

            string separator = $",{Environment.NewLine}";

            string selectPart = string.Join(separator, propertiesNames);

            sb.AppendLine(selectPart);

            string fromPart = $"FROM [{typeName}]";

            sb.Append(fromPart);

            result = sb.ToString();

            return result;
        }

        /// <summary>
        /// Soft Delete of an entity
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GenerateDelete(object parameters)
        {
            ParameterValidator.ValidateObject(parameters, nameof(parameters));

            var where = GenerateWhere(parameters);

            var result = $"Update [{typeName}] set isActive=0,isDeleted=1 OUTPUT INSERTED.Id {where} ";

            return result;
        }

        /// <summary>
        /// Physical Entity Delete
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GenerateDeleteEntity(object parameters)
        {
            ParameterValidator.ValidateObject(parameters, nameof(parameters));

            var where = GenerateWhere(parameters);

            var result = $"Delete From [{typeName}] {where} ";

            return result;
        }

        public string GenerateUpdate(object pks)
        {
            ParameterValidator.ValidateObject(pks, nameof(pks));

            var pksFields = pks.GetType().GetProperties().Select(a => a.Name.ToLower()).ToArray();

            var sb = new StringBuilder($"UPDATE [{typeName}] SET ");

            var propertiesNamesDef = propertiesNames.Where(a => !pksFields.Contains(a.ToLower())).ToArray();

            var propertiesSet = propertiesNamesDef.Select(a => $"{a} = {characterParameter}{a}").ToArray();

            var strSet = string.Join(",", propertiesSet);

            var where = GenerateWhere(pks);

            sb.Append($" {strSet} OUTPUT INSERTED.Id {where} ");

            var result = sb.ToString();

            return result;
        }

        public string GenerateSelect(object fieldsFilter)
        {
            ParameterValidator.ValidateObject(fieldsFilter, nameof(fieldsFilter));

            var initialSelect = GenerateSelect();

            var where = GenerateWhere(fieldsFilter);

            var result = $" {initialSelect} {where}";

            return result;
        }


        private string GenerateWhere(object filtersPKs)
        {
            ParameterValidator.ValidateObject(filtersPKs, nameof(filtersPKs));

            var filtersPksFields = filtersPKs.GetType().GetProperties().Select(a => a.Name).ToArray();

            if (!filtersPksFields?.Any() ?? true) { throw new ArgumentException($"El parameter filtersPks isn't valid. This parameter must be a class type", nameof(filtersPKs)); }

            var propertiesWhere = filtersPksFields.Select(a => $"{a} = {characterParameter}{a}").ToArray();

            var strWhere = string.Join(" AND ", propertiesWhere);

            var result = $" WHERE {strWhere} ";

            return result;
        }

        private bool IsComplexType(PropertyInfo propertyInfo)
        {
            bool result;

            result = (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType.Name != "String") || propertyInfo.PropertyType.IsInterface;

            return result;
        }

        #region Max Query

        public string GenerateMaxSelect(string identityField = null)
        {
            var result = string.Empty;

            var sb = new StringBuilder("SELECT ISNULL(Max(" + identityField.ToLower() + "),0)");

            string fromPart = $"FROM [{typeName}](NOLOCK)";

            sb.Append(fromPart);

            result = sb.ToString();

            return result;
        }

        #endregion

    }
}
