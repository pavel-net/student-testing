using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemForTesting.Models;
using SystemForTesting.Repositories;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace SystemForTesting.ViewModels
{
    public class AnswerViewModel : ViewModelBase
    {
        #region Поля
        #endregion

        #region Конструкторы
        public AnswerViewModel(Answer answer = null)
        {
            AnswerObject = answer ?? new Answer();
            FlagCorrectly2 = FlagCorrectly == "Y";
        }
        #endregion

        #region Свойства
        public override string Title => "Ответ";

        [Model]
        public Answer AnswerObject
        {
            get { return GetValue<Answer>(AnswerObjectProperty); }
            set { SetValue(AnswerObjectProperty, value); }
        }
        public static readonly PropertyData AnswerObjectProperty =
            RegisterProperty(nameof(AnswerObject), typeof(Answer), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.TwoWay)]
        public string Content
        {
            get { return GetValue<string>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly PropertyData ContentProperty =
            RegisterProperty(nameof(Content), typeof(string), null);

        [ViewModelToModel(Mode = ViewModelToModelMode.TwoWay)]
        public string FlagCorrectly
        {
            get { return GetValue<string>(FlagCorrectlyProperty); }
            set { SetValue(FlagCorrectlyProperty, value); }
        }
        public static readonly PropertyData FlagCorrectlyProperty =
            RegisterProperty(nameof(FlagCorrectly), typeof(string), null);

        public bool FlagCorrectly2
        {
            get { return GetValue<bool>(FlagCorrectly2Property); }
            set
            {
                SetValue(FlagCorrectly2Property, value);
                FlagCorrectly = value ? "Y" : "N";
            }
        }
        public static readonly PropertyData FlagCorrectly2Property =
            RegisterProperty(nameof(FlagCorrectly2), typeof(bool), false);

        [ViewModelToModel(Mode = ViewModelToModelMode.TwoWay)]
        public byte[] ContentImage
        {
            get { return GetValue<byte[]>(ContentImageProperty); }
            set { SetValue(ContentImageProperty, value); }
        }
        public static readonly PropertyData ContentImageProperty =
            RegisterProperty(nameof(ContentImage), typeof(byte[]), null);

        //[ViewModelToModel(Mode = ViewModelToModelMode.TwoWay)]
        //public bool FlagCorrectly
        //{
        //    get { return GetValue<bool>(FlagCorrectlyProperty); }
        //    set { SetValue(FlagCorrectlyProperty, value); }
        //}
        //public static readonly PropertyData FlagCorrectlyProperty =
        //    RegisterProperty(nameof(FlagCorrectly), typeof(bool), false);
        #endregion

        #region Commands
        #endregion

        #region Methods
        //protected override async Task<bool> SaveAsync()
        //{
        //    UpdateExplicitViewModelToModelMappings();
        //    return await base.SaveAsync();
        //}

        #endregion
    }
}
