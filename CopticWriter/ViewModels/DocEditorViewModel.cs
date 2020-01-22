using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CopticWriter.ViewModels
{
    public class DocEditorViewModel : INotifyPropertyChanged
    {
        private string _nextButtonText;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public DocEditorViewModel()
        {
            this.NextButtonText = "Next";
        }

        public string NextButtonText {
            get { return this._nextButtonText; }
            set {
                this._nextButtonText = value;
                this.OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
