using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;
using System.Windows.Media;

namespace ArcGISApp1
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        {
        }

        const double EarthRadius_km = 6371.0;

        public static void RadialToLatitudeLongitude(double r, double phi, double burstPointLatitude, double burstPointLongitude, out double latitude, out double longitude)
        {
            double POTHT = r / EarthRadius_km;

            // Compute vectors relative to burst point.
            double ROT2Z = Math.Cos(POTHT);
            double ROT2XY = Math.Sin(POTHT);
            double ROT2X = -ROT2XY * Math.Cos(phi);
            double ROTY = ROT2XY * Math.Sin(phi);

            // Undo the rotation around y, so that we are now in real latitudes.
            double ROTX = ROT2X * Math.Sin(burstPointLatitude) + ROT2Z * Math.Cos(burstPointLatitude);
            double ROTZ = ROT2Z * Math.Sin(burstPointLatitude) - ROT2X * Math.Cos(burstPointLatitude);

            // Convert to latitude, longitude; including undo longitude rotation.
            System.Diagnostics.Debug.Assert(Math.Abs(ROTZ) <= 1.0001);
            if (ROTZ > 1.0)
                ROTZ = 1.0;
            if (ROTZ < -1.0)
                ROTZ = -1.0;

            latitude = Math.Asin(ROTZ);
            longitude = Atan2X(ROTY, ROTX) + burstPointLongitude;

            if (longitude > Math.PI)
                longitude = longitude - 2.0 * Math.PI;
            if (longitude < -Math.PI)
                longitude = longitude + 2.0 * Math.PI;
        }

        public static double Atan2X(double y, double x)
        {
            double atan = 0.0;
            if (x != 0.0 || y != 0.0)
                atan = Math.Atan2(y, x);

            while (atan > 2.0 * Math.PI)
                atan -= 2.0 * Math.PI;
            while (atan < 0.0)
                atan += 2.0 * Math.PI;

            return atan;
        }

        public Polygon CreateCirclePolygon(double lat, double lon, double radiuskm)
        {
            var mappoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);



            double lat_rad1; //outer ring
            double lon_rad1;
            for (var i = 0; i < 35; i++)
            {
                var theta = (float)(2 * Math.PI * i) / 35;
                //outer ring
                RadialToLatitudeLongitude(radiuskm, theta, lat * Math.PI / 180, lon * Math.PI / 180, out lat_rad1, out lon_rad1);

                mappoints.Add(lon_rad1 * 180.0 / Math.PI, lat_rad1 * 180.0 / Math.PI, 10);
            }

            return new Polygon(mappoints);
        }
        public void CreateCircle()
        {
            var mapOL = new GraphicsOverlay();
            mapOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;
            
            var sceneOL = new GraphicsOverlay();
            sceneOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            double lat = 55;
            double lon = -57;
            double radiuskm = 5000;
            Color colorRed = Colors.Red;
            colorRed.A = 180;
            int zOrder = 1;

            var poly = CreateCirclePolygon(lat, lon, radiuskm);

            mapOL.Graphics.Add(CreateGraphic(poly, colorRed, zOrder, false)[0]);
            _mapgraphicsOverlays.Clear();
            _mapgraphicsOverlays.Add(mapOL);

            var poly2 = CreateCirclePolygon(lat, lon, radiuskm);

            sceneOL.Graphics.Add(CreateGraphic(poly2, colorRed, zOrder, false)[0]);
            _scenegraphicsOverlays.Clear();
            _scenegraphicsOverlays.Add(sceneOL);

            OnPropertyChanged("MapGraphicsOverlays");
            OnPropertyChanged("SceneGraphicsOverlays");
        }

        public void CreateSmallCircles()
        {
            var mapOL = new GraphicsOverlay();
            mapOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            var sceneOL = new GraphicsOverlay();
            sceneOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            double lat = -180;
            double lon = 0;
            double radiuskm = 500;
            Color colorRed = Colors.Red;
            colorRed.A = 180;
            int zOrder = 1;

            var polyA1 = CreateCirclePolygon(lat, lon, radiuskm);
            var polyA2 = CreateCirclePolygon(lat, lon, radiuskm / 2);
            var polyA12 = GeometryEngine.Difference(polyA1, polyA2);

            mapOL.Graphics.Add(CreateGraphic(polyA12, colorRed, zOrder, false)[0]);
            _mapgraphicsOverlays.Clear();
            _mapgraphicsOverlays.Add(mapOL);

            var polyB1 = CreateCirclePolygon(lat, lon, radiuskm);
            var polyB2 = CreateCirclePolygon(lat, lon, radiuskm / 2);
            var polyB12 = GeometryEngine.Difference(polyB1, polyB2);

            sceneOL.Graphics.Add(CreateGraphic(polyB12, colorRed, zOrder, false)[0]);
            _scenegraphicsOverlays.Clear();
            _scenegraphicsOverlays.Add(sceneOL);

            OnPropertyChanged("MapGraphicsOverlays");
            OnPropertyChanged("SceneGraphicsOverlays");
        }



        public void CreateSmallCircle()
        {
            var mapOL = new GraphicsOverlay();
            mapOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            var sceneOL = new GraphicsOverlay();
            sceneOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;

            double lat = 180;
            double lon = 0;
            double radiuskm = 500;
            Color colorRed = Colors.Red;
            colorRed.A = 180;
            int zOrder = 1;

            var polyA1 = CreateCirclePolygon(lat, lon, radiuskm);

            mapOL.Graphics.Add(CreateGraphic(polyA1, colorRed, zOrder, false)[0]);
            _mapgraphicsOverlays.Clear();
            _mapgraphicsOverlays.Add(mapOL);

            var polyB1 = CreateCirclePolygon(lat, lon, radiuskm);

            sceneOL.Graphics.Add(CreateGraphic(polyB1, colorRed, zOrder, false)[0]);
            _scenegraphicsOverlays.Clear();
            _scenegraphicsOverlays.Add(sceneOL);

            OnPropertyChanged("MapGraphicsOverlays");
            OnPropertyChanged("SceneGraphicsOverlays");
        }


        protected List<Graphic> CreateGraphic(Esri.ArcGISRuntime.Geometry.Geometry poly, System.Windows.Media.Color color, int zOrder, bool bDrawEdge = true)
        {
            List<Graphic> graphicList = new List<Graphic>();

            graphicList.Add(new Graphic() { Geometry = poly, Symbol = new SimpleFillSymbol() { Color = color }, ZIndex = zOrder });

            if (bDrawEdge)
                graphicList.Add(new Graphic() { Geometry = poly, Symbol = new SimpleLineSymbol() { Color = System.Windows.Media.Colors.Black, Width = 1 }, ZIndex = zOrder });

            return graphicList;
        }
        protected Polygon CreatePolygon(Esri.ArcGISRuntime.Geometry.PointCollection mappoints)
        {
            var poly = new Polygon(mappoints); // new List<Esri.ArcGISRuntime.Geometry.PointCollection> { points }, sref);

            GeometryEngine.BufferGeodetic(poly, 0, LinearUnits.Kilometers);
            //GeometryEngine.DensifyGeodetic(poly, 10, LinearUnits.Kilometers);
            return poly;
        }

        public void CreateOpacity()
        {
            Color colorRed = Colors.Red;
            colorRed.A = 180;

            {
                var mapOL = new GraphicsOverlay();
                mapOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;
                var listpc = new List<Esri.ArcGISRuntime.Geometry.PointCollection>();
                var pc1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                pc1.Add(-5, -5, 10);
                pc1.Add(5, -5, 10);
                pc1.Add(5, 5, 10);
                pc1.Add(-5, 5, 10);

                listpc.Add(pc1);

                var pc2 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                pc2.Add(-2, -2, 10);
                pc2.Add(2, -2, 10);
                pc2.Add(2, 2, 10);
                pc2.Add(-2, 2, 10);

                listpc.Add(pc2);


                var poly1 = new Polygon(listpc);
                var g1 = new Graphic(poly1, new SimpleFillSymbol() { Color = Colors.Red, Outline = new SimpleLineSymbol() { Color = Colors.Black, Width = 2, Style = SimpleLineSymbolStyle.Solid }, Style = SimpleFillSymbolStyle.Null }) { ZIndex = 2 };

                mapOL.Graphics.Add(g1);

                var poly2 = new Polygon(listpc);
                var g2 = new Graphic(poly1, new SimpleFillSymbol() { Color = colorRed, Outline = new SimpleLineSymbol() { Color = Colors.Transparent, Width = 0, Style = SimpleLineSymbolStyle.Null }, Style = SimpleFillSymbolStyle.Solid }) { ZIndex = 1 };

                mapOL.Graphics.Add(g2);

                _mapgraphicsOverlays.Clear();


                _mapgraphicsOverlays.Add(mapOL);
            }
            {
                var sceneOL = new GraphicsOverlay();
                sceneOL.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;
                sceneOL.RenderingMode = GraphicsRenderingMode.Static;

                var listpc = new List<Esri.ArcGISRuntime.Geometry.PointCollection>();
                var pc1 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                pc1.Add(-5, -5, 10);
                pc1.Add(5, -5, 10);
                pc1.Add(5, 5, 10);
                pc1.Add(-5, 5, 10);

                listpc.Add(pc1);

                var pc2 = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                pc2.Add(-2, -2, 10);
                pc2.Add(-2, 2, 10);
                pc2.Add(2, 2, 10);
                pc2.Add(2, -2, 10);

                listpc.Add(pc2);



                var poly1 = new Polygon(listpc);
                var g1 = new Graphic(poly1, new SimpleFillSymbol() { Color = Colors.Red, Outline = new SimpleLineSymbol() { Color = Colors.Black, Width = 2, Style = SimpleLineSymbolStyle.Solid }, Style = SimpleFillSymbolStyle.Null }) { ZIndex = 2 };

                sceneOL.Graphics.Add(g1);

                var poly2 = new Polygon(listpc);
                var g2 = new Graphic(poly1, new SimpleFillSymbol() { Color = colorRed, Outline = new SimpleLineSymbol() { Color = Colors.Transparent, Width = 0, Style = SimpleLineSymbolStyle.Null }, Style = SimpleFillSymbolStyle.Solid }) { ZIndex = 1 };

                sceneOL.Graphics.Add(g2);

                _scenegraphicsOverlays.Clear();


                _scenegraphicsOverlays.Add(sceneOL);
            }

            OnPropertyChanged("MapGraphicsOverlays");
            OnPropertyChanged("SceneGraphicsOverlays");
        }

        private GraphicsOverlayCollection _mapgraphicsOverlays = new GraphicsOverlayCollection();
        private GraphicsOverlayCollection _scenegraphicsOverlays = new GraphicsOverlayCollection();
        private Map _map = new Map(Basemap.CreateStreetsVector());
        private Scene _scene = new Scene(Basemap.CreateStreets());

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
        }

        public Scene Scene
        {
            get { return _scene; }
            set { _scene = value; OnPropertyChanged(); }
        }

        public GraphicsOverlayCollection MapGraphicsOverlays
        {
            get { return _mapgraphicsOverlays; }
            set { _mapgraphicsOverlays = value; OnPropertyChanged(); }
        }

        public GraphicsOverlayCollection SceneGraphicsOverlays
        {
            get { return _scenegraphicsOverlays; }
            set { _scenegraphicsOverlays = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}