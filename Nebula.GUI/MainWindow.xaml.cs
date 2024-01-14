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
using System.Windows.Markup;
using System.Windows.Interop;
using Nebula.Core.Utilities;

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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Nebula_DropFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                NebulaCore.FilePath = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();
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
                    NebulaCore.CurrentReader.LoadGame(fileReader!, NebulaCore.FilePath);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Interface_PostRead();
                    }));
                });
                readTask.Start();
            }
        }

        private void Interface_PostRead()
        {
            PackageData data = NebulaCore.PackageData;
            for (int i = 0; i < data.Frames.Count; i++)
            {
                TreeViewItem tVI = new TreeViewItem();
                tVI.Header = data.Frames[i].FrameName;
                tVI.Foreground = TV_Frames.Foreground;
                tVI.Tag = i;
                tVI.Selected += TV_FrameSelected;
                TV_Frames.Items.Add(tVI);
            }

            FrameView.Header = data.Frames.First().FrameName;
            FrameView.Content = FrameView_CreateView(data.Frames.First());
        }

        private Grid FrameView_CreateView(Frame frm)
        {
            Grid grid = new Grid();
            grid.MouseWheel += FrameView_OnScroll;
            grid.MouseMove += FrameView_MoveView;
            grid.MouseLeave += FrameView_ResetMouse;
            grid.ClipToBounds = true;

            Viewbox viewbox = new Viewbox();
            viewbox.RenderTransformOrigin = new Point(0.5, 0.5);

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
            viewbox.RenderTransform = trn;

            Rectangle bounds = new Rectangle();
            bounds.Fill = new SolidColorBrush(Color.FromArgb(255,
                                                             frm.FrameHeader.Background.R,
                                                             frm.FrameHeader.Background.G,
                                                             frm.FrameHeader.Background.B));
            bounds.Width = frm.FrameHeader.Width;
            bounds.Height = frm.FrameHeader.Height;
            bounds.HorizontalAlignment = HorizontalAlignment.Center;
            bounds.VerticalAlignment = VerticalAlignment.Center;
            viewbox.Child = bounds;
            grid.Children.Add(viewbox);
            return grid;
        }

        bool loadingFrameListSelected;
        private void TV_FrameListSelected(object sender, RoutedEventArgs e)
        {
            if (loadingFrameListSelected) return;
            loadingFrameListSelected = true;
            Task frmsListTask = new Task(() =>
            {
                foreach (Frame frm in NebulaCore.PackageData.Frames)
                    if (frm.BitmapCache == null)
                        Utilities.MakeFrameImg(frm);

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    FramesListView.Content = FramesList_CreateView();
                    loadingFrameListSelected = false;
                }));
            });
            frmsListTask.Start();
        }

        private void TV_FrameSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tVI = (TreeViewItem)sender;
            Frame frm = NebulaCore.PackageData.Frames[(int)tVI.Tag];
            FrameView.Header = frm.FrameName;
            FrameView.Content = FrameView_CreateView(frm);
        }

        private void FrameView_OnScroll(object sender, MouseWheelEventArgs e)
        {
            Grid sndr = (Grid)sender;
            Viewbox box = (Viewbox)sndr.Children[0];
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
            if (NebulaCore.PackageData == null || NebulaCore.PackageData.Frames.Count == 0) return new Grid();
            Grid grid = new Grid();
            grid.ClipToBounds = true;

            ListBox listBox = new ListBox();
            listBox.Background = Brushes.Transparent;
            grid.Children.Add(listBox);

            foreach (Frame frm in NebulaCore.PackageData.Frames)
            {
                ListBoxItem lBI = new ListBoxItem();
                lBI.Height = 90;
                lBI.BorderBrush = TV_Main.BorderBrush;
                lBI.BorderThickness = new Thickness(0, 0, 0, 1);

                Grid lBIGrid = new Grid();
                grid.ClipToBounds = true;

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

                lBIGrid.Children.Add(lBIImg);
                lBIGrid.Children.Add(lBIName);
                lBIGrid.Children.Add(lBISize);
                lBIGrid.Children.Add(lBIObjCnt);
                lBI.Content = lBIGrid;

                listBox.Items.Add(lBI);
            }
            return grid;
        }
    }
}