# Calculate the distance between two cities
Gets latitude/longitude for the two cities of your choice and calculates the distance between them. Requires an Open Map Quest API key | written for Net Core 3.1

```GetLatLngAsync``` method could be simplified considerably by requesting a CSV string instead of json from
the Map Quest API, but the main reason for this exercise was familiarization with C#/.NET Core 3.1, so I chose to play
around with ```System.Text.Json```
