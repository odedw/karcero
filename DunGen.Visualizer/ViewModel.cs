using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using DunGen.Engine;
using DunGen.Engine.Contracts;
using DunGen.Engine.Implementations;
using DunGen.Engine.Models;
using System.Linq;
namespace DunGen.Visualizer
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly DungeonConfiguration mConfiguration = new DungeonConfiguration()
        {
            Height = 16,
            Width = 16,
            ChanceToRemoveDeadends = 0.5,
            Sparseness = 0.7,
            Randomness = 0.5,
            MinRoomHeight = 3,
            MaxRoomHeight = 6,
            MinRoomWidth = 3,
            MaxRoomWidth = 6,
            RoomCount = 10
        };

        public Dispatcher Dispatcher { get; set; }
        private BindingList<Cell> mCells = new BindingList<Cell>();
        private Thread mWorkerThread;
        public BindingList<Cell> Cells
        {
            get { return mCells; }
            set
            {
                if (mCells == value) return;
                mCells = value;
                RaisePropertyChanged("Cells");
            }
        }

        private Map mMap;

        public Map Map
        {
            get { return mMap; }
            set
            {
                if (mMap == value) return;
                mMap = value;
                RaisePropertyChanged("Map");
            }
        }

        

        public bool IsRunning { get; set; }
        private int mWidth;
        public int Width
        {
            get { return mWidth; }
            set
            {
                if (mWidth == value) return;
                mWidth = value;
                RaisePropertyChanged("Width");
            }
        }

        public ICommand GenerateCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        private readonly DunGenerator mGenerator = new DunGenerator();

        public ViewModel()
        {
            //mGenerator.MapChanged += MapChangedHandler;
            Width = mConfiguration.Width;

            GenerateCommand = new RelayCommand(StartGeneration);
            RefreshCommand = new RelayCommand(o =>
            {
                var cells = Cells.ToList();
                Cells.Clear();
                foreach (var cell in cells)
                {
                    Cells.Add(cell);
                }
            });       
        }

        private void StartGeneration(object input)
        {
            Width = mConfiguration.Width;

            if (IsRunning)
            {
                mWorkerThread.Abort();
            }
            Cells.Clear();

            IsRunning = true;
            mWorkerThread = new Thread(() =>
            {
                //-173632285
                var map = mGenerator.Generate(mConfiguration);
                Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(delegate()
                {
                    Map = map;
                    if (Cells.Count == 0)
                    {
                        for (int i = 0; i < map.Height; i++)
                        {
                            for (var j = 0; j < map.Width; j++)
                            {
                                Cells.Add(map.GetCell(i, j));
                            }
                        }
                    }
                    Width = map.Width;
                }));
                IsRunning = false;
            }) { IsBackground = true };
            mWorkerThread.Start();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
