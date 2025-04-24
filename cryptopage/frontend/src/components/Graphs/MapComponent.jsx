import React, { useState, useEffect } from 'react';
import { ComposableMap, Geographies, Geography, Annotation } from 'react-simple-maps';

const geoUrl = 'https://cdn.jsdelivr.net/npm/world-atlas@2/countries-110m.json';

const MapComponent = ({ countryData = [], excludedCountries = [] }) => {
  const [geoData, setGeoData] = useState(null);
  const [distributionWithCoordinates, setDistributionWithCoordinates] = useState([]);
  
  // Function to get color based on percentage
  const getCountryColor = (percentage) => {
    if (percentage >= 30) return "#38B2AC"; // teal-500
    if (percentage >= 20) return "#4FD1C5"; // teal-400
    if (percentage >= 10) return "#81E6D9"; // teal-300
    if (percentage >= 5) return "#B2F5EA";  // teal-200
    return "#E6FFFA";  // teal-100
  };
  
  // Load GeoJSON data
  useEffect(() => {
    fetch(geoUrl)
      .then(response => response.json())
      .then(data => {
        setGeoData(data);
        
        // Process country data to add coordinates if missing
        const updatedDistribution = countryData.map(country => {
          // If country already has coordinates, use them
          if (country.coordinates && country.coordinates.length === 2) {
            return country;
          }
          
          // Otherwise, find coordinates from GeoJSON
          const countryGeo = data.objects.countries.geometries.find(
            geo => geo.properties.name === country.name || 
                  geo.properties.name.includes(country.name) ||
                  country.name.includes(geo.properties.name)
          );
          
          if (countryGeo && data.objects.countries.geometries) {
            // Find the feature in GeoJSON
            const feature = data.features?.find(f => 
              f.properties.name === countryGeo.properties.name
            );
            
            // Extract centroid or use a default mechanism to find coordinates
            let coordinates = country.coordinates; // Default to existing
            
            if (feature && feature.geometry) {
              // Simple centroid calculation for Point type
              if (feature.geometry.type === "Point") {
                coordinates = feature.geometry.coordinates;
              } 
              // For polygons or other types, you'd need a centroid algorithm
              // This is a simplified placeholder
              else if (feature.geometry.coordinates && feature.geometry.coordinates[0]) {
                const coords = feature.geometry.coordinates[0];
                // Very simple centroid approximation (first point or average)
                coordinates = coords[0] || [0, 0];
              }
            }
            
            return { ...country, coordinates: coordinates || [0, 0] };
          }
          
          return country; // Return unchanged if no match found
        });
        
        setDistributionWithCoordinates(updatedDistribution);
      })
      .catch(error => console.error("Error loading GeoJSON:", error));
  }, [countryData]);
  
  // Calculate total members
  const totalMembers = distributionWithCoordinates.reduce(
    (sum, country) => sum + (country.members || 0), 
    0
  );
  
  return (
    <div className="flex flex-col h-full bg-gray-800 rounded-lg p-4 shadow-lg">
      <h2 className="text-xl font-semibold mb-4">Team Distribution Map</h2>
      
      <div className="flex-1 relative">
        <ComposableMap projectionConfig={{ scale: 140 }}>
          {geoData && (
            <Geographies geography={geoUrl}>
              {({ geographies }) =>
                geographies
                  .filter(geo => !excludedCountries.includes(geo.id) && 
                                !excludedCountries.includes(geo.properties.name))
                  .map(geo => {
                    const countryData = distributionWithCoordinates.find(
                      country => geo.properties.name === country.name ||
                               geo.properties.name.includes(country.name) ||
                               country.name.includes(geo.properties.name)
                    );

                    const fillColor = countryData 
                      ? getCountryColor(countryData.percentage) 
                      : "#6B7280";

                    return (
                      <Geography
                        key={geo.rsmKey}
                        geography={geo}
                        fill={fillColor}
                        stroke="#374151"
                        strokeWidth={0.5}
                        style={{
                          default: { outline: 'none' },
                          hover: { fill: countryData ? "#2C7A7B" : "#4B5563", outline: 'none' },
                          pressed: { outline: 'none' }
                        }}
                      />
                    );
                  })
              }
            </Geographies>
          )}
          
          {/* Annotations with percentages */}
          {distributionWithCoordinates.map((country) => (
            country.coordinates && (
              <Annotation
                key={country.name}
                subject={country.coordinates}
                dx={0}
                dy={0}
              >
                <text
                  x={0}
                  y={0}
                  fontSize={8}
                  fontWeight="bold"
                  textAnchor="middle"
                  fill="#FFFFFF"
                  stroke="#000000"
                  strokeWidth={0.5}
                  paintOrder="stroke"
                >
                  {`${country.percentage}%`}
                </text>
              </Annotation>
            )
          ))}
        </ComposableMap>
      </div>
      
      {/* Legend for country distribution */}
      <div className="mt-4">
        <h3 className="text-sm font-medium mb-2">Distribution by Country</h3>
        <div className="grid grid-cols-3 gap-2">
          {distributionWithCoordinates.map((country) => (
            <div key={country.name} className="flex items-center">
              <div 
                className="w-3 h-3 rounded-sm mr-1" 
                style={{ backgroundColor: getCountryColor(country.percentage) }}
              />
              <span className="text-xs truncate">
                {country.name.length > 15 ? country.name.substring(0, 15) + "..." : country.name}
              </span>
              <span className="text-xs text-gray-400 ml-1">{country.percentage}%</span>
            </div>
          ))}
        </div>
      </div>
      
      <div className="flex items-center justify-between mt-4 text-xs text-gray-400">
        <div>{totalMembers} Team Members Total</div>
        <div className="flex items-center space-x-1">
          <div className="w-2 h-2 rounded-sm" style={{ backgroundColor: "#E6FFFA" }}></div>
          <div className="w-2 h-2 rounded-sm" style={{ backgroundColor: "#B2F5EA" }}></div>
          <div className="w-2 h-2 rounded-sm" style={{ backgroundColor: "#81E6D9" }}></div>
          <div className="w-2 h-2 rounded-sm" style={{ backgroundColor: "#4FD1C5" }}></div>
          <div className="w-2 h-2 rounded-sm" style={{ backgroundColor: "#38B2AC" }}></div>
          <span>Concentration</span>
        </div>
      </div>
    </div>
  );
};

export default MapComponent;