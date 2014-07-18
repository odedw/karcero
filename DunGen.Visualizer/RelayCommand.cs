using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DunGen.Visualizer
{
    public class RelayCommand : ICommand 
    { 
        #region Fields 
        readonly Action<object> mExecute; 
        readonly Predicate<object> mCanExecute; 
        #endregion 
        
        // Fields 
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute"); 
            mExecute = execute; 
            mCanExecute = canExecute;
        } 
        #endregion // Constructors 
        
        #region ICommand Members 

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return mCanExecute == null || mCanExecute(parameter);
        } 
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; } 
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            mExecute(parameter);
        } 
        #endregion // ICommand Members 
    }
}
