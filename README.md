# web-scraper-api

Exposes an endpoint which will use HtmlAgilityPack to get all upcoming events from [Red Rocks' Website](https://www.redrocksonline.com/events/#), pull event data out of the html and return a collection with all scheduled concerts with their dates.

# Improvements

- [ ] Deploy to cloud
- [ ] Get Spotify artist IDs and include in collection (maybe have this in a different API)
- [ ] Restructure code so it isn't all in the controller
- [ ] Better error handling with HTTP status codes
- [ ] Unit tests
