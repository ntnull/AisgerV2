using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Aisger.Helpers
{
    public static class DisplayNameHelper
    {
        public static string GetDisplayName(object obj, string propertyName)
        {
            if (obj == null) return null;
            return GetDisplayName(obj.GetType(), propertyName);

        }

        public static string GetDisplayName(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            if (property == null) return null;

            return GetDisplayName(property);
        }

        public static string GetDisplayName(PropertyInfo property)
        {
            var attrName = GetAttributeDisplayName(property);
            if (!string.IsNullOrEmpty(attrName))
                return attrName;

            var metaName = GetMetaDisplayName(property);
            if (!string.IsNullOrEmpty(metaName))
                return metaName;

            return property.Name.ToString();
        }

        private static string GetAttributeDisplayName(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return null;
            return (atts[0] as DisplayNameAttribute).DisplayName;
        }

        private static string GetMetaDisplayName(PropertyInfo property)
        {
            var atts = property.DeclaringType.GetCustomAttributes(
                typeof(MetadataTypeAttribute), true);
            if (atts.Length == 0)
                return null;

            var metaAttr = atts[0] as MetadataTypeAttribute;
            var metaProperty =
                metaAttr.MetadataClassType.GetProperty(property.Name);
            if (metaProperty == null)
                return null;
            return GetAttributeDisplayName(metaProperty);
        }
        

        public static string GetFieldDisplayName(Type type, string fieldName)
        {
            var field = type.GetField(fieldName);
            if (field == null) return null;

            return GetFieldDisplayName(field);
        }

        public static string GetFieldDisplayName(FieldInfo fieldInfo)
        {
            var attrName = GetAttributeDisplayName(fieldInfo);
            if (!string.IsNullOrEmpty(attrName))
                return attrName;

            var metaName = GetMetaDisplayName(fieldInfo);
            if (!string.IsNullOrEmpty(metaName))
                return metaName;

            return fieldInfo.Name.ToString();
        }

        private static string GetAttributeDisplayName(FieldInfo field)
        {
            var atts = field.GetCustomAttributes(
                typeof(DisplayAttribute), true);
            if (atts.Length == 0)
                return null;
            return (atts[0] as DisplayAttribute).Name;
        }

        private static string GetMetaDisplayName(FieldInfo field)
        {
            var atts = field.DeclaringType.GetCustomAttributes(
                typeof(MetadataTypeAttribute), true);
            if (atts.Length == 0)
                return null;

            var metaAttr = atts[0] as MetadataTypeAttribute;
            var metaProperty =
                metaAttr.MetadataClassType.GetProperty(field.Name);
            if (metaProperty == null)
                return null;
            return GetAttributeDisplayName(metaProperty);
        }
    }
}