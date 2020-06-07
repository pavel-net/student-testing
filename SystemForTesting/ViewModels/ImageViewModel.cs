using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;

namespace SystemForTesting.ViewModels
{
    public class ImageViewModel : ViewModelBase
    {
        #region Поля
        #endregion

        #region Конструкторы
        public ImageViewModel(byte[] imageBytes)
        {
            ImageObject = imageBytes;
        }
        #endregion

        #region Свойства

        public byte[] ImageObject
        {
            get { return GetValue<byte[]>(ImageObjectProperty); }
            set { SetValue(ImageObjectProperty, value); }
        }
        public static readonly PropertyData ImageObjectProperty =
            RegisterProperty(nameof(ImageObject), typeof(byte[]), null);

        public override string Title => "Изображение";

        #endregion

        #region Commands
        
        #endregion

        #region Methods
        #endregion
    }
}
