using Nebula.Core.Data;
using Nebula.Core.FileReaders;
using Nebula.Core.Memory;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;
using Frame = Nebula.Core.Data.Chunks.FrameChunks.Frame;
using System.Windows.Interop;
using Nebula.Core.Utilities;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Image = System.Windows.Controls.Image;
using Bitmap = System.Drawing.Bitmap;
using System.Windows.Documents;

namespace Nebula.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int SelectedFrame = 0;
        public List<TransformGroup> FrameTransforms = new();
        public Point MouseOld = new Point();
        public List<string> OpenedTabs = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            Interface_CreateTab("Home").Content = Home_CreateView();
        }

        private void Nebula_DropFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                NebulaCore.FilePath = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
                Nebula_ReadFile();
            }
        }

        private void Nebula_OpenFileDialog(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Select a Clickteam Fusion application";
            if (dlg.ShowDialog() == true)
            {
                NebulaCore.FilePath = dlg.FileName;
                Nebula_ReadFile();
            }
        }

        private void Nebula_ReadFile()
        {
            Task readTask = new Task(() =>
            {
                ByteReader fileReader = new ByteReader(File.ReadAllBytes(NebulaCore.FilePath));

                string ext = Path.GetExtension(NebulaCore.FilePath).ToLower();
                switch (ext)
                {
                    default:
                        NebulaCore.CurrentReader = new CCNFileReader();
                        ((CCNFileReader)NebulaCore.CurrentReader).CheckUnpacked(fileReader!);
                        break;
                    case ".exe":
                        NebulaCore.CurrentReader = new EXEFileReader();
                        break;
                    case ".mfa":
                        NebulaCore.CurrentReader = new MFAFileReader();
                        break;
                    case ".anm":
                        NebulaCore.CurrentReader = new ANMFileReader();
                        break;
                    case ".tmp":
                        NebulaCore.CurrentReader = new AGMIFileReader();
                        break;
                    case ".zip":
                        NebulaCore.CurrentReader = new OpenFileReader();
                        break;
                    case ".apk":
                        NebulaCore.CurrentReader = new APKFileReader();
                        break;
                    case ".ipa":
                        NebulaCore.CurrentReader = new IPAFileReader();
                        break;
                    case ".gam":
                        if (File.Exists(Path.Combine(Path.GetDirectoryName(NebulaCore.FilePath), Path.GetFileNameWithoutExtension(NebulaCore.FilePath) + ".img")))
                            NebulaCore.CurrentReader = new KNPFileReader();
                        else
                            NebulaCore.CurrentReader = new CCNFileReader();
                        break;
                }

                try
                {
                    NebulaCore.CurrentReader.LoadGame(fileReader!, NebulaCore.FilePath);

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Interface_PostRead();
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), $"Failed to read \"{Path.GetFileName(NebulaCore.FilePath)}\"", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            readTask.Start();
        }

        private void Interface_PostRead()
        {
            Interface_OpenFramesList();
        }

        private void Interface_OpenFramesList(object sender, MouseButtonEventArgs e)
        {
            Interface_OpenFramesList();
            ((ListBoxItem)((Image)sender).Parent).IsSelected = false;
            ((ListBox)((ListBoxItem)((Image)sender).Parent).Parent).SelectedIndex = -1;
        }

        private TabItem Interface_CreateTab(string name)
        {
            if (OpenedTabs.Contains(name))
            {
                Tabs.SelectedIndex = OpenedTabs.IndexOf(name);
                return (TabItem)Tabs.SelectedItem;
            }
            if (name != "Home" && OpenedTabs.Contains("Home"))
            {
                Tabs.Items.Remove(Tabs.Items[OpenedTabs.IndexOf("Home")]);
                OpenedTabs.Remove("Home");
            }

            TabItem newTab = new TabItem();
            newTab.Header = name;
            newTab.Background = Brushes.Transparent;
            newTab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
            newTab.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));

            ContextMenu conMenu = new ContextMenu();
            conMenu.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF262626"));
            conMenu.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
            conMenu.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
            MenuItem closeTab = new MenuItem();
            closeTab.Header = "Close";
            closeTab.Click += Interface_CloseTab;
            conMenu.Items.Add(closeTab);
            newTab.ContextMenu = conMenu;

            OpenedTabs.Add(name);
            Tabs.Items.Add(newTab);
            Tabs.SelectedItem = newTab;
            return newTab;
        }

        private void Interface_CloseTab(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            OpenedTabs.Remove(tab.Header.ToString()!);
            Tabs.SelectedIndex -= 1;
            Tabs.Items.Remove(tab);
            if (Tabs.Items.Count == 0)
                Interface_CreateTab("Home").Content = Home_CreateView();
        }

        private Grid FrameView_CreateView(Frame frm)
        {
            Grid grid = new Grid();
            grid.MouseWheel += FrameView_OnScroll;
            grid.MouseMove += FrameView_MoveView;
            grid.MouseLeave += FrameView_ResetMouse;
            grid.ClipToBounds = true;
            grid.Margin = new Thickness(-3, -2, -3, -3);
            grid.Background = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0));
            ColumnDefinition framePanDef = new ColumnDefinition();
            ColumnDefinition layerPanDef = new ColumnDefinition();
            framePanDef.Width = new GridLength(1, GridUnitType.Star);
            layerPanDef.Width = new GridLength(25, GridUnitType.Pixel);
            grid.ColumnDefinitions.Add(framePanDef);
            grid.ColumnDefinitions.Add(layerPanDef);

            ListBox listBox = new ListBox();
            listBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF262626"));
            listBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
            listBox.BorderThickness = new Thickness(1, 0, 0, 0);
            ScrollViewer.SetHorizontalScrollBarVisibility(listBox, ScrollBarVisibility.Hidden);
            ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Hidden);

            for (int i = 0; i < frm.FrameLayers.Layers.Length; i++)
            {
                CheckBox lyrCB = new CheckBox();
                lyrCB.IsChecked = true;
                lyrCB.Tag = i;
                lyrCB.Click += FrameView_ToggleLayer;
                listBox.Items.Add(lyrCB);
            }

            Viewbox viewBox = new Viewbox();
            viewBox.RenderTransformOrigin = new Point(0.5, 0.5);

            TransformGroup trn = new TransformGroup();
            ScaleTransform scl = new ScaleTransform();
            scl.ScaleX = 0.5d;
            scl.ScaleY = 0.5d;
            SkewTransform sk = new SkewTransform();
            RotateTransform rot = new RotateTransform();
            TranslateTransform trnsl = new TranslateTransform();
            trn.Children.Add(scl);
            trn.Children.Add(sk);
            trn.Children.Add(rot);
            trn.Children.Add(trnsl);
            viewBox.RenderTransform = trn;
            grid.Children.Add(viewBox);
            grid.Children.Add(listBox);
            Grid.SetColumn(viewBox, 0);
            Grid.SetColumn(listBox, 1);
            return grid;
        }

        private void FrameView_ToggleLayer(object sender, RoutedEventArgs e)
        {
            FrameView_RefreshView();
        }

        private void FrameView_RefreshView()
        {
            TabItem FrameView = (TabItem)Tabs.SelectedItem;
            Frame frm = NebulaCore.PackageData.Frames[(int)FrameView.Tag];
            Viewbox viewBox = (Viewbox)((Grid)FrameView.Content).Children[0];
            ListBox listBox = (ListBox)((Grid)FrameView.Content).Children[1];

            Grid frmGrid = new Grid();
            frmGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            frmGrid.VerticalAlignment = VerticalAlignment.Stretch;

            Rectangle bounds = new Rectangle();
            bounds.Fill = new SolidColorBrush(Color.FromArgb(255,
                                                             frm.FrameHeader.Background.R,
                                                             frm.FrameHeader.Background.G,
                                                             frm.FrameHeader.Background.B));
            bounds.Width = frm.FrameHeader.Width;
            bounds.Height = frm.FrameHeader.Height;
            bounds.HorizontalAlignment = HorizontalAlignment.Center;
            bounds.VerticalAlignment = VerticalAlignment.Center;
            frmGrid.Children.Add(bounds);

            foreach (FrameInstance inst in frm.FrameInstances.Instances)
            {
                if (!NebulaCore.PackageData.FrameItems.Items.ContainsKey((int)inst.ObjectInfo) ||
                    ((CheckBox)listBox.Items[(int)inst.Layer]).IsChecked == false)
                    continue;
                ObjectInfo oI = NebulaCore.PackageData.FrameItems.Items[(int)inst.ObjectInfo];
                Border hitBox = new Border();
                hitBox.BorderBrush = new SolidColorBrush(Color.FromArgb(50, 255, 20, 20));
                hitBox.Margin = new Thickness(0);
                hitBox.Padding = new Thickness(0);
                hitBox.BorderThickness = new Thickness(1);
                hitBox.HorizontalAlignment = HorizontalAlignment.Center;
                hitBox.VerticalAlignment = VerticalAlignment.Center;
                Image img = new Image();
                img.Margin = new Thickness(0);
                img.HorizontalAlignment = HorizontalAlignment.Center;
                img.VerticalAlignment = VerticalAlignment.Center;
                TransformGroup objTrn = new TransformGroup();
                ScaleTransform objScl = new ScaleTransform();
                SkewTransform objSk = new SkewTransform();
                RotateTransform objRot = new RotateTransform();
                TranslateTransform objTrnsl = new TranslateTransform();
                objTrn.Children.Add(objScl);
                objTrn.Children.Add(objSk);
                objTrn.Children.Add(objRot);
                objTrn.Children.Add(objTrnsl);
                hitBox.RenderTransform = objTrn;
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                ImageBank imgBank = NebulaCore.PackageData.ImageBank;
                Core.Data.Chunks.BankChunks.Images.Image objImg;
                IntPtr handle;
                float alpha;
                if (oI.Header.InkEffect != 1)
                    alpha = oI.Header.BlendCoeff / 255.0f;
                else
                    alpha = oI.Header.InkEffectParam * 2.0f / 255.0f;
                img.Opacity = 1f - alpha;

                switch (oI.Header.Type)
                {
                    case 0: // Quick Backdrop
                        ObjectQuickBackdrop oQB = (ObjectQuickBackdrop)oI.Properties;
                        if (oQB.Shape.FillType != 3)
                            continue;
                        objImg = imgBank.Images[oQB.Shape.Image];
                        objTrnsl.X = inst.PositionX + oQB.Width / 2f - bounds.Width / 2f;
                        objTrnsl.Y = inst.PositionY + oQB.Height / 2f - bounds.Height / 2f;
                        Rectangle qBDRect = new Rectangle();
                        qBDRect.Width = oQB.Width;
                        qBDRect.Height = oQB.Height;
                        ImageBrush imgBrush = new ImageBrush();
                        imgBrush.TileMode = TileMode.Tile;
                        handle = objImg.GetBitmap().GetHbitmap();
                        imgBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        imgBrush.ViewportUnits = BrushMappingMode.Absolute;
                        imgBrush.Viewport = new Rect(0, 0, objImg.Width, objImg.Height);
                        qBDRect.Fill = imgBrush;
                        hitBox.Child = qBDRect;
                        break;
                    case 1: // Backdrop
                        ObjectBackdrop oB = (ObjectBackdrop)oI.Properties;
                        objImg = imgBank.Images[oB.Image];
                        objTrnsl.X = inst.PositionX + objImg.Width / 2f - bounds.Width / 2f;
                        objTrnsl.Y = inst.PositionY + objImg.Height / 2f - bounds.Height / 2f;
                        handle = objImg.GetBitmap().GetHbitmap();
                        img.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        break;
                    default:
                        ObjectCommon oC = (ObjectCommon)oI.Properties;
                        switch (oI.Header.Type)
                        {
                            case 2: // Active
                                objImg = imgBank.Images[oC.ObjectAnimations.Animations.First().Value.Directions[0].Frames[0]];
                                objTrnsl.X = inst.PositionX - objImg.HotspotX + objImg.Width / 2f - bounds.Width / 2f;
                                objTrnsl.Y = inst.PositionY - objImg.HotspotY + objImg.Height / 2f - bounds.Height / 2f;
                                handle = objImg.GetBitmap().GetHbitmap();
                                img.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                break;
                            case 7: // Counter
                                ObjectCounter cntr = oC.ObjectCounter;
                                Bitmap cntrBmp = Utilities.GetCounterBmp(cntr, oC.ObjectValue);
                                if (cntr.DisplayType == 1)
                                {
                                    objTrnsl.X = inst.PositionX - cntrBmp.Width / 2f - bounds.Width / 2f;
                                    objTrnsl.Y = inst.PositionY - cntrBmp.Height / 2f - bounds.Height / 2f;
                                }
                                else if (cntr.DisplayType == 4)
                                {
                                    objTrnsl.X = inst.PositionX + cntrBmp.Width / 2f - bounds.Width / 2f;
                                    objTrnsl.Y = inst.PositionY + cntrBmp.Height / 2f - bounds.Height / 2f;
                                }
                                else continue;
                                handle = cntrBmp.GetHbitmap();
                                img.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                break;
                            case 3: // String
                            case 4: // Question and Answer
                            case 5: // Score
                            case 6: // Lives
                            case 8: // Formatted Text
                            case 9: // Sub-Application
                            default: // Extensions
                                break;
                        }
                        break;
                }
                if (img.Source == null && oI.Header.Type != 0)
                    continue;
                else if (oI.Header.Type != 0)
                    hitBox.Child = img;
                frmGrid.Children.Add(hitBox);
            }

            viewBox.Child = frmGrid;
        }

        bool loadingFrameListSelected;
        private void Interface_OpenFramesList()
        {
            if (loadingFrameListSelected) return;
            loadingFrameListSelected = true;
            Task frmsListTask = new Task(() =>
            {
                if (NebulaCore.CurrentReader != null)
                    foreach (Frame frm in NebulaCore.PackageData.Frames.Values)
                        if (frm.BitmapCache == null)
                            Utilities.MakeFrameImg(frm);

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    Interface_CreateTab("Frames List").Content = FramesList_CreateView();
                    loadingFrameListSelected = false;
                }));
            });
            frmsListTask.Start();
        }

        private void Interface_OpenHomeTab(object sender, MouseButtonEventArgs e)
        {
            Interface_CreateTab("Home").Content = Home_CreateView();
        }

        private void Interface_FrameSelected(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem tVI = (ListBoxItem)sender;
            Frame frm = NebulaCore.PackageData.Frames[(int)tVI.Tag];
            TabItem FrameView = Interface_CreateTab(frm.FrameName + " - Objects");
            FrameView.Visibility = Visibility.Visible;
            FrameView.Content = FrameView_CreateView(frm);
            FrameView.Tag = tVI.Tag;
            FrameView_RefreshView();
            FrameView.Focus();
            tVI.IsSelected = false;
            ((ListBox)tVI.Parent).SelectedIndex = -1;
        }

        private void FrameView_OnScroll(object sender, MouseWheelEventArgs e)
        {
            Grid sndr = (Grid)sender;
            Viewbox box = (Viewbox)sndr.Children[0];
            Point mouse = e.GetPosition(box);
            box.RenderTransformOrigin = new Point(mouse.X / box.ActualWidth, mouse.Y / box.ActualHeight);
            TransformGroup trn = (TransformGroup)box.RenderTransform;
            ScaleTransform scl = (ScaleTransform)trn.Children[0];
            double newScl = scl.ScaleX;
            if (e.Delta > 0)
                newScl += 0.01d + newScl / 3d;
            else if (e.Delta < 0)
                newScl -= 0.01d + newScl / 3d;
            scl.ScaleX = newScl;
            scl.ScaleY = newScl;
        }

        private void FrameView_MoveView(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Cursor = Cursors.None;
                Grid sndr = (Grid)sender;
                Viewbox box = (Viewbox)sndr.Children[0];
                TransformGroup trn = (TransformGroup)box.RenderTransform;
                TranslateTransform trnsl = (TranslateTransform)trn.Children[3];
                Point dlta = e.GetPosition(this);
                dlta.X -= MouseOld.X;
                dlta.Y -= MouseOld.Y;
                trnsl.X += dlta.X;
                trnsl.Y += dlta.Y;
            }
            else
                Cursor = Cursors.Arrow;
            MouseOld = e.GetPosition(this);
        }

        private void FrameView_ResetMouse(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private Grid FramesList_CreateView()
        {
            if (NebulaCore.CurrentReader == null || NebulaCore.PackageData == null || NebulaCore.PackageData.Frames.Count == 0) return new Grid();
            Grid grid = new Grid();
            grid.ClipToBounds = true;
            grid.Margin = new Thickness(-3);
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;

            ListBox listBox = new ListBox();
            listBox.Background = Brushes.Transparent;
            listBox.BorderThickness = new Thickness(0);
            listBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Children.Add(listBox);

            int tag = 0;
            foreach (Frame frm in NebulaCore.PackageData.Frames.Values)
            {
                ListBoxItem lBI = new ListBoxItem();
                lBI.Height = 90;
                lBI.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                lBI.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
                lBI.BorderThickness = new Thickness(0, 0, 0, 1);
                lBI.MouseDoubleClick += Interface_FrameSelected;
                lBI.Tag = tag++;

                Grid lBIGrid = new Grid();
                lBIGrid.ClipToBounds = true;

                Image lBIImg = new Image();
                lBIImg.Width = 130;
                lBIImg.Height = 82;
                lBIImg.Stretch = Stretch.Fill;
                lBIImg.HorizontalAlignment = HorizontalAlignment.Left;
                lBIImg.Margin = new Thickness(3, 0, 0, 0);

                RenderOptions.SetBitmapScalingMode(lBIImg, BitmapScalingMode.NearestNeighbor);
                var handle = Utilities.MakeFrameImg(frm, -1, false, false).ResizeImage(130, 82).GetHbitmap();
                lBIImg.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                Label lBIName = new Label();
                lBIName.Content = frm.FrameName;
                lBIName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
                lBIName.Padding = new Thickness(140, 0, 0, 0);
                lBIName.FontSize = 16;

                Label lBISize = new Label();
                lBISize.Content = frm.FrameHeader.Width + "x" + frm.FrameHeader.Height;
                lBISize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#90E6E8E6"));
                lBISize.Padding = new Thickness(140, 22, 0, 0);
                lBISize.FontSize = 12;

                Label lBIObjCnt = new Label();
                lBIObjCnt.Content = frm.FrameInstances.Instances.Length + " objects";
                lBIObjCnt.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#90E6E8E6"));
                lBIObjCnt.Padding = new Thickness(140, 38, 0, 0);
                lBIObjCnt.FontSize = 12;

                Button lBIVFBtn = new Button();
                lBIVFBtn.Width = 95;
                lBIVFBtn.Content = "View Frame";
                lBIVFBtn.HorizontalAlignment = HorizontalAlignment.Right;
                lBIVFBtn.VerticalAlignment = VerticalAlignment.Top;
                lBIVFBtn.Padding = new Thickness(12, 1, 12, 1);
                lBIVFBtn.Margin = new Thickness(0, 3, 5, 0);
                lBIVFBtn.Background = Brushes.Transparent;
                lBIVFBtn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
                lBIVFBtn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
                lBIVFBtn.Click += FramesList_ViewFrame;

                Button lBIVOBtn = new Button();
                lBIVOBtn.Width = 95;
                lBIVOBtn.Content = "View Objects";
                lBIVOBtn.HorizontalAlignment = HorizontalAlignment.Right;
                lBIVOBtn.VerticalAlignment = VerticalAlignment.Top;
                lBIVOBtn.Padding = new Thickness(12, 1, 12, 1);
                lBIVOBtn.Margin = new Thickness(0, 28, 5, 0);
                lBIVOBtn.Background = Brushes.Transparent;
                lBIVOBtn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
                lBIVOBtn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
                lBIVOBtn.Click += FramesList_ViewObjects;

                lBIGrid.Children.Add(lBIImg);
                lBIGrid.Children.Add(lBIName);
                lBIGrid.Children.Add(lBISize);
                lBIGrid.Children.Add(lBIObjCnt);
                lBIGrid.Children.Add(lBIVFBtn);
                lBIGrid.Children.Add(lBIVOBtn);
                lBI.Content = lBIGrid;

                listBox.Items.Add(lBI);
            }
            return grid;
        }

        private void FramesList_ViewFrame(object sender, RoutedEventArgs e)
        {
            ListBoxItem tVI = (ListBoxItem)((Grid)((Button)sender).Parent).Parent;
            Frame frm = NebulaCore.PackageData.Frames[(int)tVI.Tag];
            TabItem FrameView = Interface_CreateTab(frm.FrameName + " - Frame");
            FrameView.Visibility = Visibility.Visible;
            FrameView.Content = FrameView_CreateView(frm);
            FrameView.Tag = tVI.Tag;
            FrameView_RefreshView();
            FrameView.Focus();
        }

        private void FramesList_ViewObjects(object sender, RoutedEventArgs e)
        {
            ListBoxItem tVI = (ListBoxItem)((Grid)((Button)sender).Parent).Parent;
            Frame frm = NebulaCore.PackageData.Frames[(int)tVI.Tag];
            TabItem FrameView = Interface_CreateTab(frm.FrameName + " - Objects");
            FrameView.Visibility = Visibility.Visible;
            FrameView.Content = ObjectList_CreateView(frm);
            FrameView.Tag = tVI.Tag;
            FrameView.Focus();
        }

        private Grid Home_CreateView()
        {
            Grid grid = new Grid();
            grid.ClipToBounds = true;
            grid.Margin = new Thickness(-3);

            Label welcome = new Label();
            welcome.Content = "Welcome to Nebula!";
            welcome.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
            welcome.FontWeight = FontWeights.Bold;
            welcome.Margin = new Thickness(10, 0, 0, 0);

            Separator separator = new Separator();
            separator.Margin = new Thickness(10, 25, 10, 0);
            separator.VerticalAlignment = VerticalAlignment.Top;

            Label desc = new Label();
            desc.Content = "Open a Fusion file to get started, then click on the items on the left to view them.";
            desc.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
            desc.Margin = new Thickness(10, 23, 0, 0);

            Button openFile = new Button();
            openFile.Content = "Open File";
            openFile.HorizontalAlignment = HorizontalAlignment.Left;
            openFile.VerticalAlignment = VerticalAlignment.Top;
            openFile.Padding = new Thickness(12, 1, 12, 1);
            openFile.Margin = new Thickness(10, 50, 0, 0);
            openFile.Background = Brushes.Transparent;
            openFile.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
            openFile.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
            openFile.Click += Nebula_OpenFileDialog;

            grid.Children.Add(welcome);
            grid.Children.Add(separator);
            grid.Children.Add(desc);
            grid.Children.Add(openFile);
            return grid;
        }

        private Grid ObjectList_CreateView(Frame frm)
        {
            Grid grid = new Grid();
            grid.ClipToBounds = true;
            grid.Margin = new Thickness(-3);
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;

            ListBox list = new ListBox();
            list.Background = Brushes.Transparent;
            list.BorderBrush = Brushes.Transparent;
            list.BorderThickness = new Thickness(0);
            list.HorizontalAlignment = HorizontalAlignment.Stretch;

            foreach (FrameInstance inst in frm.FrameInstances.Instances)
            {
                if (!NebulaCore.PackageData.FrameItems.Items.ContainsKey((int)inst.ObjectInfo))
                    continue;
                ObjectInfo oI = NebulaCore.PackageData.FrameItems.Items[(int)inst.ObjectInfo];

                ListBoxItem listItem = new ListBoxItem();
                listItem.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
                listItem.BorderThickness = new Thickness(0, 0, 0, 1);
                listItem.HorizontalAlignment = HorizontalAlignment.Stretch;

                Grid itemGrid = new Grid();
                itemGrid.Height = 50;
                itemGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

                ColumnDefinition imgDef = new ColumnDefinition();
                imgDef.Width = new GridLength(48, GridUnitType.Pixel);
                itemGrid.ColumnDefinitions.Add(imgDef);

                ColumnDefinition contDef = new ColumnDefinition();
                contDef.Width = new GridLength(1, GridUnitType.Star);
                itemGrid.ColumnDefinitions.Add(contDef);

                Border bdr = new Border();
                bdr.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF957FEF"));
                bdr.BorderThickness = new Thickness(0, 0, 1, 0);
                bdr.Margin = new Thickness(0, -1, -4, -1);

                Image img = new Image();
                img.Width = 48;
                img.Height = 48;
                img.Stretch = Stretch.Uniform;
                img.StretchDirection = StretchDirection.DownOnly;
                img.HorizontalAlignment = HorizontalAlignment.Center;
                img.VerticalAlignment = VerticalAlignment.Center;

                Label lbl = new Label();
                lbl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE6E8E6"));
                lbl.HorizontalAlignment = HorizontalAlignment.Left;
                lbl.Margin = new Thickness(8, 0, 0, 29);
                lbl.Padding = new Thickness(0);
                lbl.Content = oI.Name;

                Grid.SetColumn(bdr, 0);
                Grid.SetColumn(img, 0);
                Grid.SetColumn(lbl, 1);
                itemGrid.Children.Add(bdr);
                itemGrid.Children.Add(img);
                itemGrid.Children.Add(lbl);
                listItem.Content = itemGrid;

                ImageBank imgBank = NebulaCore.PackageData.ImageBank;
                string BaseUri = "pack://application:,,,/Nebula.GUI;component/Plugins/ObjectIcons/";
                switch (oI.Header.Type)
                {
                    case 0: // Quick Backdrop
                        ObjectQuickBackdrop oQB = (ObjectQuickBackdrop)oI.Properties;
                        if (oQB.Shape.FillType != 3)
                            img.Source = new BitmapImage(new Uri("Plugins\\ObjectIcons\\MMFQuickBackdrop.png", UriKind.Relative));
                        else
                            img.Source = Imaging.CreateBitmapSourceFromHBitmap(imgBank.Images[oQB.Shape.Image].GetBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        break;
                    case 1: // Backdrop
                        ObjectBackdrop oB = (ObjectBackdrop)oI.Properties;
                        if (imgBank.Images.ContainsKey(oB.Image))
                            img.Source = Imaging.CreateBitmapSourceFromHBitmap(imgBank.Images[oB.Image].GetBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        else
                            img.Source = new BitmapImage(new Uri("Plugins\\ObjectIcons\\MMFBackdrop.png", UriKind.Relative));
                        break;
                    default:
                        ObjectCommon oC = (ObjectCommon)oI.Properties;
                        switch (oI.Header.Type)
                        {
                            case 2: // Active
                                if (oC.ObjectAnimations.Animations.Count > 0 &&
                                    oC.ObjectAnimations.Animations.First().Value.Directions.Count > 0 &&
                                    oC.ObjectAnimations.Animations.First().Value.Directions.First().Frames.Length > 0 &&
                                    imgBank.Images.ContainsKey(oC.ObjectAnimations.Animations.First().Value.Directions.First().Frames[0]))
                                    img.Source = Imaging.CreateBitmapSourceFromHBitmap(imgBank.Images[oC.ObjectAnimations.Animations.First().Value.Directions.First().Frames[0]].GetBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                else
                                    img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFActive.png")));
                                break;
                            case 3: // String
                                img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFString.png")));
                                break;
                            case 4: // Question and Answer
                                img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFQ&A.png")));
                                break;
                            case 5: // Score
                                ObjectCounter scr = oC.ObjectCounter;
                                Bitmap scrBmp = Utilities.GetCounterBmp(scr, oC.ObjectValue);
                                if (scrBmp != null)
                                    img.Source = Imaging.CreateBitmapSourceFromHBitmap(scrBmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                else
                                    img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFScore.png")));
                                break;
                            case 6: // Lives
                                ObjectCounter lvs = oC.ObjectCounter;
                                Bitmap lvsBmp = Utilities.GetCounterBmp(lvs, oC.ObjectValue);
                                if (lvsBmp != null)
                                    img.Source = Imaging.CreateBitmapSourceFromHBitmap(lvsBmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                else
                                    img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFLives.png")));
                                break;
                            case 7: // Counter
                                ObjectCounter cntr = oC.ObjectCounter;
                                Bitmap cntrBmp = Utilities.GetCounterBmp(cntr, oC.ObjectValue);
                                if (cntrBmp != null)
                                    img.Source = Imaging.CreateBitmapSourceFromHBitmap(cntrBmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                else
                                    img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFCounter.png")));
                                break;
                            case 8: // Formatted Text
                                img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFFormattedText.png")));
                                break;
                            case 9: // Sub-Application
                                img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\MMFSubApplication.png")));
                                break;
                            default: // Extensions
                                foreach (Core.Data.Chunks.AppChunks.Extension posExt in NebulaCore.PackageData.Extensions.Exts.Values)
                                    if (posExt.Handle == oI.Header.Type - 32 && File.Exists("Plugins\\ObjectIcons\\" + posExt.Name + ".png"))
                                    {
                                        img.Source = new BitmapImage(new Uri(Path.GetFullPath("Plugins\\ObjectIcons\\" + posExt.Name + ".png")));
                                        break;
                                    }
                                break;
                        }
                        break;
                }
                list.Items.Add(listItem);
            }

            grid.Children.Add(list);
            return grid;
        }
    }
}