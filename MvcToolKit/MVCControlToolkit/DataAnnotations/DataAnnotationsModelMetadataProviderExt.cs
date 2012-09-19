using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Resources;
using System.Globalization;
using System.Reflection;

namespace MVCControlsToolkit.DataAnnotations
{
    public class DataAnnotationsModelValidatorProviderExt : DataAnnotationsModelValidatorProvider
    {
        protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProviderExt(type).GetTypeDescriptor(type);
        }
    }
    public class DataAnnotationsModelMetadataProviderExt : DataAnnotationsModelMetadataProvider
    {
        
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            ModelMetadata res = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            FormatAttribute formatAttribute = attributes.OfType<FormatAttribute>().FirstOrDefault();
            if (formatAttribute != null)
            {
                string clientFormat;
                string prefix;
                string postfix;
                formatAttribute.GetClientFormat(out prefix, out postfix, out clientFormat);
                res.AdditionalValues.Add("MVCControlsToolkit.ClientFormatString", clientFormat);
                res.AdditionalValues.Add("MVCControlsToolkit.ClientFormatPrefix", prefix);
                res.AdditionalValues.Add("MVCControlsToolkit.ClientFormatPostfix", postfix);
            }
            DisplayAttribute display = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            string name = null;
            if (display != null)
            {
                res.Description = display.GetDescription();
                res.ShortDisplayName = display.GetShortName();
                res.Watermark = display.GetPrompt();
                

                name = display.GetName();
                if (!string.IsNullOrWhiteSpace(name)) res.DisplayName = name;
            }
            return res;
        }

        protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProviderExt(type).GetTypeDescriptor(type);
        }
    }
    public class AssociatedMetadataTypeTypeDescriptionProviderExt : System.ComponentModel.TypeDescriptionProvider
    {
        public AssociatedMetadataTypeTypeDescriptionProviderExt(Type x)
            : base(TypeDescriptor.GetProvider(x))
        {
        }
        private Type _associatedMetadataType;
        public AssociatedMetadataTypeTypeDescriptionProviderExt(Type x, Type y)
            : this(x)
        {
            if (y == null)
            {
                throw new ArgumentNullException("associatedMetadataType");
            }
            this._associatedMetadataType = y;
        }
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor typeDescriptor = base.GetTypeDescriptor(objectType, instance);
            return new AssociatedMetadataTypeTypeDescriptor(typeDescriptor, objectType, this._associatedMetadataType);
        }
        
    }
    internal class AssociatedMetadataTypeTypeDescriptor : CustomTypeDescriptor
    {
        private static class TypeDescriptorCache
        {
            private static readonly Attribute[] emptyAttributes = new Attribute[0];
            private static readonly ConcurrentDictionary<Type, Type> _metadataTypeCache = new ConcurrentDictionary<Type, Type>();
            private static readonly ConcurrentDictionary<Tuple<Type, string>, Attribute[]> _typeMemberCache = new ConcurrentDictionary<Tuple<Type, string>, Attribute[]>();
            private static readonly ConcurrentDictionary<Tuple<Type, Type>, bool> _validatedMetadataTypeCache = new ConcurrentDictionary<Tuple<Type, Type>, bool>();
            public static void ValidateMetadataType(Type type, Type associatedType)
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(type, associatedType);
                if (!AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache._validatedMetadataTypeCache.ContainsKey(key))
                {
                    AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache.CheckAssociatedMetadataType(type, associatedType);
                    AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache._validatedMetadataTypeCache.TryAdd(key, true);
                }
            }
            public static Type GetAssociatedMetadataType(Type type)
            {
                Type type2 = null;
                if (AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache._metadataTypeCache.TryGetValue(type, out type2))
                {
                    return type2;
                }
                MetadataTypeAttribute metadataTypeAttribute = (MetadataTypeAttribute)Attribute.GetCustomAttribute(type, typeof(MetadataTypeAttribute));
                if (metadataTypeAttribute != null)
                {
                    type2 = metadataTypeAttribute.MetadataClassType;
                }
                AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache._metadataTypeCache.TryAdd(type, type2);
                return type2;
            }
            private static void CheckAssociatedMetadataType(Type mainType, Type associatedMetadataType)
            {

            }
            public static Attribute[] GetAssociatedMetadata(Type type, string memberName)
            {
                Tuple<Type, string> key = new Tuple<Type, string>(type, memberName);
                Attribute[] customAttributes;
                if (AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache._typeMemberCache.TryGetValue(key, out customAttributes))
                {
                    return customAttributes;
                }
                MemberTypes type2 = MemberTypes.Field | MemberTypes.Property;
                BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
                MemberInfo memberInfo = type.GetMember(memberName, type2, bindingAttr).FirstOrDefault<MemberInfo>();
                if (memberInfo != null)
                {
                    customAttributes = Attribute.GetCustomAttributes(memberInfo, true);
                }
                else
                {
                    customAttributes = AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache.emptyAttributes;
                }
                AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache._typeMemberCache.TryAdd(key, customAttributes);
                return customAttributes;
            }
        }
        private Type AssociatedMetadataType
        {
            get;
            set;
        }
        public AssociatedMetadataTypeTypeDescriptor(ICustomTypeDescriptor parent, Type type, Type associatedMetadataType)
            : base(parent)
        {
            this.AssociatedMetadataType = (associatedMetadataType ?? AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache.GetAssociatedMetadataType(type));
            if (this.AssociatedMetadataType != null)
            {
                AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache.ValidateMetadataType(type, this.AssociatedMetadataType);
            }
        }
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return this.GetPropertiesWithMetadata(base.GetProperties(attributes));
        }
        public override PropertyDescriptorCollection GetProperties()
        {
            return this.GetPropertiesWithMetadata(base.GetProperties());
        }
        public override AttributeCollection GetAttributes()
        {
            AttributeCollection attributeCollection = base.GetAttributes();
            if (this.AssociatedMetadataType != null)
            {
                Attribute[] newAttributes = TypeDescriptor.GetAttributes(this.AssociatedMetadataType).OfType<Attribute>().ToArray<Attribute>();
                attributeCollection = AttributeCollection.FromExisting(attributeCollection, newAttributes);
            }
            return attributeCollection;
        }
        private PropertyDescriptorCollection GetPropertiesWithMetadata(PropertyDescriptorCollection originalCollection)
        {
            if (this.AssociatedMetadataType == null)
            {
                return originalCollection;
            }
            bool flag = false;
            List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor propertyDescriptor in originalCollection)
            {
                Attribute[] associatedMetadata = AssociatedMetadataTypeTypeDescriptor.TypeDescriptorCache.GetAssociatedMetadata(this.AssociatedMetadataType, propertyDescriptor.Name);
                PropertyDescriptor item = propertyDescriptor;
                if (associatedMetadata.Length > 0)
                {
                    item = new MetadataPropertyDescriptorWrapper(propertyDescriptor, associatedMetadata);
                    flag = true;
                }
                list.Add(item);
            }
            if (flag)
            {
                return new PropertyDescriptorCollection(list.ToArray(), true);
            }
            return originalCollection;
        }
    }
    internal class MetadataPropertyDescriptorWrapper : PropertyDescriptor
    {
        private PropertyDescriptor _descriptor;
        private bool _isReadOnly;
        public override Type ComponentType
        {
            get
            {
                return this._descriptor.ComponentType;
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return this._isReadOnly || this._descriptor.IsReadOnly;
            }
        }
        public override Type PropertyType
        {
            get
            {
                return this._descriptor.PropertyType;
            }
        }
        public override bool SupportsChangeEvents
        {
            get
            {
                return this._descriptor.SupportsChangeEvents;
            }
        }
        public MetadataPropertyDescriptorWrapper(PropertyDescriptor descriptor, Attribute[] newAttributes)
            : base(descriptor, newAttributes)
        {
            this._descriptor = descriptor;
            ReadOnlyAttribute readOnlyAttribute = newAttributes.OfType<ReadOnlyAttribute>().FirstOrDefault<ReadOnlyAttribute>();
            this._isReadOnly = (readOnlyAttribute != null && readOnlyAttribute.IsReadOnly);
        }
        public override void AddValueChanged(object component, EventHandler handler)
        {
            this._descriptor.AddValueChanged(component, handler);
        }
        public override bool CanResetValue(object component)
        {
            return this._descriptor.CanResetValue(component);
        }
        public override object GetValue(object component)
        {
            return this._descriptor.GetValue(component);
        }
        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            this._descriptor.RemoveValueChanged(component, handler);
        }
        public override void ResetValue(object component)
        {
            this._descriptor.ResetValue(component);
        }
        public override void SetValue(object component, object value)
        {
            this._descriptor.SetValue(component, value);
        }
        public override bool ShouldSerializeValue(object component)
        {
            return this._descriptor.ShouldSerializeValue(component);
        }
    }
}
