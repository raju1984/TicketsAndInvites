//Set up some of our variables.
var map, infoWindow; //Will contain map object.
var lat1;
var long1;
var cityName;
var AddressLine;
var Country;
var CountryCode;
//Function called to initialize / create the map.
//This is called when the page has loaded.
function initMap() {

    debugger;
    //The center location of our map.
    if(!lat1 || !long1)
    {
        lat1 = '52.357971';
        long1 = '-6.516758'
    }
    var myCenter = new google.maps.LatLng(lat1, long1);
    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: lat1, lng: long1 },
        zoom: 16,
        scrollwheel: false,
        navigationControl: false,
        mapTypeControl: false,
        scaleControl: false,
        draggable: true,
    });
    setMarkers(map)
    
}
//Load the map when the page has finished loading.
google.maps.event.addDomListener(window, 'load', GetLocations);
function GetLocations() {
    var addrestosearch = "";
    var geocoder = new google.maps.Geocoder();
    debugger;
    var country = $('#CountryId option:selected').text(); //document.getElementById("CountryId").value;
    
    var address = document.getElementById("AddressLine").value;
    var cityname = document.getElementById("CityName").value;

    if (country && country != 'Please select a country') {
        addrestosearch = country;
    }
    
    if (cityname)
    {
        addrestosearch = addrestosearch+" "+cityname;
    }
    if (address) {
        addrestosearch = address + addrestosearch;
    }
    if (addrestosearch != '' && addrestosearch) {
        geocoder.geocode({ 'address': addrestosearch }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {

                lat1 = results[0].geometry.location.lat();
                long1 = results[0].geometry.location.lng();
                $('#lat').val(lat1);
                $('#lng').val(long1);
                initMap();
            } else {
                alert("Request failed.")
            }
        });
    }
    else {
        navigator.geolocation.getCurrentPosition(
            function (position) { // success cb

                /* Current Coordinate */
                lat1 = position.coords.latitude;
                long1 = position.coords.longitude;
                $('#lat').val(lat1);
                $('#lng').val(long1);
                // alert("Latitude: " + lat + "\nLongitude: " + long);
                initMap();
            },
            function () { // fail cb


            }
        );
    }
};
function getDetail(lat1, lng1) {
    debugger;
    var zipcode = [];
    var google_map_pos = new google.maps.LatLng(lat1, lng1);

    /* Use Geocoder to get address */
    var google_maps_geocoder = new google.maps.Geocoder();
    google_maps_geocoder.geocode(
        { 'latLng': google_map_pos },
        function (results, status) {
            if (status == google.maps.GeocoderStatus.OK && results[0]) {
                //console.log(results[0]);
                var AddressLine = "";
                for (var i = 0; i < results[0].address_components.length; i++) {

                    if (results[0].address_components[i].types[0] == "locality") {
                        //this is the object you are looking for City
                        city = results[0].address_components[i];
                        //console.log(city);
                        cityName = city.long_name;

                    }
                    //if (results[0].address_components[i].types[0] == "administrative_area_level_1") {
                    //    //this is the object you are looking for State
                    //    region = results[0].address_components[i];
                    //   // console.log(region.long_name);
                    //}
                    if (results[0].address_components[i].types[0] == "country") {
                        //this is the object you are looking for
                        country = results[0].address_components[i];
                        //console.log(country);
                        //GetCountryId(country.long_name, country.short_name)
                    }
                    if (results[0].address_components[i].types[0] == "postal_code") {
                        //this is the object you are looking for
                        zipcode = results[0].address_components[i];
                        //console.log(zipcode);
                        //$('#Pincode').val(zipcode.long_name);
                    }

                    if (results[0].address_components[i].types[0] == "political" && results[0].address_components[i].types[2] == "sublocality_level_2") {
                        //this is the object you are looking for State
                        region = results[0].address_components[i];
                        AddressLine = AddressLine + " " + region.long_name
                        //console.log(region.long_name);
                    }

                    if (results[0].address_components[i].types[0] == "premise") {
                        //this is the object you are looking for State
                        region = results[0].address_components[i];
                        AddressLine = AddressLine + " " + region.long_name
                        // console.log(region.long_name);
                    }

                    if (results[0].address_components[i].types[0] == "political" && results[0].address_components[i].types[2] == "sublocality_level_1") {
                        //this is the object you are looking for State
                        region = results[0].address_components[i];
                        // AddressLine = AddressLine + " " + region.long_name
                        // console.log(region.long_name);
                    }

                    if (results[0].address_components[i].types[0] == "route") {
                        //this is the object you are looking for State
                        region = results[0].address_components[i];
                        // this Is For Address Area
                    }

                    if (results[0].address_components[i].types[0] == "street_number") {
                        //this is the object you are looking for State
                        region = results[0].address_components[i];
                        AddressLine = AddressLine + " " + region.long_name
                        // console.log(region.long_name);
                    }
                }
                if (cityName)
                {
                    $("#CityName").val(cityName);
                }
                if (zipcode)
                {
                    $("#hfzipcode").val(zipcode.long_name);
                }
                $("#spnaddress").text(results[0].formatted_address);
                $("#address_").val(results[0].formatted_address);
                // $.cookie('Addressvalue', results[0].formatted_address);

            }
        }
    );
}
function setMarkers(map) {

    // Adds markers to the map.

    // Marker sizes are expressed as a Size of X,Y where the origin of the image
    // (0,0) is located in the top left of the image.

    // Origins, anchor positions and coordinates of the marker increase in the X
    // direction to the right and in the Y direction down.
    var image = {
        url: '/images/Pinboll.png',
        // url: 'https://markethall.adequateshop.com/images/Pinboll.png',
        // This marker is 32 pixels wide by 32 pixels high.
        size: new google.maps.Size(48, 48),
        // The origin for this image is (0, 0).
        origin: new google.maps.Point(0, 0),
        // The anchor for this image is just off centre (0, 10).
        anchor: new google.maps.Point(0, 10)
    };
    // Shapes define the clickable region of the icon. The type defines an HTML
    // <area> element 'poly' which traces out a polygon as a series of X,Y points.
    // The final coordinate closes the poly by connecting to the first coordinate.
    var shape = {
        coords: [1, 1, 1, 32, 30, 32, 30, 1],
        type: 'poly'
    };

    var marker = new google.maps.Marker({
        position: { lat: lat1, lng: long1 },
        map: map,
        draggable: true,
        icon: image,
        shape: shape,
        animation: google.maps.Animation.DROP,
        title: 'Your Location',
        zIndex: 1,

    });

    var lattiude
    var longitude
    marker.addListener('dragend', function () {
        debugger;
        lattiude = this.getPosition().lat();
        longitude = this.getPosition().lng();
        $('#lat').val(lattiude);
        $('#lng').val(longitude);
        getDetail(lattiude, longitude);
    });



}