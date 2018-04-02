// Project Renfrew
// Copyright(C) 2018 Stephen Workman (workman.stephen@gmail.com)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Renfrew;
using Renfrew.Core.Grammars.MousePlot;

namespace Magnifier_Tester {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      private Magnifier _magnifier;

      private Timer _tmr = null;


      public MainWindow() {
         InitializeComponent();
         _magnifier = new Magnifier();


         _tmr = new Timer(state =>
         {
            _magnifier.Update(1920 / 2 - 150, 1080 / 2 - 150);
            _tmr.Change(1, Timeout.Infinite);
         }, null, 1, Timeout.Infinite);
        
      }
      
      private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
         //var z = new ZoomWindow();

         //z.Show();

         //z.Magnifier.Child = _magnifier;
         //_magnifier.Initialize();

         //_tmr.Change(1, 10);

         //WindowState = WindowState.Minimized;
      }
   }
}
