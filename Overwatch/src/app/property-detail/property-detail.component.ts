import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OnDestroy } from '@angular/core/src/metadata/lifecycle_hooks';
import { HttpClient } from '@angular/common/http';
import * as $ from 'jquery';
declare var google: any;
declare var MarkerClusterer: any;

@Component({
  selector: 'app-property-detail',
  templateUrl: './property-detail.component.html',
  styleUrls: ['./property-detail.component.css']
})
export class PropertyDetailComponent implements OnInit, OnDestroy {
  id: number;
  private sub: any;
  detail: any;
  latestAlertDescription: string;
  imageUrl: string;
  weatherData: any;

  constructor(private route: ActivatedRoute, private httpClient: HttpClient) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
       this.id = +params['id'];

       this.httpClient.get('http://hpk-overwatch-func.azurewebsites.net/api/PropertyRetrieval?propertyId=' + this.id).subscribe(data => {
        this.detail = data;
        if (this.detail.latestPropertyAlert) {
          this.latestAlertDescription = this.detail.latestPropertyAlert.propertyAlert;
          this.imageUrl = this.detail.latestPropertyAlert.imageUrl;
        }
        
        console.log(this.detail);
        console.log(this.detail.latitude);
        console.log(this.detail.longitude);

        this.httpClient.get('http://hpk-overwatch-func.azurewebsites.net/api/Weather?lat='
                            + this.detail.latitude  + '&long=' + this.detail.longitude)
        .subscribe(weatherData => {
          this.weatherData = weatherData;
        });

        ///
      var map, heatmap;
      var latitude = this.detail.latitude;
      var longitude = this.detail.longitude;
      var incidents = [];
      var infowindow = new google.maps.InfoWindow();
      var wellInfoWindow = new google.maps.InfoWindow();
      var crimeDataSource = "https://data.fortworthtexas.gov/resource/fauf-2yhn.json";
      var heatMapPoints = [];
      var markerCluster: any;
      var markers;

    function initMap() {
      map = new google.maps.Map(document.getElementById('map'), {
        zoom:10,
        center: new google.maps.LatLng(latitude, longitude),
        mapTypeId: 'satellite'
      });

      $.ajax({
        type: "GET",
        url: crimeDataSource + "?$order=reported_date%20desc&$limit=500",
      }).done(function (data) {
        for (var i = 0; i < data.length; i++) {
          
                        if (typeof data[i].location_1 != "undefined" && data[i].location_1.coordinates) {
                            incidents.push(data[i]);
                        }
                    }
          
                    //// Note: The code uses the JavaScript Array.prototype.map() method to
                    //// create an array of markers based on a given "incidents" array.
                    //// The map() method here has nothing to do with the Google Maps API.
                    markers = incidents.map(function (data, i) {
          
                        var latLng = new google.maps.LatLng(data.location_1.coordinates[1], data.location_1.coordinates[0]);
                       // console.log(latLng)
                        heatMapPoints.push(latLng);
          
                        var marker = new google.maps.Marker({
                            position: latLng,
                            title: data.offense_desc
                        });
                        var contentString = '<div id="content">' +
                                        '<h4 id="header">' + data.offense_desc + '</h4>' +
                                        '<div id="bodyContent">' +
                                        '<p><b>Date:</b> ' + (new Date(data.reported_date)).toLocaleDateString("en-US") + '<br />' +
                                           '<b>Location Type:</b> ' + data.locationtypedescription + '<br />' +
                                         '<b>Address:</b> ' + data.block_address + '<br />' +
                                                        '</div></div>';
                        //console.log(contentString)
                        google.maps.event.addListener(marker, 'click', (function (marker, contentString, infowindow) {
                            return function () {
                                infowindow.setContent(contentString);
                                infowindow.open(map, marker);
                            };
                        })(marker, contentString, infowindow));
                        
                        return marker;
                    });
          
                  //  buildClusterer();
          
                    //console.log(heatMapPoints);
                    heatmap = new google.maps.visualization.HeatmapLayer({
                        data: heatMapPoints,
                        map: map
                    });

                    // Maps Icons Collection https://mapicons.mapsmarker.com
                    var wellMarker = new google.maps.Marker({
                      position: new google.maps.LatLng(latitude, longitude),
                      title: 'Well',
                      map: map,
                      icon: 'https://hpkoverwatchfunb4ed.blob.core.windows.net/azure-webjobs-hosts/icons/oilpumpjack.png'
                  });

                  var wellContentString = '<div id="content">' +
                  '<h4 id="header">Well</h4>' +
                  '<div id="bodyContent">' +
                                  '</div></div>';
                  wellInfoWindow.setContent(wellContentString);
                  wellMarker.addListener('click', function() {
                    wellInfoWindow.open(map, wellMarker);
                  });

      });
      
        
     
    }
      
    function buildClusterer() {

        //// Add a marker clusterer to manage the markers.
        markerCluster = new MarkerClusterer(map, markers,
            { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

        //markerCluster.fitMapToMarkers();
    }

    $( document ).ready(function() {
        initMap();

        $('#btn-markers').click(function(){
          heatmap.setMap(null);
          buildClusterer();
          $('#btn-markers').css('background-color', 'black');
          $('#btn-heatmap').css('background-color', '');
        });
        $('#btn-heatmap').click(function(){
          heatmap.setMap(map);
          markerCluster.clearMarkers();
          markerCluster.resetViewport(true);
          $('#btn-heatmap').css('background-color', 'black');
          $('#btn-markers').css('background-color', '');
        });
    });

      ///
      });
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}
