using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using EWLDitital.Managers;
using System.Windows.Media.Animation;
using EWLDitital.PresentationLayer.ViewModels;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using EWLDitital.PresentationLayer.Models;
using System.Data;
using System.Collections;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.IO;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.Portal;
using System.Xml;
using Polyline = Esri.ArcGISRuntime.Geometry.Polyline;
using System.Globalization;

namespace EWLDitital.PresentationLayer.Views
{

    public partial class MapCateloge : UserControl
    {
        private const string LabelDefinitionJson =
           @"{
                    ""labelExpressionInfo"":{""expression"":""$feature.ShortName""},
                    ""labelPlacement"":""esriServerPolygonPlacementAlwaysHorizontal"",
                    ""symbol"":
                        { 
                            ""angle"":0,
                            ""backgroundColor"":[0,0,0,0],
                            ""borderLineColor"":[0,0,0,0],
                            ""borderLineSize"":0,
                            ""borderLineSize"":0,
                            ""color"":[255,255,255,255],
                            ""font"":
                                {
                                    ""decoration"":""none"",
                                    ""size"":10,
                                    ""style"":""normal"",
                                    ""weight"":""normal""
                                },
                            ""horizontalAlignment"":""center"",
                            ""kerning"":false,
                            ""type"":""esriTS"",
                            ""verticalAlignment"":""middle"",
                            ""xoffset"":0,
                            ""yoffset"":0
                        }
               }";

        private const string portLabelDefinitionJson =
           @"{
                    ""labelExpressionInfo"":{""expression"":""$feature.MyCustomKey""},
                    ""labelPlacement"":""esriServerPointLabelPlacementAboveLeft"",
                    ""symbol"":
                        { 
                            ""angle"":0,
                            ""backgroundColor"":[0,0,0,0],
                            ""borderLineColor"":[0,0,0,0],
                            ""borderLineSize"":0,
                              ""color"":[29 ,30,30,255],
                            ""font"":
                                {
                                    ""decoration"":""none"",
                                    ""size"":9,
                                    ""style"":""normal"",
                                    ""weight"":""normal""
                                },
                            ""haloColor"":[52, 231, 246,255],
                            ""haloSize"":1,
                            ""horizontalAlignment"":""center"",
                            ""kerning"":false,
                            ""type"":""esriTS"",
                            ""verticalAlignment"":""middle"",
                            ""xoffset"":0,
                            ""yoffset"":0
                        }
               }";

        private const string polylineazimuthDefinitionJson =
      @"{
                    ""labelExpressionInfo"":{""expression"":""$feature.ShortName""},
                    ""labelPlacement"":""esriServerLinePlacementAboveAlong"",
                    ""symbol"":
                        { 
                           
                            ""angle"":0,
                            ""backgroundColor"":[0,0,0,0],
                            ""borderLineColor"":[0,0,0,0],
                            ""borderLineSize"":0,
                            ""color"":[255,0,0,255],
                            ""font"":
                                {
                                    ""decoration"":""none"",
                                    ""size"":8,
                                    ""style"":""normal"",
                                    ""weight"":""normal""
                                },
                            ""haloColor"":[255,255,255,255],
                            ""haloSize"":2,
                            ""horizontalAlignment"":""center"",
                            ""kerning"":false,
                            ""type"":""esriTS"",
                            ""verticalAlignment"":""middle"",
                            ""xoffset"":0,
                            
                            ""yoffset"":0
                        },
                            ""repeatLabelDistance"":432,
                            ""repeatLabel"":true,
                            ""minScale"": 8000000,
                            ""maxScale"": 62500
                        
                       
               }";

        private const string waypointLabelDefinitionJson =
        @"{
                    ""labelExpressionInfo"":{""expression"":""$feature.ShortName""},
                    ""labelPlacement"":""esriServerPointLabelPlacementCenterRight"",
                    ""symbol"":
                        { 
                           
                            ""angle"":0,
                            ""backgroundColor"":[0,0,0,0],
                            ""borderLineColor"":[0,0,0,0],
                            ""borderLineSize"":0,
                            ""color"":[255,0,0,255],
                            ""font"":
                                {
                                    ""decoration"":""none"",
                                    ""size"":8,
                                    ""style"":""normal"",
                                    ""weight"":""normal""
                                },
                            ""haloColor"":[255,255,255,255],
                            ""haloSize"":2,
                            ""horizontalAlignment"":""center"",
                            ""kerning"":false,
                            ""type"":""esriTS"",
                            ""verticalAlignment"":""middle"",
                            ""repeatLabelDistance"":0,
                            ""xoffset"":0,
                            ""yoffset"":0
                        },
                    
                            ""minScale"": 8000000,
                            ""maxScale"": 62500
                        
                       
               }";
        public static LabelDefinition waypointlabelDefinition = LabelDefinition.FromJson(waypointLabelDefinitionJson);

        public static LabelDefinition portlabeldefintion = LabelDefinition.FromJson(portLabelDefinitionJson);
        public static LabelDefinition labelDefinition = LabelDefinition.FromJson(LabelDefinitionJson);
        public static LabelDefinition polylinelabelDef = LabelDefinition.FromJson(polylineazimuthDefinitionJson);

        PolylineBuilder editlinebuilder;
        IReadOnlyList<MapPoint> editlinemp = new List<MapPoint>();

        //GraphicsOverlay portnameoverlay;
        //GraphicsOverlay redportnameoverlay;
        //GraphicsOverlay greenportnameoverlay;

        GraphicsOverlay portnameoverlay = new GraphicsOverlay()
        {
            Id = "Portoverlay",
            LabelsEnabled = true,
            LabelDefinitions = { portlabeldefintion }
        };
        GraphicsOverlay greenportnameoverlay = new GraphicsOverlay()
        {
            Id = "greenPortoverlay",
            LabelsEnabled = true,
            MaxScale = 62500,
            MinScale = 32000000,
            LabelDefinitions = { portlabeldefintion }
        };
        GraphicsOverlay redportnameoverlay = new GraphicsOverlay()
        {
            Id = "redPortoverlay",
            LabelsEnabled = true,
            MaxScale = 62500,
            MinScale = 8000000,
            LabelDefinitions = { portlabeldefintion }
        };

        GraphicsOverlay overviewoverlay = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "1",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay generaloverlay = new GraphicsOverlay()
        {
            LabelsEnabled = true,
            MaxScale = 62500,
            MinScale = 32000000,
            Id = "2",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay coastaloverlay = new GraphicsOverlay()
        {
            LabelsEnabled = true,
            MaxScale = 62500,
            MinScale = 8000000,
            Id = "3",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay approachoverlay = new GraphicsOverlay()
        {
            LabelsEnabled = true,
            Id = "4",
            MaxScale = 62500,
            MinScale = 2000000,
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay harbouroverlay = new GraphicsOverlay()
        {
            LabelsEnabled = true,
            MaxScale = 62500,
            MinScale = 500000,
            Id = "5",
            LabelDefinitions = { labelDefinition }
        };

        GraphicsOverlay berthingoverlay = new GraphicsOverlay()
        {
            LabelsEnabled = true,
            MaxScale = 62500,
            MinScale = 125000,
            Id = "6",
            LabelDefinitions = { labelDefinition }
        };


        GraphicsOverlay _sketchOverlay = new GraphicsOverlay()
        {
            LabelsEnabled = true,
            LabelDefinitions = { polylinelabelDef }
        };


        GraphicsOverlay region_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "7",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay transit_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "9",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay port_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 32000000,
            LabelsEnabled = true,
            Id = "8",
            LabelDefinitions = { labelDefinition }
        };

        GraphicsOverlay ADRS1345_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "10",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay ADRS2_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "11",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay ADRS6_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "12",
            LabelDefinitions = { labelDefinition }
        };

        GraphicsOverlay lol_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "13",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay enpsd_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "14",
            LabelDefinitions = { labelDefinition }
        };
        GraphicsOverlay totaltide_Graphic = new GraphicsOverlay()
        {
            MaxScale = 62500,
            MinScale = 128000000,
            LabelsEnabled = true,
            Id = "15",
            LabelDefinitions = { labelDefinition }
        };

        GraphicsOverlay routewaypointoverlay = new GraphicsOverlay()
        {
            Id = "17",
            LabelsEnabled = true,
            LabelDefinitions = { waypointlabelDefinition }
        };
        Graphic routeline = null;
        Graphic polygondrawgrp = null;

        List<Graphic> twogrp = new List<Graphic>();//declare variable
        List<Graphic> gridrouteline = new List<Graphic>();//declare variable

        private Graphic _polygonGraphic;
        public Dictionary<string, object> graphattribute;

        public ObservableCollection<Family> Families { get; set; }
        Common.CommonConst objcont = new Common.CommonConst();
        public Dictionary<string, Esri.ArcGISRuntime.Geometry.Geometry> geometrycollection = new Dictionary<string, Esri.ArcGISRuntime.Geometry.Geometry>();
        readonly AVCSManager objMgr = new AVCSManager();

        GraphicsOverlay _sketchRectOverlay = new GraphicsOverlay()
        {
            Id= "_sketchRectOverlay"
        };
        public string crew = "";
        public SketchCreationMode creationMode;
        public Esri.ArcGISRuntime.Geometry.Geometry g1;

        PolylineBuilder polylineBuilder = null;
        PolygonBuilder polygonbuild = null;
        List<Graphic> stopgraphiclin = new List<Graphic>();

        PolyList polist = new PolyList();
        IReadOnlyList<MapPoint> _polypointcollection = new List<MapPoint>();

        EnvelopeBuilder panenvelope = null;

        List<ShoppingCart> cart = new List<ShoppingCart>();
        List<Temperatures> cart1 = new List<Temperatures>();
        List<ShoppingCart> checkout = new List<ShoppingCart>();

        List<string> lstSelectedEmpNo = new List<string>();
        List<string> lstSelectedRoute = new List<string>();

        public string Maincat = "";

        public List<string> ScaleItems = new List<string>();
        public async void Bindcheckpout()
        {
            try
            {


                bool result = false;
                tabcontroler.Items.Clear();
                int i = 0;
                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();
                DataTable dt5 = new DataTable();

                LoadingAdorner.IsAdornerVisible = true;
                await Task.Delay(2000);


                DataTable chkdt = new DataTable();

                chkdt = objCart.GetShoppingCartByAVCS();

                List<DataRow> listRowsToDelete = new List<DataRow>();
                List<GraphicsOverlay> grc = new List<GraphicsOverlay>();

                foreach (var item in MyMapView.GraphicsOverlays)
                {
                    if (item.Id == "7" || item.Id == "8" || item.Id == "9")
                    {
                        grc.Add(item);
                    }
                }
                string sa = "";
                foreach (DataRow item1 in chkdt.Rows)
                {
                    sa += item1["ShortName"].ToString() + ",";

                }

                foreach (var item in grc.SelectMany(x => x.Graphics))
                {
                    if (sa.Contains(item.Attributes["ShortName"].ToString()))
                    {
                        XElement xele = XElement.Parse(item.Attributes["SubUnit"].ToString());
                        var dddd = xele.Descendants();
                        string sd = "";
                        foreach (var item1 in dddd)
                        {
                            sd += item1.Value + ",";

                        }
                        foreach (DataRow row in chkdt.Rows)
                        {
                            string shname = row["ShortName"].ToString();
                            if (sd.Contains(shname))
                            {
                                listRowsToDelete.Add(row);
                                result = true;
                            }
                        }
                    }
                }

                foreach (DataRow drowToDelete in listRowsToDelete.Distinct())
                {
                    if (sa.Contains(drowToDelete["ShortName"].ToString()))
                    {
                            chkdt.Rows.Remove(drowToDelete);
                            chkdt.AcceptChanges();
                    }
                }
                

                tabcontroler.Items.Clear();

                dt = objCart.GetShoppingCartByAVCSProduct("AVCS Products");

                dt1 = objCart.GetShoppingCartByProduct("ADLL");
                dt2 = objCart.GetShoppingCartByProduct("ADRS");
                dt3 = objCart.GetShoppingCartByProduct("e-Nautical Publications");
                dt4 = objCart.GetShoppingCartByProduct("Misc Publications");
                dt5 = objCart.GetShoppingCartByProduct("TotalTide");
                if (chkdt.Rows.Count > 0)
                {
                    i = i + 1;
                    avcs_tab.Visibility = Visibility.Visible;

                    tabcontroler.Items.Add(avcs_tab);
                    avcs_tab.IsSelected = true;
                    if (result)
                    {
                        checkoutGrid.ItemsSource = chkdt.DefaultView;
                    }
                    else
                    {

                        checkoutGrid.ItemsSource = dt.DefaultView;
                    }
                }
                else
                {
                    avcs_tab.Visibility = Visibility.Hidden;
                    checkoutGrid.ItemsSource = null;
                }

                if (dt1.Rows.Count > 0)
                {
                    i = i + 1;
                    adll_tab.Visibility = Visibility.Visible;
                    tabcontroler.Items.Add(adll_tab);
                    checkoutGrid1.ItemsSource = dt1.DefaultView;
                    if (i == 1)
                    {
                        adll_tab.IsSelected = true;
                    }
                }
                else
                {
                    adll_tab.Visibility = Visibility.Hidden;
                    checkoutGrid1.ItemsSource = null;
                }
                if (dt2.Rows.Count > 0)
                {
                    i = i + 1;
                    adrs_tab.Visibility = Visibility.Visible;
                    tabcontroler.Items.Add(adrs_tab);
                    checkoutGrid2.ItemsSource = dt2.DefaultView;
                    if (i == 1)
                    {
                        adrs_tab.IsSelected = true;
                    }
                }
                else
                {
                    adrs_tab.Visibility = Visibility.Hidden;
                    checkoutGrid2.ItemsSource = null;
                }
                if (dt3.Rows.Count > 0)
                {
                    i = i + 1;
                    enp_tab.Visibility = Visibility.Visible;
                    tabcontroler.Items.Add(enp_tab);
                    checkoutGrid3.ItemsSource = dt3.DefaultView;
                    if (i == 1)
                    {
                        enp_tab.IsSelected = true;
                    }
                }
                else
                {
                    enp_tab.Visibility = Visibility.Hidden;
                    checkoutGrid3.ItemsSource = null;
                }
                if (dt4.Rows.Count > 0)
                {
                    i = i + 1;
                    misc_tab.Visibility = Visibility.Visible;
                    tabcontroler.Items.Add(misc_tab);
                    checkoutGrid4.ItemsSource = dt4.DefaultView;
                    if (i == 1)
                    {
                        misc_tab.IsSelected = true;
                    }
                }
                else
                {
                    misc_tab.Visibility = Visibility.Hidden;
                    checkoutGrid4.ItemsSource = null;
                }
                if (dt5.Rows.Count > 0)
                {
                    i = i + 1;
                    tides_tab.Visibility = Visibility.Visible;
                    checkoutGrid5.ItemsSource = dt5.DefaultView;
                    tabcontroler.Items.Add(tides_tab);
                    if (i == 1)
                    {
                        tides_tab.IsSelected = true;
                    }
                }
                else
                {
                    tides_tab.Visibility = Visibility.Hidden;
                    checkoutGrid5.ItemsSource = null;
                }
                LoadingAdorner.IsAdornerVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public void BindRouteLine(string RouteType)
        {
            try
            {
                DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                DataTable dt = new DataTable();
                dt = objCart.GetRouteLine(RouteType);
                if (RouteType == "1")
                {
                    if (dt.Rows.Count > 0)
                    {
                        RouteGrid.ItemsSource = dt.DefaultView;
                    }
                    else
                    {
                        RouteGrid.ItemsSource = null;
                    }
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        RouteGrid1.ItemsSource = dt.DefaultView;
                    }
                    else
                    {
                        RouteGrid1.ItemsSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public MapCateloge()
        {

            InitializeComponent();

            MyMapView.SketchEditor.GeometryChanged += (s, e) =>
            {
                undoStack.Push(e.OldGeometry);
            };

            this.Families = ItemHelper.BindTree();

        }

        public void PortnameLabelgenerator()
        {
            try
            {
                string queuePath = AppDomain.CurrentDomain.BaseDirectory + "\\XMLFiles" + "\\PortsGazeteer.xml";
                // Create a new unique value renderer.
                XDocument prtnamesLabel = XDocument.Load(queuePath);


                var commands = from cmd in prtnamesLabel.Descendants("PortsGazeteer")
                               select new
                               {
                                   PORT_ID = (string)cmd.Element("PORT_ID"),
                                   NAME = (string)cmd.Element("NAME"),
                                   LAT = (string)cmd.Element("LAT"),
                                   Long = (string)cmd.Element("LONG"),
                                   SCAMIN = (string)cmd.Element("SCAMIN"),


                               };
                foreach (var item in commands)
                {

                    if (item.NAME != null)
                    {
                        var lat = double.Parse(item.LAT);
                        var longi = double.Parse(item.Long);
                        var text = item.NAME;
                        var scamin = item.SCAMIN;
                        MapPoint mp = new MapPoint(longi, lat, SpatialReferences.Wgs84);
                        portpoint_graphic_creation(mp, text, scamin);
                    }
                    else
                    {

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void portpoint_graphic_creation(MapPoint point, string Name, string Scamin)
        {

            if (Scamin == "1")
            {

                // define the location for the point (lower left of text)
                var mapPoint = point;

                // create a text symbol: define color, font, size, and text for the label
                TextSymbol textSym = new TextSymbol(Name, System.Drawing.Color.Blue, 10, Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Left,
                                                             Esri.ArcGISRuntime.Symbology.VerticalAlignment.Bottom);
                // create a marker symbol
                var markerSym = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 2.5);
                // create a graphic for the text (apply TextSymbol)
                var textGraphic = new Graphic(mapPoint, textSym);

                // create a graphic for the marker (apply SimpleMarkerSymbol)
                var markerGraphic = new Graphic(mapPoint, markerSym);
                markerGraphic.Attributes["MyCustomKey"] = Name;
                portnameoverlay.Graphics.Add(markerGraphic);
                // add text and marker graphics
                // _portlabeNameblueloverlay.Graphics.Add(textGraphic);
                // _portblueloverlay.Graphics.Add(markerGraphic);

            }

            else if (Scamin == "2")
            {
                // define the location for the point (lower left of text)
                var mapPoint = point;

                // create a text symbol: define color, font, size, and text for the label
                TextSymbol textSym = new TextSymbol(Name, System.Drawing.Color.Blue, 10, Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Left,
                                                             Esri.ArcGISRuntime.Symbology.VerticalAlignment.Bottom);
                var markerSym = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 2.5);
                // create a graphic for the text (apply TextSymbol)
                // var textGraphic = new Graphic(mapPoint, textSym);

                // create a graphic for the marker (apply SimpleMarkerSymbol)
                var markerGraphic = new Graphic(mapPoint, markerSym);
                markerGraphic.Attributes["MyCustomKey"] = Name;
                //portnameoverlay.Graphics.Add(markerGraphic);
                greenportnameoverlay.Graphics.Add(markerGraphic);
                // add text and marker graphics
                //  _portlabeNamegreenloverlay.Graphics.Add(textGraphic);
                // _portgreenloverlay.Graphics.Add(markerGraphic);

            }
            else if (Scamin == "3" || Scamin == "4")
            {
                // define the location for the point (lower left of text)
                var mapPoint = point;

                // create a text symbol: define color, font, size, and text for the label
                TextSymbol textSym = new TextSymbol(Name, System.Drawing.Color.Blue, 10, Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Left,
                                                             Esri.ArcGISRuntime.Symbology.VerticalAlignment.Bottom);
                var markerSym = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 2.5);
                // create a graphic for the text (apply TextSymbol)
                var textGraphic = new Graphic(mapPoint, textSym);

                // create a graphic for the marker (apply SimpleMarkerSymbol)
                var markerGraphic = new Graphic(mapPoint, markerSym);
                markerGraphic.Attributes["MyCustomKey"] = Name;
                //  portnameoverlay.Graphics.Add(markerGraphic);
                // add text and marker graphics
                redportnameoverlay.Graphics.Add(markerGraphic);
                //  _portlabeNameredloverlay.Graphics.Add(textGraphic);
                //  _portredloverlay.Graphics.Add(markerGraphic);
            }
        }

        public void Initialize()
        {
            try
            {
                MyMapView.SelectionProperties.Color = System.Drawing.Color.Transparent;
                mylblClickCount1.Content = "Scale View - 1 : 128000000";
                MyMapView.GraphicsOverlays.Add(portnameoverlay);
                MyMapView.GraphicsOverlays.Add(redportnameoverlay);
                MyMapView.GraphicsOverlays.Add(greenportnameoverlay);

                MyMapView.GraphicsOverlays.Add(generaloverlay);
                MyMapView.GraphicsOverlays.Add(overviewoverlay);
                MyMapView.GraphicsOverlays.Add(coastaloverlay);
                MyMapView.GraphicsOverlays.Add(approachoverlay);
                MyMapView.GraphicsOverlays.Add(harbouroverlay);
                MyMapView.GraphicsOverlays.Add(berthingoverlay);

                MyMapView.GraphicsOverlays.Add(region_Graphic);
                MyMapView.GraphicsOverlays.Add(port_Graphic);
                MyMapView.GraphicsOverlays.Add(transit_Graphic);

                MyMapView.GraphicsOverlays.Add(ADRS1345_Graphic);
                MyMapView.GraphicsOverlays.Add(ADRS2_Graphic);
                MyMapView.GraphicsOverlays.Add(ADRS6_Graphic);

                MyMapView.GraphicsOverlays.Add(lol_Graphic);
                MyMapView.GraphicsOverlays.Add(enpsd_Graphic);
                MyMapView.GraphicsOverlays.Add(totaltide_Graphic);

                MyMapView.GraphicsOverlays.Add(_sketchOverlay);

                MyMapView.GraphicsOverlays.Add(routewaypointoverlay);

                MyMapView.GraphicsOverlays.Add(_sketchRectOverlay);

                _sketchOverlay.Id = "seven";

                graphiclaysoff();


                //await Task.Run(async () =>
                //{
                GetAVCS();
                GetAVCSFolio();
                GetADRS();
                scaleforoverlay();
                PortnameLabelgenerator();
                //});

                MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                MyMapView.MouseMove += MapView_MouseMoved;
                MyMapView.PreviewMouseWheel += mouseWheel_scale;
                MyMapView.MouseRightButtonDown += mouseWheel_rightclic;
                MyMapView.PreviewMouseMove += MapView_Mouse_move_cursor_line;
                // MyMapView.PreviewMouseWheel += mouseWheel_Changed;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void mouseWheel_rightclic(object sender, MouseEventArgs e)//declaring a function
        {
            try
            {
                string message = "Are you sure you want to complete the route line?";
                string caption = "Confirmation";
                lstSelectedRoute.Clear();
                //SelectedRoutName = "";

                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Question;
                if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                {
                    // OK code here
                    geodesic_savbtnpoints_webmerccollection.Clear();
                    //Mouse.OverrideCursor = Cursors.Cross;
                    _sketchOverlay.Graphics.Clear();
                    routewaypointoverlay.Graphics.Clear();
                    gridrouteline.Clear();//add this new line
                    importedlinepoints.Clear();
                    normalizedimportedpoints.Clear();
                    symbolspointslist_savebtn.Clear();
                    // Cursor = systemCursor2;
                    save_Click1(sender, e);
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped -= MyMapViewOnGeoViewTapped;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void mouseWheel_scale(object sender, MouseWheelEventArgs e)//declaring a function
        {
            try
            {
                var scal = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                var scal1 = scal.TargetScale;
                var displayed_value = scal1.ToString("N0");
                var displ1 = Convert.ToDouble(displayed_value);
                var formated_val = displ1.ToString("N0", CultureInfo.InvariantCulture);
                var finalstring = "Scale View - 1 :" + formated_val;
                //var displayed_value1 = formated_val.ToString("N0");
                mylblClickCount1.Content = finalstring;
            }
            catch
            {

            }
        }

        private async void MapView_Mouse_move_cursor_line(object sender, MouseEventArgs e)
        {
            try
            {
                if (MyMapView.SketchEditor.Geometry != null)
                {
                    Mouse.OverrideCursor = Cursors.Cross;
                }
                else
                {
                    var pixelTolerance = 10;
                    var returnPopupsOnly = false;
                    var maxResults = 100;
                    System.Windows.Point tapScreenPoint = e.GetPosition(MyMapView);
                    // MapPoint mapPoint = geoViewInputEventArgs.Location;

                    IReadOnlyList<IdentifyGraphicsOverlayResult> idGraphicOverlayResults = await MyMapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);
                    var sert = await MyMapView.IdentifyGraphicsOverlayAsync(_sketchOverlay, tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);
                    if (sert.Graphics.Count >= 1)
                    {

                        Mouse.OverrideCursor = Cursors.Cross;

                    }
                    else
                    {
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                }
                
            }
            catch
            {

            }

        }
        public void  scalrangelable()
        {
            try
            {
                var scal = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                var scal1 = scal.TargetScale;
                var displayed_value = scal1.ToString("N0");
                var displ1 = Convert.ToDouble(displayed_value);
                var formated_val = displ1.ToString("N0", CultureInfo.InvariantCulture);
                var finalstring = "Scale View - 1 :" + formated_val;
                //var displayed_value1 = formated_val.ToString("N0");
                mylblClickCount1.Content = finalstring;
            }
            catch
            {

            }
        }
        public void GetAVCS()
        {
            try
            {
                string queuePath = AppDomain.CurrentDomain.BaseDirectory + "\\XMLFiles" + "\\avcs_catalogue.xml";
                XDocument doc = XDocument.Load(queuePath);
                int ScalarVariableCount = doc.Root.Descendants("Products")
                                      .Elements("ENC").Count();
                Console.WriteLine("count{0}", ScalarVariableCount);
                var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);

                // var polygonPoints_list = new List<string, string>();
                var labelPointBass = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                // IDictionary<string, Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84) > dict = new Dictionary<string, string>();
                var query = from Polygon in doc.Root.Descendants("ENC")
                            group Polygon by Polygon.Element("ShortName").Value into gr
                            select gr;

                var shortname = doc.Root.Descendants("shortname");
                var srt = shortname.ToString();
                var reasons = doc.Descendants("Polygon").ToList();


                var latitude = new List<string>();
                var longitude = new List<string>();

                Func<XElement, string, string, string> getAttributeValue = (xElement, name, text) => xElement.Element(name).Value;


                foreach (var value in query)
                {
                    List<XElement> str = new List<XElement>();
                    // textname.Add(value.Key);
                    //  textname.Add(value.Key);
                    var q2 = value.Descendants("Polygon").SelectMany(array => array.Value.ToList());

                    value.Descendants("Polygon").ToList().ForEach(item =>
                    {
                        str.Add(item);
                    });
                    if (str.Count > 1)
                    {
                        foreach (var set1 in str)
                        {
                            set1.Descendants("Position").ToList().ForEach(item =>
                            {
                                latitude.Add(item.Attribute("latitude").Value);
                                longitude.Add(item.Attribute("longitude").Value);
                                polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));
                            });
                            var q5 = value.Descendants("Metadata").Select(s => new
                            {
                                ONE = s.Element("Scale").Value,
                                TWO = s.Element("Usage").Value,
                                THREE = s.Element("DatasetTitle").Value,
                                FOUR = s.Element("Status").Value,
                                Five = s.Element("Unit").Element("ID").Value,
                                SIX = s.Element("Unit").Element("Title").Value
                            }).ToList();
                            string title = q5[0].THREE;
                            string Usage = q5[0].TWO;
                            string Scale = q5[0].ONE;
                            string Status = q5[0].FOUR;
                            string UnitId = q5[0].Five;
                            string UnitTitle = q5[0].SIX;

                            CreateGraphic_Label(polygonPoints1, value.Key, Usage, Scale, title, Status, UnitId, UnitTitle);
                            polygonPoints1.Clear();
                        }
                    }
                    else
                    {
                        value.Descendants("Position").ToList().ForEach(item =>
                        {
                            latitude.Add(item.Attribute("latitude").Value);
                            longitude.Add(item.Attribute("longitude").Value);
                            polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));


                        });
                        var q5 = value.Descendants("Metadata").Select(s => new
                        {
                            ONE = s.Element("Scale").Value,
                            TWO = s.Element("Usage").Value,
                            THREE = s.Element("DatasetTitle").Value,
                            FOUR = s.Element("Status").Value,
                            Five = s.Element("Unit").Element("ID").Value,
                            SIX = s.Element("Unit").Element("Title").Value

                        }).ToList();
                        string title = q5[0].THREE;
                        string Usage = q5[0].TWO;
                        string Scale = q5[0].ONE;
                        string Status = q5[0].FOUR;
                        string UnitId = q5[0].Five;
                        string UnitTitle = q5[0].SIX;

                        CreateGraphic_Label(polygonPoints1, value.Key, Usage, Scale, title, Status, UnitId, UnitTitle);

                    }
                    polygonPoints1.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void GetAVCSFolio()
        {

            try
            {
                string s3 = "";
                IEnumerable<IGrouping<string, XElement>> query;

                query = objMgr.GetAVCSFolio(s3);

                var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                geometrycollection.Clear();
                //_graphicsOverlay.Graphics.Clear();
                //_polygonlabelOverlay.Graphics.Clear();
                polygonPoints1.Clear();

                var textname = new List<string>();
                var latitude = new List<string>();
                var longitude = new List<string>();
                var scale = new List<string>();
                var usage = new List<string>();
                var DatasetTitle = new List<string>();
                foreach (var value in query)
                {
                    List<XElement> str = new List<XElement>();
                    // textname.Add(value.Key);
                    //  textname.Add(value.Key);
                    var q2 = value.Descendants("Polygon").SelectMany(array => array.Value.ToList());

                    value.Descendants("Polygon").ToList().ForEach(item =>
                    {
                        str.Add(item);
                    });
                    if (str.Count > 1)
                    {
                        foreach (var set1 in str)
                        {
                            set1.Descendants("Position").ToList().ForEach(item =>
                            {
                                latitude.Add(item.Attribute("latitude").Value);
                                longitude.Add(item.Attribute("longitude").Value);
                                polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));
                            });
                            var q5 = value.Select(s => new
                            {
                                ONE = s.Element("UnitType").Value,
                                TWO = s.Element("ID").Value,
                                TRHEE = s.Element("Title").Value,
                                FOUR = s.Element("SubUnit").ToString()
                            }).ToList();
                            string four = q5[0].ONE.Replace("AVCS Folio Regional", "7").Replace("AVCS Folio Port", "8").Replace("AVCS Folio Transit", "9");
                            string title = q5[0].TRHEE;
                            string Unite = q5[0].FOUR;
                            CreateGraphic_Label(polygonPoints1, value.Key, four, "", title, "", Unite, "");
                            polygonPoints1.Clear();
                        }
                    }
                    else
                    {
                        value.Descendants("Position").ToList().ForEach(item =>
                        {
                            latitude.Add(item.Attribute("latitude").Value);
                            longitude.Add(item.Attribute("longitude").Value);
                            polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));


                        });
                        var q5 = value.Select(s => new
                        {
                            ONE = s.Element("UnitType").Value,
                            TWO = s.Element("ID").Value,
                            TRHEE = s.Element("Title").Value,
                            FOUR = s.Element("SubUnit").ToString()
                        }).ToList();
                        string four = q5[0].ONE.Replace("AVCS Folio Regional", "7").Replace("AVCS Folio Port", "8").Replace("AVCS Folio Transit", "9");
                        string title = q5[0].TRHEE;
                        string Unite = q5[0].FOUR;
                        CreateGraphic_Label(polygonPoints1, value.Key, four, "", title, "", Unite, "");
                    }

                    polygonPoints1.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void GetADRS()
        {
            try
            {
                IEnumerable<IGrouping<string, XElement>> query;
                string s3 = "";
                query = objMgr.Getalrsdigital(s3);


                var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                geometrycollection.Clear();
                polygonPoints1.Clear();

                var textname = new List<string>();
                var latitude = new List<string>();
                var longitude = new List<string>();
                var scale = new List<string>();
                var usage = new List<string>();
                var DatasetTitle = new List<string>();


                foreach (var value in query)
                {
                    var q1 = value.Descendants("Metadata").Select(s => new
                    {
                        ONE = s.Element("DatasetTitle").Value

                    }).ToList();
                    string three = q1[0].ONE;
                    string final = "";
                    if (three.Contains("ADRS1345"))
                    {
                        final = "10";
                    }
                    if (three.Contains("ADRS2"))
                    {
                        final = "11";
                    }
                    if (three.Contains("ADRS6"))
                    {
                        final = "12";
                    }
                    List<XElement> str = new List<XElement>();
                    var q2 = value.Descendants("Polygon").SelectMany(array => array.Value.ToList());
                    value.Descendants("Polygon").ToList().ForEach(item =>
                    {
                        str.Add(item);
                    });
                    if (str.Count > 1)
                    {
                        foreach (var set1 in str)
                        {
                            set1.Descendants("Position").ToList().ForEach(item =>
                            {
                                latitude.Add(item.Attribute("latitude").Value);
                                longitude.Add(item.Attribute("longitude").Value);
                                polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));
                            });

                            var q3 = value.Descendants("Metadata").Select(s => new
                            {
                                ONE = s.Element("DatasetTitle").Value,
                                TWO = s.Element("Status").Value
                            }).ToList();
                            string title = q3[0].ONE;
                            string status = q3[0].TWO;
                            CreateGraphic_Label(polygonPoints1, value.Key, final, "", title, status, "", "");
                            polygonPoints1.Clear();
                        }
                    }
                    else
                    {
                        value.Descendants("Position").ToList().ForEach(item =>
                        {
                            latitude.Add(item.Attribute("latitude").Value);
                            longitude.Add(item.Attribute("longitude").Value);
                            polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));

                        });
                        var q3 = value.Descendants("Metadata").Select(s => new
                        {
                            ONE = s.Element("DatasetTitle").Value,
                            TWO = s.Element("Status").Value
                        }).ToList();

                        string title = q3[0].ONE;
                        string status = q3[0].TWO;
                        CreateGraphic_Label(polygonPoints1, value.Key, final, "", title, status, "", "");

                    }

                    polygonPoints1.Clear();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void graphiclaysoff()
        {
            generaloverlay.IsVisible = false; //generallabeloverlay.IsVisible = false;
            overviewoverlay.IsVisible = false;//overviewlabeloverlay.IsVisible = true;
            coastaloverlay.IsVisible = false; //coastallabeloverlay.IsVisible = false;
            approachoverlay.IsVisible = false; //approachlabeloverlay.IsVisible = false;
            harbouroverlay.IsVisible = false; //harbourlabeloverlay.IsVisible = false;
            berthingoverlay.IsVisible = false;//berthinglabeloverlay.IsVisible = false;
            region_Graphic.IsVisible = false;
            port_Graphic.IsVisible = false;
            transit_Graphic.IsVisible = false;

            ADRS1345_Graphic.IsVisible = false;
            ADRS2_Graphic.IsVisible = false;
            ADRS6_Graphic.IsVisible = false;

            lol_Graphic.IsVisible = false;
            enpsd_Graphic.IsVisible = false;
            totaltide_Graphic.IsVisible = false;
        }
        private void scaleforoverlay()
        {
            // overviewoverlay.MinScale = generaloverlay.MinScale= coastaloverlay.MinScale = approachoverlay.MinScale= harbouroverlay.MinScale= berthingoverlay.MinScale= 100000;
            overviewoverlay.MaxScale = 10000;
            //generaloverlay.MaxScale= coastaloverlay.MaxScale= approachoverlay.MaxScale= harbouroverlay.MaxScale= berthingoverlay.MaxScale =   100000;

            overviewoverlay.MinScale = 128000000;


            // generaloverlay.MinScale = 102400000;

            //coastaloverlay.MinScale = 25600000;

            // approachoverlay.MinScale = 6400000;

            // harbouroverlay.MinScale = 1600000;

            // berthingoverlay.MinScale = 400000;


        }
        public void RemoveHeilight()
        {
            try
            {
                Storyboard sb = Resources["sbHideRightMenu"] as Storyboard;
                sb.Begin(pnlRightMenu);
                pnlRightMenu.Visibility = Visibility.Hidden;
                checkout.Clear();

                DataTable ndt = new DataTable();
                ndt = objMgr.GetENC();
                if (ndt.Rows.Count > 0)
                {
                    foreach (DataRow row in ndt.Rows)
                    {
                        checkout.Add(new ShoppingCart()
                        {
                            ShortName = row["ShortName"].ToString(),
                            ProductName = row["ProductName"].ToString()
                        });
                        //Mouse_tap_AddedCart(row["ImagePath"].ToString());
                    }
                }
                List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                foreach (var item in MyMapView.GraphicsOverlays)
                {
                    if (item.Id != "seven")
                    {
                        grc.Add(item);
                    }
                }
                foreach (var ter in grc)
                {
                    foreach (var item in ter.Graphics)
                    {

                        var aa = item.Attributes.Keys.FirstOrDefault();

                        if (cart.Any(x => x.ShortName == aa) || cart1.Any(x => x.FProductId == aa))
                        {
                            item.IsSelected = true;
                            Mouse_tap_select(item);
                        }
                        else
                        {
                            item.IsSelected = false;
                            Mouse_Tap_Unselect(item);
                        }
                        if (Maincat == "AVCS Chart")
                        {
                            if (checkout.Any(x => x.ShortName == aa && x.ProductName == "AVCS Products"))
                            {
                                Mouse_tap_AddedCart(item);
                            }
                        }
                        if (Maincat == "AVCS Folio")
                        {
                            if (checkout.Any(x => x.ShortName == aa && x.ProductName == "AVCS Products"))
                            {
                                Mouse_tap_AddedCart(item);
                            }
                        }
                        if (Maincat == "Digital List of Lights")
                        {
                            if (checkout.Any(x => x.ShortName == aa && x.ProductName == "ADLL"))
                            {
                                Mouse_tap_AddedCart(item);
                            }
                        }
                        if (Maincat == "Digital List of Radio Signals")
                        {
                            if (checkout.Any(x => x.ShortName == aa && x.ProductName == "ADRS"))
                            {
                                Mouse_tap_AddedCart(item);
                            }
                        }
                        if (Maincat == "e-NP Sailing Direction")
                        {
                            if (checkout.Any(x => x.ShortName == aa && x.ProductName == "e-Nautical Publications"))
                            {
                                Mouse_tap_AddedCart(item);
                            }
                        }
                        if (Maincat == "TotalTide")
                        {
                            if (checkout.Any(x => x.ShortName == aa && x.ProductName == "TotalTide"))
                            {
                                Mouse_tap_AddedCart(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void mouseWheel_Changed(object sender, MouseWheelEventArgs e)
        {
            try
            {
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

                var list = new[] { 62500, 125000, 250000, 500000, 1000000, 2000000, 4000000, 8000000, 16000000, 32000000, 64000000, 128000000 };
                var input = tes.TargetScale;
                var diffList = from number in list
                               select new
                               {
                                   number,
                                   difference = Math.Abs(number - input)
                               };
                var result = (from diffItem in diffList
                              orderby diffItem.difference
                              select diffItem).First().number;

                double ImageScale = result;

                if (e.Delta > 0)
                {
                    if (Maincat == "AVCS Chart")
                    {

                        if (ImageScale > 62500 && ImageScale <= 125000)
                        {
                            if (crew.Contains("Berthing"))
                            {
                                berthingoverlay.IsVisible = true;

                            }
                            else
                            {
                                berthingoverlay.IsVisible = false;
                            }
                        }
                        else if (ImageScale > 125000 && ImageScale <= 500000)
                        {
                            if (crew.Contains("Harbour"))
                            {
                                harbouroverlay.IsVisible = true;
                            }
                            else
                            {
                                harbouroverlay.IsVisible = false;
                                berthingoverlay.IsVisible = false;
                            }
                        }
                        else if (tes.TargetScale > 500000 && tes.TargetScale <= 2000000)
                        {
                            if (crew.Contains("Approach"))
                            {
                                approachoverlay.IsVisible = true;

                            }
                            else
                            {
                                approachoverlay.IsVisible = false;
                                harbouroverlay.IsVisible = false;
                                berthingoverlay.IsVisible = false;
                            }

                        }
                        else if (ImageScale > 2000000 && ImageScale <= 8000000)
                        {
                            if (crew.Contains("Coastal"))
                            {
                                coastaloverlay.IsVisible = true;
                            }
                            else
                            {
                                coastaloverlay.IsVisible = false;
                                approachoverlay.IsVisible = false;
                                harbouroverlay.IsVisible = false;
                                berthingoverlay.IsVisible = false;
                            }


                        }
                        else if (ImageScale > 8000000 && ImageScale <= 32000000)
                        {
                            if (crew.Contains("General"))
                            {
                                generaloverlay.IsVisible = true;
                            }
                            else
                            {
                                generaloverlay.IsVisible = false;
                                coastaloverlay.IsVisible = false;
                                approachoverlay.IsVisible = false;
                                harbouroverlay.IsVisible = false;
                                berthingoverlay.IsVisible = false;
                            }
                        }
                        else if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            if (crew.Contains("Overview"))
                            {
                                overviewoverlay.IsVisible = true;
                            }
                            else
                            {
                                overviewoverlay.IsVisible = false;
                                generaloverlay.IsVisible = false;
                                coastaloverlay.IsVisible = false;
                                approachoverlay.IsVisible = false;
                                harbouroverlay.IsVisible = false;
                                berthingoverlay.IsVisible = false;
                            }
                        }
                    }
                    if (Maincat == "AVCS Folio")
                    {
                        if (ImageScale > 62500 && ImageScale <= 32000000)
                        {
                            if (crew.Contains("Port"))
                            {
                                port_Graphic.IsVisible = true;
                            }
                            else
                            {

                                port_Graphic.IsVisible = false;
                            }

                        }
                        else if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            if (crew.Contains("Regional"))
                            {
                                region_Graphic.IsVisible = true;
                            }
                            else
                            {

                                region_Graphic.IsVisible = false;
                            }

                            if (crew.Contains("Transit"))
                            {
                                transit_Graphic.IsVisible = true;
                            }
                            else
                            {

                                transit_Graphic.IsVisible = false;
                            }
                        }
                    }
                    if (Maincat == "Digital List of Radio Signals")
                    {

                        if (crew.Contains("ADRS2"))
                        {
                            ADRS1345_Graphic.IsVisible = true;
                        }
                        else
                        {

                            ADRS1345_Graphic.IsVisible = false;
                        }


                        if (crew.Contains("ADRS1345"))
                        {
                            ADRS2_Graphic.IsVisible = true;
                        }
                        else
                        {

                            ADRS2_Graphic.IsVisible = false;
                        }

                        if (crew.Contains("ADRS6"))
                        {
                            ADRS6_Graphic.IsVisible = true;
                        }
                        else
                        {

                            ADRS6_Graphic.IsVisible = false;
                        }
                    }
                    if (Maincat == "Digital List of Lights")
                    {
                        lol_Graphic.IsVisible = true;
                    }
                    else
                    {

                        lol_Graphic.IsVisible = false;
                    }
                    if (Maincat == "e-NP Sailing Direction")
                    {
                        enpsd_Graphic.IsVisible = true;
                    }
                    else
                    {

                        enpsd_Graphic.IsVisible = false;
                    }
                    if (Maincat == "TotalTide")
                    {
                        totaltide_Graphic.IsVisible = true;
                    }
                    else
                    {

                        totaltide_Graphic.IsVisible = false;
                    }
                }
                else if (e.Delta < 0)
                {

                    if (Maincat == "AVCS Chart")
                    {
                        if (ImageScale > 62500 && ImageScale <= 125000)
                        {
                            berthingoverlay.IsVisible = false;
                        }
                        else if (ImageScale > 125000 && ImageScale <= 500000)
                        {

                            harbouroverlay.IsVisible = false;
                            berthingoverlay.IsVisible = false;

                        }
                        else if (ImageScale > 500000 && ImageScale <= 2000000)
                        {
                            approachoverlay.IsVisible = false;
                            harbouroverlay.IsVisible = false;
                            berthingoverlay.IsVisible = false;
                        }
                        else if (ImageScale > 2000000 && ImageScale <= 8000000)
                        {
                            coastaloverlay.IsVisible = false;
                            approachoverlay.IsVisible = false;
                            harbouroverlay.IsVisible = false;
                            berthingoverlay.IsVisible = false;
                        }
                        else if (ImageScale > 8000000 && ImageScale <= 32000000)
                        {

                            generaloverlay.IsVisible = false;
                            coastaloverlay.IsVisible = false;
                            approachoverlay.IsVisible = false;
                            berthingoverlay.IsVisible = false;
                            harbouroverlay.IsVisible = false;

                        }
                        else if (tes.TargetScale > 32000000 && tes.TargetScale <= 128000000)
                        {
                            if (crew.Contains("Overview"))
                            {
                                overviewoverlay.IsVisible = true;
                                generaloverlay.IsVisible = false;
                                coastaloverlay.IsVisible = false;
                                approachoverlay.IsVisible = false;
                                berthingoverlay.IsVisible = false;
                                harbouroverlay.IsVisible = false;
                            }
                        }
                    }
                    if (Maincat == "AVCS Folio")
                    {
                        if (ImageScale > 62500 && ImageScale <= 32000000)
                        {
                            if (crew.Contains("Port"))
                            {
                                port_Graphic.IsVisible = true;
                            }

                        }

                        else if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            if (crew.Contains("Regional"))
                            {
                                region_Graphic.IsVisible = true;
                            }

                            if (crew.Contains("Transit"))
                            {
                                transit_Graphic.IsVisible = true;
                            }
                            port_Graphic.IsVisible = false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
        private void btnLeftMenuShow_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu1("sbHideLeftMenu", btnLeftMenuShow, pnlLeftMenu);
        }
        private void btnTopMenuShow_Click(object sender, RoutedEventArgs e)
        {
            Bindcheckpout();
            chkgrid.Visibility = Visibility.Visible;
        }
        private void btnCheckouthide_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["sbHideRightMenu"] as Storyboard;
            sb.Begin(pnlRightMenu);
            pnlRightMenu.Visibility = Visibility.Hidden;
            RemoveHeilight();
        }
        private void btnCheckouthide1_Click(object sender, RoutedEventArgs e)
        {
            chkgrid.Visibility = Visibility.Hidden;
            //ShowHideMenu("sbHideRightCheckout", btnLeftMenuShow, pnlRightchecout);
        }
        private void btnRoutehide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideRightRoute", btnLeftMenuShow, pnlRightRoute);
        }
        private void btnRoutehide_Click1(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideRightRoute", btnLeftMenuShow, pnlRightRoute1);
        }
        private void ShowHideMenu(string Storyboard, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);
        }
        private void ShowHideMenu(string Storyboard, Button btnShow, System.Windows.Controls.Grid pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);
        }
        private void ShowHideMenu1(string Storyboard, Button btnShow, StackPanel pnl)
        {
            try
            {
                if (btnShow.HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                {
                    treeView.Visibility = Visibility.Visible;
                    var uriSource = new Uri("/Icons/left.png", UriKind.Relative);

                    fltimg.Source = new BitmapImage(uriSource);
                    fltimg.Width = 16;

                    Storyboard sb = Resources["sbShowLeftMenu"] as Storyboard;
                    sb.Begin(pnl);
                    btnShow.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                }
                else
                {
                    treeView.Visibility = Visibility.Hidden;
                    var uriSource = new Uri("/Icons/Right.png", UriKind.Relative);
                    fltimg.Source = new BitmapImage(uriSource);
                    fltimg.Width = 35;
                    //treeView1.ItemsSource = null;

                    Storyboard sb = Resources["sbHideLeftMenu"] as Storyboard;
                    sb.Begin(pnl);
                    btnShow.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void UnChecked_PrintCrew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoadingAdorner.IsAdornerVisible == false)
                {
                    LoadingAdorner.IsAdornerVisible = true;
                }
                await Task.Delay(2000);
                RemoveHeilight();

                CheckBox chk = (CheckBox)sender;

                string chkname = chk.Content.ToString();
                crew = "";

                foreach (Family family in this.Families)
                {
                    foreach (Person person in family.Members)
                    {
                        if (ItemHelper.GetIsChecked(person) == true)
                        {
                            crew += person.Name + ",";

                            Maincat = family.Name;
                        }
                    }
                }
                crew = crew.TrimEnd(',');
                if (Maincat == "AVCS Chart")
                {

                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                   
                    string s2 = crew.Replace("Overview", "1").Replace("General", "2").Replace("General", "2").Replace("Coastal", "3").Replace("Approach", "4").Replace("Harbour", "5").Replace("Berthing", "6");
                    string s3 = s2.TrimEnd(',');

                    if (crew.Contains("Overview"))
                    {
                        overviewoverlay.IsVisible = true;
                    }
                    else
                    {
                        overviewoverlay.IsVisible = false;
                    }

                    if (crew.Contains("Berthing"))
                    {

                    }
                    else
                    {
                        berthingoverlay.IsVisible = false;
                    }

                    if (crew.Contains("Harbour"))
                    {

                    }
                    else
                    {
                        harbouroverlay.IsVisible = false;
                    }

                    if (crew.Contains("Approach"))
                    {


                    }
                    else
                    {
                        approachoverlay.IsVisible = false;
                    }


                    if (crew.Contains("Coastal"))
                    {

                    }
                    else
                    {
                        coastaloverlay.IsVisible = false;
                    }



                    if (crew.Contains("General"))
                    {

                    }
                    else
                    {
                        generaloverlay.IsVisible = false;
                    }

                }
                if (Maincat == "AVCS Folio")
                {
                    dataGrid2.Visibility = Visibility.Visible;
                    dataGrid1.Visibility = Visibility.Hidden;
                    AVCSscroll.Visibility = Visibility.Hidden;
                    Folioscroll.Visibility = Visibility.Visible;
                   
                    string s2 = crew.Replace("Regional", "AVCS Folio Regional").Replace("Port", "AVCS Folio Port").Replace("Transit", "AVCS Folio Transit");
                    string s3 = s2.TrimEnd(',');

                    if (crew.Contains("Regional"))
                    {
                        region_Graphic.IsVisible = true;
                    }
                    else
                    {
                        region_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("Transit"))
                    {
                        transit_Graphic.IsVisible = true;
                    }
                    else
                    {
                        transit_Graphic.IsVisible = false;
                    }
                    if (crew.Contains("Port"))
                    {
                        port_Graphic.IsVisible = true;
                    }
                    else
                    {
                        port_Graphic.IsVisible = false;
                    }
                }

                if (Maincat == "Digital List of Radio Signals")
                {
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                   
                    string s2 = crew.Replace("ADRS1345", "ADRS1345").Replace("ADRS2", "ADRS2").Replace("ADRS6", "ADRS6");
                    string s3 = s2.TrimEnd(',');


                    if (crew.Contains("ADRS1345"))
                    {
                        ADRS1345_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS1345_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("ADRS2"))
                    {
                        ADRS2_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS2_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("ADRS6"))
                    {
                        ADRS6_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS6_Graphic.IsVisible = false;
                    }
                    
                }
                LoadingAdorner.IsAdornerVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void Checked_PrintCrew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoadingAdorner.IsAdornerVisible == false)
                {
                    LoadingAdorner.IsAdornerVisible = true;
                }
                await Task.Delay(2000);
                
                CheckBox chk = (CheckBox)sender;

                string chkname = chk.Content.ToString();


                string aa = ItemHelper.SelectedParent;
                //graphiclaysoff();
                if (aa == Maincat)
                {

                }
                else
                {
                    crew = "";
                    cart.Clear();
                    cart1.Clear();
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                    dataGrid2.ItemsSource = null;
                    dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id); 

                }
                foreach (Family family in this.Families)
                {
                    foreach (Person person in family.Members)
                    {
                        if (ItemHelper.GetIsChecked(person) == true)
                        {
                            if (chkname == person.Name)
                            {
                                crew += "," + person.Name;

                                Maincat = family.Name;
                            }
                        }
                    }
                }
                crew = crew.TrimStart(',');
                foreach (Family family in this.Families)
                {
                    if (family.Name == Maincat)
                    {
                        ItemHelper.SetIsExpanded(family, true);
                    }
                    else
                    {
                        ItemHelper.SetIsExpanded(family, false);
                        ItemHelper.SetIsChecked(family, false);
                    }
                }

                if (Maincat == "AVCS Chart")
                {

                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    dataGrid1.Columns[0].Visibility = Visibility.Visible;
               

                    if (crew.Contains("Overview"))
                    {
                        overviewoverlay.IsVisible = true;
                    }
                    else
                    {
                        overviewoverlay.IsVisible = false;
                    }
                    if (crew.Contains("General"))
                    {
                        generaloverlay.IsVisible = true;
                    }
                    else
                    {
                        generaloverlay.IsVisible = false;
                    }
                    if (crew.Contains("Coastal"))
                    {
                        coastaloverlay.IsVisible = true;
                    }
                    else
                    {
                        coastaloverlay.IsVisible = false;
                    }
                    if (crew.Contains("Approach"))
                    {
                        approachoverlay.IsVisible = true;
                    }
                    else
                    {
                        approachoverlay.IsVisible = false;
                    }
                    if (crew.Contains("Harbour"))
                    {
                        harbouroverlay.IsVisible = true;
                    }
                    else
                    {
                        harbouroverlay.IsVisible = false;
                    }

                    if (crew.Contains("Berthing"))
                    {
                        berthingoverlay.IsVisible = true;
                    }
                    else
                    {
                        berthingoverlay.IsVisible = false;
                    }

                    string s2 = crew.Replace("Overview", "1").Replace("General", "2").Replace("General", "2").Replace("Coastal", "3").Replace("Approach", "4").Replace("Harbour", "5").Replace("Berthing", "6");
                    string s3 = s2.TrimEnd(',');
                }
                if (Maincat == "AVCS Folio")
                {
                    dataGrid2.Visibility = Visibility.Visible;
                    dataGrid1.Visibility = Visibility.Hidden;
                    AVCSscroll.Visibility = Visibility.Hidden;
                    Folioscroll.Visibility = Visibility.Visible;
                    
                    string s2 = crew.Replace("Regional", "AVCS Folio Regional").Replace("Port", "AVCS Folio Port").Replace("Transit", "AVCS Folio Transit");
                    string s3 = s2.TrimEnd(',');
                   

                    if (crew.Contains("Transit"))
                    {
                        transit_Graphic.IsVisible = true;
                    }
                    else
                    {
                        transit_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("Regional"))
                    {
                        region_Graphic.IsVisible = true;
                    }
                    else
                    {
                        region_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("Port"))
                    {
                        port_Graphic.IsVisible = true;
                    }
                    else
                    {
                        port_Graphic.IsVisible = false;
                    }
                }
                if (Maincat == "Digital List of Radio Signals")
                {
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    dataGrid1.Columns[0].Visibility = Visibility.Visible;
                   
                    string s2 = crew.Replace("ADRS1345", "ADRS1345").Replace("ADRS2", "ADRS2").Replace("ADRS6", "ADRS6");
                    string s3 = s2.TrimEnd(',');


                    if (crew.Contains("ADRS1345"))
                    {
                        ADRS1345_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS1345_Graphic.ClearSelection();
                        ADRS1345_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("ADRS2"))
                    {
                        ADRS2_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS2_Graphic.ClearSelection();
                        ADRS2_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("ADRS6"))
                    {
                        ADRS6_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS6_Graphic.ClearSelection();
                        ADRS6_Graphic.IsVisible = false;
                    }
                   
                }
                RemoveHeilight();
                LoadingAdorner.IsAdornerVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoadingAdorner.IsAdornerVisible == false)
                {
                    LoadingAdorner.IsAdornerVisible = true;
                }
                await Task.Delay(3000);

                cart.Clear();
                cart1.Clear();
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                dataGrid2.ItemsSource = null;
                dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id); ;


                CheckBox chk = (CheckBox)sender;
                string chkname = chk.Content.ToString();
                string aa = ItemHelper.SelectedParent;
                //graphiclaysoff();
                if (aa == Maincat)
                {

                }
                else
                {
                    crew = "";
                }
                foreach (Family family in this.Families)
                {
                    foreach (Person person in family.Members)
                    {
                        if (ItemHelper.GetIsChecked(person) == true)
                        {
                            if (!crew.Contains(person.Name))
                            {
                                crew += person.Name + " ,";
                            }

                            Maincat = family.Name;
                        }
                    }
                }

                crew = crew.TrimEnd(',');
                foreach (Family family in this.Families)
                {
                    if (family.Name == chkname)
                    {

                        ItemHelper.SetIsExpanded(family, true);
                    }
                    else
                    {
                        ItemHelper.SetIsExpanded(family, false);
                        ItemHelper.SetIsChecked(family, false);
                    }
                }
                if (chkname == "AVCS Chart")
                {
                    Maincat = chkname;

                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    micsgrid.Visibility = Visibility.Hidden;

                    dataGrid1.Columns[0].Visibility = Visibility.Visible;

                    Maincat = chkname;



                    if (crew.Contains("Overview"))
                    {
                        overviewoverlay.IsVisible = true;
                    }
                    else
                    {
                        overviewoverlay.IsVisible = false;
                    }
                    if (crew.Contains("General"))
                    {
                        generaloverlay.IsVisible = true;
                    }
                    else
                    {
                        generaloverlay.IsVisible = false;
                    }
                    if (crew.Contains("Coastal"))
                    {
                        coastaloverlay.IsVisible = true;
                    }
                    else
                    {
                        coastaloverlay.IsVisible = false;
                    }
                    if (crew.Contains("Approach"))
                    {
                        approachoverlay.IsVisible = true;
                    }
                    else
                    {
                        approachoverlay.IsVisible = false;
                    }
                    if (crew.Contains("Harbour"))
                    {
                        harbouroverlay.IsVisible = true;
                    }
                    else
                    {
                        harbouroverlay.IsVisible = false;
                    }

                    if (crew.Contains("Berthing"))
                    {
                        berthingoverlay.IsVisible = true;
                    }
                    else
                    {
                        berthingoverlay.IsVisible = false;
                    }
                }
                else
                {
                    overviewoverlay.IsVisible = false;
                }

                if (chkname == "AVCS Folio")
                {
                    Maincat = chkname;

                    dataGrid2.Visibility = Visibility.Visible;
                    dataGrid1.Visibility = Visibility.Hidden;
                    AVCSscroll.Visibility = Visibility.Hidden;
                    Folioscroll.Visibility = Visibility.Visible;
                    micsgrid.Visibility = Visibility.Hidden;

                    dataGrid1.Columns[0].Visibility = Visibility.Visible;



                    string s2 = crew.Replace("Regional", "AVCS Folio Regional").Replace("Port", "AVCS Folio Port").Replace("Transit", "AVCS Folio Transit");
                    string s3 = s2.TrimEnd(',');

                    if (crew.Contains("Transit"))
                    {
                        transit_Graphic.IsVisible = true;
                    }

                    if (crew.Contains("Regional"))
                    {
                        region_Graphic.IsVisible = true;
                    }
                    if (crew.Contains("Port"))
                    {
                        port_Graphic.IsVisible = true;
                    }
                    else
                    {
                        port_Graphic.IsVisible = false;
                    }
                }
                else
                {
                    region_Graphic.IsVisible = false;
                    transit_Graphic.IsVisible = false;
                    port_Graphic.IsVisible = false;
                }


                if (chkname == "Digital List of Radio Signals")
                {
                    Maincat = chkname;
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    micsgrid.Visibility = Visibility.Hidden;

                    dataGrid1.Columns[0].Visibility = Visibility.Visible;

                    string s2 = crew.Replace("ADRS1345", "ADRS1345").Replace("ADRS2", "ADRS2").Replace("ADRS6", "ADRS6");
                    string s3 = s2.TrimEnd(',');

                    if (crew.Contains("ADRS1345"))
                    {
                        ADRS2_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS2_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("ADRS2"))
                    {
                        ADRS1345_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS1345_Graphic.IsVisible = false;
                    }
                    if (crew.Contains("ADRS6"))
                    {
                        ADRS6_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS6_Graphic.IsVisible = false;
                    }
                }
                else
                {
                    ADRS1345_Graphic.IsVisible = false;
                    ADRS2_Graphic.IsVisible = false;
                    ADRS6_Graphic.IsVisible = false;

                }

                if (chkname == "Digital List of Lights")
                {
                    lol_Graphic.ClearSelection();
                    lol_Graphic.Graphics.Clear();
                    Maincat = chkname;
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    micsgrid.Visibility = Visibility.Hidden;

                    dataGrid1.Columns[0].Visibility = Visibility.Visible;


                    string s2 = "";
                    string s3 = s2.TrimEnd(',');
                    IEnumerable<IGrouping<string, XElement>> query;

                    query = objMgr.GetLOLdigital();

                    var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                    geometrycollection.Clear();
                    //_graphicsOverlay.Graphics.Clear();
                    //_polygonlabelOverlay.Graphics.Clear();
                    polygonPoints1.Clear();

                    var textname = new List<string>();
                    var latitude = new List<string>();
                    var longitude = new List<string>();
                    var scale = new List<string>();
                    var usage = new List<string>();
                    var DatasetTitle = new List<string>();

                    foreach (var value in query)
                    {
                        List<XElement> str = new List<XElement>();

                        var dssd = value.Descendants("Polygon");
                        var q2 = value.Descendants("Polygon").SelectMany(array => array.Value.ToList());

                        value.Descendants("Polygon").ToList().ForEach(item =>
                        {
                            str.Add(item);
                        });
                        if (str.Count > 1)
                        {
                            foreach (var set1 in str)
                            {
                                set1.Descendants("Position").ToList().ForEach(item =>
                                {
                                    latitude.Add(item.Attribute("latitude").Value);
                                    longitude.Add(item.Attribute("longitude").Value);
                                    polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));
                                });

                                var q3 = value.Descendants("Metadata").Select(s => new
                                {
                                    ONE = s.Element("DatasetTitle").Value,
                                    TWO = s.Element("Status").Value
                                }).ToList();

                                string title = q3[0].ONE;
                                string status = q3[0].TWO;
                                CreateGraphic_Label(polygonPoints1, value.Key, "13", "", title, status, "", "");
                                polygonPoints1.Clear();
                            }
                        }
                        else
                        {
                            value.Descendants("Position").ToList().ForEach(item =>
                            {
                                latitude.Add(item.Attribute("latitude").Value);
                                longitude.Add(item.Attribute("longitude").Value);
                                polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));


                            });

                            var q3 = value.Descendants("Metadata").Select(s => new
                            {
                                ONE = s.Element("DatasetTitle").Value,
                                TWO = s.Element("Status").Value
                            }).ToList();

                            string title = q3[0].ONE;
                            string status = q3[0].TWO;
                            CreateGraphic_Label(polygonPoints1, value.Key, "13", "", title, status, "", "");

                        }
                        polygonPoints1.Clear();

                    }

                    lol_Graphic.IsVisible = true;


                }
                else
                {
                    lol_Graphic.IsVisible = false;
                }

                if (chkname == "e-NP Sailing Direction")
                {
                    enpsd_Graphic.ClearSelection();
                    enpsd_Graphic.Graphics.Clear();
                    Maincat = chkname;
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    micsgrid.Visibility = Visibility.Hidden;

                    dataGrid1.Columns[0].Visibility = Visibility.Visible;


                    string s2 = "";
                    string s3 = s2.TrimEnd(',');
                    IEnumerable<IGrouping<string, XElement>> query;

                    query = objMgr.GetENPSDdigital();

                    var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                    geometrycollection.Clear();
                    //_graphicsOverlay.Graphics.Clear();
                    //_polygonlabelOverlay.Graphics.Clear();
                    polygonPoints1.Clear();

                    var textname = new List<string>();
                    var latitude = new List<string>();
                    var longitude = new List<string>();
                    var scale = new List<string>();
                    var usage = new List<string>();
                    var DatasetTitle = new List<string>();

                    foreach (var value in query)
                    {
                        List<XElement> str = new List<XElement>();
                        // textname.Add(value.Key);
                        //  textname.Add(value.Key);
                        var dssd = value.Descendants("Polygon");
                        var q2 = value.Descendants("Polygon").SelectMany(array => array.Value.ToList());

                        value.Descendants("Polygon").ToList().ForEach(item =>
                        {
                            str.Add(item);
                        });
                        if (str.Count > 1)
                        {
                            foreach (var set1 in str)
                            {
                                set1.Descendants("Position").ToList().ForEach(item =>
                                {
                                    latitude.Add(item.Attribute("latitude").Value);
                                    longitude.Add(item.Attribute("longitude").Value);
                                    polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));
                                });

                                var q3 = value.Descendants("Metadata").Select(s => new
                                {
                                    ONE = s.Element("DatasetTitle").Value,
                                    TWO = s.Element("Status").Value
                                }).ToList();

                                string title = q3[0].ONE;
                                string status = q3[0].TWO;
                                CreateGraphic_Label(polygonPoints1, value.Key, "14", "", title, status, "", "");
                                polygonPoints1.Clear();
                            }
                        }
                        else
                        {
                            value.Descendants("Position").ToList().ForEach(item =>
                            {
                                latitude.Add(item.Attribute("latitude").Value);
                                longitude.Add(item.Attribute("longitude").Value);
                                polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));


                            });

                            var q3 = value.Descendants("Metadata").Select(s => new
                            {
                                ONE = s.Element("DatasetTitle").Value,
                                TWO = s.Element("Status").Value
                            }).ToList();

                            string title = q3[0].ONE;
                            string status = q3[0].TWO;
                            CreateGraphic_Label(polygonPoints1, value.Key, "14", "", title, status, "", "");

                        }

                        polygonPoints1.Clear();

                    }

                    enpsd_Graphic.IsVisible = true;

                }
                else
                {
                    enpsd_Graphic.IsVisible = false;
                }
                if (chkname == "e-NP Other")
                {
                    Maincat = chkname;
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    micsgrid.Visibility = Visibility.Visible;

                    dataGrid1.Columns[0].Visibility = Visibility.Hidden;

                    string s2 = "";
                    string s3 = s2.TrimEnd(',');

                    var query = objMgr.GetENPOther();

                    var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                    geometrycollection.Clear();
                    //_graphicsOverlay.Graphics.Clear();
                    //_polygonlabelOverlay.Graphics.Clear();
                    polygonPoints1.Clear();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductName", typeof(string));
                    dt.Columns.Add("DatasetTitle", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    foreach (var item in query)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ProductName"] = (string)item.Element("ShortName");
                        dr["DatasetTitle"] = (string)item.Element("Metadata").Element("DatasetTitle");
                        dr["Status"] = (string)item.Element("Metadata").Element("Status");
                        dt.Rows.Add(dr);
                    }
                    datagridmisc.ItemsSource = dt.DefaultView;

                }
                if (chkname == "Miscellaneous")
                {
                    Maincat = chkname;
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    micsgrid.Visibility = Visibility.Visible;

                    dataGrid1.Columns[0].Visibility = Visibility.Hidden;


                    string s2 = "";
                    string s3 = s2.TrimEnd(',');

                    var query = objMgr.Getmiscellaneous();

                    var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                    geometrycollection.Clear();
                    //_graphicsOverlay.Graphics.Clear();
                    //_polygonlabelOverlay.Graphics.Clear();
                    polygonPoints1.Clear();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductName", typeof(string));
                    dt.Columns.Add("DatasetTitle", typeof(string));
                    dt.Columns.Add("UnitId", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    foreach (var item in query)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ProductName"] = (string)item.Element("ShortName");
                        dr["DatasetTitle"] = (string)item.Element("Metadata").Element("DatasetTitle");
                        dr["UnitId"] = (string)item.Element("Metadata").Element("Unit").Element("ID");
                        dr["Status"] = (string)item.Element("Metadata").Element("Status");
                        dt.Rows.Add(dr);
                    }
                    datagridmisc.ItemsSource = dt.DefaultView;


                }
                if (chkname == "TotalTide")
                {
                    totaltide_Graphic.ClearSelection();
                    totaltide_Graphic.Graphics.Clear();
                    Maincat = chkname;
                    dataGrid2.Visibility = Visibility.Hidden;
                    dataGrid1.Visibility = Visibility.Visible;
                    AVCSscroll.Visibility = Visibility.Visible;
                    Folioscroll.Visibility = Visibility.Hidden;
                    micsgrid.Visibility = Visibility.Hidden;

                    dataGrid1.Columns[0].Visibility = Visibility.Visible;


                    string s2 = "";
                    string s3 = s2.TrimEnd(',');
                    IEnumerable<IGrouping<string, XElement>> query;

                    query = objMgr.GetTotalTide();

                    var polygonPoints1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                    geometrycollection.Clear();
                    //_graphicsOverlay.Graphics.Clear();
                    //_polygonlabelOverlay.Graphics.Clear();
                    polygonPoints1.Clear();

                    var textname = new List<string>();
                    var latitude = new List<string>();
                    var longitude = new List<string>();
                    var scale = new List<string>();
                    var usage = new List<string>();
                    var DatasetTitle = new List<string>();

                    foreach (var value in query)
                    {
                        List<XElement> str = new List<XElement>();

                        var dssd = value.Descendants("Polygon");
                        var q2 = value.Descendants("Polygon").SelectMany(array => array.Value.ToList());

                        value.Descendants("Polygon").ToList().ForEach(item =>
                        {
                            str.Add(item);
                        });
                        if (str.Count > 1)
                        {
                            foreach (var set1 in str)
                            {
                                set1.Descendants("Position").ToList().ForEach(item =>
                                {
                                    latitude.Add(item.Attribute("latitude").Value);
                                    longitude.Add(item.Attribute("longitude").Value);
                                    polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));
                                });

                                var q3 = value.Descendants("Metadata").Select(s => new
                                {
                                    ONE = s.Element("DatasetTitle").Value,
                                    TWO = s.Element("Status").Value
                                }).ToList();

                                string title = q3[0].ONE;
                                string status = q3[0].TWO;
                                CreateGraphic_Label(polygonPoints1, value.Key, "15", "", title, status, "", "");
                                polygonPoints1.Clear();
                            }
                        }
                        else
                        {
                            value.Descendants("Position").ToList().ForEach(item =>
                            {
                                latitude.Add(item.Attribute("latitude").Value);
                                longitude.Add(item.Attribute("longitude").Value);
                                polygonPoints1.Add(new MapPoint(Convert.ToDouble(item.Attribute("longitude").Value), Convert.ToDouble(item.Attribute("latitude").Value)));


                            });

                            var q3 = value.Descendants("Metadata").Select(s => new
                            {
                                ONE = s.Element("DatasetTitle").Value,
                                TWO = s.Element("Status").Value
                            }).ToList();

                            string title = q3[0].ONE;
                            string status = q3[0].TWO;
                            CreateGraphic_Label(polygonPoints1, value.Key, "15", "", title, status, "", "");

                        }



                        polygonPoints1.Clear();
                        //}
                    }

                    totaltide_Graphic.IsVisible = true;


                }
                else
                {
                    totaltide_Graphic.ClearSelection();
                    totaltide_Graphic.IsVisible = false;
                }
                RemoveHeilight();
                LoadingAdorner.IsAdornerVisible = false;
            }
            catch
            {
                LoadingAdorner.IsAdornerVisible = false;
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveHeilight();

                graphiclaysoff();
                geometrycollection.Clear();

                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                dataGrid2.ItemsSource = null;
                dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id); ;
                micsgrid.Visibility = Visibility.Hidden;

                mylistbox.Items.Clear();
            }
            catch
            {

            }
        }

        private void CreateGraphic_Label(Esri.ArcGISRuntime.Geometry.PointCollection _pointc, string key, string encName, string scale, string title, string status, string UnitId, string UnitTitle)
        {
            try
            {
                //overviewoverlay.RenderingMode = GraphicsRenderingMode.Static;
                var Polygon = new Esri.ArcGISRuntime.Geometry.Polygon(_pointc);

                // Define the symbology of the polygon
                var polygonSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(20, 60, 62, 66),
                                       new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(46, 50, 54), 0.5));

                // Define two label definitions: one for capital cities and one for everything else
                StringBuilder capitalLabelsBuilder = new StringBuilder();



                graphattribute = new Dictionary<string, object>();

                graphattribute.Add(key, encName);
                // graphattribute.Add(t1, two);
                _polygonGraphic = new Graphic(Polygon, graphattribute, polygonSymbol);



                if (encName == "1")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Scale"] = scale;
                    _polygonGraphic.Attributes["title"] = title;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    _polygonGraphic.Attributes["UnitId"] = UnitId;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["UnitTitle"] = UnitTitle;

                    overviewoverlay.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "2")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Scale"] = scale;
                    _polygonGraphic.Attributes["title"] = title;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    _polygonGraphic.Attributes["UnitId"] = UnitId;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["UnitTitle"] = UnitTitle;
                    generaloverlay.Graphics.Add(_polygonGraphic);

                }
                else if (encName == "3")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Scale"] = scale;
                    _polygonGraphic.Attributes["title"] = title;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    _polygonGraphic.Attributes["UnitId"] = UnitId;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["UnitTitle"] = UnitTitle;
                    coastaloverlay.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "4")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Scale"] = scale;
                    _polygonGraphic.Attributes["title"] = title;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    _polygonGraphic.Attributes["UnitId"] = UnitId;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["UnitTitle"] = UnitTitle;
                    approachoverlay.Graphics.Add(_polygonGraphic);

                }
                else if (encName == "5")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Scale"] = scale;
                    _polygonGraphic.Attributes["title"] = title;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    _polygonGraphic.Attributes["UnitId"] = UnitId;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["UnitTitle"] = UnitTitle;
                    harbouroverlay.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "6")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Scale"] = scale;
                    _polygonGraphic.Attributes["title"] = title;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    _polygonGraphic.Attributes["UnitId"] = UnitId;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["UnitTitle"] = UnitTitle;
                    berthingoverlay.Graphics.Add(_polygonGraphic);

                }

                if (encName == "7")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["SubUnit"] = UnitId;
                    _polygonGraphic.Attributes["Usage"] = encName;

                    region_Graphic.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "8")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["SubUnit"] = UnitId;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    port_Graphic.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "9")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["SubUnit"] = UnitId;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    transit_Graphic.Graphics.Add(_polygonGraphic);
                }

                else if (encName == "10")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    ADRS1345_Graphic.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "11")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    ADRS2_Graphic.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "12")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    ADRS6_Graphic.Graphics.Add(_polygonGraphic);
                }

                else if (encName == "13")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    lol_Graphic.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "14")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    enpsd_Graphic.Graphics.Add(_polygonGraphic);
                }
                else if (encName == "15")
                {
                    _polygonGraphic.Attributes["ShortName"] = key;
                    _polygonGraphic.Attributes["Title"] = title;
                    _polygonGraphic.Attributes["Status"] = status;
                    _polygonGraphic.Attributes["Usage"] = encName;
                    totaltide_Graphic.Graphics.Add(_polygonGraphic);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void remove(object sender, RoutedEventArgs e)
        {
            try
            {
                ShoppingCart row = (ShoppingCart)((Button)e.Source).DataContext;
                if (Maincat == "AVCS Chart")
                {
                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == row.ShortName && r.ProductName == "AVCS Products");
                    if (itemToRemove != null)
                    {
                        cart.Remove(itemToRemove);
                        objMgr.DeleteShoppingCartByProduct(itemToRemove.ProductName, itemToRemove.ShortName);
                        checkout.Remove(itemToRemove);
                    }
                }
                if (Maincat == "AVCS Folio")
                {
                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == row.ShortName && r.ProductName == "AVCS Products");
                    if (itemToRemove != null)
                    {
                        cart.Remove(itemToRemove);
                        objMgr.DeleteShoppingCartByProduct(itemToRemove.ProductName, itemToRemove.ShortName);
                        checkout.Remove(itemToRemove);
                    }
                }
                if (Maincat == "Digital List of Lights")
                {
                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == row.ShortName && r.ProductName == "ADLL");
                    if (itemToRemove != null)
                    {
                        cart.Remove(itemToRemove);
                        objMgr.DeleteShoppingCartByProduct(itemToRemove.ProductName, itemToRemove.ShortName);
                        checkout.Remove(itemToRemove);
                    }
                }

                if (Maincat == "Digital List of Radio Signals")
                {
                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == row.ShortName && r.ProductName == "ADRS");
                    if (itemToRemove != null)
                    {
                        cart.Remove(itemToRemove);
                        objMgr.DeleteShoppingCartByProduct(itemToRemove.ProductName, itemToRemove.ShortName);
                        checkout.Remove(itemToRemove);
                    }
                }

                if (Maincat == "e-NP Sailing Direction")
                {
                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == row.ShortName && r.ProductName == "e-Nautical Publications");
                    if (itemToRemove != null)
                    {
                        cart.Remove(itemToRemove);
                        objMgr.DeleteShoppingCartByProduct(itemToRemove.ProductName, itemToRemove.ShortName);
                        checkout.Remove(itemToRemove);
                    }
                }

                if (Maincat == "TotalTide")
                {
                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == row.ShortName && r.ProductName == "TotalTide");
                    if (itemToRemove != null)
                    {
                        cart.Remove(itemToRemove);
                         objMgr.DeleteShoppingCartByProduct(itemToRemove.ProductName, itemToRemove.ShortName);
                        checkout.Remove(itemToRemove);
                    }
                }
                



                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;

                //GraphicCollection graphicoverlaycollection = _sketchOverlay.Graphics;
                //var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                //var scale = tes.TargetScale;
                //var ree1 = graphicoverlaycollection.SelectMany(graphic => graphic.Attributes, (gr_key, gr_value) => new { gr_key, gr_value });
                //foreach (var tem in ree1)
                //{
                //    if (tem.gr_value.Key == row.ShortName)
                //    {
                //        tem.gr_key.IsSelected = false;
                //    }
                //}

                List<GraphicsOverlay> graphicoverlaycollection = new List<GraphicsOverlay>();
                var ter = MyMapView.GraphicsOverlays;
                foreach (var item in ter)
                {
                    graphicoverlaycollection.Add(item);
                }
                // GraphicCollection graphicoverlaycollection1 = graphicoverlaycollection.Graphics;
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                var scale = tes.TargetScale;
                foreach (var item in graphicoverlaycollection)
                {

                    foreach (var item1 in item.Graphics)
                    {
                        if (item1.Attributes.ContainsKey(row.ShortName))
                        {
                            item1.IsSelected = false;
                            Mouse_Tap_Unselect(item1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        private void remove1(object sender, RoutedEventArgs e)
        {
            try
            {
                Temperatures row = (Temperatures)((Button)e.Source).DataContext;
                var itemToRemove = cart1.FirstOrDefault(r => r.FProductId == row.FProductId);
                if (itemToRemove != null)
                {
                    cart1.Remove(itemToRemove);
                    objMgr.DeleteShoppingCartByShortname(itemToRemove.FProductId);
                   // checkout.Remove(itemToRemove);
                }
                dataGrid2.ItemsSource = null;
                dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id); 

                List<GraphicsOverlay> graphicoverlaycollection = new List<GraphicsOverlay>();
                var ter = MyMapView.GraphicsOverlays;
                foreach (var item in ter)
                {
                    graphicoverlaycollection.Add(item);
                }
                // GraphicCollection graphicoverlaycollection1 = graphicoverlaycollection.Graphics;
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                var scale = tes.TargetScale;
                foreach (var item in graphicoverlaycollection)
                {

                    foreach (var item1 in item.Graphics)
                    {
                        if (item1.Attributes.ContainsKey(row.FProductId))
                        {
                            item1.IsSelected = false;
                            Mouse_Tap_Unselect(item1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        static List<MapPoint> Mapcoordinates_Aftertransform(IReadOnlyList<MapPoint> _mappoint)
        {
            List<MapPoint> polypts = new List<MapPoint>();
            try
            {
                foreach (var se in _mappoint)
                {
                    double x = se.X;
                    double y = se.Y;
                    MapPoint Point1 = new MapPoint(x, y, SpatialReferences.WebMercator);
                    MapPoint afterPoint1 = (MapPoint)GeometryEngine.Project(Point1, SpatialReference.Create(4326));
                    polypts.Add(afterPoint1);
                }
                return polypts;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return polypts;
            }
        }

        private void mylistbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                string str = null;

                if (mylistbox.SelectedIndex < 0)
                {
                    //_graphicsOverlay.ClearSelection();
                    return;
                }

                str = (mylistbox.SelectedItem as ListBoxItem).Content.ToString();

                if (mylistbox.Items.Count > 1)
                {



                    List<GraphicsOverlay> graphicoverlaycollection = new List<GraphicsOverlay>();
                    var ter = MyMapView.GraphicsOverlays;
                    foreach (var items in ter)
                    {
                        if (items.Id != "seven" && items.Id != "Portoverlay" && items.Id != "greenPortoverlay" && items.Id != "redPortoverlay" && items.Id != "17" && items.Id != "_sketchRectOverlay")
                        {
                            graphicoverlaycollection.Add(items);
                        }
                    }
                    
                    foreach (var items in graphicoverlaycollection)
                    {
                        foreach (var item1 in items.Graphics)
                        {

                            string named = item1.Attributes.Keys.FirstOrDefault();

                            if (item1.Attributes.ContainsKey(str))
                            {

                                if (Maincat == "AVCS Chart")
                                {

                                    item1.IsSelected = true;
                                    Mouse_tap_select(item1);
                                    var aa = item1.Attributes.Keys.FirstOrDefault();
                                    var shname = item1.Attributes["ShortName"].ToString();
                                    var scale = item1.Attributes["scale"].ToString();
                                    var usage = item1.Attributes["Usage"].ToString();
                                    var title = item1.Attributes["title"].ToString();


                                    bool alreadyExists = cart.Any(x => x.ShortName == shname && x.ProductName == "AVCS Products");
                                    if (!alreadyExists)
                                    {
                                        item1.IsSelected = true;
                                        Mouse_tap_select(item1);
                                        string exist = "";
                                        string ss = shname;
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }

                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            ShortName = shname,
                                            Title = title,
                                            Scale = scale,
                                            Status = item1.Attributes["Status"].ToString(),
                                            Usage = objcont.Band(usage),
                                            UnitId = item1.Attributes["UnitId"].ToString(),
                                            UnitTitle = item1.Attributes["UnitTitle"].ToString(),
                                            ProductName = "AVCS Products"
                                        });
                                       
                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id);

                                   
                                }
                                if (Maincat == "AVCS Folio")
                                {
                                    item1.IsSelected = true;
                                    Mouse_tap_select(item1);
                                    string exist = "";

                                    bool alreadyExists = cart1.Any(x => x.FProductId == item1.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {

                                        string ss = item1.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        List<TemperaturesDetails> CartDetails = new List<TemperaturesDetails>();
                                        XElement xele = XElement.Parse(item1.Attributes["SubUnit"].ToString());
                                        var dddd = xele.Descendants();
                                        string sd = "";
                                        foreach (var item2 in dddd)
                                        {
                                            sd += item2.Value + ",";

                                        }

                                        sd = sd.TrimEnd(',');
                                        foreach (var ter1 in ter)
                                        {
                                            if (ter1.Id != "seven" && ter1.Id != "Portoverlay" && ter1.Id != "greenPortoverlay" && ter1.Id != "redPortoverlay" && ter1.Id != "17" && ter1.Id != "_sketchRectOverlay")
                                            {
                                                foreach (var item2 in ter1.Graphics)
                                                {

                                                    var sub = item2.Attributes.Keys.FirstOrDefault();
                                                    if (sd.Contains(sub))
                                                    {
                                                        if (item2.Attributes["ShortName"] != null)
                                                        {
                                                            CartDetails.Add(new TemperaturesDetails()
                                                            {
                                                                ProductId = item2.Attributes["ShortName"].ToString(),
                                                                Title = item2.Attributes["title"].ToString(),
                                                                Band = objcont.Band(item2.Attributes["Usage"].ToString())
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        cart1.Add(new Temperatures()
                                        {
                                            Exists = exist,
                                            FProductId = item1.Attributes["ShortName"].ToString(),
                                            FTitle = item1.Attributes["Title"].ToString(),
                                            DetailsItems = CartDetails
                                        });
                                    }
                                   
                                    dataGrid2.ItemsSource = null;
                                    dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                                    return;
                                }
                                if (Maincat == "Digital List of Radio Signals")
                                {
                                    //var que = objMgr.GetALRSByShortName(named);
                                    //var val = que.FirstOrDefault();
                                   
                                        item1.IsSelected = true;
                                        Mouse_tap_select(item1);
                                    bool alreadyExists = cart.Any(x => x.ShortName == item1.Attributes["ShortName"].ToString() && x.ProductName == "ADRS");
                                    if (!alreadyExists)
                                    {
                                       
                                        string exist = "";
                                        string ss = item1.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "ADRS");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            ShortName = item1.Attributes["ShortName"].ToString(),
                                            Title = item1.Attributes["title"].ToString(),
                                            Status = item1.Attributes["Status"].ToString(),
                                            Usage = item1.Attributes["Usage"].ToString(),
                                            ProductName = "ADRS"
                                        }); ;

                                    }
                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        static MapPoint Mapcoordinates_Change(MapPoint mapPoints)
        {
            try
            {
                // Create a point geometry in NYC in WGS84
                MapPoint startingPoint = new MapPoint(mapPoints.X, mapPoints.Y, SpatialReferences.WebMercator);
                // Perform the same projection without specified transformation
                MapPoint afterPoint1 = (MapPoint)GeometryEngine.Project(mapPoints, SpatialReference.Create(4326));
                return afterPoint1;
            }
            catch
            {

                MapPoint startingpoint = new MapPoint(42.85888, -79.40261);
                return startingpoint;
            }
        }
        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                mylistbox.Items.Clear();
                Storyboard sb = Resources["sbHideRightMenu"] as Storyboard;
                sb.Begin(pnlRightMenu);

                //twogrp.Clear();
                lstSelectedEmpNo.Clear();
                dataGrid1.ItemsSource = null;
                dataGrid2.ItemsSource = null;
                cart.Clear();
                cart1.Clear();

                stopgraphiclin.Clear();
                mylistbox.Items.Clear();
                RemoveHeilight();
            }
            catch
            {


            }
        }
        PolylineBuilder loadedpolylinebuilder = null;
        private void UnderpolylineRoot_Click(object sender, RoutedEventArgs e)//add this new method and in xaml event  for products under button in routeline tree
        {
            try
            {
                var temproutegeom = MyMapView.SketchEditor.Geometry;//for routeline incomplete draw
                if (temproutegeom != null)//check tempgeom  
                {
                    Esri.ArcGISRuntime.Geometry.Polyline poly = (Esri.ArcGISRuntime.Geometry.Polyline)GeometryEngine.NormalizeCentralMeridian(temproutegeom);
                    if (poly.Parts.Count > 1)
                    {
                        var tempgraphic = coordinatesystem_polyline_new(poly);
                        foreach (var item in tempgraphic)
                        {
                            var ge = Graphiccoordinates_Aftertransform(item);
                            var graphic = new Graphic(ge);
                            Geometry_OnviewTap(graphic);
                            // Geometry_OnviewTap(item);
                        }
                    }
                    else
                    {
                        var tempgraphic = coordinatesystem_polyline(temproutegeom);
                        Geometry_OnviewTap(tempgraphic);
                    }

                }
                else if (importedlinepoints.Count > 1) //check for any import line condition
                {
                    if (normalizedimportedpoints.Count > 1)
                    {
                        var pointline = loadrouteline_create(normalizedimportedpoints);

                        var roadPolyline = pointline as Esri.ArcGISRuntime.Geometry.Polyline;
                        // var roadPolyline = graphic.Geometry as Esri.ArcGISRuntime.Geometry.Polyline;

                        Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(roadPolyline, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                        this.polylineBuilder = new PolylineBuilder(roadPolyline);
                        if (polylineBuilder.Parts.Count > 1)
                        {
                            var item = coordinatesystem_polyline_new(pathGeometry1);
                            foreach (var item1 in item)
                            {
                                var ge = Graphiccoordinates_Aftertransform(item1);
                                var graphic = new Graphic(ge);
                                Geometry_OnviewTap(graphic);
                            }
                        }
                        else
                        {
                            var loadedgraphic = coordinatesystem_polyline(pathGeometry1);
                            var ge = Graphiccoordinates_Aftertransform(loadedgraphic);
                            var graphic = new Graphic(ge);
                            Geometry_OnviewTap(graphic);
                            // route_symbolsadding_webmerc(get);
                        }
                    }
                    else
                    {
                        //a
                        normalizedimportedpoints = importedlinepoints;
                        Polyline routeLine = new Polyline(normalizedimportedpoints);
                        Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(routeLine, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                        var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                        var graphic1 = new Graphic(pathGeometry1, polys3);
                        Geometry_OnviewTap(graphic1);

                    }
                }
                else if (symbolspointslist_savebtn.Count > 1)
                {
                    Polyline routeLine = new Polyline(symbolspointslist_savebtn);
                    Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(routeLine, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                    var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                    var graphic1 = new Graphic(pathGeometry1, polys3);
                    Geometry_OnviewTap(graphic1);
                }
                else
                {
                    if (SelectedRoutName != "")
                    {
                        DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
                        DataTable dt = new DataTable();
                        dt = objRout.GetRouteLineDetails(SelectedRoutName);
                        Esri.ArcGISRuntime.Geometry.PointCollection pointcollection_load = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
                        List<MapPoint> loadpointlist = new List<MapPoint>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                            var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                            MapPoint mp1 = new MapPoint(latit, longit, SpatialReferences.WebMercator);
                            pointcollection_load.Add(latit, longit);
                            loadpointlist.Add(mp1);
                        }

                        var l1 = loadrouteline_create(pointcollection_load);
                        var loadedpolyline = l1 as Polyline;
                        this.loadedpolylinebuilder = new PolylineBuilder(loadedpolyline);
                        var item2geom = loadrouteline_geom_create_new(pointcollection_load);
                        Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(loadedpolyline, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                        if (loadedpolylinebuilder.Parts.Count > 1)
                        {
                            var tempgraphic = coordinatesystem_polyline_new(pathGeometry1);
                            foreach (var item in tempgraphic)
                            {
                                var ge = Graphiccoordinates_Aftertransform(item);
                                var graphic = new Graphic(ge);
                                Geometry_OnviewTap(graphic);
                                // Geometry_OnviewTap(item);
                            }
                        }
                        else
                        {
                            var grap = coordinatesystem_polyline(pathGeometry1);
                            Geometry_OnviewTap(grap);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private async void AddtoCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingAdorner.IsAdornerVisible = true;
                await Task.Delay(2000);
                if (Maincat == "AVCS Folio")
                {
                    foreach (var val in cart1)
                    {
                        if (val.Exists == "NotExists")
                        {
                            var result = objMgr.InsertENC(val.FProductId, val.FTitle, val.FProductId, "", "", "", "AVCS Products", val.FTitle);
                            val.Exists = "Exists";
                        }
                    }

                    dataGrid2.ItemsSource = null;
                    dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                }
                else
                {
                    foreach (var val in cart)
                    {
                        if (val.Exists == "NotExists")
                        {
                            var result = objMgr.InsertENC(val.ShortName, val.Title, val.UnitId, val.Usage, val.Status, val.Scale, val.ProductName, val.UnitTitle);
                            val.Exists = "Exists";
                        }
                    }

                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                }
                RemoveHeilight();
                Bindcheckpout();
                LoadingAdorner.IsAdornerVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Graphic CreateGraphic(Esri.ArcGISRuntime.Geometry.Geometry geometry)
        {
            Symbol symbol = null;
            try
            {
                // Create a graphic to display the specified geometry
                
                switch (geometry.GeometryType)
                {
                    // Symbolize with a fill symbol
                    case GeometryType.Envelope:
                    case GeometryType.Polygon:
                        {
                            symbol = new SimpleFillSymbol()
                            {
                                Color = System.Drawing.Color.Transparent,
                                Style = SimpleFillSymbolStyle.Solid
                            };
                            break;
                        }
                    // Symbolize with a line symbol
                    case GeometryType.Polyline:
                        {
                            symbol = new SimpleLineSymbol()
                            {
                                Color = System.Drawing.Color.Red,
                                Style = SimpleLineSymbolStyle.Solid,
                                Width = 5d
                            };
                            break;
                        }
                    // Symbolize with a marker symbol
                    case GeometryType.Point:
                    case GeometryType.Multipoint:
                        {

                            symbol = new SimpleMarkerSymbol()
                            {
                                Color = System.Drawing.Color.Red,
                                Style = SimpleMarkerSymbolStyle.Circle,
                                Size = 15d
                            };
                            break;
                        }
                }
            }
            catch
            {
                
            }
            // pass back a new graphic with the appropriate symbol
            return new Graphic(geometry, symbol);
        }
        private Graphic coordinatesystem_polyline(Esri.ArcGISRuntime.Geometry.Geometry geometry)
        {
            Graphic _polylineGraphic = null;
            try
            {
                
                var roadPolyline = geometry as Esri.ArcGISRuntime.Geometry.Polyline;
                // var roadPolyline = graphic.Geometry as Esri.ArcGISRuntime.Geometry.Polyline;
                this.polylineBuilder = new PolylineBuilder(roadPolyline);
                foreach (var r in polylineBuilder.Parts)
                {
                    IReadOnlyList<MapPoint> mapPoints = r.Points;
                    var polypoints = Mapcoordinates_Aftertransform(mapPoints);
                    _polypointcollection = polypoints;
                    //HandleMapTap1(polypoints);

                    //var polypoints = Mapcoordinates_Aftertransform(mapPoints);
                    //_polypointcollection = polypoints;//new
                    var polyline = new Esri.ArcGISRuntime.Geometry.Polyline(polypoints);
                    //Create symbol for polyline
                    //  var polylineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 0, 255), 3);
                    // var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Green, 3);
                    var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Red, 1);
                    // polys3.MarkerPlacement = SimpleLineSymbolMarkerPlacement.End;
                    // polys3.ma = getpic1();
                    //polys3.MarkerStyle = SimpleLineSymbolMarkerStyle.Arrow;
                    //polys3.MarkerPlacement = SimpleLineSymbolMarkerPlacement.BeginAndEnd;

                    //Create a polyline graphic with geometry and symbol
                    _polylineGraphic = new Graphic(polyline, polys3);

                    //Add polyline to graphics overlay

                    //  _sketchPolylineOverlay.Graphics.Add(_polylineGraphic);
                    Esri.ArcGISRuntime.Geometry.Geometry gr = polyline;
                }
            }
            catch
            {

            }
            return _polylineGraphic;
        }
        private Graphic coordinatesystem_polygon(Graphic graphic)
        {
            Graphic polylgonGraphic = null;
            try
            {
                var poly = graphic.Geometry as Esri.ArcGISRuntime.Geometry.Polygon;
                this.polygonbuild = new PolygonBuilder(poly);
                foreach (var re in polygonbuild.Parts)
                {
                    IReadOnlyList<MapPoint> mapPoints = re.Points;
                    var polypoints = Mapcoordinates_Aftertransform(mapPoints);

                    var polygon = new Esri.ArcGISRuntime.Geometry.Polygon(polypoints);

                    //Create symbol for polyline
                    var polylineSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Transparent,
                        new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 0, 255), 2));
                    //Create a polyline graphic with geometry and symbol
                    polylgonGraphic = new Graphic(polygon, polylineSymbol);

                    //_sketchOverlay.Graphics.Add(polylgonGraphic);
                    Esri.ArcGISRuntime.Geometry.Geometry gr = polygon;

                }
                return polylgonGraphic;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return polylgonGraphic;
            }
        }
        private Graphic point_graphic_creation(MapPoint point)
        {
            Graphic _polypointGraphic = null;
            try
            {
                var pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Transparent, 10);
                pointSymbol.Outline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Transparent, 2);

                //Create point graphic with geometry & symbol
                _polypointGraphic = new Graphic(point, pointSymbol);

                //Add point graphic to graphic overlay
                // _sketchOverlay.Graphics.Add(_polypointGraphic);
                return _polypointGraphic;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return _polypointGraphic;
            }

        }
        private void completecommand(SketchCreationMode sketch)
        {
            try
            {
                if (MyMapView.SketchEditor.CompleteCommand.CanExecute(null))
                {
                    MyMapView.SketchEditor.CompleteCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async Task<PictureMarkerSymbol> GetPictureMarker()
        {
            PictureMarkerSymbol pictureMarkerSym;
            try
            {
                // create a new PictureMarkerSymbol with an image file stored locally
                var picPath = AppDomain.CurrentDomain.BaseDirectory + "\\concentric-circles-png.png";
                ////var picPath = @"E:\concentric-circles-png.png";
                //var picPath = new Uri("/Icons/concentric-circles-png.png", UriKind.Relative);


                using (System.IO.FileStream picStream = new System.IO.FileStream(picPath, System.IO.FileMode.Open))
                {
                    // pass the image file stream to the symbol constructor
                    pictureMarkerSym = await PictureMarkerSymbol.CreateAsync(picStream);
                }

                pictureMarkerSym.Width = 12;
                pictureMarkerSym.Height = 12;
                pictureMarkerSym.LeaderOffsetX = 0;
                pictureMarkerSym.OffsetY = 0;
                pictureMarkerSym.Angle = 0;
                return pictureMarkerSym;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private void MapView_MouseMoved(object sender, MouseEventArgs e)
        {
            try
            {
                System.Windows.Point hoverPoint = e.GetPosition(MyMapView);

                // Get the physical map location corresponding to the mouse position.
                MapPoint hoverLocation = MyMapView.ScreenToLocation(hoverPoint);
                MapPoint aftertransformhoverloc = Mapcoordinates_Change(hoverLocation);
                if (!double.IsNaN(aftertransformhoverloc.X))
                {
                    mylblClickCount.Content = CoordinateFormatter.ToLatitudeLongitude(aftertransformhoverloc, LatitudeLongitudeFormat.DegreesDecimalMinutes, 4);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void route_symbolsadding(IReadOnlyList<MapPoint> mp)
        {
            try
            {

                foreach (var tem in mp)
                {
                    polist.polylinepointcollection = null;
                    switch (SampleState.AddingStops)
                    {
                        case SampleState.AddingStops:
                            // Get the name of this stmop.
                            string stopName = $"W{routewaypointoverlay.Graphics.Count + 1 }";
                            // polist.WaypointCount = _sketchRouteOverlay.Graphics.Count + 1;
                            // Create the marker to show underneath the stop number.
                            PictureMarkerSymbol pushpinMarker = await GetPictureMarker();

                            polist.latitude = tem.X;
                            polist.longitude = tem.Y;


                            TextSymbol stopSymbol = new TextSymbol(stopName, System.Drawing.Color.Transparent, 15,
                                Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Right, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                            stopSymbol.OffsetY = 0;

                            CompositeSymbol combinedSymbol = new CompositeSymbol(new MarkerSymbol[] { pushpinMarker, stopSymbol });

                            Graphic stopGraphic = new Graphic(tem, combinedSymbol);
                            // Graphic stopGraphic = new Graphic(tem);
                            stopGraphic.Attributes["ShortName"] = stopName;
                            routewaypointoverlay.Graphics.Add(stopGraphic);
                            break;
                    }

                }
            }
            catch
            {

            }
            // Normalize geometry - important for geometries that will be sent to a server for processing.
            //mapLocation = (MapPoint)GeometryEngine.NormalizeCentralMeridian(mapLocation);
            // var ste = Mapcoordinates_Change(mapLocation);
        }
        private async void new_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveHeilight();
                SelectedRoutName = "";
                Mouse.OverrideCursor = Cursors.Cross;
                MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                MyMapView.Cursor = Mouse.OverrideCursor = Cursors.Cross;
                _sketchOverlay.Graphics.Clear();
                routewaypointoverlay.Graphics.Clear();
                gridrouteline.Clear();
                savemenu.IsEnabled = true;
                routeline = null;
                polygondrawgrp = null;


                var config = new SketchEditConfiguration()
                {
                    AllowVertexEditing = true,
                    AllowMove = true,
                    AllowRotate = false,

                    ResizeMode = SketchResizeMode.None
                };
                sketchEditor();

                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.Polyline;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await MyMapView.SketchEditor.StartAsync(creationMode, true);

                Graphic graphic = CreateGraphic(geometry);

                if (geometry.GeometryType.ToString() == "Polyline")
                {
                    routeline = coordinatesystem_polyline(geometry);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch //(Exception ex)
            {

            }
        }
        private async void MapViewTapped_Mouse_Point(object sender, GeoViewInputEventArgs geoViewInputEventArgs)
        {
            //_sketchOverlay.Graphics.Clear();
            RemoveHeilight();
            try
            {
                var pixelTolerance = 1;
                var returnPopupsOnly = false;
                var maxResults = 100;
                System.Windows.Point tapScreenPoint = geoViewInputEventArgs.Position;
                MapPoint mapPoint = geoViewInputEventArgs.Location;
                var es = Mapcoordinates_Change(mapPoint);
                Graphic _pointgraph = point_graphic_creation(es);

                if (Maincat == "AVCS Chart")
                {
                    IReadOnlyList<IdentifyGraphicsOverlayResult> idGraphicOverlayResults1 = await MyMapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                   var idGraphicOverlayResults = idGraphicOverlayResults1.Where(r=>r.GraphicsOverlay.Id != "seven" && r.GraphicsOverlay.Id != "Portoverlay" && r.GraphicsOverlay.Id != "greenPortoverlay" && r.GraphicsOverlay.Id != "redPortoverlay" && r.GraphicsOverlay.Id != "17" &&  r.GraphicsOverlay.Id != "_sketchRectOverlay").Distinct().ToList();


                    if (idGraphicOverlayResults.Count() == 1)
                    {
                        var idGraphicResult = idGraphicOverlayResults.FirstOrDefault();

                        if (idGraphicResult.Graphics.Count > 1)
                        {
                            mylistbox.Items.Clear();
                            Geometry_OnviewTap(_pointgraph);
                           
                            return;
                        }

                        var g = idGraphicResult.Graphics.FirstOrDefault();
                        var aa = g.Attributes.Keys.FirstOrDefault();
                        var shname = g.Attributes["ShortName"].ToString();
                        var scale = g.Attributes["scale"].ToString();
                        var usage = g.Attributes["Usage"].ToString();
                        var title = g.Attributes["title"].ToString();
                        var UnitId = g.Attributes["UnitId"].ToString();

                        if (g.IsSelected == true)
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = false;
                                        Mouse_Tap_Unselect(item3);
                                        if (checkout.Any(x => x.ShortName == aa))
                                        {
                                            var itemToRemove1 = checkout.FirstOrDefault(r => r.ShortName == aa);
                                            if (itemToRemove1 != null)
                                            {
                                                checkout.Remove(itemToRemove1);
                                            }
                                            //Mouse_tap_AddedCart(item3);
                                            objMgr.DeleteShoppingCartByProduct("AVCS Products", shname);
                                            //Mouse_Tap_Unselect(item3);
                                        }
                                    }
                                }
                            }

                            var itemToRemove = cart.FirstOrDefault(r => r.ShortName == aa);
                            if (itemToRemove != null)
                            {
                                cart.Remove(itemToRemove);
                                
                            }
                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                        else
                        {

                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        if (checkout.Any(x => x.ShortName == aa))
                                        {
                                            var itemToRemove = checkout.FirstOrDefault(r => r.ShortName == aa);
                                            if (itemToRemove != null)
                                            {
                                                checkout.Remove(itemToRemove);
                                            }
                                            objMgr.DeleteShoppingCartByProduct("AVCS Products", shname);
                                            Mouse_Tap_Unselect(item3);
                                            return;
                                        }
                                        else
                                        {
                                            item3.IsSelected = true;
                                            Mouse_tap_select(item3);
                                        }
                                    }
                                }
                            }
                            //var que = objMgr.GetByShortName(aa);
                            //var val = que.FirstOrDefault();

                            bool alreadyExists = cart.Any(x => x.ShortName == shname);
                            if (!alreadyExists)
                            {
                                string exist = "";
                                string ss = shname;
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }


                                cart.Add(new ShoppingCart()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    ShortName = shname,
                                    Title = title,
                                    Scale = scale,
                                    Status = g.Attributes["Status"].ToString(),
                                    Usage = objcont.Band(usage),
                                    UnitId = g.Attributes["UnitId"].ToString(),
                                    UnitTitle = g.Attributes["UnitTitle"].ToString(),
                                    ProductName = "AVCS Products"
                                });

                            }

                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id);
                        }

                    }
                    else if (idGraphicOverlayResults.Count() > 1)
                    {
                        mylistbox.Items.Clear();
                        ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        Geometry_OnviewTap(_pointgraph);
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "AVCS Folio")
                {
                    IReadOnlyList<IdentifyGraphicsOverlayResult> idGraphicOverlayResults1 = await MyMapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                    var idGraphicOverlayResults = idGraphicOverlayResults1.Where(r => r.GraphicsOverlay.Id != "seven" && r.GraphicsOverlay.Id != "Portoverlay" && r.GraphicsOverlay.Id != "greenPortoverlay" && r.GraphicsOverlay.Id != "redPortoverlay" && r.GraphicsOverlay.Id != "17" && r.GraphicsOverlay.Id != "_sketchRectOverlay").ToList();

                    if (idGraphicOverlayResults.Count == 1)
                    {
                        var idGraphicResult = idGraphicOverlayResults.FirstOrDefault();

                        if (idGraphicResult.Graphics.Count > 1)
                        {
                            mylistbox.Items.Clear();
                           
                            Geometry_OnviewTap(_pointgraph);
                            
                            return;
                        }

                        var g = idGraphicResult.Graphics.FirstOrDefault();
                        var aa = g.Attributes.Keys.FirstOrDefault();
                        if (g.IsSelected == true)
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = false;
                                        Mouse_Tap_Unselect(item3);
                                        if (checkout.Any(x => x.ShortName == aa))
                                        {
                                            var itemToRemove1 = checkout.FirstOrDefault(r => r.ShortName == aa);
                                            if (itemToRemove1 != null)
                                            {
                                                checkout.Remove(itemToRemove1);
                                            }
                                            //Mouse_tap_AddedCart(item3);
                                            objMgr.DeleteShoppingCartByProduct("AVCS Products",aa);
                                        }
                                    }
                                }
                            }

                            var itemToRemove = cart1.FirstOrDefault(r => r.FProductId == aa);
                            if (itemToRemove != null)
                            {
                                cart1.Remove(itemToRemove);
                            }
                            dataGrid2.ItemsSource = null;
                            dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                        }
                        else
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = true;
                                        Mouse_tap_select(item3);
                                    }
                                }
                            }

                            //var que = objMgr.GetByFolioID(aa);
                            //var val = que.FirstOrDefault();
                            string exist = "";
                            var ter = MyMapView.GraphicsOverlays;

                            bool alreadyExists = cart.Any(x => x.ShortName == g.Attributes["ShortName"].ToString() && x.ProductName == "AVCS Products");
                            if (!alreadyExists)
                            {

                                string ss = g.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }


                                List<TemperaturesDetails> CartDetails = new List<TemperaturesDetails>();
                                XElement xele = XElement.Parse(g.Attributes["SubUnit"].ToString());
                                var dddd = xele.Descendants();
                                string sd = "";
                                foreach (var item2 in dddd)
                                {
                                    sd += item2.Value + ",";

                                }


                                foreach (var ter1 in ter)
                                {
                                    
                                    if (ter1.Id != "seven" && ter1.Id != "Portoverlay" && ter1.Id != "greenPortoverlay" && ter1.Id != "redPortoverlay" && ter1.Id != "_sketchRectOverlay")
                                    {
                                        foreach (var item2 in ter1.Graphics)
                                        {

                                            var sub = item2.Attributes.Keys.FirstOrDefault();
                                            if (sd.Contains(sub))
                                            {
                                                if (item2.Attributes["ShortName"] != null)
                                                {
                                                    CartDetails.Add(new TemperaturesDetails()
                                                    {
                                                        ProductId = item2.Attributes["ShortName"].ToString(),
                                                        Title = item2.Attributes["title"].ToString(),
                                                        Band = objcont.Band(item2.Attributes["Usage"].ToString())
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }

                                cart1.Add(new Temperatures()
                                {
                                    Exists = exist,
                                    Id = cart1.Count > 0 ? cart1.Max(x => x.Id) + 1 : 1,
                                    FProductId = g.Attributes["ShortName"].ToString(),
                                    FTitle = g.Attributes["Title"].ToString(),
                                    DetailsItems = CartDetails
                                });
                            }

                            dataGrid2.ItemsSource = null;
                            dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                        }

                    }
                    else if (idGraphicOverlayResults.Count > 1)
                    {
                        mylistbox.Items.Clear();
                        ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        Geometry_OnviewTap(_pointgraph);
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "Digital List of Lights")
                {
                    IReadOnlyList<IdentifyGraphicsOverlayResult> idGraphicOverlayResults1 = await MyMapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                    var idGraphicOverlayResults = idGraphicOverlayResults1.Where(r => r.GraphicsOverlay.Id != "seven" && r.GraphicsOverlay.Id != "Portoverlay" && r.GraphicsOverlay.Id != "greenPortoverlay" && r.GraphicsOverlay.Id != "redPortoverlay" && r.GraphicsOverlay.Id != "17" && r.GraphicsOverlay.Id != "_sketchRectOverlay").Distinct().ToList();

                    if (idGraphicOverlayResults.Count == 1)
                    {
                        var idGraphicResult = idGraphicOverlayResults.FirstOrDefault();

                        //if (idGraphicResult.Graphics.Count > 1)
                        //{
                        //    mylistbox.Items.Clear();
                        //    ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        //    Geometry_OnviewTap(_pointgraph);
                        //    pnlRightMenu.Visibility = Visibility.Visible;
                        //    return;
                        //}

                        var g = idGraphicResult.Graphics.FirstOrDefault();
                        var aa = g.Attributes.Keys.FirstOrDefault();

                        if (g.IsSelected == true)
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = false;
                                        Mouse_Tap_Unselect(item3);
                                        if (checkout.Any(x => x.ShortName == aa))
                                        {
                                            var itemToRemove1 = checkout.FirstOrDefault(r => r.ShortName == aa);
                                            if (itemToRemove1 != null)
                                            {
                                                checkout.Remove(itemToRemove1);
                                            }
                                            objMgr.DeleteShoppingCartByProduct("ADLL",aa);
                                        }
                                    }
                                }
                            }

                            var itemToRemove = cart.FirstOrDefault(r => r.ShortName == aa);
                            if (itemToRemove != null)
                            {
                                cart.Remove(itemToRemove);
                            }
                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                        else
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = true;
                                        Mouse_tap_select(item3);
                                    }
                                }
                            }
                            //var que = objMgr.GetLOLByShortName(aa);
                            //var val = que.FirstOrDefault();

                            bool alreadyExists = cart.Any(x => x.ShortName == g.Attributes["ShortName"].ToString());
                            if (!alreadyExists)
                            {
                                string exist = "";
                                string ss = g.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "ADLL");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }


                                cart.Add(new ShoppingCart()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    ShortName = g.Attributes["ShortName"].ToString(),
                                    Title = g.Attributes["Title"].ToString(),
                                    Status = g.Attributes["Status"].ToString(),
                                    ProductName = "ADLL"
                                });

                            }


                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }

                    }
                    else if (idGraphicOverlayResults.Count > 1)
                    {
                        mylistbox.Items.Clear();
                        ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        Geometry_OnviewTap(_pointgraph);
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "Digital List of Radio Signals")
                {
                    IReadOnlyList<IdentifyGraphicsOverlayResult> idGraphicOverlayResults1 = await MyMapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                    var idGraphicOverlayResults = idGraphicOverlayResults1.Where(r => r.GraphicsOverlay.Id != "seven" && r.GraphicsOverlay.Id != "Portoverlay" && r.GraphicsOverlay.Id != "greenPortoverlay" && r.GraphicsOverlay.Id != "redPortoverlay" && r.GraphicsOverlay.Id != "17" && r.GraphicsOverlay.Id != "_sketchRectOverlay").ToList();

                    if (idGraphicOverlayResults.Count == 1)
                    {
                        var idGraphicResult = idGraphicOverlayResults.FirstOrDefault();

                        if (idGraphicResult.Graphics.Count > 1)
                        {
                            mylistbox.Items.Clear();
                            pnlRightMenu.Visibility = Visibility.Visible;
                            return;
                        }

                        var g = idGraphicResult.Graphics.FirstOrDefault();
                        var aa = g.Attributes.Keys.FirstOrDefault();
                        if (g.IsSelected == true)
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = false;
                                        Mouse_Tap_Unselect(item3);
                                        if (checkout.Any(x => x.ShortName == aa))
                                        {
                                            var itemToRemove1 = checkout.FirstOrDefault(r => r.ShortName == aa);
                                            if (itemToRemove1 != null)
                                            {
                                                checkout.Remove(itemToRemove1);
                                            }
                                            objMgr.DeleteShoppingCartByProduct("ADRS",aa);
                                        }
                                    }
                                }
                            }

                            var itemToRemove = cart.FirstOrDefault(r => r.ShortName == aa);
                            if (itemToRemove != null)
                            {
                                cart.Remove(itemToRemove);
                            }
                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                        else
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = true;
                                        Mouse_tap_select(item3);
                                    }
                                }
                            }

                            bool alreadyExists = cart.Any(x => x.ShortName == g.Attributes["ShortName"].ToString());
                            if (!alreadyExists)
                            {
                                string exist = "";
                                string ss = g.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "ADRS");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }


                                cart.Add(new ShoppingCart()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    ShortName = g.Attributes["ShortName"].ToString(),
                                    Title = g.Attributes["Title"].ToString(),
                                    Status = g.Attributes["Status"].ToString(),
                                    ProductName = "ADRS"
                                });
                            }

                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }

                    }
                    else if (idGraphicOverlayResults.Count > 1)
                    {
                        mylistbox.Items.Clear();
                        ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        Geometry_OnviewTap(_pointgraph);
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "e-NP Sailing Direction")
                {
                    IReadOnlyList<IdentifyGraphicsOverlayResult> idGraphicOverlayResults1 = await MyMapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                    var idGraphicOverlayResults = idGraphicOverlayResults1.Where(r => r.GraphicsOverlay.Id != "seven" && r.GraphicsOverlay.Id != "Portoverlay" && r.GraphicsOverlay.Id != "greenPortoverlay" && r.GraphicsOverlay.Id != "redPortoverlay" && r.GraphicsOverlay.Id != "17" && r.GraphicsOverlay.Id != "_sketchRectOverlay").ToList();


                    if (idGraphicOverlayResults.Count == 1)
                    {
                        var idGraphicResult = idGraphicOverlayResults.FirstOrDefault();

                        //if (idGraphicResult.Graphics.Count > 1)
                        //{
                        //    mylistbox.Items.Clear();
                        //    ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        //    Geometry_OnviewTap(_pointgraph);
                        //    pnlRightMenu.Visibility = Visibility.Visible;
                        //    return;
                        //}

                        var g = idGraphicResult.Graphics.FirstOrDefault();
                        var aa = g.Attributes.Keys.FirstOrDefault();

                        if (g.IsSelected == true)
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = false;
                                        Mouse_Tap_Unselect(item3);
                                        if (checkout.Any(x => x.ShortName == aa))
                                        {
                                            var itemToRemove1 = checkout.FirstOrDefault(r => r.ShortName == aa);
                                            if (itemToRemove1 != null)
                                            {
                                                checkout.Remove(itemToRemove1);
                                            }
                                            objMgr.DeleteShoppingCartByProduct("e-Nautical Publications", aa);
                                        }
                                    }
                                }
                            }

                            var itemToRemove = cart.FirstOrDefault(r => r.ShortName == aa);
                            if (itemToRemove != null)
                            {
                                cart.Remove(itemToRemove);
                            }
                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                        else
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = true;
                                        Mouse_tap_select(item3);
                                    }
                                }
                            }
                            //var que = objMgr.GetENPSDByShortName(aa);
                            //var val = que.FirstOrDefault();

                            bool alreadyExists = cart.Any(x => x.ShortName == g.Attributes["ShortName"].ToString());
                            if (!alreadyExists)
                            {
                                string exist = "";

                                string ss = g.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "ADRS");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }


                                cart.Add(new ShoppingCart()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    ShortName = g.Attributes["ShortName"].ToString(),
                                    Title = g.Attributes["Title"].ToString(),
                                    Status = g.Attributes["Status"].ToString(),
                                    ProductName = "e-Nautical Publications"
                                });
                            }

                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                    }
                    else if (idGraphicOverlayResults.Count > 1)
                    {
                        mylistbox.Items.Clear();
                        ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        Geometry_OnviewTap(_pointgraph);
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }

                }
                if (Maincat == "TotalTide")
                {
                    IReadOnlyList<IdentifyGraphicsOverlayResult> idGraphicOverlayResults1 = await MyMapView.IdentifyGraphicsOverlaysAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                    var idGraphicOverlayResults = idGraphicOverlayResults1.Where(r => r.GraphicsOverlay.Id != "seven" && r.GraphicsOverlay.Id != "Portoverlay" && r.GraphicsOverlay.Id != "greenPortoverlay" && r.GraphicsOverlay.Id != "redPortoverlay" && r.GraphicsOverlay.Id != "17" && r.GraphicsOverlay.Id != "_sketchRectOverlay").ToList();

                    if (idGraphicOverlayResults.Count == 1)
                    {
                        var idGraphicResult = idGraphicOverlayResults.FirstOrDefault();

                        //if (idGraphicResult.Graphics.Count > 1)
                        //{
                        //    mylistbox.Items.Clear();
                        //    ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        //    Geometry_OnviewTap(_pointgraph);
                        //    pnlRightMenu.Visibility = Visibility.Visible;
                        //    return;
                        //}

                        var g = idGraphicResult.Graphics.FirstOrDefault();
                        var aa = g.Attributes.Keys.FirstOrDefault();
                        if (g.IsSelected == true)
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = false;
                                        Mouse_Tap_Unselect(item3);
                                        if (checkout.Any(x => x.ShortName == aa))
                                        {
                                            var itemToRemove1 = checkout.FirstOrDefault(r => r.ShortName == aa);
                                            if (itemToRemove1 != null)
                                            {
                                                checkout.Remove(itemToRemove1);
                                            }
                                            objMgr.DeleteShoppingCartByProduct("TotalTide",aa);
                                        }
                                    }
                                }
                            }

                            var itemToRemove = cart.FirstOrDefault(r => r.ShortName == aa);
                            if (itemToRemove != null)
                            {
                                cart.Remove(itemToRemove);
                            }
                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                        else
                        {
                            foreach (var item2 in idGraphicOverlayResults)
                            {
                                foreach (var item3 in item2.GraphicsOverlay.Graphics)
                                {
                                    if (item3.Attributes.ContainsKey(aa))
                                    {
                                        item3.IsSelected = true;
                                        Mouse_tap_select(item3);
                                    }
                                }
                            }
                            //var que = objMgr.GetTotalTideByShortName(aa);

                            //var val = que.FirstOrDefault();

                            bool alreadyExists = cart.Any(x => x.ShortName == g.Attributes["ShortName"].ToString());
                            if (!alreadyExists)
                            {
                                string exist = "";
                                string ss = g.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "TotalTide");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }


                                cart.Add(new ShoppingCart()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    ShortName = g.Attributes["ShortName"].ToString(),
                                    Title = g.Attributes["Title"].ToString(),
                                    Status = g.Attributes["Status"].ToString(),
                                    ProductName = "TotalTide"
                                });

                            }

                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                    }
                    else if (idGraphicOverlayResults.Count > 1)
                    {
                        mylistbox.Items.Clear();
                        ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                        Geometry_OnviewTap(_pointgraph);
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        
        static Esri.ArcGISRuntime.Geometry.Geometry Graphiccoordinates_Aftertransform(Graphic _graphic)
        {
            Esri.ArcGISRuntime.Geometry.Geometry gr1 = _graphic.Geometry;
            try
            {

                var aftergr = (Esri.ArcGISRuntime.Geometry.Geometry)GeometryEngine.Project(gr1, SpatialReference.Create(4326));

                return aftergr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return gr1;
            }
        }
        private void UnderRoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                var temproutegeom = MyMapView.SketchEditor.Geometry;//for routeline incomplete draw
                if (twogrp.Count > 1)
                {
                    foreach (var item in twogrp)
                    {
                        var ge = Graphiccoordinates_Aftertransform(item);
                        var graphic = new Graphic(ge);
                        Geometry_OnviewTap(graphic);
                    }

                }
                else if (routeline != null)
                {
                    Geometry_OnviewTap(routeline);
                    completecommand(SketchCreationMode.Polyline);
                }

                else if (_countryBorderPolylineGraphic != null)
                {
                    if (temproutegeom == null)
                    {
                        Geometry_OnviewTap(_countryBorderPolylineGraphic);
                    }
                    else if (temproutegeom.GeometryType.ToString() == "Polyline")
                    {

                        if (temproutegeom != null)
                        {
                            var tempgraphic = coordinatesystem_polyline(temproutegeom);
                            Geometry_OnviewTap(tempgraphic);
                        }
                    }
                    else if (temproutegeom.GeometryType.ToString() == "Polygon")
                    {
                        Graphic graphic = CreateGraphic(temproutegeom);
                        polygondrawgrp = coordinatesystem_polygon(graphic);
                        if (polygondrawgrp != null)
                        {

                            Geometry_OnviewTap(polygondrawgrp);
                            completecommand(SketchCreationMode.Polygon);
                            //_sketchOverlay.Graphics.Clear();
                            _sketchRectOverlay.Graphics.Clear();
                        }
                    }
                }
                else if (temproutegeom != null && temproutegeom.GeometryType.ToString() == "Polyline")
                {
                    if (temproutegeom != null)
                    {
                        Esri.ArcGISRuntime.Geometry.Polyline poly = (Esri.ArcGISRuntime.Geometry.Polyline)GeometryEngine.NormalizeCentralMeridian(temproutegeom);
                        if (poly.Parts.Count > 1)
                        {
                            var tempgraphic = coordinatesystem_polyline_new(poly);
                            foreach (var item in tempgraphic)
                            {
                                var ge = Graphiccoordinates_Aftertransform(item);
                                var graphic = new Graphic(ge);
                                Geometry_OnviewTap(graphic);
                                // Geometry_OnviewTap(item);
                            }
                        }
                        else
                        {
                            var tempgraphic = coordinatesystem_polyline(temproutegeom);
                            Geometry_OnviewTap(tempgraphic);
                        }



                    }
                }
                else if (temproutegeom != null && temproutegeom.GeometryType.ToString() == "Polygon")
                {

                    Esri.ArcGISRuntime.Geometry.Polygon polygeom = (Esri.ArcGISRuntime.Geometry.Polygon)GeometryEngine.NormalizeCentralMeridian(temproutegeom);
                    if (polygeom.Parts.Count > 1)
                    {
                        // Graphic graphic = CreateGraphic(temproutegeom);
                        var listgraphic = coordinatesystem_polygon_new(polygeom);
                        foreach (var item in listgraphic)
                        {
                            var ge = Graphiccoordinates_Aftertransform(item);
                            var ge1 = CreateGraphic(ge);
                            Geometry_OnviewTap(ge1);
                            // Geometry_OnviewTap(item);
                        }
                        completecommand(SketchCreationMode.Polygon);
                        ///_sketchOverlay.Graphics.Clear();
                        _sketchRectOverlay.Graphics.Clear();
                    }
                    else
                    {
                        if (temproutegeom != null)
                        {
                            Graphic graphic = CreateGraphic(temproutegeom);
                            polygondrawgrp = coordinatesystem_polygon(graphic);
                            if (polygondrawgrp != null)
                            {

                                Geometry_OnviewTap(polygondrawgrp);
                                completecommand(SketchCreationMode.Polygon);
                                //_sketchOverlay.Graphics.Clear();
                                _sketchRectOverlay.Graphics.Clear();
                            }
                        }
                    }
                }
                else
                {
                    if (temproutegeom != null)
                    {
                        Graphic graphic = CreateGraphic(temproutegeom);
                        polygondrawgrp = coordinatesystem_polygon(graphic);
                        if (polygondrawgrp != null)
                        {

                            Geometry_OnviewTap(polygondrawgrp);
                            completecommand(SketchCreationMode.Polygon);
                            _sketchOverlay.Graphics.Clear();
                        }
                    }
                }
                twogrp.Clear();
                gridrouteline.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Geometry_OnviewTap(Graphic _graphic)
        {
            try
            {
                var ere = _graphic.Geometry.GeometryType.ToString();

                if (Maincat == "AVCS Chart")
                {
                    string s2 = crew.Replace("Overview", "1").Replace("General", "2").Replace("Coastal", "3").Replace("Approach", "4").Replace("Harbour", "5").Replace("Berthing", "6");
                    s2 = s2.TrimEnd(',');
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {

                        if (item.Id == "1" || item.Id == "2" || item.Id == "3" || item.Id == "4" || item.Id == "5" || item.Id == "6")
                        {
                            foreach (var item1 in selectedval)
                            {
                                var se = item1.ToString();
                                if (se == item.Id)
                                {
                                    grc.Add(item);
                                }
                            }

                        }
                    }
                    if (ere == "Polyline")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                int bb = 0;
                                var aa = item.Attributes.Keys.FirstOrDefault();


                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }
                                if (selectedval.Contains(bb))
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "AVCS Products"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }

                                        cart.Add(new ShoppingCart()
                                        {
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            Exists = exist,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["title"].ToString(),
                                            Scale = item.Attributes["scale"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            Usage = objcont.Band(item.Attributes["Usage"].ToString()),
                                            UnitId = item.Attributes["UnitId"].ToString(),
                                            UnitTitle = item.Attributes["UnitTitle"].ToString(),
                                            ProductName = "AVCS Products"
                                        });
                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else if (ere == "Polygon")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                int bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());


                                if (selectedval.Contains(bb))
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "AVCS Products"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }

                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["title"].ToString(),
                                            Scale = item.Attributes["scale"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            Usage = objcont.Band(item.Attributes["Usage"].ToString()),
                                            UnitId = item.Attributes["UnitId"].ToString(),
                                            UnitTitle = item.Attributes["UnitTitle"].ToString(),
                                            ProductName = "AVCS Products"
                                        });
                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else
                    {
                        Graphic singleitem = null;
                        foreach (var item in grc.SelectMany(i => i.Graphics.OrderBy(f => Convert.ToInt32(f.Attributes["Usage"]))).OrderBy(x => Convert.ToInt32(x.Attributes["Usage"])))
                        {

                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                var tool = objcont.Band(item.Attributes["Usage"].ToString());
                                int bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                if (selectedval.Contains(bb))
                                {
                                    if (!mylistbox.Items.Contains(aa))
                                    {
                                        ListBoxItem itm = new ListBoxItem();
                                        itm.Content = aa;
                                        ToolTipService.SetToolTip(itm, tool);
                                        mylistbox.Items.Add(itm);
                                        singleitem = item;
                                    }
                                }
                            }
                        }
                        if (mylistbox.Items.Count == 1)
                        {
                            mylistbox.Items.Clear();
                            bool alreadyExists = cart.Any(x => x.ShortName == singleitem.Attributes["ShortName"].ToString());
                            if (!alreadyExists)
                            {
                                string exist = "";
                                string ss = singleitem.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }

                                cart.Add(new ShoppingCart()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    ShortName = singleitem.Attributes["ShortName"].ToString(),
                                    Title = singleitem.Attributes["title"].ToString(),
                                    Scale = singleitem.Attributes["scale"].ToString(),
                                    Status = singleitem.Attributes["Status"].ToString(),
                                    Usage = objcont.Band(singleitem.Attributes["Usage"].ToString()),
                                    UnitId = singleitem.Attributes["UnitId"].ToString(),
                                    UnitTitle = singleitem.Attributes["UnitTitle"].ToString(),
                                    ProductName = "AVCS Products"
                                });
                            }

                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                        }
                        else
                        {
                            ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                            pnlRightMenu.Visibility = Visibility.Visible;
                        }
                    }
                }

                if (Maincat == "AVCS Folio")
                {
                    List<GraphicsOverlay> grc1 = new List<GraphicsOverlay>();
                    foreach (var item1 in MyMapView.GraphicsOverlays)
                    {
                        if (item1.Id != "seven")
                        {
                            grc1.Add(item1);
                        }
                    }
                    List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                    string s2 = crew.Replace("Regional", "7").Replace("Port", "8").Replace("Transit", "9");
                    s2 = s2.TrimEnd(',');
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id == "7" || item.Id == "8" || item.Id == "9")
                        {
                            foreach (var item1 in selectedval)
                            {
                                var se = item1.ToString();
                                if (se == item.Id)
                                {
                                    grc.Add(item);
                                }
                            }
                        }

                    }
                    if (ere == "Polyline")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {

                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                int bb = 0;
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }
                                if (selectedval.Contains(bb))
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "AVCS Products"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    string exist = "";

                                    bool alreadyExists = cart1.Any(x => x.FProductId == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }

                                        List<TemperaturesDetails> CartDetails = new List<TemperaturesDetails>();
                                        XElement xele = XElement.Parse(item.Attributes["SubUnit"].ToString());
                                        var dddd = xele.Descendants();
                                        string sd = "";
                                        foreach (var item1 in dddd)
                                        {
                                            sd += item1.Value + ",";

                                        }
                                        // var ENCfolio = objMgr.GetAVCSByFolioId(sd.TrimEnd(','));

                                        foreach (var ter in grc1)
                                        {
                                            foreach (var item2 in ter.Graphics)
                                            {
                                                var sub = item2.Attributes.Keys.FirstOrDefault();
                                                if (sd.Contains(sub))
                                                {
                                                    if (item2.Attributes["ShortName"] != null)
                                                    {
                                                        CartDetails.Add(new TemperaturesDetails()
                                                        {
                                                            ProductId = item2.Attributes["ShortName"].ToString(),
                                                            Title = item2.Attributes["title"].ToString(),
                                                            Band = objcont.Band(item2.Attributes["Usage"].ToString())
                                                        });
                                                    }
                                                }
                                            }
                                        }

                                        cart1.Add(new Temperatures()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            FProductId = item.Attributes["ShortName"].ToString(),
                                            FTitle = item.Attributes["Title"].ToString(),
                                            DetailsItems = CartDetails
                                        });
                                    }
                                    dataGrid2.ItemsSource = null;
                                    dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                                }
                            }

                        }
                    }
                    else if (ere == "Polygon")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {

                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                int bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                if (selectedval.Contains(bb))
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "AVCS Products"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    string exist = "";

                                    bool alreadyExists = cart1.Any(x => x.FProductId == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }

                                        List<TemperaturesDetails> CartDetails = new List<TemperaturesDetails>();
                                        XElement xele = XElement.Parse(item.Attributes["SubUnit"].ToString());
                                        var dddd = xele.Descendants();
                                        string sd = "";
                                        foreach (var item1 in dddd)
                                        {
                                            sd += item1.Value + ",";

                                        }
                                        // var ENCfolio = objMgr.GetAVCSByFolioId(sd.TrimEnd(','));

                                        foreach (var ter in grc1)
                                        {
                                            foreach (var item2 in ter.Graphics)
                                            {
                                                var sub = item2.Attributes.Keys.FirstOrDefault();
                                                if (sd.Contains(sub))
                                                {
                                                    CartDetails.Add(new TemperaturesDetails()
                                                    {
                                                        ProductId = item2.Attributes["ShortName"].ToString(),
                                                        Title = item2.Attributes["title"].ToString(),
                                                        Band = objcont.Band(item2.Attributes["Usage"].ToString())
                                                    });
                                                }
                                            }
                                        }

                                        cart1.Add(new Temperatures()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            FProductId = item.Attributes["ShortName"].ToString(),
                                            FTitle = item.Attributes["Title"].ToString(),
                                            DetailsItems = CartDetails
                                        });
                                    }

                                    dataGrid2.ItemsSource = null;
                                    dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        Graphic singleitem = null;
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {

                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                var tool = objcont.Band(item.Attributes["Usage"].ToString());
                                int bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                if (selectedval.Contains(bb))
                                {
                                    if (!mylistbox.Items.Contains(aa))
                                    {
                                        ListBoxItem itm = new ListBoxItem();
                                        itm.Content = aa;
                                        ToolTipService.SetToolTip(itm, tool);
                                        mylistbox.Items.Add(itm);
                                        singleitem = item;
                                    }
                                }
                            }
                        }
                        if (mylistbox.Items.Count == 1)
                        {
                            mylistbox.Items.Clear();
                            string exist = "";
                            bool alreadyExists = cart1.Any(x => x.FProductId == singleitem.Attributes["ShortName"].ToString());
                            if (!alreadyExists)
                            {

                                string ss = singleitem.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }

                                List<TemperaturesDetails> CartDetails = new List<TemperaturesDetails>();
                                XElement xele = XElement.Parse(singleitem.Attributes["SubUnit"].ToString());
                                var dddd = xele.Descendants();
                                string sd = "";
                                foreach (var item1 in dddd)
                                {
                                    sd += item1.Value + ",";

                                }
                                // var ENCfolio = objMgr.GetAVCSByFolioId(sd.TrimEnd(','));

                                foreach (var ter in grc1)
                                {
                                    foreach (var item2 in ter.Graphics)
                                    {
                                        var sub = item2.Attributes.Keys.FirstOrDefault();
                                        if (sd.Contains(sub))
                                        {
                                            CartDetails.Add(new TemperaturesDetails()
                                            {
                                                ProductId = item2.Attributes["ShortName"].ToString(),
                                                Title = item2.Attributes["title"].ToString(),
                                                Band = objcont.Band(item2.Attributes["Usage"].ToString())
                                            });
                                        }
                                    }
                                }

                                cart1.Add(new Temperatures()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    FProductId = singleitem.Attributes["ShortName"].ToString(),
                                    FTitle = singleitem.Attributes["Title"].ToString(),
                                    DetailsItems = CartDetails
                                });
                            }

                            dataGrid2.ItemsSource = null;
                            dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                        }
                        else
                        {
                            ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                            pnlRightMenu.Visibility = Visibility.Visible;
                        }
                    }
                }

                if (Maincat == "Digital List of Lights")
                {
                    List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id == "13")
                        {
                            grc.Add(item);
                        }

                    }
                    if (ere == "Polyline")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                int bb = 0;
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }
                                if (bb == 13)
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "ADLL"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "ADLL");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            Exists = exist,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "ADLL"
                                        });

                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else if (ere == "Polygon")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                string bb = item.Attributes.Values.FirstOrDefault().ToString();
                                string s2 = "13";
                                if (s2.Contains(bb))
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "ADLL"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "ADLL");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "ADLL"
                                        });

                                    }
                                }

                                dataGrid1.ItemsSource = null;
                                dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                int bb = 0;
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }
                                string s2 = "13";

                                int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));

                                if (selectedval.Contains(bb))
                                {
                                    if (!mylistbox.Items.Contains(aa))
                                    {
                                        mylistbox.Items.Add(aa);
                                    }
                                }
                            }
                        }
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }

                if (Maincat == "Digital List of Radio Signals")
                {
                    string s2 = crew.Replace("ADRS1345", "10").Replace("ADRS2", "11").Replace("ADRS6", "12");
                    s2 = s2.TrimEnd(',');
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id == "10" || item.Id == "11" || item.Id == "12")
                        {
                            foreach (var item1 in selectedval)
                            {
                                var se = item1.ToString();
                                if (se == item.Id)
                                {
                                    grc.Add(item);
                                }
                            }
                        }

                    }
                    if (ere == "Polyline")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                int bb = 0;
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }

                                if (selectedval.Contains(bb))
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "ADRS"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "ADRS");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "ADRS"
                                        });
                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else if (ere == "Polygon")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                int bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                if (selectedval.Contains(bb))
                                {
                                    item.IsSelected = true;
                                    Mouse_tap_select(item);
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "ADRS"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "ADRS");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "ADRS"
                                        });
                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else
                    {
                        Graphic singleitem = null;
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                var tool = objcont.Band(item.Attributes["Usage"].ToString());
                                int bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault());
                                if (selectedval.Contains(bb))
                                {
                                    if (!mylistbox.Items.Contains(aa))
                                    {
                                        ListBoxItem itm = new ListBoxItem();
                                        itm.Content = aa;
                                        ToolTipService.SetToolTip(itm, tool);
                                        mylistbox.Items.Add(itm);
                                        singleitem = item;
                                    }
                                }
                            }
                        }
                        if (mylistbox.Items.Count == 1)
                        {
                            mylistbox.Items.Clear();
                            bool alreadyExists = cart.Any(x => x.ShortName == singleitem.Attributes["ShortName"].ToString());
                            if (!alreadyExists)
                            {
                                string exist = "";
                                string ss = singleitem.Attributes["ShortName"].ToString();
                                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                DataTable dt = new DataTable();
                                dt = objCart.CheckShoppingCartByName(ss, "AVCS Products");
                                if (dt.Rows.Count > 0)
                                {
                                    exist = "Exists";
                                }
                                else
                                {
                                    exist = "NotExists";
                                }

                                cart.Add(new ShoppingCart()
                                {
                                    Exists = exist,
                                    Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                    ShortName = singleitem.Attributes["ShortName"].ToString(),
                                    Title = singleitem.Attributes["title"].ToString(),
                                    Scale = singleitem.Attributes["scale"].ToString(),
                                    Status = singleitem.Attributes["Status"].ToString(),
                                    Usage = objcont.Band(singleitem.Attributes["Usage"].ToString()),
                                    UnitId = singleitem.Attributes["UnitId"].ToString(),
                                    UnitTitle = singleitem.Attributes["UnitTitle"].ToString(),
                                    ProductName = "AVCS Products"
                                });
                            }

                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id);
                        }
                        else
                        {
                            ShowHideMenu("sbShowRightMenu", btnLeftMenuShow, pnlRightMenu);
                            pnlRightMenu.Visibility = Visibility.Visible;
                        }
                    }
                }

                if (Maincat == "e-NP Sailing Direction")
                {
                    List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id == "14")
                        {
                            grc.Add(item);
                        }

                    }
                    if (ere == "Polyline")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                int bb = 0;
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }
                                if (bb == 14)
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "e-Nautical Publications"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "e-Nautical Publications");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "e-Nautical Publications"
                                        });
                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else if (ere == "Polygon")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                int bb = 0;
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }
                                if (bb == 14)
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "e-Nautical Publications"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    //var val = que.FirstOrDefault();
                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "e-Nautical Publications");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "e-Nautical Publications"
                                        });
                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                int bb = 0;
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }
                                if (bb == 14)
                                {
                                    if (!mylistbox.Items.Contains(aa))
                                    {
                                        mylistbox.Items.Add(aa);
                                    }
                                }
                            }
                        }
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }

                if (Maincat == "TotalTide")
                {
                    List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id == "15")
                        {
                            grc.Add(item);
                        }

                    }
                    if (ere == "Polyline")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                int bb = 0;
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }

                                if (bb == 15)
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "TotalTide"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "TotalTide");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "TotalTide"
                                        });

                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else if (ere == "Polygon")
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                int bb = 0;
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }

                                if (bb == 15)
                                {
                                    item.IsSelected = true;
                                    if (checkout.Any(x => x.ShortName == aa && x.ProductName == "TotalTide"))
                                    {
                                        Mouse_tap_AddedCart(item);
                                    }
                                    else
                                    {
                                        Mouse_tap_select(item);
                                    }

                                    bool alreadyExists = cart.Any(x => x.ShortName == item.Attributes["ShortName"].ToString());
                                    if (!alreadyExists)
                                    {
                                        string exist = "";
                                        string ss = item.Attributes["ShortName"].ToString();
                                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                                        DataTable dt = new DataTable();
                                        dt = objCart.CheckShoppingCartByName(ss, "TotalTide");
                                        if (dt.Rows.Count > 0)
                                        {
                                            exist = "Exists";
                                        }
                                        else
                                        {
                                            exist = "NotExists";
                                        }


                                        cart.Add(new ShoppingCart()
                                        {
                                            Exists = exist,
                                            Id = cart.Count > 0 ? cart.Max(x => x.Id) + 1 : 1,
                                            ShortName = item.Attributes["ShortName"].ToString(),
                                            Title = item.Attributes["Title"].ToString(),
                                            Status = item.Attributes["Status"].ToString(),
                                            ProductName = "TotalTide"
                                        });

                                    }

                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in grc.SelectMany(i => i.Graphics))
                        {
                            if ((GeometryEngine.Intersects(item.Geometry, _graphic.Geometry) || GeometryEngine.Within(item.Geometry, _graphic.Geometry) || GeometryEngine.Overlaps(item.Geometry, _graphic.Geometry)))
                            {
                                var aa = item.Attributes.Keys.FirstOrDefault();
                                int bb = 0;
                                if (aa != null)
                                {
                                    bb = Convert.ToInt32(item.Attributes.Values.FirstOrDefault().ToString());
                                }

                                if (bb == 15)
                                {
                                    if (!mylistbox.Items.Contains(aa))
                                    {
                                        mylistbox.Items.Add(aa);
                                    }
                                }
                            }
                        }
                        pnlRightMenu.Visibility = Visibility.Visible;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        Esri.ArcGISRuntime.Geometry.PointCollection importedlinepoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
        Esri.ArcGISRuntime.Geometry.PointCollection normalizedimportedpoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
        private void Import_Click(object sender, RoutedEventArgs e)
        {

            string strfilename = "";
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            // openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                openFileDialog.DefaultExt = ".bsk";
                openFileDialog.Filter = "Basket(.bsk)|*.bsk";
                strfilename = openFileDialog.FileName;

            }
            if (strfilename.Contains(".csv"))
            {
                csv_Import(strfilename);
                // csv_export_new(sender, e, strfilename);
            }
            else if (strfilename.Contains(".rtz"))
            {
                rtz_Import(strfilename);
                //csv_export(sender, e, strfilename);
            }

            
        }

        private void csv_Import(string fpath)//add this new function as fallowing
        {
            _sketchOverlay.Graphics.Clear();
            importedlinepoints.Clear();
            routewaypointoverlay.Graphics.Clear();
            List<MapPoint> _mappoint = new List<MapPoint>();
            SelectedRoutName = "";
            string[] columns =
                                {
                                    @"LAT",
                                    @"LON"
                                };
            try
            {
                //string strfilename = "";
                // Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                // openFileDialog.Filter = "All files (*.*)|*.*";
                //if (openFileDialog.ShowDialog() == true)
                //{
                //    openFileDialog.DefaultExt = ".bsk";
                //    openFileDialog.Filter = "Basket(.bsk)|*.bsk";
                //    strfilename = openFileDialog.FileName;
                //    strfilename = openFileDialog.FileName;
                //}

                string[] csvlines = File.ReadAllLines(fpath);
                List<string> strt = new List<string>();

                var coun = csvlines.Count();

                for (int i = 0; i <= coun; i++)
                {
                    if (i == coun)
                    {
                        // MapPoint mptx = new MapPoint(mpt[i--].X, mpt[i--].Y, SpatialReferences.Wgs84);
                        // transformed_new.Add(mptx);

                        break;

                    }
                    if (i >= 4)
                    {
                        var laso = csvlines[i].ToString();
                        var ser = laso.Split(',');
                        var sear = csvlines[i].ToCharArray();
                        StringBuilder sb_1 = new StringBuilder();
                        StringBuilder sb_2 = new StringBuilder();
                        int cour = 0;
                        int cour2 = 0;
                        foreach (var ch in sear)
                        {
                            if (Char.IsLetter(ch))
                            {
                                sb_1.Append(ch);
                                cour++;
                                cour2++;
                                sb_2.Append(sb_1);
                            }
                            else
                            {
                                sb_1.Append(ch);
                            }


                            if (cour == 1)
                            {
                                sb_1.Clear();
                                cour--;
                            }
                            if (cour2 == 2)
                            {
                                strt.Add(sb_2.ToString());
                                break;
                            }
                        }
                    }
                }
                //select the indices of the columns we want
                foreach (var ser in strt)
                {
                    var rer = dotCount_manipulatestring(ser);
                    var string2 = rer.Item2;
                    var mpt_wgs = CoordinateFormatter.FromLatitudeLongitude(string2, SpatialReferences.Wgs84);
                    _mappoint.Add(mpt_wgs);
                    importedlinepoints.Add(mpt_wgs);

                }
                normalizedimportedpoints = CalcNormalize_latest(_mappoint);
                var pointline = loadrouteline_create(normalizedimportedpoints);

                var roadPolyline = pointline as Esri.ArcGISRuntime.Geometry.Polyline;
                // var roadPolyline = graphic.Geometry as Esri.ArcGISRuntime.Geometry.Polyline;
                this.polylineBuilder = new PolylineBuilder(roadPolyline);
                if (polylineBuilder.Parts.Count > 1)
                {
                    var item = coordinatesystem_polyline_new(pointline);
                    foreach (var item1 in item)
                    {
                        // var set = Graphiccoordinates_Aftertransform(item1);
                        _sketchOverlay.Graphics.Add(item1);
                        //route_symbolsadding_webmerc(get);
                        //gridrouteline.Add(item1);
                    }
                }
                else
                {
                    var loadedgraphic = coordinatesystem_polyline(pointline);
                    _sketchOverlay.Graphics.Add(loadedgraphic);
                    // route_symbolsadding_webmerc(get);

                }
                route_symbolsadding_webmerc(normalizedimportedpoints);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static (int, string) dotCount_manipulatestring(string str)
        {
            int spcctr = 0;
            string str1 = null;
            string sub2 = null;
            try
            {
                

                StringBuilder sb = new StringBuilder(str);
                sb.Replace(',', ' ');
                var fin = sb.ToString();
                var remfirst1 = fin.Remove(0, 2);
                var ser1 = remfirst1.ToCharArray();
                for (int i = 0; i < ser1.Length; i++)
                {
                    str1 = remfirst1.Substring(i, 1);
                    if (str1 == " ")
                    {
                        if (spcctr == 0)
                        {

                            ser1.SetValue(' ', i);

                        }
                        else if (spcctr == 1)
                        {

                            ser1.SetValue(' ', i);
                        }
                        else if (spcctr == 2)
                        {

                            ser1.SetValue(' ', i);

                        }
                        else if (spcctr == 3)
                        {

                            ser1.SetValue(' ', i);

                        }
                        else if (spcctr == 4)
                        {

                            ser1.SetValue(' ', i);
                        }

                        spcctr++;
                    }
                    sub2 = new string(ser1);
                }
                return (spcctr, sub2);
            }
            catch
            {
                return (spcctr, sub2);
            }
            
        }
        private void rtz_Import(string strfilename)//add this new function as fallowing
        {
            _sketchOverlay.Graphics.Clear();
            importedlinepoints.Clear();
            routewaypointoverlay.Graphics.Clear();

            SelectedRoutName = "";
            try
            {

                XDocument doc1 = XDocument.Load(strfilename);
                XNamespace ns = "http://www.cirm.org/RTZ/1/0";
                List<MapPoint> _mappoint = new List<MapPoint>();
                // Esri.ArcGISRuntime.Geometry.PointCollection importedpoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                // IReadOnlyList<MapPoint> _mappoint=null;
                foreach (XElement element in doc1.Root
                                    .Element(ns + "waypoints")
                                    .Elements(ns + "waypoint")
                                    .Elements(ns + "position"))
                {
                    Console.WriteLine("Name: {0}; Value: {1}",
                         (double)element.FirstAttribute,
                         (double)element.LastAttribute);
                    MapPoint mpt = new MapPoint((double)element.LastAttribute, (double)element.FirstAttribute);
                    _mappoint.Add(mpt);
                    importedlinepoints.Add(mpt);
                }
                //SelectPrdctsunderRoot_Click(_mappoint);
                //distancemes();

                normalizedimportedpoints = CalcNormalize_latest(_mappoint);
                var pointline = loadrouteline_create(normalizedimportedpoints);

                var roadPolyline = pointline as Esri.ArcGISRuntime.Geometry.Polyline;
                // var roadPolyline = graphic.Geometry as Esri.ArcGISRuntime.Geometry.Polyline;
                this.polylineBuilder = new PolylineBuilder(roadPolyline);
                if (polylineBuilder.Parts.Count > 1)
                {
                    var item = coordinatesystem_polyline_new(pointline);
                    foreach (var item1 in item)
                    {
                        // var set = Graphiccoordinates_Aftertransform(item1);
                        _sketchOverlay.Graphics.Add(item1);
                        //route_symbolsadding_webmerc(get);
                        //gridrouteline.Add(item1);
                    }
                }
                else
                {
                    var loadedgraphic = coordinatesystem_polyline(pointline);
                    _sketchOverlay.Graphics.Add(loadedgraphic);
                    // route_symbolsadding_webmerc(get);

                }
                route_symbolsadding_webmerc(normalizedimportedpoints);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Esri.ArcGISRuntime.Geometry.PointCollection CalcNormalize_latest(List<MapPoint> mpt)
        {
            Esri.ArcGISRuntime.Geometry.PointCollection transformed_new = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);

            try
            {
                for (int i = 0; i <= mpt.Count; i++)//reading correct values indexed
                {
                    if (i + 1 >= mpt.Count)
                    {
                        Console.WriteLine("for {0} mappoint value is {1}  ", i, mpt[i--]);
                        break;

                    }
                    Console.WriteLine("for {0} mappoint value is {1}  ", i, mpt[i]);
                }

                int j = 0;
                var p = mpt[j].X;

                for (int i = 0; i < mpt.Count; i++)
                {

                    var x0 = mpt[i].X;

                    if (i + 1 == mpt.Count + 1)
                    {
                        //MapPoint mptx = new MapPoint(mpt[i--].X, mpt[i--].Y, SpatialReferences.Wgs84);
                        //var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                        // transformed_new.Add(aftergr);


                        break;

                    }
                    if (i == 0)
                    {
                        MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                        var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                        transformed_new.Add(aftergr);
                    }
                    else
                    {
                        var x1 = mpt[i - 1].X;
                        var x2 = mpt[i].X;
                        var x3 = x2 - x1;

                        if ((p > 0 && x2 - p > 0))
                        {
                            MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                            var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                            var x4 = ((mpt[i].X - p) + p);
                            var x4new = x4 * 111319.491;
                            MapPoint aftermappoint = new MapPoint(x4new, aftergr.Y, SpatialReference.Create(3857));
                            transformed_new.Add(aftermappoint);
                        }
                        else if ((p > 0 && x2 - p < 0))
                        {
                            MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                            var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                            var x5 = (((mpt[i].X - p) + 360) + p);
                            var x5new = x5 * 111319.491;
                            MapPoint aftermappoint = new MapPoint(x5new, aftergr.Y, SpatialReference.Create(3857));
                            transformed_new.Add(aftermappoint);

                        }
                        else if ((p < 0 && x2 - p > 0))
                        {
                            MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                            var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                            var x6 = ((mpt[i].X - p) + p);
                            var x6new = x6 * 111319.491;
                            MapPoint aftermappoint = new MapPoint(x6new, aftergr.Y, SpatialReference.Create(3857));
                            transformed_new.Add(aftermappoint);
                        }
                        else if ((p < 0 && x2 - p < 0))
                        {
                            MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                            var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                            var x7 = (((mpt[i].X - p) + 360) + p);
                            var x7new = x7 * 111319.491;
                            MapPoint aftermappoint = new MapPoint(x7new, aftergr.Y, SpatialReference.Create(3857));
                            transformed_new.Add(aftermappoint);
                        }
                        else
                        {
                            MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                            var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                            transformed_new.Add(aftergr);
                        }

                    }

                }
                return transformed_new;
            }
            catch
            {
                return transformed_new;
            }
        }
        private async void route_symbolsadding_webmerc(Esri.ArcGISRuntime.Geometry.PointCollection PointCollection)
        {
            try

            {
                foreach (var tem in PointCollection)
                {
                    polist.polylinepointcollection = null;
                    switch (SampleState.AddingStops)
                    {
                        case SampleState.AddingStops:
                            // Get the name of this stmop.
                            string stopName = $"W{routewaypointoverlay.Graphics.Count + 1 }";
                            // polist.WaypointCount = _sketchRouteOverlay.Graphics.Count + 1;
                            // Create the marker to show underneath the stop number.
                            PictureMarkerSymbol pushpinMarker = await GetPictureMarker();

                            polist.latitude = tem.X;
                            polist.longitude = tem.Y;


                            TextSymbol stopSymbol = new TextSymbol(stopName, System.Drawing.Color.Transparent, 15,
                                Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Right, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                            stopSymbol.OffsetY = 0;

                            CompositeSymbol combinedSymbol = new CompositeSymbol(new MarkerSymbol[] { pushpinMarker, stopSymbol });

                            Graphic stopGraphic = new Graphic(tem, combinedSymbol);
                            // Graphic stopGraphic = new Graphic(tem);
                            stopGraphic.Attributes["ShortName"] = stopName;
                            routewaypointoverlay.Graphics.Add(stopGraphic);
                            break;
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // Normalize geometry - important for geometries that will be sent to a server for processing.
            //mapLocation = (MapPoint)GeometryEngine.NormalizeCentralMeridian(mapLocation);
            // var ste = Mapcoordinates_Change(mapLocation);
        }
        private void ExportRoute_Click(object sender, RoutedEventArgs e )
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Filter = "RTZ(.rtz)|*.rtz | Tokyo Keiki EC8x00|*.csv";
                dlg.ShowDialog();
                string fpath = dlg.FileName;
                if (fpath.Contains(".csv"))
                {
                    csv_export_new(fpath);
                }
                else if (fpath.Contains(".rtz"))
                {
                    rtz_export_new(fpath);
                }
            }
            catch
            {

            }
        }
        private void Geodetic_ExportRoute_Click_both(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Filter = "RTZ(.rtz)|*.rtz | Tokyo Keiki EC8x00|*.csv";
                dlg.ShowDialog();
                string fpath = dlg.FileName;
                if (fpath.Contains(".rtz"))
                {
                    rtz_export_new(fpath);
                }
                else if (fpath.Contains(".csv"))
                {
                    csv_export_new(fpath);
                }
            }
            catch
            {

            }
        }

        private void rtz_export_new(string fpath)
        {
            try
            {
                XmlDeclaration xmldecl;
                XmlDocument doc = new XmlDocument();
                xmldecl = doc.CreateXmlDeclaration("1.0", null, null);

                //Add the new node to the document.
                XmlElement root1 = doc.DocumentElement;
                doc.InsertBefore(xmldecl, root1);


                XmlElement root = doc.CreateElement("route");
                XmlElement id = doc.CreateElement("routeInfo");



                root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                root.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                root.SetAttribute("version", "1.0");
                root.SetAttribute("xmlns", "http://www.cirm.org/RTZ/1/0");
                root.SetAttribute("xmlns", "http://www.cirm.org/RTZ/1/0");
                id.SetAttribute("routeName", SelectedRoutName);



                root.AppendChild(id);
                XmlElement id1 = doc.CreateElement("defaultWaypoint");
                id1.SetAttribute("radius", "0.5");
                XmlElement ws = doc.CreateElement("leg");
                ws.SetAttribute("starboardXTD", "0.1");
                ws.SetAttribute("portsideXTD", "0.1");
                id1.AppendChild(ws);

                XmlElement id2 = doc.CreateElement("waypoints");
                id2.AppendChild(id1);

                DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
                DataTable dt = new DataTable();
                dt = objRout.GetRouteLineDetails(SelectedRoutName);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    XmlElement wsy = doc.CreateElement("waypoint");
                    XmlElement wsy1 = doc.CreateElement("position");

                    XmlElement wsy2 = doc.CreateElement("leg");
                    int j = i + 1;
                    wsy.SetAttribute("id", j.ToString());
                    wsy.SetAttribute("revision", "1");

                    var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                    var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                    MapPoint mp = new MapPoint(latit, longit, SpatialReferences.WebMercator);//add this


                    MapPoint aftertrans = (MapPoint)GeometryEngine.Project(mp, SpatialReference.Create(4326));//add this


                    wsy1.SetAttribute("lat", aftertrans.Y.ToString());//add this
                    wsy1.SetAttribute("lon", aftertrans.X.ToString());//add this

                    // wsy1.SetAttribute("lat", dt.Rows[i]["Latitude"].ToString());//comment this
                    // wsy1.SetAttribute("lon", dt.Rows[i]["Longitude"].ToString());//comment this

                    wsy.AppendChild(wsy1);
                    wsy.AppendChild(wsy2);

                    id2.AppendChild(wsy);
                    root.AppendChild(id2);
                }

                doc.AppendChild(root);
                doc.Save(fpath);
            }
            catch
            {

            }
        }
        private void csv_export_new(string fpath)
        {
            try
            {
                List<double> distance = new List<double>();
                Esri.ArcGISRuntime.Geometry.PointCollection geo_points = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);

                //Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                //dlg.DefaultExt = ".csv";
                //dlg.Filter = "RTZ(.rtz)|*.rtz | Tokyo Keiki EC8x00|*.csv";
                //dlg.ShowDialog();
                //string fpath = dlg.FileName;
                string fpath_new = fpath;
                //string filePath = @"C:\Users\nanip\OneDrive\Desktop\routelineexport\File7.csv";
                DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
                DataTable dt = new DataTable();
                dt = objRout.GetRouteLineDetails(SelectedRoutName);

                using (TextWriter sw = new StreamWriter(fpath_new))
                {
                    sw.WriteLine("// ROUTE SHEET exported by JRC ECDIS.");
                    sw.WriteLine("// <<NOTE>>This strings // indicate comment column/cells. You can edit freely.");
                    sw.WriteLine("//SG,<Normal>");
                    sw.WriteLine(@"// WPT No.,LAT, ,,LON, ,,PORT[NM],STBD[NM],Arr. Rad[NM],Speed[kn],Sail(RL/GC),ROT[deg/min],Turn Rad[NM],Time Zone, ,Name");
                    // sw.WriteLine(@"ID, LAT,LON, , , , , , , ,To WPT,TOTAL");



                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                        var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this

                        MapPoint mp = new MapPoint(latit, longit, SpatialReferences.WebMercator);//add this
                        MapPoint aftertrans = (MapPoint)GeometryEngine.Project(mp, SpatialReference.Create(4326));

                        geo_points.Add(aftertrans);
                    }
                    for (int j = 0; j < geo_points.Count; j++)
                    {
                        var saf_deg_dec_minu = CoordinateFormatter.ToLatitudeLongitude(geo_points[j], LatitudeLongitudeFormat.DegreesDecimalMinutes, 3);
                        var saf_dec_deg = CoordinateFormatter.ToLatitudeLongitude(geo_points[j], LatitudeLongitudeFormat.DecimalDegrees, 3);
                        var saf_deg_min_sec = CoordinateFormatter.ToLatitudeLongitude(geo_points[j], LatitudeLongitudeFormat.DegreesMinutesSeconds, 3);


                        var cnt = SpaceCount_manipulatestring(saf_deg_dec_minu);
                        var cnt1 = cnt.Item1;
                        var cnt2 = cnt.Item2;
                        var latstr = cnt2.Split(':');//[0]50-16.832N,[1]001-22.690W

                        var lat = latstr[0].Last();//N
                        var firststrsplit = latstr[0].Split('-');//[0]50,[1]16.832N
                        var firststr = firststrsplit[0].ToString();//50
                        var midlstr1count = firststrsplit[1].Length - 1;//6
                        var midlstr1 = firststrsplit[1].Substring(0, midlstr1count);//16.832

                        var longitu = latstr[1].Last();//W
                        var secondstrsplit = latstr[1].Split('-');//[0]001,[1]22.690W
                        var secondstr = secondstrsplit[0].ToString();//001
                        var midlstr2count = secondstrsplit[1].Length - 1;//6
                        var midlstr2 = secondstrsplit[1].Substring(0, midlstr2count);//22.690

                        var port = 0.2;
                        var stbd = 0.2;
                        var arr_rad = 0.5;
                        var speed = 20;
                        var sail = "RL";
                        var Rot = 38.2;
                        var turnRad = 0.5;
                        var Timezone = "09:00";//utc time zone current export time 
                        var Time_zone = "E";


                        if (j >= 1)
                        {
                            // GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geo_points.ElementAt(j - 1), geo_points.ElementAt(j), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                            // double laxidromeround = Math.Round(loxodromeMeasureResult.Distance, 2);
                            //distance.Add(laxidromeround);
                            // double totaldist = distance.ElementAt(j - 1) + distance.ElementAt(j);
                            //double rounddecim = Math.Round(totaldist, 2);
                            //distance[j] = rounddecim;
                            sw.WriteLine(@"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", j, firststr, midlstr1, lat, secondstr, midlstr2, longitu, port, stbd, arr_rad, speed, sail, Rot, turnRad, Timezone, Time_zone);

                        }
                        else
                        {

                            sw.WriteLine(@"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", j, firststr, midlstr1, lat, secondstr, midlstr2, longitu, "***", "***", "***", "***", "***", "***", "***", Timezone, Time_zone);
                            //distance.Add(loxodromeMeasureResult);
                        }


                    }
                }
            }
            catch
            {

            }
        }
        public static (int, string) SpaceCount_manipulatestring(string str)
        {
            int spcctr = 0;
            string str1 = null;
            string sub2 = null;
            try
            {
               
                var so = str.ToCharArray();
                for (int i = 0; i < str.Length; i++)
                {
                    str1 = str.Substring(i, 1);
                    if (str1 == " ")
                    {
                        if (spcctr == 0)
                        {

                            so.SetValue('-', i);

                        }
                        else if (spcctr == 1)
                        {

                            so.SetValue(':', i);
                        }
                        else if (spcctr == 2)
                        {

                            so.SetValue('-', i);

                        }
                        spcctr++;
                    }

                }
                //  string str = new string(character_array);
                sub2 = new string(so);
                return (spcctr, sub2);
            }
            catch
            {
                return (spcctr, sub2);
            }
        }
        private async void rectangle_Click(object sender, RoutedEventArgs e)
        {
            //RemoveHeilight();
           // routewaypointoverlay.Graphics.Clear();
            MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
            //_sketchOverlay.Graphics.Clear();
            _sketchRectOverlay.Graphics.Clear();
            Mouse.OverrideCursor = Cursors.Cross;
            routeline = null;
            polygondrawgrp = null;
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.Rectangle;

                Esri.ArcGISRuntime.Geometry.Geometry geometry = await MyMapView.SketchEditor.StartAsync(creationMode, true);
                //if (eDrawStatus.InProgress)
                //{

                //}
                // Create and add a graphic from the geometry the user drew
                Graphic graphic = CreateGraphic(geometry);
                //_sketchOverlay.Graphics.Add(graphic);
                if (geometry.GeometryType.ToString() == "Polygon")
                {

                    polygondrawgrp = coordinatesystem_polygon(graphic);
                }
                MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch //(Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        private async void polygon_Click(object sender, RoutedEventArgs e)
        {
            //RemoveHeilight();
            //routewaypointoverlay.Graphics.Clear();
            MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
            //_sketchOverlay.Graphics.Clear();
            _sketchRectOverlay.Graphics.Clear();
            Mouse.OverrideCursor = Cursors.Cross;
            routeline = null;
            polygondrawgrp = null;
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.Polygon;

                Esri.ArcGISRuntime.Geometry.Geometry geometry = await MyMapView.SketchEditor.StartAsync(creationMode, true);
                //if (eDrawStatus.InProgress)
                //{

                //}
                // Create and add a graphic from the geometry the user drew
                Graphic graphic = CreateGraphic(geometry);
                if (geometry.GeometryType.ToString() == "Polygon")
                {

                    polygondrawgrp = coordinatesystem_polygon(graphic);
                }
                MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        PolylineBuilder polybroute = null;
        int waypointcount = 0;
        IReadOnlyList<MapPoint> routelinepoints = null;
        IReadOnlyList<MapPoint> routelinepointlist = null;
        private void clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = "Are you sure you want to clear this route line?";
                string caption = "Confirmation";
                lstSelectedRoute.Clear();
                SelectedRoutName = "";

                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Question;
                if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                {

                    symbolspointslist_savebtn.Clear();
                    geodesic_savbtnpoints_webmerccollection.Clear();
                    //Mouse.OverrideCursor = Cursors.Cross;
                    _sketchOverlay.Graphics.Clear();
                    routewaypointoverlay.Graphics.Clear();
                    gridrouteline.Clear();//add this new line
                    importedlinepoints.Clear();
                    normalizedimportedpoints.Clear();

                    // Cursor = systemCursor2;
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped -= MyMapViewOnGeoViewTapped;
                }
                else
                {
                    return;
                }
                if (MyMapView.SketchEditor.CancelCommand.CanExecute(null))
                {
                    MyMapView.SketchEditor.CancelCommand.Execute(null);
                    // lvUsers.Items.Clear();
                    // Mouse.OverrideCursor = Cursors.Cross;
                    // Cursor = systemCursor1;
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                }
                else
                {
                    _sketchOverlay.Graphics.Clear();
                    routewaypointoverlay.Graphics.Clear();
                    gridrouteline.Clear();//add this new line
                                          // Mouse.OverrideCursor = Cursors.Cross;
                                          //  Cursor = systemCursor1;
                    importedlinepoints.Clear();
                    normalizedimportedpoints.Clear();
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped -= MyMapViewOnGeoViewTapped;
                }
                UndoRoot1.IsEnabled = false;
                RedoRoot1.IsEnabled = false;
                EditRoot1.IsEnabled = false;
                UnderRoot1.IsEnabled = false;
                MyMapView.MouseRightButtonDown -= mouseWheel_rightclic;
                Mouse.OverrideCursor = Cursors.Arrow;
                routewaypointoverlay.Graphics.Clear();
                routelineconfigclear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void clearPolygon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = "Are you sure you want to clear this selection tool?";
                string caption = "Confirmation";
                lstSelectedRoute.Clear();
                SelectedRoutName = "";
                if (MyMapView.SketchEditor.Geometry == null)
                {
                    MessageBox.Show("No Rectangle Geometry Present On Map");
                }
                else
                {
                    var ser = MyMapView.SketchEditor.Geometry.GeometryType.ToString();
                    if (ser == "Polygon")
                    {
                        MessageBoxButton buttons = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Question;
                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            if (ser == "Polygon" && MyMapView.SketchEditor.CancelCommand.CanExecute(null))
                            {
                                MyMapView.SketchEditor.CancelCommand.Execute(null);
                                // lvUsers.Items.Clear();
                                var systemCursor1 = System.Windows.Input.Cursors.Arrow;
                                Cursor = systemCursor1;
                                MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                                MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                            }

                            gridrouteline.Clear();
                        }
                        else
                        {

                            Mouse.OverrideCursor = Cursors.Arrow;
                            return;
                        }
                    }
                    else if (ser == "Polyline")
                    {
                        MessageBox.Show("No Rectangle Geometry Present On Map to Clear");
                        return;
                    }
                }

                routelineconfigclear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<Graphic> coordinatesystem_polyline_new(Esri.ArcGISRuntime.Geometry.Geometry geometry)
        {
            
            List<Graphic> _polylinelistGraphic = new List<Graphic>();
            Graphic _polylineGraphic = null;
            var roadPolyline = geometry as Esri.ArcGISRuntime.Geometry.Polyline;
            // var roadPolyline = graphic.Geometry as Esri.ArcGISRuntime.Geometry.Polyline;
            this.polylineBuilder = new PolylineBuilder(roadPolyline);
            foreach (var r in polylineBuilder.Parts)
            {
                IReadOnlyList<MapPoint> mapPoints = r.Points;
                // var polypoints = Mapcoordinates_Aftertransform(mapPoints);
                _polypointcollection = mapPoints;
                //HandleMapTap1(polypoints);

                //var polypoints = Mapcoordinates_Aftertransform(mapPoints);
                //_polypointcollection = polypoints;//new
                var polyline = new Esri.ArcGISRuntime.Geometry.Polyline(mapPoints);
                //Create symbol for polyline
                //  var polylineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 0, 255), 3);
                // var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Green, 3);
                var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                // polys3.MarkerPlacement = SimpleLineSymbolMarkerPlacement.End;
                // polys3.ma = getpic1();
                //polys3.MarkerStyle = SimpleLineSymbolMarkerStyle.Arrow;
                //polys3.MarkerPlacement = SimpleLineSymbolMarkerPlacement.BeginAndEnd;

                //Create a polyline graphic with geometry and symbol
                _polylineGraphic = new Graphic(polyline, polys3);
                _polylinelistGraphic.Add(_polylineGraphic);

            }
            return _polylinelistGraphic;
        }
        private List<Graphic> coordinatesystem_polygon_new(Esri.ArcGISRuntime.Geometry.Geometry geometry)//add this entire new method
        {
            List<Graphic> polylgonGraphic = new List<Graphic>();
            Graphic _polylgonGraphic = null;
            try
            {
                var poly = geometry as Esri.ArcGISRuntime.Geometry.Polygon;
                this.polygonbuild = new PolygonBuilder(poly);
                foreach (var re in polygonbuild.Parts)
                {
                    IReadOnlyList<MapPoint> mapPoints = re.Points;
                    //var polypoints = Mapcoordinates_Aftertransform(mapPoints);

                    var polygon = new Esri.ArcGISRuntime.Geometry.Polygon(mapPoints);

                    //Create symbol for polyline
                    var polylineSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Transparent,
                        new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 0, 255), 2));
                    //Create a polyline graphic with geometry and symbol

                    _polylgonGraphic = new Graphic(polygon, polylineSymbol);
                    polylgonGraphic.Add(_polylgonGraphic);
                    //_sketchOverlay.Graphics.Add(polylgonGraphic);
                    // Esri.ArcGISRuntime.Geometry.Geometry gr = polygon;

                }
                return polylgonGraphic;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return polylgonGraphic;
            }
        }
        private void btnSaveRout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RouteManager objRout = new RouteManager();
                var temproutegeom = MyMapView.SketchEditor.Geometry;//to know the geometry on map
                var routegeom = temproutegeom as Esri.ArcGISRuntime.Geometry.Polyline;
                this.polybroute = new PolylineBuilder(routegeom);

                if (polybroute.Parts.Count > 0)
                {
                    foreach (var r in polybroute.Parts)
                    {
                        routelinepointlist = r.Points;
                        routelinepoints = Mapcoordinates_Aftertransform(routelinepointlist);

                        waypointcount = r.PointCount;
                    }

                    DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objCart.GetRouteLineDetails(txtRouteName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        MessageBoxResult result = MessageBox.Show("Route Name Already Exists Do you want to Override", "Information", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (MessageBoxResult.Yes == result)
                        {

                            objMgr.DeleteRouteLineDetailsByName(txtRouteName.Text.Trim());
                            objMgr.DeleteRouteLineByName(txtRouteName.Text.Trim());

                            int result1 = objRout.InsertRoute(txtRouteName.Text, waypointcount,"1");
                            int count = 0;//to know edit import
                            List<MapPoint> list_editpts = new List<MapPoint>();
                            if (result1 > 0)
                            {
                                if (importedlinepoints.Count > 1)//to reset that exisiting imported points
                                {
                                    importedlinepoints.Clear();
                                    normalizedimportedpoints.Clear();
                                    count++;

                                }
                                foreach (var ter in routelinepointlist)
                                {

                                    double lat = ter.X;
                                    double longi = ter.Y;
                                    objRout.InsertRouteDetails(result1, lat.ToString(), longi.ToString());

                                }
                                //if (count >= 1)
                                //{
                                //    normalizedimportedpoints_edit = CalcNormalize_latest(list_editpts);
                                //}
                            }

                        }
                        if (MessageBoxResult.No == result)
                        {
                            return;
                        }
                    }
                    else
                    {
                        int result = objRout.InsertRoute(txtRouteName.Text, waypointcount,"1");
                        int count = 0;//to know edit import
                        
                        List<MapPoint> list_editpts = new List<MapPoint>();
                        if (result > 0)
                        {
                            if (importedlinepoints.Count > 1)
                            {
                                importedlinepoints.Clear();
                                normalizedimportedpoints.Clear();
                                count++;

                            }
                            foreach (var ter in routelinepointlist)
                            {
                                double lat = ter.X;
                                double longi = ter.Y;
                                objRout.InsertRouteDetails(result, lat.ToString(), longi.ToString());


                            }
                            //if (count >= 1)
                            //{
                            //    normalizedimportedpoints_edit = CalcNormalize_latest(list_editpts);
                            //}
                        }
                    }


                    stopgraphiclin.Clear();

                    var ret = MyMapView.SketchEditor.Geometry;

                    Esri.ArcGISRuntime.Geometry.Polyline poly = (Esri.ArcGISRuntime.Geometry.Polyline)GeometryEngine.NormalizeCentralMeridian(ret);


                    if (poly.Parts.Count > 1)
                    {
                        twogrp = coordinatesystem_polyline_new(poly);
                        foreach (var item in twogrp)
                        {
                            // var ge=Graphiccoordinates_Aftertransform(item);
                            // var newge = new Graphic(ge);
                            _sketchOverlay.Graphics.Add(item);
                        }
                    }
                    else
                    {
                        var routeline_withoutsymbols = coordinatesystem_polyline(poly);
                        _sketchOverlay.Graphics.Add(routeline_withoutsymbols);
                    }

                    route_symbolsadding(routelinepoints);//adding graphic point to map

                    completecommand(creationMode);

                    e.Handled = true;

                    routelineconfigclear();


                    savemenu.IsEnabled = false;
                    SaveRouteName.Visibility = Visibility.Hidden;

                    SelectedRoutName = txtRouteName.Text;
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                }
                else
                {
                    MessageBox.Show("Please draw at least two waypoints", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SelectedRoutName == "")
                {
                    DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objCart.GetRouteLine("1");
                    int cnt = dt.Rows.Count + 1;
                    string str = "SRoute_" + cnt;
                    txtRouteName.Text = str;
                }
                else
                {
                    txtRouteName.Text = SelectedRoutName;
                    
                }
                SaveRouteName.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void save_Click1(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SelectedRoutName == "")
                {
                    DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objCart.GetRouteLine("2");
                    int cnt = dt.Rows.Count + 1;
                    string str = "CRoute_" + cnt;
                    txtRouteName1.Text = str;
                }
                else
                {
                    txtRouteName1.Text = SelectedRoutName;

                }
                SaveRouteName1.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Graphic TempRoute = null;
        public string SelectedRoutName = "";
        Graphic _countryBorderPolylineGraphic = null;
        Esri.ArcGISRuntime.Geometry.Geometry geom = null;
        List<MapPoint> loadpoints = new List<MapPoint>();
        private void RouteGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                EditRoot1.IsEnabled = true;
                routewaypointoverlay.Graphics.Clear();
                _sketchOverlay.Graphics.Clear();
                loadpoints.Clear();
                gridrouteline.Clear();
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    //_polypointrouteOverlay.Graphics.Clear();
                    //_sketchRouteOverlay1.Graphics.Clear();
                    string aa = (RouteGrid.SelectedItem as DataRowView).Row["RouteName"].ToString();
                    SelectedRoutName = aa;
                    lstSelectedRoute.Add(aa);
                    DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objCart.GetRouteLineDetails(aa);
                    Esri.ArcGISRuntime.Geometry.PointCollection borderCountryPointCollection = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
                    foreach (DataRow row1 in dt.Rows)
                    {
                        polist.latitude = Convert.ToDouble(row1["Latitude"]);
                        polist.longitude = Convert.ToDouble(row1["Longitude"]);

                        MapPoint mp = new MapPoint(polist.latitude, polist.longitude, SpatialReferences.WebMercator);

                        borderCountryPointCollection.Add(mp);
                        loadpoints.Add(mp);

                    }
                    geom = loadrouteline_create(borderCountryPointCollection);
                    var item = coordinatesystem_polyline_new(geom);
                    if (item.Count > 1)
                    {
                        foreach (var item1 in item)
                        {
                            // var set = Graphiccoordinates_Aftertransform(item1);
                            _sketchOverlay.Graphics.Add(item1);
                            gridrouteline.Add(item1);
                        }
                    }
                    else
                    {
                        routeline = coordinatesystem_polyline(geom);//change this variable as "routeline"---var set1-->routeline
                        _sketchOverlay.Graphics.Add(routeline);
                    }
                    route_symbolsadding(loadpoints);
                    // route_symbolsadding(borderCountryPointCollection);
                    TempRoute = _countryBorderPolylineGraphic;
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Esri.ArcGISRuntime.Geometry.Geometry loadrouteline_create(Esri.ArcGISRuntime.Geometry.PointCollection PointCollection)//add this method
        {
            var polyline = new Esri.ArcGISRuntime.Geometry.Polyline(PointCollection);
            Esri.ArcGISRuntime.Geometry.Geometry geom = polyline;
            Esri.ArcGISRuntime.Geometry.Polyline poly = (Esri.ArcGISRuntime.Geometry.Polyline)GeometryEngine.NormalizeCentralMeridian(geom);
            Esri.ArcGISRuntime.Geometry.Geometry geom1 = (Esri.ArcGISRuntime.Geometry.Geometry)GeometryEngine.NormalizeCentralMeridian(geom);
            return geom1;

        }
        private void load_Click(object sender, RoutedEventArgs e)
        {
            BindRouteLine("1");
            ShowHideMenu("sbShowRightRoute", btnLeftMenuShow, pnlRightRoute);
        }

        private void load_Click1(object sender, RoutedEventArgs e)
        {
            BindRouteLine("2");
            ShowHideMenu("sbShowRightRoute", btnLeftMenuShow, pnlRightRoute1);
        }

        private void zoomIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = MyMapView.MapScale;
                string oldvalue = Convert.ToString(x);
                ScaleItems.Add(oldvalue);
                x = x * 0.5;
                string sNewScale = Convert.ToString(x);
                var r = SetScale(x);
                if (r == true)
                {
                    var se = MyMapView.SetViewpointScaleAsync(Convert.ToDouble(sNewScale));
                }
                scalrangelable();
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                var list = new[] { 62500, 125000, 250000, 500000, 1000000, 2000000, 4000000, 8000000, 16000000, 32000000, 64000000, 128000000 };
                var input = tes.TargetScale;
                var diffList = from number in list
                               select new
                               {
                                   number,
                                   difference = Math.Abs(number - input)
                               };
                var result = (from diffItem in diffList
                              orderby diffItem.difference
                              select diffItem).First().number;

                double ImageScale = result;

                if (Maincat == "AVCS Chart")
                {

                    if (ImageScale > 62500 && ImageScale <= 125000)
                    {
                        if (crew.Contains("Berthing"))
                        {
                            berthingoverlay.IsVisible = true;

                        }
                        else
                        {
                            berthingoverlay.ClearSelection();
                            berthingoverlay.IsVisible = false;
                        }
                    }
                    else if (ImageScale > 125000 && ImageScale <= 500000)
                    {
                        if (crew.Contains("Harbour"))
                        {
                            harbouroverlay.IsVisible = true;

                        }
                        else
                        {
                            harbouroverlay.ClearSelection();
                            harbouroverlay.IsVisible = false;
                        }

                    }
                    else if (tes.TargetScale > 500000 && tes.TargetScale <= 2000000)
                    {
                        if (crew.Contains("Approach"))
                        {
                            approachoverlay.IsVisible = true;

                        }
                        else
                        {
                            approachoverlay.ClearSelection();
                            approachoverlay.IsVisible = false;
                        }
                    }
                    else if (ImageScale > 2000000 && ImageScale <= 8000000)
                    {
                        if (crew.Contains("Coastal"))
                        {
                            coastaloverlay.IsVisible = true;

                        }
                        else
                        {
                            coastaloverlay.ClearSelection();
                            coastaloverlay.IsVisible = false;
                        }
                    }
                    else if (ImageScale > 8000000 && ImageScale <= 32000000)
                    {
                        if (crew.Contains("General"))
                        {
                            generaloverlay.IsVisible = true;
                        }
                        else
                        {
                            generaloverlay.ClearSelection();
                            generaloverlay.IsVisible = false;
                        }
                    }
                    else if (ImageScale > 32000000 && ImageScale <= 128000000)
                    {
                        if (crew.Contains("Overview"))
                        {
                            overviewoverlay.IsVisible = true;
                        }
                        else
                        {
                            overviewoverlay.ClearSelection();
                            overviewoverlay.IsVisible = false;
                        }
                    }
                }
                if (Maincat == "AVCS Folio")
                {
                    if (ImageScale > 8000000 && ImageScale <= 32000000)
                    {
                        if (crew.Contains("Port"))
                        {
                            port_Graphic.IsVisible = true;
                        }
                        else
                        {
                            port_Graphic.ClearSelection();
                            port_Graphic.IsVisible = false;
                        }

                    }
                    else if (ImageScale > 32000000 && ImageScale <= 128000000)
                    {
                        if (crew.Contains("Regional"))
                        {
                            region_Graphic.IsVisible = true;
                        }
                        else
                        {
                            region_Graphic.ClearSelection();
                            region_Graphic.IsVisible = false;
                        }

                        if (crew.Contains("Transit"))
                        {
                            transit_Graphic.IsVisible = true;
                        }
                        else
                        {
                            transit_Graphic.ClearSelection();
                            transit_Graphic.IsVisible = false;
                        }
                    }
                }
                if (Maincat == "Digital List of Radio Signals")
                {

                    if (crew.Contains("ADRS2"))
                    {
                        ADRS1345_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS1345_Graphic.ClearSelection();
                        ADRS1345_Graphic.IsVisible = false;
                    }


                    if (crew.Contains("ADRS1345"))
                    {
                        ADRS2_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS2_Graphic.ClearSelection();
                        ADRS2_Graphic.IsVisible = false;
                    }

                    if (crew.Contains("ADRS6"))
                    {
                        ADRS6_Graphic.IsVisible = true;
                    }
                    else
                    {
                        ADRS6_Graphic.ClearSelection();
                        ADRS6_Graphic.IsVisible = false;
                    }
                }
                if (Maincat == "Digital List of Lights")
                {
                    lol_Graphic.IsVisible = true;
                }
                else
                {
                    lol_Graphic.ClearSelection();
                    lol_Graphic.IsVisible = false;
                }
                if (Maincat == "e-NP Sailing Direction")
                {
                    enpsd_Graphic.IsVisible = true;
                }
                else
                {
                    enpsd_Graphic.ClearSelection();
                    enpsd_Graphic.IsVisible = false;
                }
                if (Maincat == "TotalTide")
                {
                    totaltide_Graphic.IsVisible = true;
                }
                else
                {
                    totaltide_Graphic.ClearSelection();
                    totaltide_Graphic.IsVisible = false;
                }
                ScaleItems.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void zoomout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = MyMapView.MapScale;
                string oldvalue = Convert.ToString(x);
                ScaleItems.Add(oldvalue);
                x = x / 0.5;
                string sNewScale = Convert.ToString(x);
                var r = SetScale(x);
                if (r == true)
                {
                    var se = MyMapView.SetViewpointScaleAsync(Convert.ToDouble(sNewScale));
                }
                scalrangelable();

                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                var list = new[] { 62500, 125000, 250000, 500000, 1000000, 2000000, 4000000, 8000000, 16000000, 32000000, 64000000, 128000000 };
                var input = tes.TargetScale;
                var diffList = from number in list
                               select new
                               {
                                   number,
                                   difference = Math.Abs(number - input)
                               };
                var result = (from diffItem in diffList
                              orderby diffItem.difference
                              select diffItem).First().number;

                double ImageScale = result;

                ScaleItems.Clear();
                if (Maincat == "AVCS Chart")
                {
                    if (ImageScale > 62500 && ImageScale <= 125000)
                    {
                        if (crew.Contains("Berthing"))
                        {
                            berthingoverlay.IsVisible = false;
                        }
                        else
                        {
                            berthingoverlay.ClearSelection();
                        }
                        // berthinglabeloverlay.IsVisible = false;

                    }
                    else if (ImageScale > 125000 && ImageScale <= 500000)
                    {
                        if (crew.Contains("Harbour"))
                        {
                            harbouroverlay.IsVisible = false;
                        }
                        else
                        {
                            harbouroverlay.ClearSelection();
                        }
                        // harbourlabeloverlay.IsVisible = false;


                    }
                    else if (ImageScale > 500000 && ImageScale <= 2000000)
                    {
                        if (crew.Contains("Approach"))
                        {
                            approachoverlay.IsVisible = false;

                        }
                        else
                        {
                            approachoverlay.ClearSelection();
                        }
                        // approachlabeloverlay.IsVisible = false;

                    }
                    else if (ImageScale > 2000000 && ImageScale <= 8000000)
                    {
                        if (crew.Contains("Coastal"))
                        {
                            coastaloverlay.IsVisible = false;
                        }
                        else
                        {
                            coastaloverlay.ClearSelection();
                        }
                        //coastallabeloverlay.IsVisible = false;

                    }
                    else if (ImageScale > 8000000 && ImageScale <= 32000000)
                    {
                        if (crew.Contains("General"))
                        {
                            generaloverlay.IsVisible = true;
                            coastaloverlay.IsVisible = false;
                            approachoverlay.IsVisible = false;
                            berthingoverlay.IsVisible = false;
                            harbouroverlay.IsVisible = false;

                        }
                        else
                        {
                            generaloverlay.ClearSelection();
                        }

                        //generallabeloverlay.IsVisible = false;

                    }
                    else if (tes.TargetScale > 32000000 && tes.TargetScale <= 128000000)
                    {
                        if (crew.Contains("Overview"))
                        {
                            overviewoverlay.IsVisible = true;
                            generaloverlay.IsVisible = false;
                            coastaloverlay.IsVisible = false;
                            approachoverlay.IsVisible = false;
                            berthingoverlay.IsVisible = false;
                            harbouroverlay.IsVisible = false;
                        }
                        else
                        {
                            overviewoverlay.ClearSelection();
                        }
                    }

                }
                if (Maincat == "AVCS Folio")
                {
                    if (ImageScale > 8000000 && ImageScale <= 32000000)
                    {
                        if (crew.Contains("Port"))
                        {
                            port_Graphic.IsVisible = false;
                        }
                        else
                        {
                            port_Graphic.ClearSelection();
                        }

                    }
                    else if (ImageScale > 32000000 && ImageScale <= 128000000)
                    {
                        if (crew.Contains("Regional"))
                        {
                            region_Graphic.IsVisible = true;
                        }
                        else
                        {
                            region_Graphic.ClearSelection();
                        }

                        if (crew.Contains("Transit"))
                        {
                            transit_Graphic.IsVisible = true;
                        }
                        else
                        {
                            transit_Graphic.ClearSelection();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public bool SetScale(double dNewScale)
        {
            try
            {
                if (double.IsNaN(dNewScale))
                    return false;
                if (dNewScale <= 0)
                    return false;
                string sNewScale = "1:" + dNewScale.ToString("N0");
                string sOldScale = ScaleItems[0].ToString();
                if (sNewScale == sOldScale)
                    return false;
                ScaleItems[0] = sNewScale;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void PanRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

                var pEnvelope = MyMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
                this.panenvelope = new EnvelopeBuilder(pEnvelope.Extent);
                if (MyMapView.IsWrapAroundEnabled)
                {
                    pEnvelope = GeometryEngine.NormalizeCentralMeridian(pEnvelope);
                }

                //Create a point to pan to
                MapPointBuilder pPoint = new MapPointBuilder(SpatialReferences.WebMercator);

                // pPoint = new MapPoint;
                pPoint.X = ((pEnvelope.Extent.XMin + pEnvelope.Extent.XMax) / 2) + (pEnvelope.Extent.Height / (100 / GetPanFactor()));
                pPoint.Y = ((pEnvelope.Extent.YMin + pEnvelope.Extent.YMax) / 2);

                MapPoint afetrPoint = new MapPoint(pPoint.X, pPoint.Y);
                Envelope afterEnv = new Envelope(afetrPoint, pEnvelope.Extent.Width, pEnvelope.Extent.Height);

                //Center the envelope on the point
                MyMapView.SetViewpointGeometryAsync(afterEnv);
                MyMapView.SetViewpointCenterAsync(afetrPoint, tes.TargetScale);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PanLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
                var pEnvelope = MyMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
                this.panenvelope = new EnvelopeBuilder(pEnvelope.Extent);
                if (MyMapView.IsWrapAroundEnabled)
                {
                    pEnvelope = GeometryEngine.NormalizeCentralMeridian(pEnvelope);
                }

                //Create a point to pan to
                MapPointBuilder pPoint = new MapPointBuilder(SpatialReferences.WebMercator);

                // pPoint = new MapPoint;
                pPoint.X = ((pEnvelope.Extent.XMin + pEnvelope.Extent.XMax) / 2) - (pEnvelope.Extent.Height / (100 / GetPanFactor()));
                pPoint.Y = ((pEnvelope.Extent.YMin + pEnvelope.Extent.YMax) / 2);

                MapPoint afterPoint = new MapPoint(pPoint.X, pPoint.Y);
                Envelope afterEnv = new Envelope(afterPoint, pEnvelope.Extent.Width, pEnvelope.Extent.Height);
                MyMapView.SetViewpointGeometryAsync(afterEnv);
                MyMapView.SetViewpointCenterAsync(afterPoint, tes.TargetScale);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private long GetPanFactor()
        {
            return 50;
        }
        private void PanUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

                var pEnvelope = MyMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
                this.panenvelope = new EnvelopeBuilder(pEnvelope.Extent);
                if (MyMapView.IsWrapAroundEnabled)
                {
                    pEnvelope = GeometryEngine.NormalizeCentralMeridian(pEnvelope);
                }

                //Create a point to pan to
                MapPointBuilder pPoint = new MapPointBuilder(SpatialReferences.WebMercator);
                pPoint.X = (pEnvelope.Extent.XMin + pEnvelope.Extent.XMax) / 2;
                pPoint.Y = ((pEnvelope.Extent.YMin + pEnvelope.Extent.YMax) / 2) + (pEnvelope.Extent.Height / (100 / GetPanFactor()));
                MapPoint afetrPoint = new MapPoint(pPoint.X, pPoint.Y);
                Envelope afterEnv = new Envelope(afetrPoint, pEnvelope.Extent.Width, pEnvelope.Extent.Height);

                //Center the envelope on the point
                MyMapView.SetViewpointGeometryAsync(afterEnv);
                MyMapView.SetViewpointCenterAsync(afetrPoint, tes.TargetScale);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PanDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tes = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

                var pEnvelope = MyMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
                this.panenvelope = new EnvelopeBuilder(pEnvelope.Extent);
                if (MyMapView.IsWrapAroundEnabled)
                {
                    pEnvelope = GeometryEngine.NormalizeCentralMeridian(pEnvelope);
                }

                //Create a point to pan to
                MapPointBuilder pPoint = new MapPointBuilder(SpatialReferences.WebMercator);
                pPoint.X = (pEnvelope.Extent.XMin + pEnvelope.Extent.XMax) / 2;
                pPoint.Y = ((pEnvelope.Extent.YMin + pEnvelope.Extent.YMax) / 2) - (pEnvelope.Extent.Height / (100 / GetPanFactor()));
                MapPoint afetrPoint = new MapPoint(pPoint.X, pPoint.Y);
                Envelope afterEnv = new Envelope(afetrPoint, pEnvelope.Extent.Width, pEnvelope.Extent.Height);

                //Center the envelope on the point
                MyMapView.SetViewpointGeometryAsync(afterEnv);
                MyMapView.SetViewpointCenterAsync(afetrPoint, tes.TargetScale);

                var list = new[] { 10000, 62500, 125000, 250000, 500000, 1000000, 2000000, 4000000, 8000000, 16000000, 32000000, 64000000, 128000000 };
                var input = tes.TargetScale;
                var diffList = from number in list
                               select new
                               {
                                   number,
                                   difference = Math.Abs(number - input)
                               };
                var result = (from diffItem in diffList
                              orderby diffItem.difference
                              select diffItem).First().number;

                double ImageScale = result;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (avcs_tab.IsSelected)
                {

                    if (avc)
                    {
                        var result = objMgr.DeleteShoppingCartByAVCSProdut("AVCS Products");
                    }
                    else
                    {
                        foreach (string eno in lstSelectedEmpNo)
                        {
                            var result = objMgr.DeleteShoppingCartByAVCS(eno);

                            foreach (var val in cart1)
                            {
                                if (val.FProductId == eno)
                                {
                                    val.Exists = "NotExists";
                                }
                            }

                            foreach (var val in cart)
                            {
                                if (val.ShortName == eno)
                                {
                                    val.Exists = "NotExists";
                                }
                            }
                        }
                    }
                }
                if (adll_tab.IsSelected)
                {
                    foreach (string eno in lstSelectedEmpNo)
                    {
                        var result = objMgr.DeleteShoppingCartByProduct("ADLL", eno);

                        foreach (var val in cart1)
                        {
                            if (val.FProductId == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }

                        foreach (var val in cart)
                        {
                            if (val.ShortName == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }
                    }
                }
                if (adrs_tab.IsSelected)
                {
                    foreach (string eno in lstSelectedEmpNo)
                    {
                        var result = objMgr.DeleteShoppingCartByProduct("ADRS", eno);

                        foreach (var val in cart1)
                        {
                            if (val.FProductId == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }

                        foreach (var val in cart)
                        {
                            if (val.ShortName == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }
                    }
                }
                if (enp_tab.IsSelected)
                {
                    foreach (string eno in lstSelectedEmpNo)
                    {
                        var result = objMgr.DeleteShoppingCartByProduct("e-Nautical Publications", eno);

                        foreach (var val in cart1)
                        {
                            if (val.FProductId == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }

                        foreach (var val in cart)
                        {
                            if (val.ShortName == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }
                    }
                }
                if (misc_tab.IsSelected)
                {
                    foreach (string eno in lstSelectedEmpNo)
                    {
                        var result = objMgr.DeleteShoppingCartByProduct("Misc Publications", eno);

                        foreach (var val in cart1)
                        {
                            if (val.FProductId == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }

                        foreach (var val in cart)
                        {
                            if (val.ShortName == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }
                    }
                }
                if (tides_tab.IsSelected)
                {
                    foreach (string eno in lstSelectedEmpNo)
                    {
                        var result = objMgr.DeleteShoppingCartByProduct("TotalTide", eno);

                        foreach (var val in cart1)
                        {
                            if (val.FProductId == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }

                        foreach (var val in cart)
                        {
                            if (val.ShortName == eno)
                            {
                                val.Exists = "NotExists";
                            }
                        }
                    }
                }
                lstSelectedEmpNo.Clear();

                Bindcheckpout();
                RemoveHeilight();
                dataGrid2.ItemsSource = null;
                dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id); ;
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkoutGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.RemovedItems.Count == 0)
                {
                    DataGridRow row = (DataGridRow)checkoutGrid.ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]);

                    if (row != null)
                    {
                        string aa = (checkoutGrid.SelectedItem as DataRowView).Row["UnitId"].ToString();
                        lstSelectedEmpNo.Add(aa);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public bool avc = false;
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (avcs_tab.IsSelected)
                {
                    avc = true;
                    checkoutGrid.SelectAll();
                    lstSelectedEmpNo.Clear();

                    var grid = checkoutGrid;
                    var selected = grid.SelectedItems;

                    foreach (var item in selected)
                    {
                        var dog = item as DataRowView;
                        lstSelectedEmpNo.Add(dog["UnitId"].ToString());
                    }
                }
                if (adll_tab.IsSelected)
                {
                    checkoutGrid1.SelectAll();
                    lstSelectedEmpNo.Clear();

                    var grid = checkoutGrid1;
                    var selected = grid.SelectedItems;

                    foreach (var item in selected)
                    {
                        var dog = item as DataRowView;
                        lstSelectedEmpNo.Add(dog["ShortName"].ToString());
                    }
                }
                if (adrs_tab.IsSelected)
                {
                    checkoutGrid2.SelectAll();
                    lstSelectedEmpNo.Clear();

                    var grid = checkoutGrid2;
                    var selected = grid.SelectedItems;

                    foreach (var item in selected)
                    {
                        var dog = item as DataRowView;
                        lstSelectedEmpNo.Add(dog["ShortName"].ToString());
                    }
                }
                if (enp_tab.IsSelected)
                {
                    checkoutGrid3.SelectAll();
                    lstSelectedEmpNo.Clear();

                    var grid = checkoutGrid3;
                    var selected = grid.SelectedItems;

                    foreach (var item in selected)
                    {
                        var dog = item as DataRowView;
                        lstSelectedEmpNo.Add(dog["ShortName"].ToString());
                    }
                }
                if (misc_tab.IsSelected)
                {
                    checkoutGrid4.SelectAll();
                    lstSelectedEmpNo.Clear();

                    var grid = checkoutGrid4;
                    var selected = grid.SelectedItems;

                    foreach (var item in selected)
                    {
                        var dog = item as DataRowView;
                        lstSelectedEmpNo.Add(dog["ShortName"].ToString());
                    }
                }
                if (tides_tab.IsSelected)
                {
                    checkoutGrid5.SelectAll();
                    lstSelectedEmpNo.Clear();

                    var grid = checkoutGrid5;
                    var selected = grid.SelectedItems;

                    foreach (var item in selected)
                    {
                        var dog = item as DataRowView;
                        lstSelectedEmpNo.Add(dog["ShortName"].ToString());
                    }
                }
            }
            catch
            {

            }
        }
        public static FrameworkElement GetTemplateChildByName(DependencyObject parent, string name)
        {
            int childnum = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childnum; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement &&

                        ((FrameworkElement)child).Name == name)
                {
                    return child as FrameworkElement;
                }
                else
                {
                    var s = GetTemplateChildByName(child, name);
                    if (s != null)
                        return s;
                }
            }
            return null;
        }
        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = false;
                string filename = "";
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".bsk";
                dlg.Filter = "Basket(.bsk)|*.bsk";
                if (dlg.ShowDialog() == true)
                {
                    filename = dlg.FileName;
                }
                LoadingAdorner.IsAdornerVisible = true;
                await Task.Delay(2000);

                DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                DataTable dt = new DataTable();
                dt = objCart.GetShoppingCart();

                DataTable dt1 = new DataTable();
                dt1 = objCart.GetShoppingCartByAVCS();

                List<DataRow> listRowsToDelete = new List<DataRow>();
                List<GraphicsOverlay> grc = new List<GraphicsOverlay>();

                foreach (var item in MyMapView.GraphicsOverlays)
                {
                    if (item.Id == "7" || item.Id == "8" || item.Id == "9")
                    {
                        grc.Add(item);
                    }
                }
                string sa = "";
                foreach (DataRow item1 in dt1.Rows)
                {
                    sa += item1["ShortName"].ToString() + ",";

                }

                foreach (var item in grc.SelectMany(i => i.Graphics))
                {
                    if (sa.Contains(item.Attributes["ShortName"].ToString()))
                    {
                        XElement xele = XElement.Parse(item.Attributes["SubUnit"].ToString());
                        var dddd = xele.Descendants();
                        string sd = "";
                        foreach (var item1 in dddd)
                        {
                            sd += item1.Value + ",";

                        }
                        foreach (DataRow row in dt1.Rows)
                        {
                            string shname = row["ShortName"].ToString();
                            if (sd.Contains(shname))
                            {
                                listRowsToDelete.Add(row);
                                result = true;
                            }
                        }
                    }
                }

                foreach (DataRow drowToDelete in listRowsToDelete.Distinct())
                {
                    if (sa.Contains(drowToDelete["ShortName"].ToString()))
                    {
                        dt1.Rows.Remove(drowToDelete);
                        dt1.AcceptChanges();
                    }
                }

                DataTable chkdt = new DataTable();
                chkdt = objCart.GetShoppingCartGroupBy();

                if (result)
                {
                    dt.Merge(dt1);
                }
                else
                {
                    dt.Merge(chkdt);
                }

                await ExportDataTabletoFile(dt, "\t", false, filename);
                LoadingAdorner.IsAdornerVisible = false;
            }
            catch
            {
                LoadingAdorner.IsAdornerVisible = false;
            }
        }
        public async Task<string> ExportDataTabletoFile(DataTable datatable, string delimited, bool exportcolumnsheader, string file)
        {
            await Task.Delay(2000);
            if (file != "")
            {
                StreamWriter str = new StreamWriter(file, false, System.Text.Encoding.Default);

                foreach (DataRow datarow in datatable.Rows)
                {
                    string row = "0" + delimited + datarow["ProductName"].ToString() + delimited + datarow["ShortName"].ToString() + delimited + datarow["Title"].ToString() + " ";
                    str.WriteLine(row.Remove(row.Length - 1, 1));
                }
                str.Flush();
                str.Close();

                return "1";
            }
            else
            {
                return "2";
            }

        }
        private void datagridmisc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string producyType = "";
                if (Maincat == "Miscellaneous")
                {
                    producyType = "Misc Publications";
                }
                if (Maincat == "e-NP Other")
                {
                    producyType = "e-Nautical Publications";
                }

                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    string aa = (datagridmisc.SelectedItem as DataRowView).Row["ProductName"].ToString();
                    string aab = (datagridmisc.SelectedItem as DataRowView).Row["DatasetTitle"].ToString();
                    bool alreadyExists = cart.Any(x => x.ShortName == aa);
                    if (!alreadyExists)
                    {
                        string exist = "";

                        DataAccessLayer.CartRepository objCart = new DataAccessLayer.CartRepository();
                        DataTable dt = new DataTable();
                        dt = objCart.CheckShoppingCartByName(aa, producyType);
                        if (dt.Rows.Count > 0)
                        {
                            exist = "Exists";
                        }
                        else
                        {
                            exist = "NotExists";
                        }

                        cart.Add(new ShoppingCart()
                        {
                            Exists = exist,
                            ShortName = aa,
                            Title = aab,
                            //Scale = val.Element("Metadata").Element("Scale").Value,
                            Status = "ForSale",
                            //Usage = objcont.Band(val.Element("Metadata").Element("Usage").Value),
                            //UnitId = val.Element("Metadata").Element("Unit").Element("ID").Value,
                            ProductName = producyType
                        }); ;

                    }

                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                }
            }
            catch
            {

            }
        }
        private void strsearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            SearchGrid.ItemsSource = null;
            Nodata.Visibility = Visibility.Hidden;
            ShowHideMenu("sbShowRightRoute", btnLeftMenuShow, pnlRightSearch);

        }
        private void btnsearh_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (Maincat == "")
                {
                    MessageBox.Show("Please Selct the Product Category in Product Filter", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                RemoveHeilight();
                List<ShoppingCart> searchgrid = new List<ShoppingCart>();
                List<GraphicsOverlay> grc = new List<GraphicsOverlay>();




                if (Maincat == "AVCS Chart")
                {
                    string serch = txtSearch.Text.ToLower();
                    string s2 = "1,2,3,4,5,6";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                        {
                            if (selectedval.Contains(Convert.ToInt32(item.Id)))
                            {
                                grc.Add(item);
                            }
                        }
                    }

                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString().ToLower().Contains(serch) || item.Attributes["Title"].ToString().ToLower().Contains(serch))
                            {
                                //SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                                searchgrid.Add(new ShoppingCart()
                                {
                                    ShortName = item.Attributes["ShortName"].ToString(),
                                    Title = item.Attributes["Title"].ToString(),
                                    Usage = item.Attributes["Usage"].ToString()
                                });
                            }
                        }
                    }
                    if (searchgrid.Count() > 0)
                    {
                        SearchGrid.Visibility = Visibility.Visible;
                        SearchGrid.ItemsSource = null;
                        SearchGrid.ItemsSource = searchgrid.OrderBy(f => f.Usage);
                    }
                    else
                    {
                        SearchGrid.Visibility = Visibility.Hidden;
                        Nodata.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "AVCS Folio")
                {
                    string serch = txtSearch.Text.ToLower();
                    string s2 = "7,8,9";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                        {
                            if (selectedval.Contains(Convert.ToInt32(item.Id)))
                            {
                                grc.Add(item);
                            }
                        }
                    }
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString().ToLower().Contains(serch) || item.Attributes["Title"].ToString().ToLower().Contains(serch))
                            {
                                //SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                                searchgrid.Add(new ShoppingCart()
                                {
                                    ShortName = item.Attributes["ShortName"].ToString(),
                                    Title = item.Attributes["Title"].ToString(),
                                    Usage = item.Attributes["Usage"].ToString()
                                });
                            }
                        }
                    }
                    if (searchgrid.Count() > 0)
                    {
                        SearchGrid.Visibility = Visibility.Visible;
                        Nodata.Visibility = Visibility.Hidden;
                        SearchGrid.ItemsSource = null;
                        SearchGrid.ItemsSource = searchgrid.OrderBy(f => f.Usage);
                    }
                    else
                    {
                        SearchGrid.Visibility = Visibility.Hidden;
                        Nodata.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "Digital List of Lights")
                {
                    string serch = txtSearch.Text.ToLower();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                        {
                            if (item.Id == "13")
                            {
                                grc.Add(item);
                            }
                        }
                    }
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString().ToLower().Contains(serch) || item.Attributes["Title"].ToString().ToLower().Contains(serch))
                            {
                                //SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                                searchgrid.Add(new ShoppingCart()
                                {
                                    ShortName = item.Attributes["ShortName"].ToString(),
                                    Title = item.Attributes["Title"].ToString(),
                                    Usage = item.Attributes["Usage"].ToString()
                                });
                            }
                        }
                    }
                    if (searchgrid.Count() > 0)
                    {
                        SearchGrid.Visibility = Visibility.Visible;
                        Nodata.Visibility = Visibility.Hidden;
                        SearchGrid.ItemsSource = null;
                        SearchGrid.ItemsSource = searchgrid.OrderBy(f => f.Usage);
                    }
                    else
                    {
                        SearchGrid.Visibility = Visibility.Hidden;
                        Nodata.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "Digital List of Radio Signals")
                {
                    string serch = txtSearch.Text.ToLower();
                    string s2 = "10,11,12";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                        {
                            if (selectedval.Contains(Convert.ToInt32(item.Id)))
                            {
                                grc.Add(item);
                            }
                        }
                    }
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString().ToLower().Contains(serch) || item.Attributes["Title"].ToString().ToLower().Contains(serch))
                            {
                                //SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                                searchgrid.Add(new ShoppingCart()
                                {
                                    ShortName = item.Attributes["ShortName"].ToString(),
                                    Title = item.Attributes["Title"].ToString(),
                                    Usage = item.Attributes["Usage"].ToString()
                                });
                            }
                        }
                    }
                    if (searchgrid.Count() > 0)
                    {
                        SearchGrid.Visibility = Visibility.Visible;
                        Nodata.Visibility = Visibility.Hidden;
                        SearchGrid.ItemsSource = null;
                        SearchGrid.ItemsSource = searchgrid.OrderBy(f => f.Usage);
                    }
                    else
                    {
                        SearchGrid.Visibility = Visibility.Hidden;
                        Nodata.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "e-NP Sailing Direction")
                {
                    string serch = txtSearch.Text.ToLower();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                        {
                            if (item.Id == "14")
                            {
                                grc.Add(item);
                            }
                        }
                    }
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString().ToLower().Contains(serch) || item.Attributes["Title"].ToString().ToLower().Contains(serch))
                            {
                                //SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                                searchgrid.Add(new ShoppingCart()
                                {
                                    ShortName = item.Attributes["ShortName"].ToString(),
                                    Title = item.Attributes["Title"].ToString(),
                                    Usage = item.Attributes["Usage"].ToString()
                                });
                            }
                        }
                    }
                    if (searchgrid.Count() > 0)
                    {
                        SearchGrid.Visibility = Visibility.Visible;
                        Nodata.Visibility = Visibility.Hidden;
                        SearchGrid.ItemsSource = null;
                        SearchGrid.ItemsSource = searchgrid.OrderBy(f => f.Usage);
                    }
                    else
                    {
                        SearchGrid.Visibility = Visibility.Hidden;
                        Nodata.Visibility = Visibility.Visible;
                    }
                }
                if (Maincat == "TotalTide")
                {
                    string serch = txtSearch.Text.ToLower();
                    foreach (var item in MyMapView.GraphicsOverlays)
                    {
                        if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                        {
                            if (item.Id == "15")
                            {
                                grc.Add(item);
                            }
                        }
                    }
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString().ToLower().Contains(serch) || item.Attributes["Title"].ToString().ToLower().Contains(serch))
                            {
                                //SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                                searchgrid.Add(new ShoppingCart()
                                {
                                    ShortName = item.Attributes["ShortName"].ToString(),
                                    Title = item.Attributes["Title"].ToString(),
                                    Usage = item.Attributes["Usage"].ToString()
                                });
                            }
                        }
                    }
                    if (searchgrid.Count() > 0)
                    {
                        SearchGrid.Visibility = Visibility.Visible;
                        Nodata.Visibility = Visibility.Hidden;
                        SearchGrid.ItemsSource = null;
                        SearchGrid.ItemsSource = searchgrid.OrderBy(f => f.Usage);
                        
                    }
                    else
                    {
                        SearchGrid.Visibility = Visibility.Hidden;
                        Nodata.Visibility = Visibility.Visible;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void ZoomToLocation(Graphic graphic, double scale)
        {
            MyMapView.SetViewpointGeometryAsync(graphic.Geometry);
            MyMapView.SetViewpointScaleAsync(scale);
        }
        private void searchhide_Click(object sender, RoutedEventArgs e)
        {
            searchpnl.Visibility = Visibility.Hidden;
            Nodata.Visibility = Visibility.Hidden;
        }
        private void checkoutGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    string aa = (checkoutGrid1.SelectedItem as DataRowView).Row["ShortName"].ToString();
                    lstSelectedEmpNo.Add(aa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void checkoutGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    string aa = (checkoutGrid2.SelectedItem as DataRowView).Row["ShortName"].ToString();
                    lstSelectedEmpNo.Add(aa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void checkoutGrid3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    string aa = (checkoutGrid3.SelectedItem as DataRowView).Row["ShortName"].ToString();
                    lstSelectedEmpNo.Add(aa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void checkoutGrid4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    string aa = (checkoutGrid4.SelectedItem as DataRowView).Row["ShortName"].ToString();
                    lstSelectedEmpNo.Add(aa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void checkoutGrid5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    string aa = (checkoutGrid5.SelectedItem as DataRowView).Row["ShortName"].ToString();
                    lstSelectedEmpNo.Add(aa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RouteRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string aa = (RouteGrid.SelectedItem as DataRowView).Row["RouteName"].ToString();
                objMgr.DeleteRouteLineDetailsByName(aa);
                var result = objMgr.DeleteRouteLineByName(aa);
                //foreach (string eno in lstSelectedRoute)
                //{
                //    objMgr.DeleteRouteLineDetailsByName(eno);
                //    var result = objMgr.DeleteRouteLineByName(eno);

                //}
                //lstSelectedRoute.Clear();

                BindRouteLine("1");


                //_sketchOverlay.Graphics.Clear();
                //_sketchPolylineOverlay.Graphics.Clear();
                //_sketchRouteOverlay.Graphics.Clear();
                //_sketchRouteOverlay1.Graphics.Clear();
                //_polypointrouteOverlay.Graphics.Clear();

                routelineconfigclear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RouteRemove_Click1(object sender, RoutedEventArgs e)
        {
            try
            {

                string aa = (RouteGrid1.SelectedItem as DataRowView).Row["RouteName"].ToString();
                objMgr.DeleteRouteLineDetailsByName(aa);
                var result = objMgr.DeleteRouteLineByName(aa);
                //foreach (string eno in lstSelectedRoute)
                //{
                //    objMgr.DeleteRouteLineDetailsByName(eno);
                //    var result = objMgr.DeleteRouteLineByName(eno);

                //}
                //lstSelectedRoute.Clear();

                BindRouteLine("2");


                //_sketchOverlay.Graphics.Clear();
                //_sketchPolylineOverlay.Graphics.Clear();
                //_sketchRouteOverlay.Graphics.Clear();
                //_sketchRouteOverlay1.Graphics.Clear();
                //_polypointrouteOverlay.Graphics.Clear();

                routelineconfigclear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveHeilight();
                ShoppingCart row = (ShoppingCart)((Button)e.Source).DataContext;

                List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                foreach (var item in MyMapView.GraphicsOverlays)
                {
                    if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                    {
                        grc.Add(item);
                    }
                }
                string serch = row.ShortName.ToString();

                if (Maincat == "AVCS Chart")
                {
                    string s2 = "1,2,3,4,5,6";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && selectedval.Contains(Convert.ToInt32(item.Attributes["Usage"].ToString())))
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "AVCS Folio")
                {
                    string s2 = "7,8,9";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && selectedval.Contains(Convert.ToInt32(item.Attributes["Usage"].ToString())))
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "Digital List of Lights")
                {
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && Convert.ToInt32(item.Attributes["Usage"].ToString()) == 13)
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "Digital List of Radio Signals")
                {
                    string s2 = "10,11,12";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && selectedval.Contains(Convert.ToInt32(item.Attributes["Usage"].ToString())))
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "e-NP Sailing Direction")
                {
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && Convert.ToInt32(item.Attributes["Usage"].ToString()) == 14)
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "TotalTide")
                {
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && Convert.ToInt32(item.Attributes["Usage"].ToString()) == 15)
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void SetZoomScaleAndHieghlight(int key, Graphic item1)
        {
            try
            {
                var li1 = new[] { 10000, 62500 };
                var li2 = new[] { 125000, 250000 };
                var li3 = new[] { 500000, 1000000 };
                var li4 = new[] { 2000000, 4000000 };
                var li5 = new[] { 8000000, 16000000 };
                var li6 = new[] { 32000000, 64000000 };
                var tes1 = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

                var list = new[] { 10000, 62500, 125000, 250000, 500000, 1000000, 2000000, 4000000, 8000000, 16000000, 32000000, 64000000, 128000000 };
                var input = tes1.TargetScale;
                var diffList = from number in list
                               select new
                               {
                                   number,
                                   difference = Math.Abs(number - input)
                               };
                var result = (from diffItem in diffList
                              orderby diffItem.difference
                              select diffItem).First().number;

                double ImageScale = result;
                switch (key)
                {
                    case 1:
                        if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li6.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        break;
                    case 2:
                        if (ImageScale > 8000000 && ImageScale <= 32000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li5.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }

                        break;
                    case 3:
                        if (ImageScale > 2000000 && ImageScale <= 8000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li4.Max());
                            ImageScale = ser;
                            Search_polygon_selection(item1);
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 4:
                        if (ImageScale > 500000 && ImageScale <= 2000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li3.Max());
                            ImageScale = ser;
                            Search_polygon_selection(item1);
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 5:
                        if (ImageScale > 125000 && ImageScale <= 500000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li2.Max());
                            ImageScale = ser;
                            Search_polygon_selection(item1);
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 6:
                        if (ImageScale > 10000 && ImageScale <= 125000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li1.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        break;
                    case 7:
                        if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            var ser = Convert.ToDouble(li5.Max());
                            ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li5.Max());
                            ImageScale = ser;
                            Search_polygon_selection(item1);
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 8:
                        if (ImageScale > 8000000 && ImageScale <= 32000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);
                        }
                        else
                        {
                            Search_polygon_selection(item1);
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li3.Max());
                            ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 9:
                        if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            //var ser = Convert.ToDouble(li5.Max());
                            //ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                            Search_polygon_selection(item1);

                        }
                        else
                        {
                            Search_polygon_selection(item1);
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li6.Max());
                            ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 10:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);
                        Search_polygon_selection(item1);

                        break;
                    case 11:

                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);
                        Search_polygon_selection(item1);
                        break;
                    case 12:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);
                        Search_polygon_selection(item1);
                        break;
                    case 13:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);
                        Search_polygon_selection(item1);
                        break;
                    case 14:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);
                        Search_polygon_selection(item1);
                        break;
                    case 15:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);
                        Search_polygon_selection(item1);
                        break;

                }
            }
            catch
            {

            }
        }

        public void SetZoomScale(int key, Graphic item1)
        {
            try
            {
                var li1 = new[] { 10000, 62500 };
                var li2 = new[] { 125000, 250000 };
                var li3 = new[] { 500000, 1000000 };
                var li4 = new[] { 2000000, 4000000 };
                var li5 = new[] { 8000000, 16000000 };
                var li6 = new[] { 32000000, 64000000 };
                var tes1 = MyMapView.GetCurrentViewpoint(ViewpointType.CenterAndScale);

                var list = new[] { 10000, 62500, 125000, 250000, 500000, 1000000, 2000000, 4000000, 8000000, 16000000, 32000000, 64000000, 128000000 };
                var input = tes1.TargetScale;
                var diffList = from number in list
                               select new
                               {
                                   number,
                                   difference = Math.Abs(number - input)
                               };
                var result = (from diffItem in diffList
                              orderby diffItem.difference
                              select diffItem).First().number;

                double ImageScale = result;
                switch (key)
                {
                    case 1:
                        if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li6.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        break;
                    case 2:
                        if (ImageScale > 8000000 && ImageScale <= 32000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li5.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }

                        break;
                    case 3:
                        if (ImageScale > 2000000 && ImageScale <= 8000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li4.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 4:
                        if (ImageScale > 500000 && ImageScale <= 2000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li3.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 5:
                        if (ImageScale > 125000 && ImageScale <= 500000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li2.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 6:
                        if (ImageScale > 10000 && ImageScale <= 125000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li1.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        break;
                    case 7:
                        if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            var ser = Convert.ToDouble(li5.Max());
                            ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {
                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li5.Max());
                            ImageScale = ser;

                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 8:
                        if (ImageScale > 8000000 && ImageScale <= 32000000)
                        {
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);

                        }
                        else
                        {

                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li3.Max());
                            ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 9:
                        if (ImageScale > 32000000 && ImageScale <= 128000000)
                        {
                            //var ser = Convert.ToDouble(li5.Max());
                            //ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);


                        }
                        else
                        {

                            item1.GraphicsOverlay.IsVisible = true;
                            var ser = Convert.ToDouble(li6.Max());
                            ImageScale = ser;
                            item1.IsSelected = true;
                            ZoomToLocation(item1, ImageScale);
                        }
                        break;
                    case 10:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);


                        break;
                    case 11:

                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);

                        break;
                    case 12:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);

                        break;
                    case 13:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);

                        break;
                    case 14:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);

                        break;
                    case 15:
                        item1.GraphicsOverlay.IsVisible = true;
                        item1.IsSelected = true;
                        ZoomToLocation(item1, ImageScale);

                        break;

                }
            }
            catch
            {

            }
        }
        private void routelineconfigclear()
        {
            //  MyMapView.SketchEditor.Style.VertexSymbol = null;
            MyMapView.SketchEditor.Style.ShowNumbersForVertices = true;
            MyMapView.SketchEditor.Style.MidVertexSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 5);
            MyMapView.SketchEditor.Style.VertexSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Diamond, System.Drawing.Color.Red, 10);
            MyMapView.SketchEditor.Style.LineSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Transparent,
                    new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 0, 255), 2));

        }
        private void bottomspliter_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeNS;
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string licenseKey = "runtimelite,1000,rud7227169606,none,JFB3LNBHPDC0A1GJH146";

            ArcGISRuntimeEnvironment.SetLicense(licenseKey);

            LoadingAdorner.IsAdornerVisible = true;

            Initialize();
            await Task.Delay(10000);
            LoadingAdorner.IsAdornerVisible = false;
        }
        private void bottomspliter_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        private void folioSearch_Click(object sender, RoutedEventArgs e)
        {
            RemoveHeilight();
            Temperatures row = (Temperatures)((Button)e.Source).DataContext;

            List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
            foreach (var item in MyMapView.GraphicsOverlays)
            {
                if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                {
                    grc.Add(item);
                }
            }
            string serch = row.FProductId.ToString();

            if (Maincat == "AVCS Folio")
            {
                string s2 = "7,8,9";
                int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                foreach (var ter in grc)
                {
                    foreach (var item in ter.Graphics)
                    {

                        if (item.Attributes["ShortName"].ToString() == serch && selectedval.Contains(Convert.ToInt32(item.Attributes["Usage"].ToString())))
                        {
                            SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                        }
                    }
                }
            }
        }
        private void btnRouteClose_Click(object sender, RoutedEventArgs e)
        {
            SaveRouteName.Visibility = Visibility.Hidden;
        }
        private void SaveRoutehide_Click(object sender, RoutedEventArgs e)
        {
            SaveRouteName.Visibility = Visibility.Hidden;
        }

        private void SaveRoutehide_Click1(object sender, RoutedEventArgs e)
        {
            SaveRouteName1.Visibility = Visibility.Hidden;
        }

        private async Task<PictureMarkerSymbol> getpic1()//for middle circles in polyline
        {
            PictureMarkerSymbol pictureMarkerSym=null;
            try
            {
                // create a new PictureMarkerSymbol with an image file stored locally
                var picPath = AppDomain.CurrentDomain.BaseDirectory + "circle_PNG47.png";
                
                using (System.IO.FileStream picStream = new System.IO.FileStream(picPath, System.IO.FileMode.Open))
                {
                    // pass the image file stream to the symbol constructor
                    pictureMarkerSym = await PictureMarkerSymbol.CreateAsync(picStream);
                }

                SketchStyle srt = new SketchStyle();
                var item = srt.MidVertexSymbol.CreateSwatchAsync(96);

                pictureMarkerSym.Width = 10;
                pictureMarkerSym.Height = 10;
                pictureMarkerSym.LeaderOffsetX = 0;
                pictureMarkerSym.OffsetY = 0;
                pictureMarkerSym.Angle = 90;
                return pictureMarkerSym;
            }
            catch
            {
                return pictureMarkerSym;
            }

        }
        private async void sketchEditor()
        {
            try
            {
                // string stnme=
                string stopName = "";//add this
                                     // polist.WaypointCount = _sketchRouteOverlay.Graphics.Count + 1;

                MyMapView.SketchEditor.Style.ShowNumbersForVertices = false;
                PictureMarkerSymbol midmarker = await getpic1();
                PictureMarkerSymbol pushpinMarker = await GetPictureMarker();
                //  MyMapView.SketchEditor.Style.SelectionColor = System.Drawing.Color.Red;
                TextSymbol stopSymbol = new TextSymbol(stopName, System.Drawing.Color.Red, 15,
                        Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Left, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                stopSymbol.OffsetX = 0;
                stopSymbol.OffsetY = 0;
                //  stopSymbol.OffsetY = 5;
                TextSymbol midvertextext = new TextSymbol("", System.Drawing.Color.Red, 8,
                        Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Justify, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                stopSymbol.OffsetY = 20;

                MyMapView.SketchEditor.Style.LineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Red, 1);
                // PictureMarkerSymbol pushpinMarker = await GetPictureMarker();
                // new CompositeSymbol(new MarkerSymbol[] { pushpinMarker, stopSymbol });
                MyMapView.SketchEditor.Style.VertexSymbol = new CompositeSymbol(new MarkerSymbol[] { pushpinMarker, stopSymbol });
                MyMapView.SketchEditor.Style.MidVertexSymbol = new CompositeSymbol(new MarkerSymbol[] { midmarker, midvertextext });
                MyMapView.SketchEditor.Style.VertexTextSymbol = new TextSymbol(stopName, System.Drawing.Color.Red, 15,
                Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Left, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                // Graphic near=new Graphic()
                //_sketchOverlay.Graphics.Add()
                // sect.VertexEditMode=
                // geometry2 =await MyMapView.SketchEditor.Style.LineSymbol
            }
            catch
            {

            }
        }

        private Graphic Mouse_tap_AddedCart(Graphic graphic)
        {
            graphic.Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 255, 255, 0),
                            new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 255, 0), 2));
            return graphic;
        }
        private Graphic Mouse_tap_select(Graphic graphic)
        {
            graphic.Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 0, 0, 0),
                            new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(46, 50, 54), 2));
            return graphic;
        }
        private Graphic Mouse_Tap_Unselect(Graphic graphic)
        {
            graphic.Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 60, 62, 66),
                            new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(46, 50, 54), 0.5));
            return graphic;
        }
        private Graphic Rightpanel_polygon_selection(Graphic graphic)
        {
            graphic.Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(70, 0, 0, 255),
                            new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(0, 0, 255), 2));
            return graphic;
        }

        private Graphic Search_polygon_selection(Graphic graphic)
        {
            graphic.Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(70, 225, 56, 255),
                            new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(225, 56, 255), 2.5));
            return graphic;
        }
        private void clearButton_GraphicSelction(GraphicCollection grc)
        {
            foreach (var item in grc)
            {
                item.Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(20, 60, 62, 66),
                            new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(46, 50, 54), 0.5));
            }



        }

        private void view(object sender, RoutedEventArgs e)
        {
            try
            {
                foliodetails.Visibility = Visibility.Visible;
                Temperatures row = (Temperatures)((Button)e.Source).DataContext;
                TemperaturesDetails obj = new TemperaturesDetails();
                var itemToRemove = cart1.FirstOrDefault(r => r.FProductId == row.FProductId);

                foliodetailgrid.ItemsSource = null;
                foliodetailgrid.ItemsSource = itemToRemove.DetailsItems;

            }
            catch
            {

            }
        }

        private void detailshide_Click(object sender, RoutedEventArgs e)
        {
            foliodetails.Visibility = Visibility.Hidden;
        }

        private void btnrightsearh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveHeilight();
                ShoppingCart row = (ShoppingCart)((Button)e.Source).DataContext;

                List<GraphicsOverlay> grc = new List<GraphicsOverlay>();
                foreach (var item in MyMapView.GraphicsOverlays)
                {
                    if (item.Id != "seven" && item.Id != "Portoverlay" && item.Id != "greenPortoverlay" && item.Id != "redPortoverlay" && item.Id != "_sketchRectOverlay")
                    {
                        grc.Add(item);
                    }
                }
                string serch = row.ShortName.ToString();
                if (Maincat == "AVCS Chart")
                {
                    string s2 = "1,2,3,4,5,6";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && selectedval.Contains(Convert.ToInt32(item.Attributes["Usage"].ToString())))
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "AVCS Folio")
                {
                    string s2 = "7,8,9";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && selectedval.Contains(Convert.ToInt32(item.Attributes["Usage"].ToString())))
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "Digital List of Lights")
                {
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && Convert.ToInt32(item.Attributes["Usage"].ToString()) == 13)
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "Digital List of Radio Signals")
                {
                    string s2 = "10,11,12";
                    int[] selectedval = Array.ConvertAll(s2.Split(','), s => int.Parse(s));
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && selectedval.Contains(Convert.ToInt32(item.Attributes["Usage"].ToString())))
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "e-NP Sailing Direction")
                {
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && Convert.ToInt32(item.Attributes["Usage"].ToString()) == 14)
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
                if (Maincat == "TotalTide")
                {
                    foreach (var ter in grc)
                    {
                        foreach (var item in ter.Graphics)
                        {

                            if (item.Attributes["ShortName"].ToString() == serch && Convert.ToInt32(item.Attributes["Usage"].ToString()) == 15)
                            {
                                SetZoomScaleAndHieghlight(Convert.ToInt32(item.Attributes["Usage"].ToString()), item);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void SearchRighthide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideRightRoute", btnLeftMenuShow, pnlRightSearch);
        }
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRoutName == "" && importedlinepoints.Count > 0)
            {
                try
                {
                    savemenu.IsEnabled = true;
                    _sketchOverlay.Graphics.Clear();
                    routewaypointoverlay.Graphics.Clear();
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    Mouse.OverrideCursor = Cursors.Cross;
                    savemenu.IsEnabled = true;
                    // Esri.ArcGISRuntime.Geometry.PointCollection databasepointcollection_web = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
                    // List<MapPoint> databasepointslist = new List<MapPoint>();
                    var l1 = loadrouteline_create(normalizedimportedpoints);
                    var editPolyline1 = l1 as Polyline;
                    this.editlinebuilder = new PolylineBuilder(editPolyline1);
                    var item2geom = loadrouteline_geom_create_new(normalizedimportedpoints);
                    var editPolyline2 = item2geom as Polyline;//add this
                    if (editlinebuilder.Parts.Count > 1)
                    {

                        var config = new SketchEditConfiguration()
                        {
                            AllowVertexEditing = true,
                            AllowMove = true,
                            AllowRotate = false,

                            ResizeMode = SketchResizeMode.None
                        };
                        sketchEditor();
                        Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(editPolyline2);
                    }
                    else
                    {
                        var config = new SketchEditConfiguration()
                        {
                            AllowVertexEditing = true,
                            AllowMove = true,
                            AllowRotate = false,

                            ResizeMode = SketchResizeMode.None
                        };
                        sketchEditor();
                        Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(editPolyline1);// add this

                    }

                }
                catch (TaskCanceledException)
                {
                    // Ignore ... let the user cancel editing
                }


            }
            else
            {


                try
                {
                    savemenu.IsEnabled = true;
                    _sketchOverlay.Graphics.Clear();
                    routewaypointoverlay.Graphics.Clear();
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    Mouse.OverrideCursor = Cursors.Cross;
                    savemenu.IsEnabled = true;
                    Esri.ArcGISRuntime.Geometry.PointCollection databasepointcollection_web = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
                    List<MapPoint> databasepointslist = new List<MapPoint>();

                    DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objRout.GetRouteLineDetails(SelectedRoutName);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {


                        var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                        var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                        MapPoint mp1 = new MapPoint(latit, longit, SpatialReferences.WebMercator);
                        databasepointcollection_web.Add(latit, longit);
                        databasepointslist.Add(mp1);


                    }


                    var l1 = loadrouteline_create(databasepointcollection_web);
                    var editPolyline1 = l1 as Polyline;
                    this.editlinebuilder = new PolylineBuilder(editPolyline1);
                    var item2geom = loadrouteline_geom_create_new(databasepointcollection_web);
                    var editPolyline2 = item2geom as Polyline;//add this

                    if (editlinebuilder.Parts.Count > 1)
                    {

                        var config = new SketchEditConfiguration()
                        {
                            AllowVertexEditing = true,
                            AllowMove = true,
                            AllowRotate = false,

                            ResizeMode = SketchResizeMode.None
                        };
                        sketchEditor();
                        Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(editPolyline2);
                    }
                    else
                    {
                        var config = new SketchEditConfiguration()
                        {
                            AllowVertexEditing = true,
                            AllowMove = true,
                            AllowRotate = false,

                            ResizeMode = SketchResizeMode.None
                        };
                        sketchEditor();
                        Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(editPolyline1);// add this

                    }
                }
                catch (TaskCanceledException)
                {
                    // Ignore ... let the user cancel editing
                }
            }
        }
        private Esri.ArcGISRuntime.Geometry.Geometry loadrouteline_geom_create_new(Esri.ArcGISRuntime.Geometry.PointCollection pc)//add this method
        {
            var polyline = new Esri.ArcGISRuntime.Geometry.Polyline(pc);
            Esri.ArcGISRuntime.Geometry.Geometry geom = polyline;
            return geom;

        }
        private readonly Stack<Esri.ArcGISRuntime.Geometry.Geometry> undoStack = new Stack<Esri.ArcGISRuntime.Geometry.Geometry>();
        private void UndoButton_click(object sender, RoutedEventArgs e)
        {
            if (MyMapView.SketchEditor.UndoCommand.CanExecute(null))
            {
                MyMapView.SketchEditor.UndoCommand.Execute(null);
            }
        }
        private void Redobutton_click(object sender, RoutedEventArgs e)
        {
            if (MyMapView.SketchEditor.RedoCommand.CanExecute(null))
            {
                MyMapView.SketchEditor.RedoCommand.Execute(null);
            }
        }

        private void mylistbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string str = null;

                if (mylistbox.SelectedIndex < 0)
                {
                    return;
                }

                str = (mylistbox.SelectedItem as ListBoxItem).Content.ToString();


                if (mylistbox.Items.Count > 1)
                {
                    List<GraphicsOverlay> graphicoverlaycollection = new List<GraphicsOverlay>();
                    var ter = MyMapView.GraphicsOverlays;
                    foreach (var items in ter)
                    {
                        if (items.Id != "seven" && items.Id != "Portoverlay" && items.Id != "greenPortoverlay" && items.Id != "redPortoverlay" && items.Id != "_sketchRectOverlay")
                        {
                            graphicoverlaycollection.Add(items);
                        }
                       
                    }

                    foreach (var items in graphicoverlaycollection)
                    {
                        foreach (var item1 in items.Graphics)
                        {
                            string named = item1.Attributes.Keys.FirstOrDefault();
                            if (mylistbox.Items.Cast<ListBoxItem>().Any(x => x.Content.ToString() == named) && !cart.Any(x => x.ShortName == named) && !cart1.Any(x => x.FProductId == named))
                            {
                                if (!item1.IsSelected)
                                {
                                    if (str == named)
                                    {
                                        string use = item1.Attributes["Usage"].ToString();
                                        item1.IsSelected = true;
                                        SetZoomScale(Convert.ToInt32(use), item1);
                                        Rightpanel_polygon_selection(item1);
                                    }
                                }
                                else
                                {
                                    item1.IsSelected = false;
                                    selection(named, item1);

                                }
                            }
                            else
                            {
                                if (item1.IsSelected)
                                {
                                    if (mylistbox.Items.Cast<ListBoxItem>().Any(x => x.Content.ToString() == named) && (cart.Any(x => x.ShortName == named) || cart1.Any(x => x.FProductId == named)))
                                    {
                                        if (str == named)
                                        {
                                            string use = item1.Attributes["Usage"].ToString();
                                            Rightpanel_polygon_selection(item1);
                                            SetZoomScale(Convert.ToInt32(use), item1);
                                        }
                                        else
                                        {
                                            selection(named, item1);
                                        }
                                    }
                                }
                                else
                                {
                                    item1.IsSelected = false;
                                    selection(named, item1);
                                }
                            }
                        }
                    }
                }

                e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void selection (string named ,Graphic item1)
        {
            if (Maincat == "AVCS Chart")
            {
                if (checkout.Any(x => x.ShortName == named && x.ProductName == "AVCS Products"))
                {
                    Mouse_tap_AddedCart(item1);
                }
                else
                {
                    Mouse_Tap_Unselect(item1);
                }
            }
            if (Maincat == "AVCS Folio")
            {
                if (checkout.Any(x => x.ShortName == named && x.ProductName == "AVCS Products"))
                {
                    Mouse_tap_AddedCart(item1);
                }
                else
                {
                    Mouse_Tap_Unselect(item1);
                }
            }
            if (Maincat == "Digital List of Lights")
            {
                if (checkout.Any(x => x.ShortName == named && x.ProductName == "ADLL"))
                {
                    Mouse_tap_AddedCart(item1);
                }
                else
                {
                    Mouse_Tap_Unselect(item1);
                }
            }
            if (Maincat == "Digital List of Radio Signals")
            {
                if (checkout.Any(x => x.ShortName == named && x.ProductName == "ADRS"))
                {
                    Mouse_tap_AddedCart(item1);
                }
                else
                {
                    Mouse_Tap_Unselect(item1);
                }
            }
            if (Maincat == "e-NP Sailing Direction")
            {
                if (checkout.Any(x => x.ShortName == named && x.ProductName == "e-Nautical Publications"))
                {
                    Mouse_tap_AddedCart(item1);
                }
                else
                {
                    Mouse_Tap_Unselect(item1);
                }
            }
            if (Maincat == "TotalTide")
            {
                if (checkout.Any(x => x.ShortName == named && x.ProductName == "TotalTide"))
                {
                    Mouse_tap_AddedCart(item1);
                }
                else
                {
                    Mouse_Tap_Unselect(item1);
                }
            }
        }
        private void mylistbox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string str = null;

                if (mylistbox.SelectedIndex < 0)
                {
                    //_graphicsOverlay.ClearSelection();
                    return;
                }

                str = (mylistbox.SelectedItem as ListBoxItem).Content.ToString();

                if (mylistbox.Items.Count > 1)
                {

                    List<GraphicsOverlay> graphicoverlaycollection = new List<GraphicsOverlay>();
                    var ter = MyMapView.GraphicsOverlays;
                    foreach (var items in ter)
                    {
                        if (items.Id != "seven" && items.Id != "Portoverlay" && items.Id != "greenPortoverlay" && items.Id != "redPortoverlay" && items.Id != "_sketchRectOverlay")
                        {
                            graphicoverlaycollection.Add(items);
                        }
                    }

                    foreach (var items in graphicoverlaycollection)
                    {
                        foreach (var item1 in items.Graphics)
                        {

                            string named = item1.Attributes.Keys.FirstOrDefault();

                            if (item1.Attributes.ContainsKey(str))
                            {

                                if (Maincat == "AVCS Chart")
                                {

                                    item1.IsSelected = false;
                                    Mouse_Tap_Unselect(item1);
                                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == str);
                                    if (itemToRemove != null)
                                    {
                                        cart.Remove(itemToRemove);
                                    }
                                    objMgr.DeleteShoppingCartByProduct("AVCS Products", str);
                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;


                                }
                                if (Maincat == "AVCS Folio")
                                {
                                    item1.IsSelected = false;
                                    Mouse_Tap_Unselect(item1);
                                    var itemToRemove = cart1.FirstOrDefault(r => r.FProductId == str);
                                    if (itemToRemove != null)
                                    {
                                        cart1.Remove(itemToRemove);
                                    }
                                    objMgr.DeleteShoppingCartByProduct("AVCS Products",str);
                                    dataGrid2.ItemsSource = null;
                                    dataGrid2.ItemsSource = cart1.OrderByDescending(f => f.Id);
                                    return;
                                }
                                if (Maincat == "Digital List of Radio Signals")
                                {
                                    item1.IsSelected = false;
                                    Mouse_Tap_Unselect(item1);
                                    var itemToRemove = cart.FirstOrDefault(r => r.ShortName == str);
                                    if (itemToRemove != null)
                                    {
                                        cart.Remove(itemToRemove);
                                    }
                                    objMgr.DeleteShoppingCartByProduct("ADRS",str);
                                    dataGrid1.ItemsSource = null;
                                    dataGrid1.ItemsSource = cart.OrderByDescending(f => f.Id); ;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private async void geodetic_line(object sender, RoutedEventArgs e)
        {
            _sketchOverlay.Graphics.Clear();
            routewaypointoverlay.Graphics.Clear();
            UnderRoot1.IsEnabled = false;
            geodesic_points.Clear();
            symbolspointslist_savebtn.Clear();
            Mouse.OverrideCursor = Cursors.Cross;
            savemenu1.IsEnabled = true;
            MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
            // Add a graphic at JFK to serve as the origin.
            // MyMapView.GeoViewTapped += MyMapViewOn_GeoView_Tapped_new;
            try
            {
                MyMapView.MouseRightButtonDown -= mouseWheel_rightclic;
                MyMapView.MouseRightButtonDown += mouseWheel_rightclic;
                UndoRoot1.IsEnabled = true;
                RedoRoot1.IsEnabled = true;
                var config = new SketchEditConfiguration()
                {
                    AllowVertexEditing = true,
                    AllowMove = true,
                    AllowRotate = false,

                    ResizeMode = SketchResizeMode.None
                };
                sketchEditor();

                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.Polyline;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await MyMapView.SketchEditor.StartAsync(creationMode, true);
            }
            catch (Exception ex)
            {
                MyMapView.MouseRightButtonDown -= mouseWheel_rightclic;
                // MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        PolylineBuilder builder = new PolylineBuilder(SpatialReferences.Wgs84);
        List<MapPoint> symbolspointslist_savebtn = new List<MapPoint>();
        Esri.ArcGISRuntime.Geometry.PointCollection geodesic_points = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
        Esri.ArcGISRuntime.Geometry.PointCollection geodesic_savbtnpoints_webmerccollection = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
        private void MyMapViewOnGeoViewTapped(object sender, GeoViewInputEventArgs geoViewInputEventArgs)
        {

            MapPoint start = new MapPoint(geoViewInputEventArgs.Location.X, geoViewInputEventArgs.Location.Y, SpatialReferences.WebMercator);
            SimpleMarkerSymbol startMarker = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 10);
            var start_transform = (MapPoint)GeometryEngine.Project(start, SpatialReference.Create(4326));
            var start_graphic = new Graphic(start_transform, startMarker);
            _sketchOverlay.Graphics.Add(start_graphic);

            geodesic_points.Add(start_transform);
            symbolspointslist_savebtn.Add(start_transform);
            geodesic_savbtnpoints_webmerccollection.Add(start);

            if (geodesic_points.Count > 1)
            {
                Polyline routeLine1 = new Polyline(geodesic_points);
                GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geodesic_points.ElementAt(0), geodesic_points.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                GeodeticDistanceResult loxodromeMeasureResult1 = GeometryEngine.DistanceGeodetic(geodesic_points.ElementAt(0), geodesic_points.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Geodesic);
                var loxdres = Math.Round(loxodromeMeasureResult.Distance, 1);
                var loxdres1 = Math.Round(loxodromeMeasureResult1.Distance, 1);

                if (loxodromeMeasureResult.Distance <= 400)
                {
                    Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(routeLine1, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Loxodrome);
                    var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                    var pathgraphic = new Graphic(pathGeometry1, linesym);
                    pathgraphic.Attributes["ShortName"] = loxdres.ToString() + " " + "NM";
                    // pathgraphic.Attributes["ShortName_"] = loxodromeMeasureResult.AzimuthUnit.ToString() + " " + "T";
                    //routewaypointoverlay.Graphics.Add(pathgraphic);
                    _sketchOverlay.Graphics.Add(pathgraphic);
                    route_symbolsadding_pointcollection(geodesic_points);
                    geodesic_points.RemoveAt(0);
                    // symbolspointslist.RemoveAt(0);
                }
                else
                {
                    // Densify the polyline to show the geodesic curve.
                    Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(routeLine1, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Geodesic);
                    var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                    var pathgraphic = new Graphic(pathGeometry1, linesym);
                    pathgraphic.Attributes["ShortName"] = loxdres1.ToString() + " " + "NM";//change this variable as beside when ever its occurance
                    // pathgraphic.Attributes["ShortName_"] = loxodromeMeasureResult1.AzimuthUnit.ToString() + " " + "T";
                    // routewaypointoverlay.Graphics.Add(pathgraphic);
                    _sketchOverlay.Graphics.Add(pathgraphic);
                    route_symbolsadding_pointcollection(geodesic_points);
                    geodesic_points.RemoveAt(0);
                    // symbolspointslist.RemoveAt(0);
                }

            }
        }
        private void Geodetic_RouteGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                routewaypointoverlay.Graphics.Clear();
                _sketchOverlay.Graphics.Clear();
                symbolspointslist_savebtn.Clear();
                List<MapPoint> geodeticloadpoints = new List<MapPoint>();
                Esri.ArcGISRuntime.Geometry.PointCollection geodeticPointCollection = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
                List<MapPoint> geodesicload_pointlist = new List<MapPoint>();
                EditRoot1.IsEnabled = true;

                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    string aa = (RouteGrid1.SelectedItem as DataRowView).Row["RouteName"].ToString();
                    SelectedRoutName = aa;
                    lstSelectedRoute.Add(aa);
                    DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objCart.GetRouteLineDetails(aa);
                    foreach (DataRow row1 in dt.Rows)
                    {
                        polist.latitude = Convert.ToDouble(row1["Latitude"]);
                        polist.longitude = Convert.ToDouble(row1["Longitude"]);

                        MapPoint mp = new MapPoint(polist.latitude, polist.longitude, SpatialReferences.WebMercator);

                        geodeticPointCollection.Add(mp);
                        geodeticloadpoints.Add(mp);

                    }
                }
                for (int i = 0; i < geodeticPointCollection.Count; i++)
                {
                    if (i + 1 == geodeticPointCollection.Count + 1)
                    {

                        break;

                    }
                    else
                    {
                        MapPoint loadstartpoint = new MapPoint(geodeticPointCollection[i].X, geodeticPointCollection[i].Y, SpatialReferences.WebMercator);
                        var start_transform = (MapPoint)GeometryEngine.Project(loadstartpoint, SpatialReference.Create(4326));
                        geodesicload_pointlist.Add(start_transform);



                        if (geodesicload_pointlist.Count > 1)
                        {
                            GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                            GeodeticDistanceResult loxodromeMeasureResult1 = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Geodesic);
                            var loxdres = Math.Round(loxodromeMeasureResult.Distance, 1);
                            var loxdres1 = Math.Round(loxodromeMeasureResult1.Distance, 1);
                            if (loxodromeMeasureResult.Distance <= 400)
                            {
                                Polyline routeLine2 = new Polyline(geodesicload_pointlist);
                                // Densify the polyline to show the geodesic curve.
                                Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine2, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Loxodrome);
                                var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                var _pathGraphic1 = new Graphic(aftereditpathgeom, linesym);
                                _pathGraphic1.Attributes["ShortName"] = loxdres.ToString() + " " + "Kn";
                                _sketchOverlay.Graphics.Add(_pathGraphic1);
                                // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                //  route_symbolsadding(geodesicload_pointlist);
                                // builder.AddPart(geodesicpointlist);
                                geodesicload_pointlist.RemoveAt(0);

                            }
                            else
                            {
                                Polyline routeLine2 = new Polyline(geodesicload_pointlist);
                                // Densify the polyline to show the geodesic curve.
                                Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine2, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Geodesic);
                                var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                var _pathGraphic2 = new Graphic(aftereditpathgeom, linesym);
                                _pathGraphic2.Attributes["ShortName"] = loxdres1.ToString() + " " + "NM";
                                _sketchOverlay.Graphics.Add(_pathGraphic2);
                                // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                // route_symbolsadding(geodesicload_pointlist);
                                // builder.AddPart(geodesicpointlist);
                                geodesicload_pointlist.RemoveAt(0);
                            }
                            //Polyline routeLine1 = new Polyline(geodesicload_pointlist);
                            //Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(routeLine1, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                            // var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                            // var graphic_new = new Graphic(pathGeometry1, polys3);
                            // _sketchOverlay.Graphics.Add(graphic_new);
                            // geodesicload_pointlist.RemoveAt(0);
                        }
                        //route_symbolsadding_pointcollection(geodeticPointCollection);
                        //var x1 = borderCountryPointCollection[i].X;
                        //var x2 = borderCountryPointCollection[i+1].X;

                    }


                }
                route_symbolsadding(geodeticloadpoints);
                //  geom = loadrout
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void route_symbolsadding_pointcollection(Esri.ArcGISRuntime.Geometry.PointCollection geodesic_points)
        {
            try
            {

                foreach (var tem in geodesic_points)
                {
                    polist.polylinepointcollection = null;
                    switch (SampleState.AddingStops)
                    {
                        case SampleState.AddingStops:
                            // Get the name of this stmop.
                            string stopName = $"W{routewaypointoverlay.Graphics.Count + 1 }";
                            // polist.WaypointCount = _sketchRouteOverlay.Graphics.Count + 1;
                            // Create the marker to show underneath the stop number.
                            PictureMarkerSymbol pushpinMarker = await GetPictureMarker();

                            polist.latitude = tem.X;
                            polist.longitude = tem.Y;


                            TextSymbol stopSymbol = new TextSymbol(stopName, System.Drawing.Color.Transparent, 15,
                                Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Right, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                            stopSymbol.OffsetY = 0;

                            CompositeSymbol combinedSymbol = new CompositeSymbol(new MarkerSymbol[] { pushpinMarker, stopSymbol });

                            Graphic stopGraphic = new Graphic(tem, combinedSymbol);
                            // Graphic stopGraphic = new Graphic(tem);
                            stopGraphic.Attributes["ShortName"] = stopName;
                            routewaypointoverlay.Graphics.Add(stopGraphic);
                            break;
                    }

                }
            }
            catch
            {

            }
            // Normalize geometry - important for geometries that will be sent to a server for processing.
            //mapLocation = (MapPoint)GeometryEngine.NormalizeCentralMeridian(mapLocation);
            // var ste = Mapcoordinates_Change(mapLocation);
        }


        private async void Geodetic_EditRoot_Click(object sender, RoutedEventArgs e)
        {
            symbolspointslist_savebtn.Clear();

            MyMapView.MouseRightButtonDown -= mouseWheel_rightclic;
            MyMapView.MouseRightButtonDown += mouseWheel_rightclic;
            symbolspointslist_savebtn.Clear();//add this line
            UnderRoot1.IsEnabled = false;
            UndoRoot1.IsEnabled = true;
            RedoRoot1.IsEnabled = true;
            EditRoot1.IsEnabled = true;
            if (SelectedRoutName == "" && importedlinepoints.Count > 0)
            {
                try
                {
                    
                    savemenu1.IsEnabled = true;
                    _sketchOverlay.Graphics.Clear();
                    routewaypointoverlay.Graphics.Clear();
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;

                    Esri.ArcGISRuntime.Geometry.PointCollection geodetcipointcollection_webmerc = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);

                    for (int i = 0; i < importedlinepoints.Count; i++)
                    {
                        if (i + 1 == importedlinepoints.Count + 1)
                        {

                            break;

                        }
                        else
                        {
                            // var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                            // var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                            // MapPoint mp2 = importedlinepoints[i];
                            MapPoint mp1 = new MapPoint(importedlinepoints[i].X, importedlinepoints[i].Y, SpatialReferences.Wgs84);
                            MapPoint mp1_aftertransfor = (MapPoint)GeometryEngine.Project(mp1, SpatialReference.Create(3857));
                            geodetcipointcollection_webmerc.Add(mp1_aftertransfor);
                        }
                    }

                    var l1 = loadrouteline_create(normalizedimportedpoints);
                    var editPolyline1 = l1 as Polyline;
                    this.editlinebuilder = new PolylineBuilder(editPolyline1);
                    var item2geom = loadrouteline_geom_create_new(normalizedimportedpoints);
                    var editPolyline2 = item2geom as Polyline;//add this

                    if (editlinebuilder.Parts.Count > 1)
                    {

                        var config = new SketchEditConfiguration()
                        {
                            AllowVertexEditing = true,
                            AllowMove = true,
                            AllowRotate = false,

                            ResizeMode = SketchResizeMode.None
                        };
                        sketchEditor();
                        Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(editPolyline2);
                    }
                    else
                    {
                        var config = new SketchEditConfiguration()
                        {
                            AllowVertexEditing = true,
                            AllowMove = true,
                            AllowRotate = false,

                            ResizeMode = SketchResizeMode.None
                        };
                        sketchEditor();
                        Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(editPolyline1);// add this

                    }

                }
                catch (TaskCanceledException)
                {
                    // Ignore ... let the user cancel editing
                }

            }
            else
            {


                try
                {
                    savemenu1.IsEnabled = true;
                    _sketchOverlay.Graphics.Clear();
                    routewaypointoverlay.Graphics.Clear();
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;

                    Esri.ArcGISRuntime.Geometry.PointCollection geodetcipointcollection_webmerc = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);

                    DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objRout.GetRouteLineDetails(SelectedRoutName);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i + 1 == dt.Rows.Count + 1)
                        {

                            break;

                        }
                        else
                        {
                            var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                            var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                            MapPoint mp1 = new MapPoint(latit, longit, SpatialReferences.WebMercator);
                            //MapPoint mp1_aftertransfor = (MapPoint)GeometryEngine.Project(mp1, SpatialReference.Create(4326));
                            geodetcipointcollection_webmerc.Add(mp1);
                        }
                    }

                    var polyline = new Esri.ArcGISRuntime.Geometry.Polyline(geodetcipointcollection_webmerc);
                    Esri.ArcGISRuntime.Geometry.Geometry geom1 = polyline;

                    // Polyline routeLine = new Polyline(geodetcipointcollection_wgs84);
                    var config = new SketchEditConfiguration()
                    {
                        AllowVertexEditing = true,
                        AllowMove = true,
                        AllowRotate = false,

                        ResizeMode = SketchResizeMode.None
                    };
                    sketchEditor();
                    Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(geom1);
                }
                catch (TaskCanceledException)
                {
                    // Ignore ... let the user cancel editing
                }
            }
            MyMapView.MouseRightButtonDown -= mouseWheel_rightclic;
        }

        private void btnSaveGeodetic_Click(object sender, RoutedEventArgs e)
        {
            try

            {
                _sketchOverlay.Graphics.Clear();
                routewaypointoverlay.Graphics.Clear();
                EditRoot1.IsEnabled = true;
                UnderRoot1.IsEnabled = true;
                IReadOnlyList<MapPoint> geodroutepoints = null;
                List<MapPoint> geodroutelinepoints = new List<MapPoint>();
                List<MapPoint> geodesicload_pointlist = new List<MapPoint>();

                Polyline routeLine = new Polyline(geodesic_savbtnpoints_webmerccollection);
                var temproutegeom = MyMapView.SketchEditor.Geometry;
                RouteManager objRout = new RouteManager();
                var geodroutgeom = routeLine as Esri.ArcGISRuntime.Geometry.Polyline;
                this.builder = new PolylineBuilder(geodroutgeom);

                if (builder.Parts.Count > 0 && temproutegeom == null)
                {
                    Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(routeLine, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                    var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                    foreach (var r in builder.Parts)
                    {
                        geodroutepoints = r.Points;
                        // geodroutelinepoints = Mapcoordinates_Aftertransform(geodesicpointlist_copy);
                        waypointcount = r.PointCount;
                    }
                    DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objCart.GetRouteLineDetails(txtRouteName1.Text);
                    if (dt.Rows.Count > 0)
                    {
                        MessageBoxResult result = MessageBox.Show("Route Name Already Exists Do you want to Override", "Information", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (MessageBoxResult.Yes == result)
                        {

                            objMgr.DeleteRouteLineDetailsByName(txtRouteName1.Text.Trim());
                            objMgr.DeleteRouteLineByName(txtRouteName1.Text.Trim());
                            int count = 0;
                            int result1 = objRout.InsertRoute(txtRouteName1.Text, waypointcount, "2");
                            if (result1 > 0)
                            {
                                if (importedlinepoints.Count > 1)//to reset that exisiting imported points
                                {
                                    importedlinepoints.Clear();
                                    normalizedimportedpoints.Clear();
                                    count++;

                                }
                                foreach (var ter in geodroutepoints)
                                {
                                    double lat = ter.X;
                                    double longi = ter.Y;
                                    objRout.InsertRouteDetails(result1, lat.ToString(), longi.ToString());
                                }

                            }
                            if (MessageBoxResult.No == result)
                            {
                                return;
                            }

                        }

                    }
                    else
                    {
                        int result1 = objRout.InsertRoute(txtRouteName1.Text, waypointcount, "2");
                        int count = 0;
                        if (result1 > 0)
                        {
                            if (importedlinepoints.Count > 1)
                            {
                                importedlinepoints.Clear();
                                normalizedimportedpoints.Clear();
                                count++;

                            }
                            foreach (var ter in geodroutepoints)
                            {
                                double lat = ter.X;
                                double longi = ter.Y;
                                objRout.InsertRouteDetails(result1, lat.ToString(), longi.ToString());
                            }
                        }
                    }

                    for (int i = 0; i < geodroutepoints.Count; i++)
                    {
                        if (i + 1 == geodroutepoints.Count + 1)
                        {

                            break;

                        }
                        else
                        {
                            MapPoint loadstartpoint = new MapPoint(geodroutepoints[i].X, geodroutepoints[i].Y, SpatialReferences.WebMercator);
                            var start_transform = (MapPoint)GeometryEngine.Project(loadstartpoint, SpatialReference.Create(4326));
                            geodesicload_pointlist.Add(start_transform);

                            if (geodesicload_pointlist.Count > 1)
                            {
                                Polyline routeLine1 = new Polyline(geodesicload_pointlist);
                                GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                                GeodeticDistanceResult loxodromeMeasureResult1 = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Geodesic);

                                var loxdres = Math.Round(loxodromeMeasureResult.Distance, 1);
                                var loxdres1 = Math.Round(loxodromeMeasureResult1.Distance, 1);

                                if (loxodromeMeasureResult.Distance <= 400)
                                {
                                    // Densify the polyline to show the geodesic curve.
                                    Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine1, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Loxodrome);
                                    var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                    var _pathGraphic1 = new Graphic(aftereditpathgeom, linesym);
                                    _pathGraphic1.Attributes["ShortName"] = loxdres.ToString() + " " + "NM";
                                    _sketchOverlay.Graphics.Add(_pathGraphic1);
                                    // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                    // route_symbolsadding(savepointlist_polydraw);
                                    // builder.AddPart(geodesicpointlist);
                                    geodesicload_pointlist.RemoveAt(0);

                                }
                                else
                                {
                                    // Densify the polyline to show the geodesic curve.
                                    Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine1, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Geodesic);
                                    var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                    var _pathGraphic2 = new Graphic(aftereditpathgeom, linesym);
                                    _pathGraphic2.Attributes["ShortName"] = loxdres1.ToString() + " " + "NM";
                                    _sketchOverlay.Graphics.Add(_pathGraphic2);
                                    // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                    // route_symbolsadding(savepointlist_polydraw);
                                    // builder.AddPart(geodesicpointlist);
                                    geodesicload_pointlist.RemoveAt(0);
                                }

                            }

                        }

                    }

                    //var graphic_new = new Graphic(pathGeometry1, polys3);
                    // _sketchOverlay.Graphics.Add(graphic_new);
                    // route_symbolsadding(symbolspointslist_savebtn);
                }
                else
                {
                    var routegeom = temproutegeom as Esri.ArcGISRuntime.Geometry.Polyline;
                    this.polybroute = new PolylineBuilder(routegeom);
                    RouteManager objRout1 = new RouteManager();
                    Esri.ArcGISRuntime.Geometry.PointCollection geodetcisavepointlist_wgs84 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);

                    if (polybroute.Parts.Count > 0)
                    {
                        foreach (var r in polybroute.Parts)
                        {
                            geodroutepoints = r.Points;
                            geodroutelinepoints = Mapcoordinates_Aftertransform(geodroutepoints);
                            waypointcount = r.PointCount;
                        }
                        DataAccessLayer.RoutRepository objCart = new DataAccessLayer.RoutRepository();
                        DataTable dt = new DataTable();
                        dt = objCart.GetRouteLineDetails(txtRouteName1.Text);
                        if (dt.Rows.Count > 0)
                        {
                            MessageBoxResult result = MessageBox.Show("Route Name Already Exists Do you want to Override", "Information", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                            if (MessageBoxResult.Yes == result)
                            {
                                objMgr.DeleteRouteLineDetailsByName(txtRouteName1.Text.Trim());
                                objMgr.DeleteRouteLineByName(txtRouteName1.Text.Trim());
                                int result1 = objRout1.InsertRoute(txtRouteName1.Text, waypointcount, "2");
                                if (result1 > 0)
                                {
                                    foreach (var ter in geodroutepoints)
                                    {

                                        double lat = ter.X;
                                        double longi = ter.Y;

                                        MapPoint mpt_web = new MapPoint(lat, longi, SpatialReferences.WebMercator);
                                        MapPoint mpt_wgs84 = (MapPoint)GeometryEngine.Project(mpt_web, SpatialReference.Create(4326));
                                        geodetcisavepointlist_wgs84.Add(mpt_wgs84);
                                        objRout1.InsertRouteDetails(result1, lat.ToString(), longi.ToString());
                                    }
                                }

                            }
                            if (MessageBoxResult.No == result)
                            {
                                return;
                            }
                        }
                        else
                        {
                            int result = objRout1.InsertRoute(txtRouteName1.Text, waypointcount, "2");
                            int count = 0;
                            if (result > 0)
                            {
                                if (importedlinepoints.Count > 1)
                                {
                                    importedlinepoints.Clear();
                                    normalizedimportedpoints.Clear();
                                    count++;

                                }
                                foreach (var ter in geodroutepoints)
                                {
                                    double lat = ter.X;
                                    double longi = ter.Y;

                                    MapPoint mpt_web = new MapPoint(lat, longi, SpatialReferences.WebMercator);
                                    MapPoint mpt_wgs84 = (MapPoint)GeometryEngine.Project(mpt_web, SpatialReference.Create(4326));
                                    geodetcisavepointlist_wgs84.Add(mpt_wgs84);


                                    objRout1.InsertRouteDetails(result, lat.ToString(), longi.ToString());
                                }
                            }
                        }
                    }


                    for (int i = 0; i < geodetcisavepointlist_wgs84.Count; i++)
                    {
                        if (i + 1 == geodetcisavepointlist_wgs84.Count + 1)
                        {

                            break;

                        }
                        else
                        {
                            MapPoint loadstartpoint = new MapPoint(geodetcisavepointlist_wgs84[i].X, geodetcisavepointlist_wgs84[i].Y, SpatialReferences.Wgs84);
                            // var start_transform = (MapPoint)GeometryEngine.Project(loadstartpoint, SpatialReference.Create(4326));
                            geodesicload_pointlist.Add(loadstartpoint);

                            if (geodesicload_pointlist.Count > 1)
                            {
                                Polyline routeLine1 = new Polyline(geodesicload_pointlist);
                                GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                                GeodeticDistanceResult loxodromeMeasureResult1 = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Geodesic);

                                var loxdres = Math.Round(loxodromeMeasureResult.Distance, 1);
                                var loxdres1 = Math.Round(loxodromeMeasureResult1.Distance, 1);

                                if (loxodromeMeasureResult.Distance <= 400)
                                {
                                    // Densify the polyline to show the geodesic curve.
                                    Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine1, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Loxodrome);
                                    var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                    var _pathGraphic1 = new Graphic(aftereditpathgeom, linesym);
                                    _pathGraphic1.Attributes["ShortName"] = loxdres.ToString() + " " + "NM";
                                    _sketchOverlay.Graphics.Add(_pathGraphic1);
                                    // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                    // route_symbolsadding(savepointlist_polydraw);
                                    // builder.AddPart(geodesicpointlist);
                                    geodesicload_pointlist.RemoveAt(0);

                                }
                                else
                                {
                                    // Densify the polyline to show the geodesic curve.
                                    Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine1, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Geodesic);
                                    var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                    var _pathGraphic2 = new Graphic(aftereditpathgeom, linesym);
                                    _pathGraphic2.Attributes["ShortName"] = loxdres1.ToString() + " " + "NM";
                                    _sketchOverlay.Graphics.Add(_pathGraphic2);
                                    // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                    // route_symbolsadding(savepointlist_polydraw);
                                    // builder.AddPart(geodesicpointlist);
                                    geodesicload_pointlist.RemoveAt(0);
                                }

                            }

                        }

                    }
                }
                if (MyMapView.SketchEditor.CancelCommand.CanExecute(null))
                {
                    MyMapView.SketchEditor.CancelCommand.Execute(null);
                    MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
                    // MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
                }
                route_symbolsadding(geodroutepoints);
                MyMapView.MouseRightButtonDown -= mouseWheel_rightclic;


                completecommand(creationMode);

                e.Handled = true;

                routelineconfigclear();
                geodesic_points.Clear();
                geodesic_savbtnpoints_webmerccollection.Clear();
                savemenu1.IsEnabled = false;
                savemenu1.IsEnabled = false;
                UndoRoot1.IsEnabled = false;
                RedoRoot1.IsEnabled = false;
                SaveRouteName1.Visibility = Visibility.Hidden;

                SelectedRoutName = txtRouteName1.Text;
                Mouse.OverrideCursor = Cursors.Arrow;
                MyMapView.GeoViewTapped -= MyMapViewOnGeoViewTapped;
                MyMapView.GeoViewTapped += MapViewTapped_Mouse_Point;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Geodetc_Import_Click_rtz(object sender, RoutedEventArgs e, string filename)
        {
            _sketchOverlay.Graphics.Clear();
            importedlinepoints.Clear();
            normalizedimportedpoints.Clear();
            EditRoot1.IsEnabled = true;
            // Esri.ArcGISRuntime.Geometry.PointCollection importedlinepoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
            List<MapPoint> geodesicload_pointlist = new List<MapPoint>();
            routewaypointoverlay.Graphics.Clear();
            SelectedRoutName = "";
            try
            {
                //string strfilename = "";
                //Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                //// openFileDialog.Filter = "All files (*.*)|*.*";
                //if (openFileDialog.ShowDialog() == true)
                //{
                //    openFileDialog.DefaultExt = ".bsk";
                //    openFileDialog.Filter = "Basket(.bsk)|*.bsk";
                //    strfilename = openFileDialog.FileName;
                //    strfilename = openFileDialog.FileName;
                //}

                XDocument doc1 = XDocument.Load(filename);
                XNamespace ns = "http://www.cirm.org/RTZ/1/0";
                List<MapPoint> _mappoint = new List<MapPoint>();
                // Esri.ArcGISRuntime.Geometry.PointCollection importedpoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                // IReadOnlyList<MapPoint> _mappoint=null;
                foreach (XElement element in doc1.Root
                                    .Element(ns + "waypoints")
                                    .Elements(ns + "waypoint")
                                    .Elements(ns + "position"))
                {
                    Console.WriteLine("Name: {0}; Value: {1}",
                         (double)element.FirstAttribute,
                         (double)element.LastAttribute);
                    MapPoint mpt = new MapPoint((double)element.LastAttribute, (double)element.FirstAttribute);
                    _mappoint.Add(mpt);
                    importedlinepoints.Add(mpt);
                }
                normalizedimportedpoints = CalcNormalize_latest(_mappoint);
                //SelectPrdctsunderRoot_Click(_mappoint);
                //distancemes();

                // Geodetic_normalizedimportedpoints = Mapcoordinates_Aftertransform_PointCollection(importedlinepoints);


                for (int i = 0; i < importedlinepoints.Count; i++)
                {
                    if (i + 1 == importedlinepoints.Count + 1)
                    {

                        break;

                    }
                    else
                    {
                        MapPoint loadstartpoint = new MapPoint(importedlinepoints[i].X, importedlinepoints[i].Y, SpatialReferences.Wgs84);
                        // var start_transform = (MapPoint)GeometryEngine.Project(loadstartpoint, SpatialReference.Create(4326));
                        geodesicload_pointlist.Add(loadstartpoint);


                        if (geodesicload_pointlist.Count > 1)
                        {
                            GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                            GeodeticDistanceResult loxodromeMeasureResult1 = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Geodesic);
                            var loxdres = Math.Round(loxodromeMeasureResult.Distance, 1);
                            var loxdres1 = Math.Round(loxodromeMeasureResult1.Distance, 1);

                            if (loxodromeMeasureResult.Distance <= 400)
                            {
                                Polyline routeLine2 = new Polyline(geodesicload_pointlist);
                                // Densify the polyline to show the geodesic curve.
                                Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine2, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Loxodrome);
                                var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                var _pathGraphic1 = new Graphic(aftereditpathgeom, linesym);
                                _pathGraphic1.Attributes["ShortName"] = loxdres.ToString() + " " + "NM";
                                _sketchOverlay.Graphics.Add(_pathGraphic1);
                                // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                //  route_symbolsadding(geodesicload_pointlist);
                                // builder.AddPart(geodesicpointlist);
                                geodesicload_pointlist.RemoveAt(0);

                            }
                            else
                            {
                                Polyline routeLine2 = new Polyline(geodesicload_pointlist);
                                // Densify the polyline to show the geodesic curve.
                                Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine2, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Geodesic);
                                var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                var _pathGraphic2 = new Graphic(aftereditpathgeom, linesym);
                                _pathGraphic2.Attributes["ShortName"] = loxdres1.ToString() + " " + "NM";
                                _sketchOverlay.Graphics.Add(_pathGraphic2);
                                // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                // route_symbolsadding(geodesicload_pointlist);
                                // builder.AddPart(geodesicpointlist);
                                geodesicload_pointlist.RemoveAt(0);
                            }

                        }
                    }
                }
                route_symbolsadding_pointcollection(importedlinepoints);
                //route_symbolsadding(_mappoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    
        private void Undergeodetic_polylineRoot_Click(object sender, RoutedEventArgs e)
        {
            Polyline routeLine = new Polyline(symbolspointslist_savebtn);
            Esri.ArcGISRuntime.Geometry.Geometry pathGeometry1 = GeometryEngine.DensifyGeodetic(routeLine, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
            var polys3 = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
            var graphic1 = new Graphic(pathGeometry1, polys3);
            Geometry_OnviewTap(graphic1);
        }

        private void Geodetc_csv_Import_Click(object sender, RoutedEventArgs e, string fpath)
        {
            _sketchOverlay.Graphics.Clear();
            importedlinepoints.Clear();
            routewaypointoverlay.Graphics.Clear();
            List<MapPoint> _mappoint = new List<MapPoint>();
            List<MapPoint> geodesicload_pointlist = new List<MapPoint>();
            SelectedRoutName = "";
            string[] columns =
                                {
                                    @"LAT",
                                    @"LON"
                                };
            try
            {
                //string strfilename = "";
                // Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                // openFileDialog.Filter = "All files (*.*)|*.*";
                //if (openFileDialog.ShowDialog() == true)
                //{
                //    openFileDialog.DefaultExt = ".bsk";
                //    openFileDialog.Filter = "Basket(.bsk)|*.bsk";
                //    strfilename = openFileDialog.FileName;
                //    strfilename = openFileDialog.FileName;
                //}

                string[] csvlines = File.ReadAllLines(fpath);
                List<string> strt = new List<string>();

                var coun = csvlines.Count();

                for (int i = 0; i <= coun; i++)
                {
                    if (i == coun)
                    {
                        // MapPoint mptx = new MapPoint(mpt[i--].X, mpt[i--].Y, SpatialReferences.Wgs84);
                        // transformed_new.Add(mptx);

                        break;

                    }
                    if (i >= 4)
                    {
                        var laso = csvlines[i].ToString();
                        var ser = laso.Split(',');
                        var sear = csvlines[i].ToCharArray();
                        StringBuilder sb_1 = new StringBuilder();
                        StringBuilder sb_2 = new StringBuilder();
                        int cour = 0;
                        int cour2 = 0;
                        foreach (var ch in sear)
                        {
                            if (Char.IsLetter(ch))
                            {
                                sb_1.Append(ch);
                                cour++;
                                cour2++;
                                sb_2.Append(sb_1);
                            }
                            else
                            {
                                sb_1.Append(ch);
                            }


                            if (cour == 1)
                            {
                                sb_1.Clear();
                                cour--;
                            }
                            if (cour2 == 2)
                            {
                                strt.Add(sb_2.ToString());
                                break;
                            }
                        }
                    }
                }
                //select the indices of the columns we want
                foreach (var ser in strt)
                {
                    var rer = dotCount_manipulatestring(ser);
                    var string2 = rer.Item2;
                    var mpt_wgs = CoordinateFormatter.FromLatitudeLongitude(string2, SpatialReferences.Wgs84);
                    _mappoint.Add(mpt_wgs);
                    importedlinepoints.Add(mpt_wgs);

                }
                normalizedimportedpoints = CalcNormalize_latest(_mappoint);
                for (int i = 0; i < importedlinepoints.Count; i++)
                {
                    if (i + 1 == importedlinepoints.Count + 1)
                    {

                        break;

                    }
                    else
                    {
                        MapPoint loadstartpoint = new MapPoint(importedlinepoints[i].X, importedlinepoints[i].Y, SpatialReferences.Wgs84);
                        // var start_transform = (MapPoint)GeometryEngine.Project(loadstartpoint, SpatialReference.Create(4326));
                        geodesicload_pointlist.Add(loadstartpoint);



                        if (geodesicload_pointlist.Count > 1)
                        {
                            GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                            GeodeticDistanceResult loxodromeMeasureResult1 = GeometryEngine.DistanceGeodetic(geodesicload_pointlist.ElementAt(0), geodesicload_pointlist.ElementAt(1), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Geodesic);
                            var loxdres = Math.Round(loxodromeMeasureResult.Distance, 1);
                            var loxdres1 = Math.Round(loxodromeMeasureResult1.Distance, 1);

                            if (loxodromeMeasureResult.Distance <= 400)
                            {
                                Polyline routeLine2 = new Polyline(geodesicload_pointlist);
                                // Densify the polyline to show the geodesic curve.
                                Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine2, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Loxodrome);
                                var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                var _pathGraphic1 = new Graphic(aftereditpathgeom, linesym);
                                _pathGraphic1.Attributes["ShortName"] = loxdres.ToString() + " " + "NM";
                                _sketchOverlay.Graphics.Add(_pathGraphic1);
                                // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                //  route_symbolsadding(geodesicload_pointlist);
                                // builder.AddPart(geodesicpointlist);
                                geodesicload_pointlist.RemoveAt(0);

                            }
                            else
                            {
                                Polyline routeLine2 = new Polyline(geodesicload_pointlist);
                                // Densify the polyline to show the geodesic curve.
                                Esri.ArcGISRuntime.Geometry.Geometry aftereditpathgeom = GeometryEngine.DensifyGeodetic(routeLine2, 1, LinearUnits.NauticalMiles, GeodeticCurveType.Geodesic);
                                var linesym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 69, 0), 1);
                                var _pathGraphic2 = new Graphic(aftereditpathgeom, linesym);
                                _pathGraphic2.Attributes["ShortName"] = loxdres1.ToString() + " " + "NM";
                                _sketchOverlay.Graphics.Add(_pathGraphic2);
                                // _tempsketchOverlay.Graphics.Add(_pathGraphic);
                                // route_symbolsadding(geodesicload_pointlist);
                                // builder.AddPart(geodesicpointlist);
                                geodesicload_pointlist.RemoveAt(0);
                            }

                        }


                    }
                }
                route_symbolsadding_pointcollection(importedlinepoints);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void import_both(object sender, RoutedEventArgs e)
        {
            string strfilename = "";
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            // openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                openFileDialog.DefaultExt = ".bsk";
                openFileDialog.Filter = "Basket(.bsk)|*.bsk";
                strfilename = openFileDialog.FileName;

            }
            if (strfilename.Contains(".csv"))
            {
                Geodetc_csv_Import_Click(sender, e, strfilename);
                // csv_export_new(sender, e, strfilename);
            }
            else if (strfilename.Contains(".rtz"))
            {
                Geodetc_Import_Click_rtz(sender, e, strfilename);
                //csv_export(sender, e, strfilename);
            }

        }

    }

    public enum SampleState
    {
        AddingStops,
        NotReady,
        Ready,
        AddingBarriers,
        Routing
    }
    class PolyList
    {
        public string Name { get; set; }

        public int WaypointCount { get; set; }

        public DateTime LastModified { get; set; }
        public Graphic _graphic { get; set; }
        public IReadOnlyList<MapPoint> polylinepointcollection { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
