using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Data;
using Catel.Data;
using Catel.MVVM;

namespace SystemForTesting.ViewModels
{
    public class SimpleEditViewModel : ViewModelBase
    {
        #region Поля
        #endregion

        #region Конструкторы
        public SimpleEditViewModel(string title, string labelText)
        {
            this.Title = title;
            LabelTextValue = labelText;
        }
        #endregion

        #region Свойства

        public override string Title { get; protected set; }

        public string LabelTextValue
        {
            get { return GetValue<string>(LabelTextValueProperty); }
            set { SetValue(LabelTextValueProperty, value); }
        }
        public static readonly PropertyData LabelTextValueProperty =
            RegisterProperty(nameof(LabelTextValue), typeof(string), null);

        public string TextValue
        {
            get { return GetValue<string>(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }
        public static readonly PropertyData TextValueProperty =
            RegisterProperty(nameof(TextValue), typeof(string), null);
        #endregion

        #region Commands

        #endregion

        #region Methods
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(TextValue?.Trim()))
            {
                validationResults.Add(FieldValidationResult.CreateError(TextValueProperty, "Название не может быть пустым"));
            }
        }
        #endregion
    }
}
