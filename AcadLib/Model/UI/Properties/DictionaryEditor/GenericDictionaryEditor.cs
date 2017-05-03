using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Globalization;

namespace AcadLib.UI.Designer
{
    /// <summary>
    /// A <see cref="System.Drawing.Design.UITypeEditor">UITypeEditor</see> for editing generic dictionaries in the <see cref="System.Windows.Forms.PropertyGrid">PropertyGrid</see>.
    /// </summary>
    /// <typeparam name="TKey">The type of the Keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the Values in the dictionary.</typeparam>
    public class GenericDictionaryEditor<TKey, TValue>: CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the GenericDictionaryEditor class using the specified collection type.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public GenericDictionaryEditor(Type type)
            : base(type)
        {

        }

        private GenericDictionaryEditorAttribute m_EditorAttribute;

        private CollectionForm m_Form;

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>A <see cref="CollectionEditor.CollectionForm"/>  to provide as the user interface for editing the collection.</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            m_Form = base.CreateCollectionForm();
            m_Form.Text = m_EditorAttribute.Title ?? "Редактор словаря";

            // Die Eigenschaft "CollectionEditable" muss hier per Reflection gesetzt werden (ist protected)
            var formType = m_Form.GetType();
            var pi = formType.GetProperty("CollectionEditable", BindingFlags.NonPublic | BindingFlags.Instance);
            pi.SetValue(m_Form, true, null);

            return m_Form;
        }

        /// <summary>
        /// Edits the value of the specified object using the specified service provider and context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information. </param>
        /// <param name="provider">A service provider object through which editing services can be obtained.</param>
        /// <param name="value">The object to edit the value of.</param>
        /// <returns>The new value of the object. If the value of the object has not changed, this should return the same object it was passed.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var attribute = context.PropertyDescriptor.Attributes[typeof(GenericDictionaryEditorAttribute)] as GenericDictionaryEditorAttribute;
            if (attribute != null)
            {
                m_EditorAttribute = attribute;

                if (m_EditorAttribute.KeyDefaultProviderType == null)
                    m_EditorAttribute.KeyDefaultProviderType = typeof(DefaultProvider<TKey>);

                if (m_EditorAttribute.ValueDefaultProviderType == null)
                    m_EditorAttribute.ValueDefaultProviderType = typeof(DefaultProvider<TValue>);
            }
            else
            {
                m_EditorAttribute = new GenericDictionaryEditorAttribute();
                m_EditorAttribute.KeyDefaultProviderType = typeof(DefaultProvider<TKey>);
                m_EditorAttribute.ValueDefaultProviderType = typeof(DefaultProvider<TValue>);
            }

            return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Gets the data type that this collection contains.
        /// </summary>
        /// <returns>The data type of the items in the collection, or an Object if no Item property can be located on the collection.</returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof(EditableKeyValuePair<TKey, TValue>);
        }

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified type.</returns>
        protected override object CreateInstance(Type itemType)
        {
            TKey key;
            TValue value;

            var KeyDefaultProvider = Activator.CreateInstance(m_EditorAttribute.KeyDefaultProviderType) as DefaultProvider<TKey>;
            if (KeyDefaultProvider != null)
                key = KeyDefaultProvider.GetDefault(DefaultUsage.Key);
            else
                key = default(TKey);

            var ValueDefaultProvider = Activator.CreateInstance(m_EditorAttribute.ValueDefaultProviderType) as DefaultProvider<TValue>;
            if (ValueDefaultProvider != null)
                value = ValueDefaultProvider.GetDefault(DefaultUsage.Value);
            else
                value = default(TValue);

            return new EditableKeyValuePair<TKey, TValue>(key, value, m_EditorAttribute);
        }

        /// <summary>
        /// Gets an array of objects containing the specified collection.
        /// </summary>
        /// <param name="editValue">The collection to edit.</param>
        /// <returns>An array containing the collection objects, or an empty object array if the specified collection does not inherit from ICollection.</returns>
        protected override object[] GetItems(object editValue)
        {
            if (editValue == null)
            {
                return new object[0];
            }
            var dictionary = editValue as Dictionary<TKey, TValue>;
            if (dictionary == null)
            {
                throw new ArgumentNullException("editValue");
            }
            var objArray = new object[dictionary.Count];
            var num = 0;
            foreach (var entry in dictionary)
            {
                var entry2 = new EditableKeyValuePair<TKey, TValue>(entry.Key, entry.Value, m_EditorAttribute);
                objArray[num++] = entry2;
            }
            return objArray;
        }

        /// <summary>
        /// Sets the specified array as the items of the collection.
        /// </summary>
        /// <param name="editValue">The collection to edit.</param>
        /// <param name="value">An array of objects to set as the collection items.</param>
        /// <returns>The newly created collection object or, otherwise, the collection indicated by the <paramref name="editValue"/> parameter.</returns>
        protected override object SetItems(object editValue, object[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var dictionary = editValue as IDictionary<TKey, TValue>;
            if (dictionary == null)
            {
                throw new ArgumentNullException("editValue");
            }
            dictionary.Clear();
            foreach (EditableKeyValuePair<TKey, TValue> entry in value)
            {
                dictionary.Add(new KeyValuePair<TKey, TValue>(entry.Key, entry.Value));
            }
            return dictionary;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>he display text for <paramref name="value"/>.</returns>
        protected override string GetDisplayText(object value)
        {
            var pair = value as EditableKeyValuePair<TKey, TValue>;
            if (pair != null)
                return string.Format(CultureInfo.CurrentCulture, "{0}={1}", pair.Key, pair.Value);
            else
                return base.GetDisplayText(value);
        }
    }
}
