let map;
let service;
let markers = []; // This should be globally accessible
window.initMap = function () {
    // Default to a predefined location in case Geolocation API fails or is not allowed
    const defaultLocation = { lat: -33.8688, lng: 151.2195 };
    map = new google.maps.Map(document.getElementById("map"), {
        center: defaultLocation,
        zoom: 13,
    });

    // Try to use the Geolocation API to set the map center to the user's current location
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            function (position) {
                const userLocation = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                map.setCenter(userLocation);
                // Optionally, add a marker at the user's location
                new google.maps.Marker({
                    map: map,
                    position: userLocation,
                    title: "You are here"
                });
            },
            function () {
                // Handle error or denial of permission
                console.error("Geolocation API not supported or permission denied.");
                // Map is already initialized with the default location, so you can add error handling here if needed
            }
        );
    } else {
        // Browser doesn't support Geolocation
        console.error("Your browser doesn't support geolocation.");
        // Map is already initialized with the default location, so you can add additional fallback logic here if needed
    }

    // Initialize the global InfoWindow object that will be reused.
    window.infowindow = new google.maps.InfoWindow();
}


window.filterMap = function (placeType) {
    if (!placeType) return; // Exit if placeType is not selected

    const request = {
        location: map.getCenter(),
        radius: '5000',
        type: [placeType]
    };

    clearResults(); // Clear existing markers

    service = new google.maps.places.PlacesService(map);
    service.nearbySearch(request, (results, status) => {
        if (status === google.maps.places.PlacesServiceStatus.OK) {
            results.forEach(place => {
                if (!place.geometry || !place.geometry.location) return;

                const marker = new google.maps.Marker({
                    map: map,
                    position: place.geometry.location,
                });

                // Create an InfoWindow for each marker, and set its content
                marker.addListener('click', () => {
                    // Content to be displayed; customize this as needed
                    const contentString = `
                        <div>
                            <h3>${place.name}</h3>
                            <p>${place.vicinity}</p>
                            <p>${place.location}</p>
                        </div>
                    `;
                    window.infowindow.setContent(contentString);
                    window.infowindow.open(map, marker);
                });

                markers.push(marker);
            });
        }
    });
}

function clearResults() {
    markers.forEach(marker => marker.setMap(null));
    markers = [];
}
